using System;

public class patch_RoomSpecificScript_SBA14
{
    public static void Patch()
    {
        On.RoomSpecificScript.SB_A14KarmaIncrease.Update += SB_A14KarmaIncrease_Update;
    }

    private static void SB_A14KarmaIncrease_Update(On.RoomSpecificScript.SB_A14KarmaIncrease.orig_Update orig, RoomSpecificScript.SB_A14KarmaIncrease self, bool eu)
    {
        orig.Invoke(self, eu);
        if (self.room.game.session is StoryGameSession)
        {
            KarmaAppetite.RefreshAllPlayers(self.room.game.session as StoryGameSession);
            foreach (AbstractCreature ac in (self.room.game.session as StoryGameSession).Players)
            {
                if (ac.realizedCreature != null && ac.realizedCreature is Player)
                {
                    Player plr = ac.realizedCreature as Player;
                    RoomCamera rCam = self.room.game.cameras[0];
                    foreach (RoomCamera.SpriteLeaser sLeaser in rCam.spriteLeasers)
                    {
                        if (sLeaser.drawableObject == plr.graphicsModule)
                        {
                            plr.graphicsModule.ApplyPalette(sLeaser, rCam, rCam.currentPalette);
                        }
                    }
                }
            }
        }
    }
}