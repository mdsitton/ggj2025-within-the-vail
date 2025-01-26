using UnityEngine;

public class EventAction_PlayAudio : EventAction
{
    public AudioSource[] audioSource;
    public PlayMode mode = PlayMode.Play;

    public override void Execute()
    {
        if (mode == PlayMode.Play)
            foreach (AudioSource audioSource in audioSource)
                audioSource.Play();
        else if (mode == PlayMode.Stop)
            foreach (AudioSource audioSource in audioSource)
                audioSource.Stop();
        else
            Debug.LogWarning($"EventAction_PlayAudio PlayMode {mode} not supported on {name}");
    }

    public enum PlayMode
    {
        Play,
        Stop
    }
}