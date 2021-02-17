//--------------------------------------------------------------------
//   Basic Unity Integration usage contents:
//
//  1: Allowing designers to select Events in the Unity Inspector
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class FMODAudioManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;
    private FMOD.Studio.PARAMETER_ID scoreParamID;

    [Header("Debug")]
    [SerializeField] public Slider scoreVisualizer = default;

    [Header("Fmod related")]
    [SerializeField, FMODUnity.EventRef] private string fModEvent;
    [SerializeField, Range(0, 1)] private float scoreParam = 0f;

    private void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(fModEvent);
        
        FMOD.Studio.EventDescription scoreEventDescription;
        instance.getDescription(out scoreEventDescription);
        FMOD.Studio.PARAMETER_DESCRIPTION scoreParameterDescription;
        scoreEventDescription.getParameterDescriptionByName("Score", out scoreParameterDescription);
        scoreParamID = scoreParameterDescription.id;

        instance.start();
    }

    private void Update()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByID(scoreParamID, scoreParam * 100f);
        scoreVisualizer.value = scoreParam;
    }
}