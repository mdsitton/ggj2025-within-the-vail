using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[RequireComponent(typeof(IAiStateMachine))]
public class AiStateController : MonoBehaviour
{
    private IAiStateMachine stateMachine;
    
    public AiState previousState;
    public AiState currentState;
    
    private List<List<AiPhaseBehaviour>> phaseBehaviours = new();
    
    private void Awake()
    {
        previousState = currentState = new AiState(AiPhase.Idle, 0);
        
        stateMachine = GetComponent<IAiStateMachine>();
        stateMachine.AiController = this;
        
        // ensure slots for all states exists
        foreach (var @enum in Enum.GetValues(typeof(AiState)))
        {
            phaseBehaviours.Add(new List<AiPhaseBehaviour>());
        }
        
        var components = GetComponents<AiPhaseBehaviour>();
        
        foreach (var component in components)
        {
            phaseBehaviours[(int)component.phase].Add(component);
        }
    }

    public void ChangeState(AiState newState)
    {
        if (newState.phase == currentState.phase)
        {
            // update the state so that any new schedule time is updated
            // because the state is the same and the time is being updated
            // we don't need to update the previous state value as well since
            // it is a continuation of the same state
            currentState = newState;
            foreach (var behaviour in phaseBehaviours[(int)currentState.phase])
            {
                behaviour.OnStateLoop();
            }

            return;
        }

        foreach (var behaviour in phaseBehaviours[(int)currentState.phase])
        {
            behaviour.OnStateDisable();
        }
        
        previousState = currentState;
        currentState = newState;
            
        foreach (var behaviour in phaseBehaviours[(int)currentState.phase])
        {
            behaviour.OnStateEnable();
        }
    }
    
    // TODO - Maybe implement a debug gizmo to show the current state of the AI in the scene view

    private void Update()
    {
        var currentTime = Time.timeSinceLevelLoad;
        
        // Implement check delay
        if (currentState.nextCheckTime <= currentTime)
        {
            previousState = currentState;

            var newState = currentState.phase switch
            {
                AiPhase.Idle => stateMachine.OnIdleState(previousState),
                AiPhase.Patrol => stateMachine.OnPatrolState(previousState),
                AiPhase.Chase => stateMachine.OnChaseState(previousState),
                AiPhase.Attack => stateMachine.OnAttackState(previousState),
                AiPhase.Dead => stateMachine.OnDeadState(previousState),
                _ => currentState
            };
            ChangeState(newState);
        }
        
        // Phase behaviours are MonoBehaviour scripts that are attached to the enemy AI GameObject
        // However they are not updated by Unity's MonoBehaviour system, a MonoBehaviour is only used so we can attach in the inspector.
        // Since each active enemy may have a ton of these scripts attached to them
        // and if each of these scripts were updated by Unity's MonoBehaviour system
        // it could be a performance bottleneck as unity Update() methods are fairly high overhead
        foreach (var behaviour in phaseBehaviours[(int)currentState.phase])
        {
            behaviour.OnStateUpdate();
        }
    }
    
}