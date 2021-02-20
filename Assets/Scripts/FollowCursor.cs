using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(SpriteRenderer))]
public class FollowCursor : MonoBehaviour
{
    Camera cam;
    Light2D light;
    SpriteRenderer renderer;

    private void Awake()
    {
        cam = Camera.main;
        renderer = GetComponent<SpriteRenderer>();
        light = GetComponentInChildren<Light2D>();
        enabled = false;
    }

    public void SetEnable(bool boolean)
    {
        Cursor.visible = !boolean;
        enabled = boolean;

        int fade = boolean ? 1 : 0;
        renderer.DOFade(fade, 1f);

        StartCoroutine(LightSwitch(fade, 1f, light));
    }

    private void Update()
    {
        Vector3 mouseInput = Input.mousePosition;

        // Distance from camera to object.  We need this to get the proper calculation.
        float camDis = cam.transform.position.y - transform.position.y;

        // Get the mouse position in world space. Using camDis for the Z axis.
        Vector3 mouse = cam.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, camDis));

        transform.position = new Vector3(mouse.x, mouse.y, 0);
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
            Debug.Log(lerp);
            yield return null;
        }

        if (endValue == 0f)
        {
            Destroy(target.gameObject);
        }
    }
}
