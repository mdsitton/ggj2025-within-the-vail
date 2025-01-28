using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    public TunnelingVignetteController controller;
    public VignetteParameters parameters = new VignetteParameters();

    public bool apply;

    public void Start()
    {
        if (controller != null)
        {
            controller.BeginTunnelingVignette(new BloodiedSettings(parameters));
        }
    }
}


[System.Serializable]
public class BloodiedSettings : ITunnelingVignetteProvider
{
    public VignetteParameters vignetteParameters { get => m_parameters; set => m_parameters = value; }

    [SerializeField]
    VignetteParameters m_parameters = new ();

    public BloodiedSettings(VignetteParameters parameters)
    {
        m_parameters = parameters;
    }
}