using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CircleFail : MonoBehaviour
{
    SpriteRenderer renderer;
    float interval = 0.1f;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        StartCoroutine(UpdateAlpha(interval));
    }

    IEnumerator UpdateAlpha(float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject == null)
        {
            // this object isn't exist anymore
            yield break;
        }
        else
        {
            int newAlpha = renderer.color.a != 0 ? 0 : 1;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, newAlpha);

            StartCoroutine(UpdateAlpha(interval));
        }
    }

    public void SetInterval(float newValue)
    {
        interval = newValue;
    }
}
