using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public TMPro.TMP_Text text;
    private int frameCount = 0;
    private float deltaTime = 0.0f;
    private float updateInterval = 1.0f;
    private float timeLeft;

    void Start()
    {
        timeLeft = updateInterval;
    }

    void Update()
    {
        frameCount++;
        deltaTime += Time.deltaTime;
        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0.0f)
        {
            float fps = frameCount / deltaTime;
            text.text = $"FPS: {fps:F1}";

            frameCount = 0;
            deltaTime = 0.0f;
            timeLeft = updateInterval;
        }
    }
}