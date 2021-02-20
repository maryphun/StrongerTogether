using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class stick : MonoBehaviour
{
    [SerializeField, Header("Reference")] private Controller controller;

    [SerializeField, Range(0f, 10f)] private float animationTime = 4.5f;

    private void OnMouseDown()
    {
        AudioManager.Instance.StopMusicWithFade(2f);
        AudioManager.Instance.PlaySFX("Click", 0.3f);
        Destroy(GetComponent<Collider2D>());

        transform.DOMoveY(transform.position.y - 20f, animationTime);
        transform.DOScale(5f, animationTime);

        var light = transform.GetChild(0);
        light.SetParent(null);
        StartCoroutine(LightSwitch(0.0f, animationTime/2f, light.GetComponent<Light2D>()));
        Destroy(light.gameObject, animationTime / 2f);

        FindObjectOfType<FollowCursor>().SetEnable(false);

        StartCoroutine(StartGameWithDelay(animationTime));
    }

    IEnumerator StartGameWithDelay(float time)
    {
        yield return new WaitForSeconds(time);

        controller.StartGame();
        Destroy(gameObject);
    }

    IEnumerator LightSwitch(float endValue, float time, Light2D target)
    {
        float startValue = target.intensity;
        float lerp = 0.0f;
        float timeElapsed = 0.0f;

        for (timeElapsed = 0; timeElapsed < time; timeElapsed += Time.deltaTime)
        {
            lerp = timeElapsed / time;
            target.intensity = Mathf.Lerp(startValue, endValue, lerp);
            yield return null;
        }
    }
}
