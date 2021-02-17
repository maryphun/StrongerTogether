using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLookAt : MonoBehaviour
{
    public Camera cam;
    Transform my;
    public Rigidbody2D body;
    public float offset = 215;


    void Awake()
    {
        cam = Camera.main;
        my = GetComponent<Transform>();
        body = GetComponent<Rigidbody2D>();
    }


    void Update()
    {
        // Distance from camera to object.  We need this to get the proper calculation.
        float camDis = cam.transform.position.y - my.position.y;

        // Get the mouse position in world space. Using camDis for the Z axis.
        Vector3 mouse = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, camDis));

        float AngleRad = Mathf.Atan2(mouse.y - my.position.y, mouse.x - my.position.x);
        float angle = (180 / Mathf.PI) * AngleRad + offset;

        body.rotation = angle;
    }
}
