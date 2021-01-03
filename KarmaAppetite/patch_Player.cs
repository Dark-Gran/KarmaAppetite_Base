﻿using System;
using System.Reflection;
using RWCustom;
using UnityEngine;

public class patch_Player
{
	public static void Patch()
	{
		On.Player.AddFood += Player_AddFood;
		On.Player.CanEatMeat += Player_CanEatMeat;
		On.Player.Collide += Player_Collide;
		On.Player.ObjectCountsAsFood += Player_ObjectCountsAsFood;
		On.Player.ObjectEaten += Player_ObjectEaten;
		On.Player.PickupCandidate += Player_PickupCandidate;
		On.Player.SetMalnourished += Player_SetMalnourished;
		On.Player.ThrownSpear += Player_ThrownSpear;
		On.Player.SpearOnBack.SpearToBack += SpearOnBack_SpearToBack;
	}

    //SLUGCAT STATS

    public static void FoodToStats(SlugcatStats self, int food, bool extraStats)
	{

		if (!self.malnourished)
		{
			self.throwingSkill = (food > 0) ? 2 : 0;

			float statBonus = food * ((extraStats) ? 0.08f : 0.04f);

			const float STAT_BASE = 0.88f; //1f - (extraStats * food_potential)
			self.runspeedFac = STAT_BASE - 0.05f + statBonus;
			self.poleClimbSpeedFac = STAT_BASE + statBonus;
			self.corridorClimbSpeedFac = STAT_BASE + statBonus;
			self.lungsFac = STAT_BASE + statBonus;

			self.generalVisibilityBonus = 0f + statBonus / 10;
			self.loudnessFac = 1.45f - statBonus / 2;
			self.visualStealthInSneakMode = 0.11f + statBonus / 2;
			self.bodyWeightFac -= statBonus / 2;
		}
		else
		{
			self.throwingSkill = 0;

			self.loudnessFac = 1.4f;
			self.generalVisibilityBonus = -0.1f;
			self.visualStealthInSneakMode = 0.3f;
		}

	}

	//STRENGTH

	private static void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
	{
		spear.spearDamageBonus = 0.25f + ((self.playerState.foodInStomach / (FOOD_POTENTIAL / 10)) * ((self.Karma >= 9) ? 1f : 0.5f));
		BodyChunk firstChunk2 = spear.firstChunk;
		float speedBoost = 0.73f + (self.playerState.foodInStomach / 10);
		if (speedBoost < 1f && self.playerState.foodInStomach > 0) { speedBoost = 1f; }
		firstChunk2.vel.x = firstChunk2.vel.x * speedBoost;
	}

	//APPETITE

	public static int FOOD_POTENTIAL = 14;

	public static IntVector2 GetFoodFromKarma(int karma)
	{
		switch (karma + 1)
		{
			case 1:
			default:
				return new IntVector2(3, 3);
			case 2:
				return new IntVector2(4, 4);
			case 3:
				return new IntVector2(5, 4);
			case 4:
				return new IntVector2(6, 5);
			case 5:
				return new IntVector2(7, 6);
			case 6:
				return new IntVector2(9, 7);
			case 7:
				return new IntVector2(10, 8);
			case 8:
				return new IntVector2(11, 9);
			case 9:
				return new IntVector2(12, 10);
			case 10:
				return new IntVector2(FOOD_POTENTIAL, 11);
		}
	}

	public static void KarmaToFood(SlugcatStats self, int karma)
	{
		self.maxFood = GetFoodFromKarma(karma).x;
		self.foodToHibernate = GetFoodFromKarma(karma).y;
	}

	private static void CheckPipsOverMax(Player self)
	{
		if (self.playerState.foodInStomach >= self.slugcatStats.maxFood && self.playerState.quarterFoodPoints > 0)
		{
			self.playerState.quarterFoodPoints -= self.playerState.quarterFoodPoints;
		}
	}

	private static void Player_SetMalnourished(On.Player.orig_SetMalnourished orig, Player self, bool m)
	{
		orig.Invoke(self, m);
		KarmaToFood(self.slugcatStats, self.Karma);
		FoodToStats(self.slugcatStats, self.playerState.foodInStomach, self.Karma >= 9);
	}

	private static bool Player_ObjectCountsAsFood(On.Player.orig_ObjectCountsAsFood orig, Player self, PhysicalObject obj)
	{
		return obj != null && obj is IPlayerEdible && ((obj as IPlayerEdible).FoodPoints != 0 || (obj as IPlayerEdible) is KarmaFlower) && (obj as IPlayerEdible).Edible && !(obj is SSOracleSwarmer) && (!(obj is Creature) || (obj.grabbedBy.Count > 0 && obj.grabbedBy[0].grabber == self) || (obj as Creature).dead);
	}

	private static void Player_ObjectEaten(On.Player.orig_ObjectEaten orig, Player self, IPlayerEdible edible)
	{
		if (self.graphicsModule != null)
		{
			(self.graphicsModule as PlayerGraphics).LookAtNothing();
		}
		bool flag = true;
		if (edible is Centipede)
		{
			flag = false;
		}
		else if (edible is VultureGrub)
		{
			flag = false;
		}
		else if (edible is Hazer)
		{
			flag = false;
		}
		else if (edible is EggBugEgg)
		{
			flag = false;
		}
		else if (edible is SmallNeedleWorm)
		{
			flag = false;
		}
		else if (edible is JellyFish)
		{
			flag = false;
		}
		if (flag && (self.playerState.slugcatCharacter != 1 || edible is Fly))
		{
			
			for (int i = 0; i < edible.FoodPoints; i++)
			{
				self.AddQuarterFood();
			}
		}
		else
		{
			self.AddFood(edible.FoodPoints);	
		}
		if (edible is KarmaFlower)
		{
			self.AddFood(1);
		}
		if (self.spearOnBack != null)
		{
			self.spearOnBack.interactionLocked = true;
		}
	}

	private static void Player_AddFood(On.Player.orig_AddFood orig, Player self, int add)
	{
		orig.Invoke(self, add);
		CheckPipsOverMax(self);
		FoodToStats(self.slugcatStats, self.playerState.foodInStomach, self.Karma >= 9);
		KarmaAppetite_Base.RefreshLight(self);
	}

	//CARNIVORE

	private static bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
	{
		return crit.dead && (crit.Template.type == CreatureTemplate.Type.Centipede || crit.Template.type == CreatureTemplate.Type.Centiwing || crit.Template.type == CreatureTemplate.Type.RedCentipede || !(crit is IPlayerEdible));
	}

	//PULL-OUT FOOD PRICE

	public static bool CanAffordPull(Player self)
	{
		return self.playerState.foodInStomach > 0 || self.playerState.quarterFoodPoints > 0 || self.Karma >= KarmaAppetite_Base.STARTING_MAX_KARMA;
	}

	private static void SpearOnBack_SpearToBack(On.Player.SpearOnBack.orig_SpearToBack orig, Player.SpearOnBack self, Spear spr)
	{
		if (spr.mode == Weapon.Mode.StuckInWall)
		{
			if (self.owner.Karma < KarmaAppetite_Base.STARTING_MAX_KARMA)
			{
				KarmaAppetite_Base.RemoveQuarterFood(self.owner);
			}
		}
		orig.Invoke(self, spr);
	}

	private static void Player_Collide(On.Player.orig_Collide orig, Player self, PhysicalObject otherObject, int myChunk, int otherChunk)
	{
		if (otherObject is Creature)
		{
			if (self.animation == Player.AnimationIndex.BellySlide)
			{
				(otherObject as Creature).Stun((!self.longBellySlide) ? 2 : 4);
				if (!self.longBellySlide && self.rollCounter > 11)
				{
					self.rollCounter = 11;
				}
				BodyChunk mainBodyChunk = self.mainBodyChunk;
				mainBodyChunk.vel.x = mainBodyChunk.vel.x + (float)self.rollDirection * 3f;
			}
			if (self.Sleeping)
			{
				self.sleepCounter = 0;
			}
		}
		if (self.Consious)
		{
			if (self.FoodInStomach < self.MaxFoodInStomach && otherObject is Fly && self.Grabability(otherObject) == Player.ObjectGrabability.OneHand)
			{
				for (int i = 0; i < 2; i++)
				{
					if (self.grasps[i] == null)
					{
						self.room.PlaySound((!(otherObject as Fly).dead) ? SoundID.Fly_Caught : SoundID.Fly_Caught_Dead, otherObject.firstChunk);
						if (!(otherObject as Fly).everBeenCaughtByPlayer)
						{
							(otherObject as Fly).everBeenCaughtByPlayer = true;
							if (self.room.game.session is StoryGameSession)
							{
								self.DepleteSwarmRoom();
							}
						}
						self.SlugcatGrab(otherObject, i);
						break;
					}
				}
			}
			else if (self.wantToPickUp > 0 && self.CanIPickThisUp(otherObject))
			{

				if (!(otherObject is Spear) || (otherObject as Spear).mode != Weapon.Mode.StuckInWall || CanAffordPull(self)) //"Don't pickup spear in wall unless affordable"
				{

					if (self.Grabability(otherObject) == Player.ObjectGrabability.TwoHands)
					{
						self.SlugcatGrab(otherObject, 0);
					}
					else
					{
						self.SlugcatGrab(otherObject, self.FreeHand());
					}
					self.wantToPickUp = 0;
				}
			}
		}
		if (self.jumpChunkCounter >= 0 && self.bodyMode == Player.BodyModeIndex.Default && myChunk == 1 && self.bodyChunks[1].pos.y > otherObject.bodyChunks[otherChunk].pos.y - otherObject.bodyChunks[otherChunk].rad / 2f)
		{
			self.jumpChunkCounter = 5;
			self.jumpChunk = otherObject.bodyChunks[otherChunk];
		}
	}

	private static PhysicalObject Player_PickupCandidate(On.Player.orig_PickupCandidate orig, Player self, float favorSpears)
	{
		PhysicalObject result = null;
		float num = float.MaxValue;
		for (int i = 0; i < self.room.physicalObjects.Length; i++)
		{
			for (int j = 0; j < self.room.physicalObjects[i].Count; j++)
			{
				if ((!(self.room.physicalObjects[i][j] is PlayerCarryableItem) || (self.room.physicalObjects[i][j] as PlayerCarryableItem).forbiddenToPlayer < 1) && Custom.DistLess(self.bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].rad + 40f) && (Custom.DistLess(self.bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].rad + 20f) || self.room.VisualContact(self.bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].pos)) && self.CanIPickThisUp(self.room.physicalObjects[i][j]))
				{
					if (!(self.room.physicalObjects[i][j] is Spear) || (self.room.physicalObjects[i][j] as Spear).mode != Weapon.Mode.StuckInWall || CanAffordPull(self)) //"Don't pickup spear in wall unless affordable"
					{
						float num2 = Vector2.Distance(self.bodyChunks[0].pos, self.room.physicalObjects[i][j].bodyChunks[0].pos);
						if (self.room.physicalObjects[i][j] is Spear)
						{
							num2 -= favorSpears;
						}
						if (self.room.physicalObjects[i][j].bodyChunks[0].pos.x < self.bodyChunks[0].pos.x == self.flipDirection < 0)
						{
							num2 -= 10f;
						}
						if (num2 < num)
						{
							result = self.room.physicalObjects[i][j];
							num = num2;
						}
					}
				}
			}
		}
		return result;
	}

}