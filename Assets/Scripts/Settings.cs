using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public static Settings Instance { get; private set; } //определяем

    public bool music_off;
    public bool sfx_off;
    public float music_vol;
    public float sfx_vol;
    public string language;

    public AudioMixer audioMixer;

    private void Awake() //запускается до всех стартов
    {
        if (Instance == null) //если объекта ещё нет
        {
            Instance = this; //говорим что вот кагбе он
            DontDestroyOnLoad(gameObject); //и говорим что его нельзя ломать между уровнями, иначе он нахер не нужен
        }
        else //но, если вдруг на уровне такой уже есть
        {
            Destroy(gameObject); //то ломаем его к херам
        }
 

    }

    private void Start()
    {
        audioMixer.SetFloat("volume_music", music_vol);
        audioMixer.SetFloat("volume_sfx", sfx_vol);
    }

}
