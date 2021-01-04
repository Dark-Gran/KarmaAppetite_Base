using RWCustom;
using System;
using System.IO;

public class patch_StoryGameSession
{
	public static void Patch()
	{
		On.StoryGameSession.ctor += StoryGameSession_ctor;
	}

	private static void StoryGameSession_ctor(On.StoryGameSession.orig_ctor orig, StoryGameSession self, int saveStateNumber, RainWorldGame game)
	{
		orig.Invoke(self, saveStateNumber, game);

		string saveFilePath = string.Concat(new object[]
		{
			Custom.RootFolderDirectory(),
			"UserData",
			Path.DirectorySeparatorChar,
			self.game.rainWorld.options.SaveFileName,
			".txt"
		});

		if (!File.Exists(saveFilePath) || !game.manager.menuSetup.LoadInitCondition || self.saveState.cycleNumber == 0) //New game
		{
			self.saveState.deathPersistentSaveData.theMark = true;
			if (self.saveState.deathPersistentSaveData.karmaCap < KarmaAppetite.STARTING_MAX_KARMA) 
			{
				self.saveState.deathPersistentSaveData.karmaCap = KarmaAppetite.STARTING_MAX_KARMA;
			}
			if (self.saveStateNumber != 2)
			{
				self.saveState.denPosition = "SB_S07";
				if (self.saveState.miscWorldSaveData.privSlOracleState == null)
				{
					self.saveState.miscWorldSaveData.privSlOracleState = new SLOrcacleState(false, saveStateNumber);
				}
				self.saveState.miscWorldSaveData.privSlOracleState.miscPearlCounter = 6;
				self.saveState.miscWorldSaveData.privSlOracleState.totalPearlsBrought = 6;
				self.saveState.miscWorldSaveData.privSlOracleState.neuronsLeft = 6;
				self.saveState.miscWorldSaveData.privSlOracleState.totNeuronsGiven = 1;
				self.saveState.miscWorldSaveData.privSlOracleState.neuronGiveConversationCounter = 1;
				self.saveState.miscWorldSaveData.privSlOracleState.totalItemsBrought = 7;
				self.saveState.miscWorldSaveData.privSlOracleState.playerEncountersWithMark = 6;
				self.saveState.miscWorldSaveData.privSlOracleState.playerEncounters = 7;
				self.saveState.miscWorldSaveData.privSlOracleState.likesPlayer = 1f;
				KarmaAppetite_MoonFix.FixInfluenceCap(self.saveState.miscWorldSaveData.privSlOracleState);
			}
		}
		int neuronTresh = (self.saveState.saveStateNumber != 2) ? 6 : 5;
		int encTresh = (self.saveState.saveStateNumber != 2) ? 7 : 0;
		KarmaAppetite_MoonFix.SetTreshholds(encTresh, neuronTresh);
		self.lastEverMetMoon = self.saveState.miscWorldSaveData.privSlOracleState != null && self.saveState.miscWorldSaveData.privSlOracleState.playerEncounters > encTresh;
		patch_Player.KarmaToFood(self.characterStats, self.saveState.deathPersistentSaveData.karma);
		patch_Player.FoodToStats(self.characterStats, self.saveState.food, self.saveState.deathPersistentSaveData.karma >= 9);
	}

}