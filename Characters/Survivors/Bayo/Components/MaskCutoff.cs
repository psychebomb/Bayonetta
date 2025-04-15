using UnityEngine;

public class MaskCutoff : MonoBehaviour
{
    public float targetAlpha = 0.1f; // 0 for fully transparent, 1 for fully opaque
    public float startTimePercent = 0.6f;
    private float currentAlpha = 1f;
    private float fadeDur = 0f;
    private float startTime = 0f;
    private SpriteMask spriteMaskRenderer;
    private ParticleSystem particleSystem;

    void Start()
    {
        spriteMaskRenderer = GetComponent<SpriteMask>();
        spriteMaskRenderer.alphaCutoff = currentAlpha;
        particleSystem = GetComponent<ParticleSystem>();
        startTime = particleSystem.main.startLifetime.constant * startTimePercent;
        fadeDur = particleSystem.main.startLifetime.constant - startTime;
        //fadeSpeed = startTime;
    }

    void Update()
    {
        if (Time.deltaTime <= startTime)
        {
            spriteMaskRenderer.enabled = false;
        }
        else
        {
            spriteMaskRenderer.enabled = true;
            currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, (Time.deltaTime - startTime) / fadeDur);
            spriteMaskRenderer.alphaCutoff = currentAlpha;
        }
    }
}
