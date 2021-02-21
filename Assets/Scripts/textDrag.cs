using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class textDrag : MonoBehaviour
{
    private Camera cam;
    private TMP_Text text;

    private void Awake()
    {
        cam = Camera.main;
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(cam.transform.position.z))));

        text.color = new Color(1, 1, 1, 0.85f - (distance / 5f));
    }
}
