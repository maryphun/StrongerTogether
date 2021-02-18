using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class FollowCursor : MonoBehaviour
{
    Camera cam;
    SpriteRenderer renderer;

    private void Awake()
    {
        cam = Camera.main;
        renderer = GetComponent<SpriteRenderer>();
        enabled = false;
    }

    public void SetEnable(bool boolean)
    {
        Cursor.visible = !boolean;
        enabled = boolean;

        int fade = boolean ? 1 : 0;
        renderer.DOFade(fade, 1f);
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
}
