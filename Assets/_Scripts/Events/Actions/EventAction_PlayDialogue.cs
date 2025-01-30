using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAction_PlayDialogue : EventAction
{
    public AudioClip clip;

    public override void Execute()
    {
        PlayerManager.instance.voice.PlayDialogue(clip);
    }
}