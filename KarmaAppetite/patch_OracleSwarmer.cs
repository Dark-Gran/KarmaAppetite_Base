using System;

public class patch_OracleSwarmer
{
    public static void Patch()
    {   
        On.OracleSwarmer.BitByPlayer += OracleSwarmer_BitByPlayer;
    }

    //NO LIGHT FROM NEURONS

    private static void OracleSwarmer_BitByPlayer(On.OracleSwarmer.orig_BitByPlayer orig, OracleSwarmer self, Creature.Grasp grasp, bool eu)
    {
        orig.Invoke(self, grasp, eu);
        KarmaAppetite.RefreshLight(grasp.grabber as Player);
    }

}