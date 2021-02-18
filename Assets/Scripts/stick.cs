using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class stick : MonoBehaviour
{
    [SerializeField, Header("Reference")] private Controller controller;

    [SerializeField, Range(0f, 10f)] private float animationTime = 4.5f;

    private void OnMouseDown()
    {
        AudioManager.Instance.PlaySFX("Click", 0.3f);
        Destroy(GetComponent<Collider2D>());

        transform.DOMoveY(transform.position.y - 13f, animationTime);
        transform.DOScale(5f, animationTime);

        StartCoroutine(StartGameWithDelay(animationTime));
    }

    IEnumerator StartGameWithDelay(float time)
    {
        yield return new WaitForSeconds(time);

        controller.StartGame();
        Destroy(gameObject);
    }
}
