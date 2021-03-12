using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back_button : MonoBehaviour
{

    public GameObject MainLayer; //слой основного меню
    public GameObject TimeLayer; //слой режима на время
    public GameObject NormalLayer; //слой нормального режима

    void Update()
    {
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            // Quit the application
            Menu();
        }
    }


    public void Menu() //возврат меню
    {
        TimeLayer.SetActive(false);
        NormalLayer.SetActive(false);
        MainLayer.SetActive(true);
    }
}
