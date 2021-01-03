using System;
using UnityEngine;

public class patch_LightSource
{
    public static void Patch()
    {
        On.LightSource.ctor += LightSource_ctor;
    }

    private static void LightSource_ctor(On.LightSource.orig_ctor orig, LightSource self, Vector2 initPos, bool environmentalLight, Color color, UpdatableAndDeletable tiedToObject)
    {
        if (tiedToObject is Player)
        {
            color = KarmaAppetite.GranOrange;
        }
        orig.Invoke(self, initPos, environmentalLight, color, tiedToObject);
    }
    
}