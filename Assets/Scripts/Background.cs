using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class Background : MonoBehaviour
{
    [SerializeField] private SpriteRenderer skyblue;
    [SerializeField] private SpriteRenderer mountain;
    [SerializeField] private SpriteRenderer ocean;
    [SerializeField] private SpriteRenderer sun;
    [SerializeField] private Light2D light;
    [SerializeField] private Light2D lightAll;

    public void BackgroundAnimations()
    {
        StartCoroutine(Event());
    }

    IEnumerator Event()
    {
        skyblue.DOFade(1.0f, 4f);
        mountain.transform.DOMoveY(-2.35f, 5f);
        mountain.DOFade(1.0f, 5.5f);
        mountain.DOFade(1.0f, 5f);

        light.gameObject.SetActive(true);
        lightAll.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        ocean.transform.DOMoveY(-0.8f, 5f);

        yield return new WaitForSeconds(4f);

        sun.transform.DOMoveY(1.65f, 5f);

        yield return new WaitForSeconds(6.5f);

        light.gameObject.SetActive(false);
        lightAll.gameObject.SetActive(true);

        StartCoroutine(SunLight(5f, 45f, 1f, Color.white, 6f, lightAll));
    }

    IEnumerator SunLight(float intensity, float radius, float lightvolumeopacity, Color color, float time, Light2D target)
    {
        float startValueIntensity = target.intensity;
        float startValueRadius = target.pointLightOuterRadius;
        float startValueOpacity = target.volumeOpacity;
        Color originalColor = target.color;
        float lerp = 0.0f;
        float timeElapsed = 0.0f;

        for (timeElapsed = 0; timeElapsed < time; timeElapsed += Time.deltaTime)
        {
            lerp = timeElapsed / time;
            target.intensity = Mathf.Lerp(startValueIntensity, intensity, lerp);
            target.pointLightOuterRadius = Mathf.Lerp(startValueRadius, radius, lerp);
            //target.volumeOpacity = Mathf.Lerp(startValueOpacity, lightvolumeopacity, lerp);
            target.color = Color.Lerp(originalColor, color, lerp);

            Light2D tmp = target;

            yield return null;
        }
    }
}
