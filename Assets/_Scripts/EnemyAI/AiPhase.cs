public enum AiPhase
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Dead
}

public static class AiPhaseExtensions
{
    public static AiState AsState(this AiPhase phase, float nextCheckTime)
    {
        return new AiState(phase, nextCheckTime);
    }
}