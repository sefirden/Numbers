using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;

[System.Serializable]
public class Sound //х-ки каждого звука
{
    public string name; //имя
    public AudioClip clip; //аудио выбираем

    [Range(0f,1f)]
    public float volume; //громкость в диапазоне 0-1
    [Range(0.1f, 3f)]
    public float pitch; //питч в диапазоне 0,1-3

    public bool loop; //зацикливать или нет
    public AudioMixerGroup mixer; //собственно мейн аудиомиксер

    [HideInInspector]
    public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; } //определяем
    public Sound[] sounds; //все звуки в этом масиве

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
        //end no audio

        foreach(Sound s in sounds) //эта фигня создает нужные источники звука, хз как точно работает, спиздил в гайде
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.mixer;
        }

    }

    private void Start() //при старте сцены играет заглавную тему 
    {
        Play("Theme"); //имя трека
    }

    public void Play(string name) //функция для проигрывания нужного трека
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Звук: " + name + " не найден!"); //если такого звука нет в списке всех звков
            return;
        }
        s.source.Play();

    }

    public void Stop(string name) //также, но стопаем проигрывание трека вызывая метод с именем трека как аргументом
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Звук: " + name + " не найден!");
            return;
        }
        s.source.Stop();

    }

    //играть в нужном месте определенный звук
    //FindObjectOfType<AudioManager>().Play("имя аудио трека");
    //стоп 
    //FindObjectOfType<AudioManager>().Stop("имя аудио трека");
}
