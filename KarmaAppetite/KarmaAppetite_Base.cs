using System;
using Partiality.Modloader;
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
        patch_OracleSwarmer.Patch();
        patch_Player.Patch();
        patch_PlayerGraphics.Patch();
        patch_PlayerProgression.Patch();
        patch_SlugcatSelectMenu.Patch();
        patch_SLOracleSwarmer.Patch();
        patch_SSOracleBehavior.Patch();
        patch_StoryGameSession.Patch();
        patch_Weapon.Patch();
    }

    //GENERAL

    public static UnityEngine.Color GranOrange = new Color(0.678f, 0.321f, 0.101f);
    public const int STARTING_MAX_KARMA = 6;

    //Ability Food-Price

    public static void RemoveQuarterFood(Player self)
    {
        if (self.playerState.quarterFoodPoints <= 0)
        {
            if (self.playerState.foodInStomach > 0)
            {
                self.AddFood(-1);
                self.playerState.quarterFoodPoints += 3;
                patch_Player.FoodToStats(self.slugcatStats, self.playerState.foodInStomach, self.Karma >= 9);
                RefreshLight(self);
            }
        }
        else
        {
            self.playerState.quarterFoodPoints--;
        }
    }

    //Light

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