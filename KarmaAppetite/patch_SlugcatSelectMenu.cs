using System;
using UnityEngine;

public class patch_SlugcatSelectMenu
{
    public static void Patch()
    {
        On.Menu.SlugcatSelectMenu.StartGame += SlugcatSelectMenu_StartGame;
    }

    private static void SlugcatSelectMenu_StartGame(On.Menu.SlugcatSelectMenu.orig_StartGame orig, Menu.SlugcatSelectMenu self, int storyGameCharacter) //Always SkipIntro
    {
		if (self.manager.rainWorld.progression.gameTinkeredWith)
		{
			return;
		}
		self.manager.arenaSitting = null;
		self.manager.rainWorld.progression.currentSaveState = null;
		self.manager.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat = storyGameCharacter;
		if (!self.restartChecked && self.manager.rainWorld.progression.IsThereASavedGame(storyGameCharacter))
		{
			if (storyGameCharacter == 2 && self.redIsDead)
			{
				self.redSaveState = self.manager.rainWorld.progression.GetOrInitiateSaveState(2, null, self.manager.menuSetup, false);
				self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Statistics);
				self.PlaySound(SoundID.MENU_Switch_Page_Out);
			}
			else
			{
				self.manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.Load;
				self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);
				self.PlaySound(SoundID.MENU_Continue_Game);
			}
		}
		else
		{
			self.manager.rainWorld.progression.WipeSaveState(storyGameCharacter);
			self.manager.menuSetup.startGameCondition = ProcessManager.MenuSetup.StoryGameInitCondition.New;
			self.manager.RequestMainProcessSwitch(ProcessManager.ProcessID.Game);
			self.PlaySound(SoundID.MENU_Start_New_Game);
		}
		if (self.manager.musicPlayer != null && self.manager.musicPlayer.song != null && self.manager.musicPlayer.song is Music.IntroRollMusic)
		{
			self.manager.musicPlayer.song.FadeOut(20f);
		}
	}
}