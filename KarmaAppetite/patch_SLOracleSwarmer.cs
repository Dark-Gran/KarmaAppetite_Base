using System;

public class patch_SLOracleSwarmer
{
    public static void Patch()
    {
        On.SLOracleSwarmer.BitByPlayer += SLOracleSwarmer_BitByPlayer;
    }

    //NO LIGHT FROM NEURONS

    private static void SLOracleSwarmer_BitByPlayer(On.SLOracleSwarmer.orig_BitByPlayer orig, SLOracleSwarmer self, Creature.Grasp grasp, bool eu)
    {
        orig.Invoke(self, grasp, eu);
        KarmaAppetite_Base.RefreshLight(grasp.grabber as Player);
    }

}