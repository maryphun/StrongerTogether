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
    
    [SerializeField] private CIRCLES[] circles;
    [SerializeField] private Animator animator;
    [SerializeField, Range(0.75f, 1.99f)] private float speedModifier = 1.0f;
    [SerializeField, Range(0.5f, 1.0f)] private float hitBoxSize = 0.75f;
    
    private int currentActiveIndex;
    private Controller controller;
    private float missDelayTime;
    private bool isTutorial;

    public Animator Animator
    {
        get => animator;
    }
    
    public void Activate(Controller referencePass, float missTimer, bool isTutorial)
    {
        // Variable Initialize
        currentActiveIndex = 0;
        controller = referencePass;
        missDelayTime = missTimer;
        this.isTutorial = isTutorial;

        // Initialization
        for (int i = 0; i < circles.Length; i++)
        {
            circles[i].sprite.color = new Color(1, 1, 1, 0);
            circles[i].timing = circles[i].timing * (2f - speedModifier);
        }

        // Only start animation after first click for tutorial mode
        if (!isTutorial)
        {
            StartAnimation(0);
        }
        else
        {
            circles[0].sprite.DOColor(Color.white, 0.1f);
        }
    }

    IEnumerator StartPattern(int startFrom)
    {
        int currentCount = 0;
        float elapsedTime = 0.0f;
        while (currentCount < circles.Length)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= circles[currentCount].timing)
            {
                circles[currentCount].sprite.DOColor(Color.white, 0.1f);
                float bonusTimeMultiplier = (currentCount == 0) ? 1.25f : 1.0f;
                StartCoroutine(Circle((missDelayTime * (2f - speedModifier)) * bonusTimeMultiplier, currentCount));
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
            circles[index].sprite.color = Color.white;
            circles[currentActiveIndex].sprite.GetComponent<Animator>().Play("circlefail");
            float animationTime = circles[currentActiveIndex].sprite.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length;
            circles[index].sprite.gameObject.AddComponent<CircleFail>();    // animation effect that update its color properties
            Destroy(circles[index].sprite.gameObject, animationTime);
            currentActiveIndex++;

            controller.CircleMissed(currentActiveIndex == circles.Length);
        }
    }
    
    private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));
        if (((Input.GetMouseButton(0) && currentActiveIndex > 0)
            || (Input.GetMouseButtonDown(0) && currentActiveIndex == 0))
            && Vector2.Distance(mousePos, circles[currentActiveIndex].sprite.transform.position) < hitBoxSize)
        {
            // check for tutorial
            if (currentActiveIndex == 0 && isTutorial)
            {
                StartAnimation(1);
                FindObjectOfType<TutorialArrow>().StartAnimation();
            }

            circles[currentActiveIndex].sprite.color = Color.white;
            StartCoroutine(FadeAfterDelay(0.2f, 0.1f, 0.0f, circles[currentActiveIndex].sprite));
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

    private void StartAnimation(int skip)
    {
        animator.SetTrigger("Start");
        animator.speed = animator.speed * speedModifier;
        StartCoroutine(StartPattern(skip));
    }
}
