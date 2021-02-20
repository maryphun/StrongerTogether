using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(Animator))]
public class TutorialArrow : MonoBehaviour
{
    public void SelfDestroy(float time)
    {
        GetComponent<SpriteRenderer>().DOFade(0, time);
        Destroy(gameObject, time);
    }

    public void StartAnimation()
    {
        GetComponent<Animator>().SetTrigger("Start");
    }
}
