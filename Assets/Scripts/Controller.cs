using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controller : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string soundEffectCircleClick = default;
    [SerializeField] private string soundEffectCircleMiss = default;
    [SerializeField] private float circleMissTiming = 1.0f;
    [SerializeField, Range(0.0f, 0.6f)] private float patternFadeTime = 0.3f;
    [SerializeField, Range(0.0f, 5.0f)] private float patternSpawnInterval = 2f;

    [Header("Score")]
    [SerializeField, Range(0.0f, 1.0f)] private float scoreAdditionForCircleClick = 0.25f;
    [SerializeField, Range(0.0f, 1.0f)] private float scoreDeductionForCircleClick = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] private float scoreMultiplierForCombo = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float scoreAdditionForPatternEnd = 0.0f;

    [Header("References")]
    [SerializeField] private FMODAudioManager audio;
    [SerializeField] private GameObject[] patterns;

    private pattern currentPattern;
    private int combo;
    private int lastPattternIndex;

    void Start()
    {
        // Reset random seed to make sure system get different random number each play
        Random.seed = System.DateTime.Now.Millisecond;

        // Variable Initialization
        combo = 0;
        currentPattern = null;

        // Instantiate First Pattern (avoid the last pattern for first spawn, if there are two pattern in this array, the first one always spawn first)
        StartCoroutine(SpawnPattern(patternSpawnInterval, patterns.Length-1));
    }

    public void CircleClicked(bool patternEnd)
    {
        AudioManager.Instance.PlaySFX(soundEffectCircleClick);
        audio.ChangeScore(scoreAdditionForCircleClick);

        if (patternEnd) PatternEnd();
    }

    public void CircleMissed(bool patternEnd)
    {
        AudioManager.Instance.PlaySFX(soundEffectCircleMiss);
        audio.ChangeScore(-scoreDeductionForCircleClick);

        if (patternEnd) PatternEnd();
    }

    private void PatternEnd()
    {
        // Destroy Object
        currentPattern.enabled = false;
        currentPattern.Animator.GetComponent<SpriteRenderer>().DOFade(0f, patternFadeTime);
        Destroy(currentPattern.gameObject, patternFadeTime);

        // Instantiate Next Pattern
        StartCoroutine(SpawnPattern(patternSpawnInterval, lastPattternIndex));
    }

    IEnumerator SpawnPattern(float interval, int avoidIndex)
    {
        yield return new WaitForSeconds(interval);
        // Random Numbers with an exception to avoid repeatation
        List<int> randomNumbers = new List<int>();
        for (int i = 0; i < patterns.Length; i++)
        {
            if (i != avoidIndex) randomNumbers.Add(i);
        }
        int random = randomNumbers[Random.Range(0, randomNumbers.Count)];

        lastPattternIndex = random; // save index to avoid repeatation later

        // Instantiation
        currentPattern = Instantiate(patterns[random]).GetComponent<pattern>();
        currentPattern.Activate(this, circleMissTiming);
    }
}
