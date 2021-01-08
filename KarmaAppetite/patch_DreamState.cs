using System;

public class patch_DreamState
{
    public static void Patch()
    {
        On.DreamsState.EndOfCycleProgress += DreamsState_EndOfCycleProgress;
    }

    private static void DreamsState_EndOfCycleProgress(On.DreamsState.orig_EndOfCycleProgress orig, DreamsState self, SaveState saveState, string currentRegion, string denPosition)
    {
        bool everSleptInSB = true;
        bool everSleptInSB_S = true;
        DreamsState.StaticEndOfCycleProgress(saveState, currentRegion, denPosition, ref self.integers[0], ref self.integers[1], ref self.integers[2], ref self.integers[5], ref self.upcomingDream, ref self.eventDream, ref everSleptInSB, ref everSleptInSB_S, ref self.guideHasShownHimselfToPlayer, ref self.integers[4], ref self.guideHasShownMoonThisRound, ref self.integers[3]);
        self.everSleptInSB = everSleptInSB;
        self.everSleptInSB_S01 = everSleptInSB_S;
    }
}