using UnityEngine;

[RequireComponent(typeof(AiStateController))]
public abstract class AiPhaseBehaviour : MonoBehaviour
{
    public AiPhase phase;

    protected AiStateController controller; 

    private void Awake()
    {
        controller = GetComponent<AiStateController>();
    }
    
    public abstract void OnStateEnable();
    public abstract void OnStateDisable();
    public abstract void OnStateLoop();
    public abstract void OnStateUpdate();
}