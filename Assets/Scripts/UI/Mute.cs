using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Mute : MonoBehaviour
{
    private bool mute = false;

    [SerializeField] private AudioMixer audioMixer;
    private float baseVolume = 0;

    private void Start()
    {
        audioMixer.GetFloat("Volume", out baseVolume);
    }

    public void MuteSound()
    {
        audioMixer.SetFloat("Volume", mute ? baseVolume : -100);
        mute = !mute;
    }
}
