using UnityEngine;

/// <summary>
/// The AiState struct is a read-only struct that represents a single phase of the enemy's AI.
/// </summary>
public readonly struct AiState
{
    /// <summary>
    /// This is the current phase the enemy is within.
    /// It will be used to determine the correct scripts to execute for any given phase. 
    /// </summary>
    public readonly AiPhase phase;
    
    /// <summary>
    /// This is the time at which the next state update check should occur.
    /// This is used to implement a delay between state changes.
    /// </summary>
    public readonly float nextCheckTime;
    
    public AiState(AiPhase phase, float nextCheckTime)
    {
        this.phase = phase;
        this.nextCheckTime = Time.timeSinceLevelLoad + nextCheckTime;
    }
}