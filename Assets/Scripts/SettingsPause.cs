using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPause : MonoBehaviour
{
    //весь код как в скрипте SettingsMenu, комментарии там же
    public GameObject MainLayer;
    public GameObject SettingsLayer;
    public Toggle music_off;
    public Toggle sfx_off;
    public Slider music_vol;
    public Slider sfx_vol;

    public AudioMixer audioMixer;


    private void Start()
    {
        music_off.isOn = Settings.Instance.music_off;
        music_vol.value = Settings.Instance.music_vol;
        sfx_off.isOn = Settings.Instance.sfx_off;
        sfx_vol.value = Settings.Instance.sfx_vol;
    }

    public void SetVolumeMusic(float volume)
    {
        if (volume == -40f)
        {
            audioMixer.SetFloat("volume_music", -80f);
            music_off.isOn = true;
        }
        else
        {
            audioMixer.SetFloat("volume_music", volume);
            music_off.isOn = false;
        }

        Settings.Instance.music_off = music_off.isOn;
        Settings.Instance.music_vol = volume;
    }

    public void SetVolumeSFX(float volume)
    {
        if (volume == -40f)
        {
            audioMixer.SetFloat("volume_sfx", -80f);
            sfx_off.isOn = true;
        }
        else
        {
            audioMixer.SetFloat("volume_sfx", volume);
            sfx_off.isOn = false;
        }

        Settings.Instance.sfx_off = sfx_off.isOn;
        Settings.Instance.sfx_vol = volume;
    }

    public void OffMusic()
    {
        if (music_off.isOn == true)
        {
            audioMixer.SetFloat("volume_music", -80f);
            music_vol.SetValueWithoutNotify(-40f);
        }
        else
        {
            audioMixer.SetFloat("volume_music", -39f);
            music_vol.SetValueWithoutNotify(-39f);
        }

        Settings.Instance.music_off = music_off.isOn;
        Settings.Instance.music_vol = music_vol.value;
    }

    public void OffSFX()
    {
        if (sfx_off.isOn == true)
        {
            audioMixer.SetFloat("volume_sfx", -80f);
            sfx_vol.SetValueWithoutNotify(-40f);
        }
        else
        {
            audioMixer.SetFloat("volume_sfx", -39f);
            sfx_vol.SetValueWithoutNotify(-39f);
        }

        Settings.Instance.sfx_off = sfx_off.isOn;
        Settings.Instance.sfx_vol = sfx_vol.value;
    }

    public void Menu()
    {
        SettingsLayer.SetActive(false);
        MainLayer.SetActive(true);
        SaveSystem.Instance.SettingsSave();
    }

}
