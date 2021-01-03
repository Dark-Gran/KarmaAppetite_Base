using RWCustom;
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class patch_PlayerProgression
{
    public static void Patch()
    {
        On.PlayerProgression.SaveToDisk += PlayerProgression_SaveToDisk;
    }

    private static void PlayerProgression_SaveToDisk(On.PlayerProgression.orig_SaveToDisk orig, PlayerProgression self, bool saveCurrentState, bool saveMaps, bool saveMiscProg)
    {
        if (saveCurrentState && self.currentSaveState != null)
        {
			self.currentSaveState.theGlow = KarmaAppetite.GetGlow(self.currentSaveState.food, self.currentSaveState.deathPersistentSaveData.karma);
		}
		orig.Invoke(self, saveCurrentState, saveMaps, saveMiscProg);
    }

}