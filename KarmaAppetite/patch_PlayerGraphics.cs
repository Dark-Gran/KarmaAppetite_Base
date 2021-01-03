using System;
using UnityEngine;

public class patch_PlayerGraphics
{
    public static void Patch()
    {
        On.PlayerGraphics.SlugcatColor += PlayerGraphics_SlugcatColor;
        On.PlayerGraphics.ApplyPalette += PlayerGraphics_ApplyPalette;
    }

    private static void PlayerGraphics_ApplyPalette(On.PlayerGraphics.orig_ApplyPalette orig, PlayerGraphics self, RoomCamera.SpriteLeaser sLeaser, RoomCamera rCam, RoomPalette palette)
    {
        orig.Invoke(self, sLeaser, rCam, palette);
		if (self.owner is Player && (self.owner as Player).Karma >= 9)
        {
            Color c = Color.Lerp(KarmaAppetite_Base.GranOrange, Color.yellow, 0.3f);
            sLeaser.sprites[9].color = Color.Lerp(c, Color.white, 0.5f);
        }
        
	}

    private static UnityEngine.Color PlayerGraphics_SlugcatColor(On.PlayerGraphics.orig_SlugcatColor orig, int i)
    {
        return KarmaAppetite_Base.GranOrange;
    }

}