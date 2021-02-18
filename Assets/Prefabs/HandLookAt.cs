using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLookAt : MonoBehaviour
{
    [SerializeField] private float offset = 215;
    [SerializeField] private float horizontalMoveMinMax = 1f;
    [SerializeField] private Vector2 detectionMargin = new Vector2(1920f, 1080f);   // only detect certain part of the screen, ignore the edge

    private Camera cam;
    private Rigidbody2D rigidbody;
    private float originalPosition;

    void Awake()
    {
        cam = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
        originalPosition = transform.position.x;
    }
    
    void Update()
    {
        //  Get mouse position, restrict the number so it won't detect mouse cursor out of screen
        Vector2 mouseInput = new Vector2(Mathf.Max(Mathf.Min(Input.mousePosition.x, Screen.width/2f + (detectionMargin.x / 2f)), Screen.width/2f - (detectionMargin.x / 2f)), 
            Mathf.Max(Mathf.Min(Input.mousePosition.y, Screen.height/2f + (detectionMargin.y / 2f)), Screen.height/2f - (detectionMargin.y / 2f)));

        // Distance from camera to object.  We need this to get the proper calculation.
        float camDis = cam.transform.position.y - transform.position.y;

        // Get the mouse position in world space. Using camDis for the Z axis.
        Vector3 mouse = cam.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, camDis));

        UpdateAngle(mouse); // make hand point to mouse
        UpdatePositionX(mouseInput.x);  // make hand move horizontally toward mouse
    }

    private void UpdateAngle(Vector3 target)
    {
        // calculation
        float AngleRad = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x);
        float angle = (180f / Mathf.PI) * AngleRad + offset;
        //Debug.Log(gameObject.name + "  =  " + angle);
        // apply
        rigidbody.rotation = angle;
    }

    private void UpdatePositionX(float target)
    {
        // calculation
        float movePercentage = (target - Screen.width / 2f) / Screen.width / 2f;
        float newPositionX = originalPosition + (horizontalMoveMinMax * movePercentage);

        // apply
        transform.position = new Vector2(newPositionX, transform.position.y);
    }
}
