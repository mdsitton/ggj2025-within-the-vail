using System.Collections;
using UnityEngine;

public class EventCondition_GunEvent : EventCondition
{
    public Gun gun;
    public GunEvent gunEvent;

    void Start()
    {
        switch (gunEvent)
        {
            case GunEvent.Fire:
                gun.onFire.AddListener(Complete);
                StartCoroutine(DelayedRelease());
                break;
            case GunEvent.HitWall:
                gun.onHitWall.AddListener(Complete);
                StartCoroutine(DelayedRelease());
                break;
            case GunEvent.HitEnemy:
                gun.onHitEnemy.AddListener(Complete);
                StartCoroutine(DelayedRelease());
                break;
            case GunEvent.DryFire:
                gun.onDryFire.AddListener(Complete);
                StartCoroutine(DelayedRelease());
                break;
            case GunEvent.Reload:
                gun.onReload.AddListener(Complete);
                StartCoroutine(DelayedRelease());
                break;
            case GunEvent.EndReload:
                gun.onEndReload.AddListener(Complete);
                StartCoroutine(DelayedRelease());
                break;
        }
    }

    public IEnumerator DelayedRelease()
    {
        yield return new WaitForEndOfFrame();
        Release();
    }

    public enum GunEvent
    {
        Fire,
        HitWall,
        HitEnemy,
        DryFire,
        Reload,
        EndReload
    }
}