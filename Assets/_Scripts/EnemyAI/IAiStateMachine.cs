/// <summary>
/// IAiStateMachine is an interface that implements state machine logic for the enemy AI.
/// This allows each type of enemy to have its own unique AI logic.
/// Which can be used to create complex enemy fighting patterns.
/// </summary>
interface IAiStateMachine
{
    public AiStateController AiController { get; set; }
    
    AiState OnIdleState(AiState previousState);
    AiState OnPatrolState(AiState previousState);
    AiState OnChaseState(AiState previousState);
    AiState OnAttackState(AiState previousState);
    AiState OnDeadState(AiState previousState);
}