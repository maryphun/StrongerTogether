using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Controller : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string mainMenuBGM = "ambience";
    [SerializeField] private string soundEffectCircleMiss = default;
    [SerializeField] private float circleMissTiming = 1.0f;
    [SerializeField, Range(0.0f, 0.6f)] private float patternFadeTime = 0.3f;
    [SerializeField, Range(0.0f, 5.0f)] private float patternSpawnInterval = 2f;
    [SerializeField, Range(0.01f, 0.99f)] private float victoryScore = 0.8f; 

    [Header("Score")]
    [SerializeField, Range(0.0f, 1.0f)] private float scoreAdditionForCircleClick = 0.25f;
    [SerializeField, Range(0.0f, 1.0f)] private float scoreDeductionForCircleClick = 0.1f;
    //[SerializeField, Range(0.0f, 1.0f)] private float scoreMultiplierForCombo = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float scoreAdditionForPatternEnd = 0.0f;

    [Header("References")]
    [SerializeField] private FMODAudioManager audio;
    [SerializeField, Range(0.0f, 0.0f)] private float stage1Score = 0.0f;   // stage1 must always be 0.0f
    [SerializeField] private GameObject[] patternsStage1;
    [SerializeField, Range(0.0f, 1.0f)] private float stage2Score = 0.2f;
    [SerializeField] private GameObject[] patternsStage2;
    [SerializeField, Range(0.0f, 1.0f)] private float stage3Score = 0.4f;
    [SerializeField] private GameObject[] patternsStage3;
    [SerializeField, Range(0.0f, 1.0f)] private float stage4Score = 0.6f;
    [SerializeField] private GameObject[] patternsStage4;
    [SerializeField] private GameObject finalPattern = default;
    [SerializeField] private Characters characterScript;
    [SerializeField] private GameObject hands;
    [SerializeField] private FollowCursor customCursor;
    [SerializeField] private GameObject tutorialPattern, tutorialArrow;

    [Header("Debug")]
    [SerializeField, Range(0, 1)] private float currentScore = 0f;

    private pattern currentPattern;
    private int combo;
    private int lastPattternIndex;
    private bool circleMissed;
    private bool inTutorial;
    private TutorialArrow spawnedTutorialArrow;
    private bool isFinale;

    void Awake()
    {
        // reference null check
        if (Object.ReferenceEquals(audio, null))
        {
            Debug.LogWarning("GameObject: [" + gameObject.name + "] missing reference of audio manager");
        }
        if (Object.ReferenceEquals(characterScript, null))
        {
            Debug.LogWarning("GameObject: [" + gameObject.name + "] missing reference of characters");
        }
        if (Object.ReferenceEquals(hands, null))
        {
            Debug.LogWarning("GameObject: [" + gameObject.name + "] missing reference of hands");
        }

        // Reset random seed to make sure system get different random number each play
        Random.seed = System.DateTime.Now.Millisecond;

        // Disable hands visual until game actually start
        hands.SetActive(false);
    }

    private void Start()
    {
        // use custom cursor
        customCursor.SetEnable(true);

        AudioManager.Instance.PlayMusicWithFade(mainMenuBGM, 2f);
        AudioManager.Instance.SetMusicVolume(0.2f);
    }

    public void StartGame()
    {
        // Variable Initialization
        combo = 0;
        currentPattern = null;

        // Instantiate Tutrial Pattern
        StartCoroutine(SpawnTutorial(patternSpawnInterval));

        // show hands
        hands.SetActive(true);
    }

    public void CircleClicked(bool patternEnd)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/NoteSFX");
        if (!isFinale)
        {
            currentScore = audio.ChangeScore(scoreAdditionForCircleClick, victoryScore);
            characterScript.UpdateScore(currentScore);
        }

        if (patternEnd) PatternEnd();
    }

    public void CircleMissed(bool patternEnd)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/MissSFX");
        currentScore = audio.ChangeScore(-scoreDeductionForCircleClick);
        characterScript.UpdateScore(currentScore);
        circleMissed = true;

        if (patternEnd) PatternEnd();
    }

    private void PatternEnd()
    {
        if (!circleMissed)
        {
            // never miss a circle in this pattern
            currentScore = audio.ChangeScore(scoreAdditionForPatternEnd, victoryScore);
            FMODUnity.RuntimeManager.PlayOneShot("event:/FinishPatternSFX");

            // get out of tutorial mode if it's in
            if (inTutorial)
            {
                inTutorial = false;
                spawnedTutorialArrow.SelfDestroy(patternSpawnInterval / 2f);

                // Reset score
                currentScore = audio.SetScore(0);

                // BGM
                audio.StartBGM();
            }
        }

        // Destroy Object
        currentPattern.enabled = false;
        currentPattern.Animator.GetComponent<SpriteRenderer>().DOFade(0f, patternFadeTime);
        Destroy(currentPattern.gameObject, patternFadeTime);

        // Determine if it's the finale
        if (isFinale)
        {
            if (!circleMissed)
            {
                EndGame();
                // this game end here. not spawning the next pattern
                return;
            }
            else
            {
                isFinale = false;
            }
        }

        // Instantiate Next Pattern
        if (!inTutorial)
        {
            StartCoroutine(SpawnPattern(patternSpawnInterval, lastPattternIndex));
        }
        else
        {
            spawnedTutorialArrow.SelfDestroy(patternSpawnInterval / 2f);
            StartCoroutine(SpawnTutorial(patternSpawnInterval * 1.5f));
        }
    }

    IEnumerator SpawnTutorial(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reset variable
        circleMissed = false;
        inTutorial = true;

        // Instantiation
        currentPattern = Instantiate(tutorialPattern).GetComponent<pattern>();
        currentPattern.Activate(this, circleMissTiming, inTutorial);
        spawnedTutorialArrow = Instantiate(tutorialArrow).GetComponent<TutorialArrow>();
    }

    IEnumerator SpawnPattern(float interval, int avoidIndex)
    {
        yield return new WaitForSeconds(interval);

        // Reset variable
        circleMissed = false;
        inTutorial = false;

        // Check what stage currently in and create a list of available patterns
        List<GameObject> patterns = new List<GameObject>();
        if (currentScore >= stage1Score) for (int i = 0; i < patternsStage1.Length; i++) patterns.Add(patternsStage1[i]);
        if (currentScore >= stage2Score) for (int i = 0; i < patternsStage2.Length; i++) patterns.Add(patternsStage2[i]);
        if (currentScore >= stage3Score) for (int i = 0; i < patternsStage3.Length; i++) patterns.Add(patternsStage3[i]);
        if (currentScore >= stage4Score) for (int i = 0; i < patternsStage4.Length; i++) patterns.Add(patternsStage4[i]);
        if (currentScore >= victoryScore)
        {
            // final pattern
            patterns.Clear();
            patterns.Add(finalPattern);
            isFinale = true;
        }

        // Random Numbers with an exception to avoid repeatation
        List<int> randomNumbers = new List<int>();
        for (int i = 0; i < patterns.Count; i++)
        {
            if (i != avoidIndex) randomNumbers.Add(i);
        }

        int random = randomNumbers[Random.Range(0, randomNumbers.Count)];

        lastPattternIndex = random; // save index to avoid repeatation later

        // Instantiation
        currentPattern = Instantiate(patterns[random]).GetComponent<pattern>();
        currentPattern.Activate(this, circleMissTiming, inTutorial);
    }

    private void EndGame()
    {
        Debug.Log("end game");
        StartCoroutine(IncreaseScoreOverTime(10f, 1.0f));
    }

    IEnumerator IncreaseScoreOverTime(float time, float targetScore)
    {
        float elapsedTime = 0.0f;
        for (elapsedTime = 0; elapsedTime < time; elapsedTime += Time.fixedDeltaTime)
        {
            float lerp = elapsedTime / time;

            currentScore = audio.SetScore(Mathf.Lerp(victoryScore, targetScore, lerp));

            yield return new WaitForEndOfFrame();
        }
    }
}
