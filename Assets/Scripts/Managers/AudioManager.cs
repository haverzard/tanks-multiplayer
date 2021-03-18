using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void setSfxLvl(float sfxLvl) {
        masterMixer.SetFloat("sfxVol", sfxLvl);
    }

    public void setMusicLvl(float musicLvl) {
        masterMixer.SetFloat("musicVol", musicLvl);
    }

    public void setDrivingLvl(float drivingLvl) {
        masterMixer.SetFloat("drivingVol", drivingLvl);
    }
    // public void setSfxLvl(float sfxLvl) {
    //     masterMixer.SetFloat("sfxVol", sfxLvl);
    // }
    // void Start()
    // {
        
    // }

    // void Update()
    // {
        
    // }
}
