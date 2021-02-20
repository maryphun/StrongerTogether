using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class MoveX : MonoBehaviour
{
    [SerializeField] private float moveSpeedPerFrame = 2f;

    private SpriteRenderer renderer;
    private Camera cam;
    private float minXValueWorld, maxXValueWorld;

    private void Awake()
    {
        minXValueWorld = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)).x;
        maxXValueWorld = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;
        renderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (moveSpeedPerFrame > 0)
        {
            if (maxXValueWorld < transform.position.x - renderer.bounds.size.x/2f)
            {
                transform.position = new Vector2(minXValueWorld - renderer.bounds.size.x / 2f, transform.position.y);
                return;
            }
        }
        else if (moveSpeedPerFrame < 0)
        {
            if (transform.position.x + renderer.bounds.size.x / 2f < minXValueWorld)
            {
                transform.position = new Vector2(maxXValueWorld + renderer.bounds.size.x / 2f, transform.position.y);
                return;
            }
        }

        transform.DOMoveX(transform.position.x + moveSpeedPerFrame * Time.fixedDeltaTime, 0f);
    }
}
