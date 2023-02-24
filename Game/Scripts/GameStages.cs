using System;
using queen.error;

public static class GameStages
{

    public static event Action<int> StageChanged;
    public static int Current { get; private set; } = 0;

    public static void TriggerNextStage()
    {
        Current += 1;
        Print.Debug($"Current Stage: {Current}");
        StageChanged?.Invoke(Current);
    }

    public static void Reset()
    {
        Current = 0;
        Print.Debug("Stages Reset");
        StageChanged?.Invoke(Current);
    }

#if DEBUG
    public static void TriggerPreviousStage()
    {
        Print.Warn("Setting game stage to previous stage. This should not be called in release builds!");
        Current -= 1;
        Print.Debug($"Current Stage: {Current}");
        StageChanged?.Invoke(Current);
    }

#endif

}