using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Linq;

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
    private Sound currentMusic;
    public bool played;
    private int indexMusicLevel;
    private Sound[] shuffleMusic;

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

    private void Start()
    {
        Shuffle();
        played = true;
        indexMusicLevel = 0;
    }

    public void Play(string name) //функция для проигрывания нужного трека
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Звук: " + name + " не найден!"); //если такого звука нет в списке всех звков
            return;
        }
        //s.source.PlayDelayed(s.source.clip.length);
        s.source.Play();
    }

    private void Shuffle()
    {
        shuffleMusic = Array.FindAll(sounds, sound => sound.name.StartsWith("music_level_"));
        var rnd = new System.Random();
        shuffleMusic = shuffleMusic.OrderBy(s => rnd.Next()).ToArray();//перемешивем масив
    }

    public IEnumerator ShufflePlay()
    {
        if(played == true)
        { 
            if(indexMusicLevel < shuffleMusic.Length)
            {
                currentMusic = shuffleMusic[indexMusicLevel];
                //currentMusic.source.Play();
                StartCoroutine(FadeIn(currentMusic.source, 1f));

                //ниже проблема, когда игра на паузе то таймер не идет, а мелодия звучит, и потом сколько секунд была пауза столько промежуток между песнями
                yield return new WaitForSecondsRealtime(currentMusic.clip.length - 2f); 

                StartCoroutine(FadeOut(currentMusic.source, 2f));
                indexMusicLevel++;
                StartCoroutine(ShufflePlay());
            }
            else
            {
                Shuffle();
                indexMusicLevel = 0;
                StartCoroutine(ShufflePlay());
            }
        }
        else
        {
            StartCoroutine(FadeOut(currentMusic.source, 2f));
            //currentMusic.source.Stop();
        }

    }

    public void StopShuffle()
    {
        played = false;
        StartCoroutine(FadeOut(currentMusic.source, 2f));
        //currentMusic.source.Stop();

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

    public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.2f;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 0.8f)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = 0.8f;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }

    public void FadeOutByName(string name, float FadeTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Звук: " + name + " не найден!");            
        }

        StartCoroutine(FadeOut(s.source, FadeTime));
    }

    public void FadeInByName(string name, float FadeTime)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Звук: " + name + " не найден!");
        }

        StartCoroutine(FadeIn(s.source, FadeTime));
    }





    //играть в нужном месте определенный звук
    //FindObjectOfType<AudioManager>().Play("имя аудио трека");
    //стоп 
    //FindObjectOfType<AudioManager>().Stop("имя аудио трека");
}
