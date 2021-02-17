using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string soundEffectCircleClick = default;
    [SerializeField] private string soundEffectCircleMiss = default;
    [SerializeField] private float circleMissTiming = 1.0f;

    [Header("References")]
    [SerializeField] private FMODAudioManager audio;
    [SerializeField] private GameObject[] patterns;

    // Start is called before the first frame update
    void Start()
    {
        GameObject tmp = Instantiate(patterns[0]);
        tmp.GetComponent<pattern>().Activate(this, circleMissTiming);
    }

    public void CircleClicked()
    {
        AudioManager.Instance.PlaySFX(soundEffectCircleClick);
        audio.ChangeScore(0.25f);
    }

    public void CircleMissed()
    {
        AudioManager.Instance.PlaySFX(soundEffectCircleMiss);
        audio.ChangeScore(-0.1f);
    }

    //private void Update()
    //{
    //    audio.ChangeScore(-0.001f);
    //}
}
