

public interface PhaseBase {

    void Start();
    void Stop();

    /// <summary>
    /// Determines whether the current phase is allowed to kill the player. This can allow the player a bit of clearance to "interact" with Teardrop or to make a certain phase more violent. This is polled every time the player enters the kill box, so it can be modulated throughout the runtime of the phase
    /// </summary>
    /// <returns> True if we are allowed to kill the player; False if not</returns>
    bool CanKillPlayer();


}