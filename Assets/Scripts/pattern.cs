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
    [SerializeField, Range(0.75f, 1.99f)] private float speedModifier = 1.0f;
    
    private int currentActiveIndex;
    private Controller controller;
    private float missDelayTime;

    public Animator Animator
    {
        get => animator;
    }

    //private void Awake()
    //{
    //    foreach (CIRCLES circle in circles)
    //    {
    //        circle.sprite.color = new Color(0, 0, 0, 0);
    //    }
    //}

    public void Activate(Controller referencePass, float missTimer)
    {
        // Variable Initialize
        currentActiveIndex = 0;
        controller = referencePass;
        missDelayTime = missTimer;
        
        // Initialization
        for (int i = 0; i < circles.Length; i++)
        {
            circles[i].sprite.color = new Color(0, 0, 0, 0);
            circles[i].timing = circles[i].timing * (2f - speedModifier);
        }

        // Start Animation
        animator.SetTrigger("Start");
        animator.speed = animator.speed * speedModifier;
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
                StartCoroutine(Circle(missDelayTime * (2f - speedModifier), currentCount));
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

            controller.CircleMissed(currentActiveIndex == circles.Length);
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
            StartCoroutine(FadeAfterDelay(0.2f, 0.1f, 0.0f, circles[currentActiveIndex].sprite));
            //circles[currentActiveIndex].sprite.DOFade(0.0f, 0.25f);
            circles[currentActiveIndex].sprite.GetComponent<Animator>().Play("circlebreak");
            Destroy(circles[currentActiveIndex].sprite.gameObject, 0.3f);
            currentActiveIndex++;

            controller.CircleClicked(currentActiveIndex == circles.Length);
        }
    }

    IEnumerator FadeAfterDelay(float delay, float fadeTime, float endValue, SpriteRenderer target)
    {
        yield return new WaitForSeconds(delay);

        target.DOFade(endValue, fadeTime);
    }
}
