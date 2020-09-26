using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;

public class SaveSystem : MonoBehaviour
{

    public SettingsSaves settings = new SettingsSaves();
    private string path;
    
    public static Dictionary<string, string> Language;
    public static SaveSystem Instance { get; private set; } //определяем

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
        SettingsLoad(); //загружаем настройки, если их тут не подгрузить, то в методе ниже с языком не подтянет выбранный язык и кнопки будут без надписей, скрипт с настройками должен загружаться раньше этого скрипта, выставить в настройках юнити
    }

    private void LoadLanguage(string lang) //загрузка языковых фраз из файла и создание словаря с парой ключ - фраза, язык передаем аргументом при загрузке настроек
    {

        if (Language == null)
        {
            Language = new Dictionary<string, string>(); //создаем словарь
        }

        Language.Clear(); //очищаем его, не помню зачем

        string allTexts = (Resources.Load(@"Languages/" + lang) as TextAsset).text; //читаем весь текст из нужного языкового файла

        string[] lines = allTexts.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None); //режем текст по строкам
        string key, value;

        //тут весь текст делим на ключ и фразу, способ из гайдов, лучше не трогать пока работает)
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].IndexOf(":") >= 0 && !lines[i].StartsWith("#"))
            {
                key = lines[i].Substring(0, lines[i].IndexOf(":"));
                value = lines[i].Substring(lines[i].IndexOf(":") + 1,
                    lines[i].Length - lines[i].IndexOf(":") - 1).Replace("\\n", Environment.NewLine);
                Language.Add(key, value);
            }
        }
    }

    public static string GetText(string key) //метод для чтения фразы по ключу из словаря
    {
        if (!Language.ContainsKey(key))
        {
            Debug.LogError("There is no key with name: [" + key + "] in your text files");
            return null;
        }

        return Language[key];
    }

    private static string hash = "Sf_4!9lpNw7"; //хеш для методов ниже

    //ниже методы из гайдов, лучше не трогать
    //кодируем
    public static string Encrypt (string input)
    {
        byte[] data = UTF8Encoding.UTF8.GetBytes(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform tr = trip.CreateEncryptor();
                byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(results, 0, results.Length);
            }
        }
    }

    //декодируем
    public static string Decrypt(string input)
    {
        byte[] data = Convert.FromBase64String(input);
        using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
        {
            byte[] key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            using (TripleDESCryptoServiceProvider trip = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB, Padding = PaddingMode.PKCS7 })
            {
                ICryptoTransform tr = trip.CreateDecryptor();
                byte[] results = tr.TransformFinalBlock(data, 0, data.Length);
                return UTF8Encoding.UTF8.GetString(results);
            }
        }
    }

    public void SettingsLoad() //сохранение настроек
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "Settings.json");
#else
        path = Path.Combine(Application.dataPath, "Settings.json"); //путь к файлу с сейвами
#endif
        if (File.Exists(path)) //если файл сейва есть
        {
            settings = JsonUtility.FromJson<SettingsSaves>(File.ReadAllText(path)); //грузим жсон из файла и применяем настройки
            Settings.Instance.music_off = settings.music_off;
            Settings.Instance.sfx_off = settings.sfx_off;
            Settings.Instance.music_vol = settings.music_vol;
            Settings.Instance.sfx_vol = settings.sfx_vol;
            Settings.Instance.language = settings.language;

            LoadLanguage(settings.language); //загружаем язык
            Debug.Log("SettingsLoad");
        }
        else //если файла сейва нет, то загружаем настройки по умолчанию
        {
            Settings.Instance.music_off = false;
            Settings.Instance.sfx_off = false;
            Settings.Instance.music_vol = 0f;
            Settings.Instance.sfx_vol = 0f;
            Settings.Instance.language = "en";

            LoadLanguage("en"); //по умолчанию английский язык
            Debug.Log("SettingsDefault");
        }
    }

    public void SettingsSave() //сохраняем настройки
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        path = Path.Combine(Application.persistentDataPath, "Settings.json");
#else
        path = Path.Combine(Application.dataPath, "Settings.json");
#endif
            settings.music_off = Settings.Instance.music_off;
            settings.sfx_off = Settings.Instance.sfx_off;
            settings.music_vol = Settings.Instance.music_vol;
            settings.sfx_vol = Settings.Instance.sfx_vol;
            settings.language = Settings.Instance.language;

            File.WriteAllText(path, JsonUtility.ToJson(settings)); //берем все и записываем в жсон
            Debug.Log("SettingsSave");
        
    }


#if UNITY_ANDROID && !UNITY_EDITOR 
    private void OnApplicationPause(bool pause) //если приложение на паузе
    {
        if (pause)
        {
      //  PlayServicesGoogle.Instance.CollectData(); //собираем данные
      //  PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON

      //  PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако
        SettingsSave(); //сохраняем настройки
        }
    }
#endif
    private void OnApplicationQuit() //при выходе из приложения
    {
        PlayServicesGoogle.Instance.CollectData(); //собираем данные
        PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON

        PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако true
        SettingsSave(); //сохраняем настройки
    }
}

[Serializable]
public class SettingsSaves //класс с настройками, нужен для сохранения
{
    public bool music_off;
    public bool sfx_off;
    public float music_vol;
    public float sfx_vol;
    public string language;
}