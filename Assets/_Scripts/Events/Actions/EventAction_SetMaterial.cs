using UnityEngine;

public class EventAction_SetMaterial : EventAction
{
    public new Renderer renderer;
    public Material material;

    public override void Execute ()
    {
        renderer.material = material;
    }
}