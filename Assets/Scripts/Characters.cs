using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public class Characters : MonoBehaviour
{
    [System.Serializable]
    private struct CHARACTER
    {
        public Transform transform;
        private SpriteRenderer sprite;
        public float score;
        private bool isHiding;
        private Light2D light;

        public SpriteRenderer Sprite
        {
            get
            {
                return this.sprite;
            }
            set
            {
                this.sprite = value;
            }
        }

        public bool IsHiding
        {
            get
            {
                return this.isHiding;
            }
            set
            {
                this.isHiding = value;
            }
        }

        public Light2D Light
        {
            get
            {
                return this.light;
            }
            set
            {
                this.light = value;
            }
        }
    }

    [SerializeField, Range(0f, 0.5f)] private float fadeTime = 4f;
    [SerializeField] private CHARACTER[] characters;

    private void Awake()
    {
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].Sprite = characters[i].transform.GetComponent<SpriteRenderer>();
            characters[i].Sprite.color = new Color(1, 1, 1, 0);
            characters[i].Light = characters[i].transform.GetComponentInChildren<Light2D>();
            characters[i].Light.pointLightOuterRadius = 0.0f;
            characters[i].IsHiding = true;
        }
    }

    public void UpdateScore(float newScore)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            if (characters[i].score <= newScore)
            {
                if (characters[i].IsHiding)
                {
                    // show character
                    characters[i].IsHiding = false;
                    characters[i].Sprite.DOFade(1.0f, fadeTime);
                    StartCoroutine(LightSwitch(1.0f, fadeTime, characters[i].Light));
                }
            }
            else
            {
                if (!characters[i].IsHiding)
                {
                    // hide character
                    characters[i].IsHiding = true;
                    characters[i].Sprite.DOFade(0.0f, fadeTime);
                    StartCoroutine(LightSwitch(0.0f, fadeTime, characters[i].Light));
                }
            }
        }
    }

    IEnumerator LightSwitch(float endValue, float time, Light2D target)
    {
        float startValue = target.pointLightOuterRadius;
        float lerp = 0.0f;
        float timeElapsed = 0.0f;

        while (startValue != endValue)
        {
            timeElapsed += Time.deltaTime;
            lerp = timeElapsed / time;
            target.pointLightOuterRadius = Mathf.Lerp(startValue, endValue, lerp);
            yield return null;
        }
    }
}