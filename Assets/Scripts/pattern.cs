using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class pattern : MonoBehaviour
{
    [System.Serializable]
    private struct CIRCLES
    {
        public SpriteRenderer sprite;
        public float timing;
    }

    [SerializeField] private Color circleColor = default;
    [SerializeField] private CIRCLES[] circles;
    [SerializeField] private Animator animator;
    
    private int currentActiveIndex;
    private Controller controller;
    private float missDelayTime;

    private void Awake()
    {
        foreach (CIRCLES circle in circles)
        {
            circle.sprite.color = new Color(0, 0, 0, 0);
        }
    }

    public void Activate(Controller referencePass, float missTimer)
    {
        currentActiveIndex = 0;
        controller = referencePass;
        missDelayTime = missTimer;
        animator.SetTrigger("Start");
        StartCoroutine(StartPattern());
    }

    IEnumerator StartPattern()
    {
        int currentCount = 0;
        float elapsedTime = 0.0f;
        while (currentCount < circles.Length)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= circles[currentCount].timing)
            {
                circles[currentCount].sprite.DOColor(circleColor, 0.1f);
                StartCoroutine(Circle(missDelayTime, currentCount));
                currentCount++;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Circle(float time, int index)
    {
        yield return new WaitForSeconds(time);

        // expired
        if (currentActiveIndex <= index)
        {
            circles[index].sprite.color = circleColor;
            circles[index].sprite.DOFade(0f, 0.25f);
            Destroy(circles[index].sprite.gameObject, 0.3f);
            currentActiveIndex++;

            controller.CircleMissed();
        }
    }

    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        if (((Input.GetMouseButton(0) && currentActiveIndex > 0)
            || (Input.GetMouseButtonDown(0) && currentActiveIndex == 0))
            && Vector2.Distance(mousePos, circles[currentActiveIndex].sprite.transform.position) < 0.75f)
        {
            circles[currentActiveIndex].sprite.color = circleColor;
            circles[currentActiveIndex].sprite.DOFade(0f, 0.25f);
            circles[currentActiveIndex].sprite.transform.DOScale(2f, 0.25f);
            Destroy(circles[currentActiveIndex].sprite.gameObject, 0.3f);
            currentActiveIndex++;

            controller.CircleClicked();

            if (currentActiveIndex == circles.Length)
            {
                EndPattern();
                enabled = false;
            }
        }
    }

    private void EndPattern()
    {
        Destroy(gameObject, 0.5f);
    }
}
