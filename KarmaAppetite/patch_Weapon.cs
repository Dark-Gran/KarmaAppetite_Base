using System;

public class patch_Weapon
{
	public static void Patch()
	{
		On.Weapon.Grabbed += Weapon_Grabbed;
	}

	//PULL-OUT PRICE

	private static void Weapon_Grabbed(On.Weapon.orig_Grabbed orig, Weapon self, Creature.Grasp grasp)
	{
		if (self.mode == Weapon.Mode.StuckInWall && grasp.grabber is Player)
		{
			if (((Player)grasp.grabber).Karma < KarmaAppetite_Base.STARTING_MAX_KARMA)
			{
				KarmaAppetite_Base.RemoveQuarterFood((Player)grasp.grabber);
			}
		}
		orig.Invoke(self, grasp);
	}

}