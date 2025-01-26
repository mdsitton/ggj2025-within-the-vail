using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    public AudioSource[] audioSources;
    //public AudioClip[] clips;

    public Vector2 timeRange = new Vector2(8, 20);

    private float nextPlay = -1;

    void OnEnable () 
    {
        nextPlay = Time.time + Random.Range(timeRange.x, timeRange.y);
    }

    void Update()
    {
        if (Time.time >= nextPlay)
        {
            nextPlay = Time.time + Random.Range(timeRange.x, timeRange.y);

            /*
            AudioSource source = audioSources[Random.Range(0, audioSources.Length)];
            source.clip = clips[Random.Range(0, clips.Length)];
            source.Play();
            */

            audioSources[Random.Range(0, audioSources.Length)].Play();
        }
    }
}
