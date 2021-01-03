using System;
using UnityEngine;

public class patch_SSOracleBehavior
{
    public static void Patch()
    {
        On.SSOracleBehavior.SeePlayer += SSOracleBehavior_SeePlayer;
    }

    private static void SSOracleBehavior_SeePlayer(On.SSOracleBehavior.orig_SeePlayer orig, SSOracleBehavior self) //Monk's Greetings for the Survivor
    {
		if (self.timeSinceSeenPlayer < 0)
		{
			self.timeSinceSeenPlayer = 0;
		}
		self.greenNeuron = null;
		for (int i = 0; i < self.oracle.room.updateList.Count; i++)
		{
			if (self.oracle.room.updateList[i] is NSHSwarmer)
			{
				self.greenNeuron = (self.oracle.room.updateList[i] as NSHSwarmer);
				break;
			}
		}
		Debug.Log(string.Concat(new object[]
		{
			"See player ",
			self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad,
			" gn?:",
			self.greenNeuron != null
		}));
		if ((self.greenNeuron != null || (self.player.objectInStomach != null && self.player.objectInStomach.type == AbstractPhysicalObject.AbstractObjectType.NSHSwarmer)) && !self.HasSeenGreenNeuron)
		{
			self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.pebblesSeenGreenNeuron = true;
			self.NewAction(SSOracleBehavior.Action.GetNeuron_Init);
		}
		else if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad == 0)
		{
			if (self.oracle.room.game.StoryCharacter == 2)
			{
				self.NewAction(SSOracleBehavior.Action.MeetRed_Init);
			}
			else
			{
				self.NewAction(SSOracleBehavior.Action.MeetYellow_Init);
			}
			if (self.oracle.room.game.StoryCharacter != 2)
			{
				self.SlugcatEnterRoomReaction();
			}
		}
		else
		{
			Debug.Log("Throw out player " + self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts);
			if (self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts > 0)
			{
				self.NewAction(SSOracleBehavior.Action.ThrowOut_KillOnSight);
			}
			else
			{
				self.NewAction(SSOracleBehavior.Action.ThrowOut_SecondThrowOut);
				self.SlugcatEnterRoomReaction();
			}
			self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiThrowOuts++;
		}
		self.oracle.room.game.GetStorySession.saveState.miscWorldSaveData.SSaiConversationsHad++;
	}

}