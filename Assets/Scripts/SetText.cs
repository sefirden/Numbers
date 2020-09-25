using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SetText : MonoBehaviour
{

    public string text_id;

    void Start()
    {
       GetComponent<Text>().text = SaveSystem.GetText(text_id); //получаем текст из файлов игры

    }

}
