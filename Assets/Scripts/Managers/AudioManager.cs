using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void setSfxLvl(float sfxLvl) {
        masterMixer.SetFloat("sfxVol", Mathf.Log10(sfxLvl) * 20);
    }

    public void setMusicLvl(float musicLvl) {
        masterMixer.SetFloat("musicVol", Mathf.Log10(musicLvl) * 20);
    }

    public void setDrivingLvl(float drivingLvl) {
        masterMixer.SetFloat("drivingVol", Mathf.Log10(drivingLvl) * 20);
    }
}
