using System;
using Menu;
using RWCustom;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class patch_Menu //Sleep Illustrations
{
    public static void Patch()
    {
        On.Menu.MenuScene.BuildScene += MenuScene_BuildScene;
        On.Menu.MenuIllustration.LoadFile_1 += MenuIllustration_LoadFile_1;
    }

    private static void MenuScene_BuildScene(On.Menu.MenuScene.orig_BuildScene orig, Menu.MenuScene self)
    {
        if (self.sceneID == Menu.MenuScene.SceneID.SleepScreen)
        {
            BuildCustomSleepScene(self);
        }
        else
        {
            orig.Invoke(self);
        }
    }

	private static void MenuIllustration_LoadFile_1(On.Menu.MenuIllustration.orig_LoadFile_1 orig, MenuIllustration self, string folder) 
	{
		if (folder.Length >= "KarmaAppetite".Length && folder.Substring(0, "KarmaAppetite".Length) == "KarmaAppetite")
		{			
			if ((self.fileName.Length >= "Sleep - 2".Length && self.fileName.Substring(0, "Sleep - 2".Length) == "Sleep - 2") || (self.fileName.Length >= "Sleep Screen".Length && self.fileName.Substring(0, "Sleep Screen".Length) == "Sleep Screen"))

			{
				CustomIllustLoad(self, folder.Remove(0, "KarmaAppetite".Length));
			}
			else
            {
				orig.Invoke(self, folder.Remove(0, "KarmaAppetite".Length));
			}
		}
		else
		{
			orig.Invoke(self, folder);
		}
	}

	private static void CustomIllustLoad(MenuIllustration self, string folder)
	{
		//Resource

		Assembly assembly = Assembly.GetExecutingAssembly();

		string name = assembly.GetManifestResourceNames().Single(str => str.EndsWith(self.fileName+".txt"));
		Debug.Log("LoadingCustomIllustration: " + name);

		string s;
		using (Stream manifestResourceStream = assembly.GetManifestResourceStream(name))
		{
			using (StreamReader streamReader = new StreamReader(manifestResourceStream))
			{
				s = streamReader.ReadToEnd();
			}
		}
		byte[] data = Convert.FromBase64String(s);

		string text = Custom.RootFolderDirectory() + self.fileName + ".txt";
		File.WriteAllBytes(text, data);
		self.www = new WWW("file:///" + text);
		
		//Apply

		self.texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
		self.texture.wrapMode = TextureWrapMode.Clamp;
		if (self.crispPixels)
		{
			self.texture.anisoLevel = 0;
			self.texture.filterMode = FilterMode.Point;
		}
		self.www.LoadImageIntoTexture(self.texture);
		HeavyTexturesCache.LoadAndCacheAtlasFromTexture(self.fileName, self.texture);
		self.www = null;
		File.Delete(text);
	}

	private static void BuildCustomSleepScene(Menu.MenuScene self)
    {
		if (self is InteractiveMenuScene)
		{
			(self as InteractiveMenuScene).idleDepths = new List<float>();
		}
		Vector2 vector = new Vector2(0f, 0f);
		int num;
		if (self.menu.manager.currentMainLoop is RainWorldGame)
		{
			num = (self.menu.manager.currentMainLoop as RainWorldGame).StoryCharacter;
		}
		else
		{
			num = self.menu.manager.rainWorld.progression.PlayingAsSlugcat;
		}
		string text = "White";
		if (num != 0)
		{
			text = ((num != 1) ? "Red" : "Yellow");
		}
		self.sceneFolder = string.Concat(new object[]
		{
			"KarmaAppetiteScenes",
			Path.DirectorySeparatorChar,
			"Sleep Screen - ",
			text
		});
		if (self.flatMode)
		{
			self.AddIllustration(new MenuIllustration(self.menu, self, self.sceneFolder, "Sleep Screen - " + text + " - Flat", new Vector2(683f, 384f), false, true));
		}
		else
		{
			self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Sleep - 5", new Vector2(23f, 17f), 3.5f, MenuDepthIllustration.MenuShader.Normal));
			self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Sleep - 4", new Vector2(23f, 17f), 2.8f, MenuDepthIllustration.MenuShader.Normal));
			self.depthIllustrations[self.depthIllustrations.Count - 1].setAlpha = new float?(0.24f);
			self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Sleep - 3", new Vector2(23f, 17f), 2.2f, MenuDepthIllustration.MenuShader.Normal));
			self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Sleep - 2 - " + text, new Vector2(23f, 17f), 1.7f, MenuDepthIllustration.MenuShader.Normal));
			self.AddIllustration(new MenuDepthIllustration(self.menu, self, self.sceneFolder, "Sleep - 1", new Vector2(23f, 17f), 1.2f, MenuDepthIllustration.MenuShader.Normal));
			(self as InteractiveMenuScene).idleDepths.Add(3.3f);
			(self as InteractiveMenuScene).idleDepths.Add(2.7f);
			(self as InteractiveMenuScene).idleDepths.Add(1.8f);
			(self as InteractiveMenuScene).idleDepths.Add(1.7f);
			(self as InteractiveMenuScene).idleDepths.Add(1.6f);
			(self as InteractiveMenuScene).idleDepths.Add(1.2f);
		}

		if (self.sceneFolder.Length >= "KarmaAppetite".Length && self.sceneFolder.Substring(0, "KarmaAppetite".Length) == "KarmaAppetite")
		{
			self.sceneFolder = self.sceneFolder.Remove(0, "KarmaAppetite".Length);
		}

		if (self.sceneFolder != string.Empty && File.Exists(string.Concat(new object[]
			{
				Custom.RootFolderDirectory(),
				Path.DirectorySeparatorChar,
				"Assets",
				Path.DirectorySeparatorChar,
				"Futile",
				Path.DirectorySeparatorChar,
				"Resources",
				Path.DirectorySeparatorChar,
				self.sceneFolder,
				Path.DirectorySeparatorChar,
				"positions.txt"
			})))
		{
			string[] array = File.ReadAllLines(string.Concat(new object[]
			{
				Custom.RootFolderDirectory(),
				Path.DirectorySeparatorChar,
				"Assets",
				Path.DirectorySeparatorChar,
				"Futile",
				Path.DirectorySeparatorChar,
				"Resources",
				Path.DirectorySeparatorChar,
				self.sceneFolder,
				Path.DirectorySeparatorChar,
				"positions.txt"
			}));
			int num2 = 0;
			while (num2 < array.Length && num2 < self.depthIllustrations.Count)
			{
				self.depthIllustrations[num2].pos.x = float.Parse(Regex.Split(array[num2], ", ")[0]) + vector.x;
				self.depthIllustrations[num2].pos.y = float.Parse(Regex.Split(array[num2], ", ")[1]) + vector.y;
				self.depthIllustrations[num2].lastPos = self.depthIllustrations[num2].pos;
				num2++;
			}
		}
	}

}