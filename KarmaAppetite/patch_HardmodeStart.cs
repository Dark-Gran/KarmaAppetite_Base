using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class patch_HardmodeStart
{
	public static void Patch()
	{
        On.HardmodeStart.Update += HardmodeStart_Update;
	}

    private static void HardmodeStart_Update(On.HardmodeStart.orig_Update orig, HardmodeStart self, bool eu)
    {
        bool needsFix = self.phase == HardmodeStart.Phase.Init && self.player != null;
        orig.Invoke(self, eu);
        if (needsFix)
        {
            self.player.playerState.foodInStomach = 1;
        }
    }

}