using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSceneAudioManager : MonoBehaviour
{
    public static OpenSceneAudioManager instance;

    //[SerializeField]
    public AudioSource buttonClick_AS;

    private void Awake()
    {
        instance = this;
    }

    public void PlayButtonClickAS() {
        buttonClick_AS.Play();
    }
}
