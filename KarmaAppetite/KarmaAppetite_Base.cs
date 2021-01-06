using System;
using Partiality.Modloader;
using RWCustom;
using UnityEngine;

public class KarmaAppetite : PartialityMod
{

    public KarmaAppetite()
    {
        instance = this;
        this.ModID = "KarmaAppetite_Base";
        this.Version = "0.1";
        this.author = "DarkGran";
    }

    public static KarmaAppetite instance;

    public override void OnEnable()
    {
        base.OnEnable();
        patch_DreamState.Patch();
        patch_FoodMeter.Patch();
        patch_HardmodeStart.Patch();
        patch_LightSource.Patch();
        patch_Menu.Patch();
        patch_OracleSwarmer.Patch();
        patch_Player.Patch();
        patch_PlayerGraphics.Patch();
        patch_PlayerProgression.Patch();
        patch_RoomSpecificScript_SBA14.Patch();
        patch_SlugcatSelectMenu.Patch();
        patch_SLOracleSwarmer.Patch();
        patch_SSOracleBehavior.Patch();
        patch_StoryGameSession.Patch();
        patch_Weapon.Patch();
    }

    //GENERAL

    public static UnityEngine.Color GranOrange = new Color(0.678f, 0.321f, 0.101f);
    public const int STARTING_MAX_KARMA = 6;

    public static void RefreshAllPlayers(StoryGameSession session)
    {
        foreach (AbstractCreature ac in session.Players)
        {
            if (ac.realizedCreature != null && ac.realizedCreature is Player)
            {
                Player player = ac.realizedCreature as Player;
                KarmaToFood(player.slugcatStats, player.Karma); //does not refresh the FoodMeter
                FoodToStats(player.slugcatStats, player.CurrentFood, player.Karma >= 9);
                KarmaAppetite.RefreshLight(player);
            }
        }
    }

    //SLUGCAT STATS

    public static void KarmaToFood(SlugcatStats self, int karma)
    {
        self.maxFood = GetFoodFromKarma(karma).x;
        self.foodToHibernate = GetFoodFromKarma(karma).y;
    }

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

    //ABILITY PRICE

    public static void RemoveQuarterFood(Player self)
    {
        if (self.playerState.quarterFoodPoints <= 0)
        {
            if (self.playerState.foodInStomach > 0)
            {
                self.AddFood(-1);
                self.playerState.quarterFoodPoints += 3;
                FoodToStats(self.slugcatStats, self.playerState.foodInStomach, self.Karma >= 9);
                RefreshLight(self);
            }
        }
        else
        {
            self.playerState.quarterFoodPoints--;
        }
    }

    //LIGHT

    public static void RefreshLight(Player self)
    {
        bool glowing = GetGlow(self.CurrentFood, self.Karma);
        if (self.glowing != glowing)
        {
            self.glowing = glowing;
            if (!glowing && self.graphicsModule != null && self.graphicsModule is PlayerGraphics)
            {
                ((PlayerGraphics)self.graphicsModule).lightSource.Destroy();
            }
            if (self.room != null)
            {
                if (self.room.game.session is StoryGameSession)
                {
                    (self.room.game.session as StoryGameSession).saveState.theGlow = glowing;
                }
            }
        }
    }

    public static bool GetGlow(int food, int karma)
    {
        return karma + 1 > 4 && food != 0;
    }

}