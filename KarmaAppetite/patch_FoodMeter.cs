using System;
using HUD;
using Menu;

public class patch_FoodMeter
{
	public static void Patch()
	{
		On.HUD.FoodMeter.ctor += FoodMeter_ctor;
		On.HUD.FoodMeter.UpdateShowCount += FoodMeter_UpdateShowCount;
		On.HUD.FoodMeter.QuarterPipShower.Update += QuarterPipShower_Update;
	}

	//APPETITE
	private static void FoodMeter_ctor(On.HUD.FoodMeter.orig_ctor orig, HUD.FoodMeter self, HUD.HUD hud, int maxFood, int survivalLimit)
	{
		if (hud.owner is SlugcatSelectMenu.SlugcatPageContinue)
		{
			maxFood = patch_Player.GetFoodFromKarma(((SlugcatSelectMenu.SlugcatPageContinue)hud.owner).saveGameData.karma).x;
			survivalLimit = patch_Player.GetFoodFromKarma(((SlugcatSelectMenu.SlugcatPageContinue)hud.owner).saveGameData.karma).y;
		}
		orig.Invoke(self, hud, maxFood, survivalLimit);
		if (hud.owner is Player && (hud.owner as Player).slugcatStats.name != SlugcatStats.Name.Red)
		{
			self.quarterPipShower = new FoodMeter.QuarterPipShower(self);
		}
	}

	//REMOVING FOOD POINTS

	private static void FoodMeter_UpdateShowCount(On.HUD.FoodMeter.orig_UpdateShowCount orig, HUD.FoodMeter self)
	{
		if (self.showCount < self.hud.owner.CurrentFood)
		{
			if (self.showCountDelay == 0)
			{
				self.showCountDelay = 10;
				if (self.showCount >= 0 && self.showCount < self.circles.Count && !self.circles[self.showCount].foodPlopped)
				{
					self.circles[self.showCount].FoodPlop();
				}
				self.showCount++;
				if (self.quarterPipShower != null)
				{
					self.quarterPipShower.Reset();
				}
			}
			else
			{
				self.showCountDelay--;
			}
		}
		else if (self.showCount > 0 && self.showCount > self.hud.owner.CurrentFood)
		{
			self.circles[self.showCount - 1].EatFade();
			self.showCount--;
		}
	}

	private static void QuarterPipShower_Update(On.HUD.FoodMeter.QuarterPipShower.orig_Update orig, HUD.FoodMeter.QuarterPipShower self)
	{
		self.lastQuarterPipSin = self.quarterPipSin;
		self.lastLightUp = self.lightUp;
		if (self.displayQuarterFood > 0)
		{
			self.quarterPipSin += 1f;
		}
		self.lightUp *= 0.95f;
		if ((self.owner.hud.owner as Player).playerState.quarterFoodPoints > self.displayQuarterFood)
		{
			self.owner.visibleCounter = 80;
			if (self.owner.fade < 0.5f)
			{
				self.quarterPipDelay = 20;
			}
			else if (self.quarterPipDelay > 0)
			{
				self.quarterPipDelay--;
			}
			else
			{
				self.quarterPipDelay = 20;
				self.displayQuarterFood++;
				self.lightUp = 1f;
				if (self.owner.showCount < self.owner.circles.Count)
				{
					self.owner.circles[self.owner.showCount].QuarterCirclePlop();
				}
			}
		}
		else if ((self.owner.hud.owner as Player).playerState.quarterFoodPoints < self.displayQuarterFood)
		{
			self.owner.visibleCounter = 80;
			self.displayQuarterFood--;
			self.lightUp = 1f;
			if (self.owner.showCount < self.owner.circles.Count)
			{
				self.owner.circles[self.owner.showCount].QuarterCirclePlop();
			}
		}
	}

}