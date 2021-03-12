using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    //слои меню и настроек
    public GameObject MainLayer;
    public GameObject SettingsLayer;

    public Toggle music_off; //выключатели музыки и эффектов
    public Toggle sfx_off;
    public Slider music_vol; //ползунки громкости и эффектов
    public Slider sfx_vol;
    public Dropdown lang_drop; //выбор языка

    [SerializeField]
    string[] myLangs; //список языков
    int index;

    public AudioMixer audioMixer;

    private void Start()
    {
        music_off.isOn = Settings.Instance.music_off; //при старте грузим переменные данные из настроек
        music_vol.value = Settings.Instance.music_vol;
        sfx_off.isOn = Settings.Instance.sfx_off;
        sfx_vol.value = Settings.Instance.sfx_vol;

        string lang = Settings.Instance.language; //грузим какой язык

        int v = Array.IndexOf(myLangs, lang);
        lang_drop.value = v;

        lang_drop.onValueChanged.AddListener(delegate { //ставим его в дропдовн меню
            index = lang_drop.value;
            Settings.Instance.language = myLangs[index];
            ApplyLanguageChanges(); //применяем настройки смены языка при выборе другого
        });
    }

    void ApplyLanguageChanges() //применяем настройки смены языка при выборе другого
    {
        SaveSystem.Instance.SettingsSave(); //сохраняем настройки с новым языком
        string lvl = SceneManager.GetActiveScene().name; //получаем имя активной сцены
        SceneManager.LoadScene(lvl); //и загружаем ее заново
    }

    void Update()
    {
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            // Quit the application
            Menu();
        }
    }


    public void SetVolumeMusic (float volume) //регулятор громкости
    {
        if (volume == -40f) //если громкость ниже половины
        {
            audioMixer.SetFloat("volume_music", -80f); //выключаем звук полностью
            music_off.isOn = true; //ставим чекбокс что звук выключен
        }
        else
        {
            audioMixer.SetFloat("volume_music", volume); //присваиваем громкость по ползунку
            music_off.isOn = false; //снимаем чекбокс с выключеного звука
        }

        Settings.Instance.music_off = music_off.isOn; //записываем переменные в настройки
        Settings.Instance.music_vol = volume;
    }

    public void SetVolumeSFX(float volume) //см выше про музик
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

    public void OffMusic() //для чекбокса выключить музыку
    {
        if (music_off.isOn == true) //если чекбокс нажали
        {
            audioMixer.SetFloat("volume_music", -80f); //выключаем звук в миксере
            music_vol.SetValueWithoutNotify(-40f); //убираем позунок в ноль
        }
        else //если чекбокс сняли
        {
            audioMixer.SetFloat("volume_music", -39f); //включаем звук на половину громкости в миксере
            music_vol.SetValueWithoutNotify(-39f); //двигаем ползунок громкости на 1 деление вверх
        }

        Settings.Instance.music_off = music_off.isOn; //записываем переменные в настройки
        Settings.Instance.music_vol = music_vol.value;
    }

    public void OffSFX() //см выше для музик
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

    public void Menu() //возврат в меню
    {
        SettingsLayer.SetActive(false); //выключаем слой настройки
        MainLayer.SetActive(true); //включаем основной слой меню
        SaveSystem.Instance.SettingsSave(); //сохраняем настройки
    }

}
