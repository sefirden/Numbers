using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{

    public int column;
    public int row;
    public int targetX;
    public int targetY;
    private Board board;
    private GameObject otherDot;

    public float swipeAngle = 0;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        row = targetY;
        column = targetX;
    }
     
    void Update()
    {
        targetX = column;
        targetY = row;

    }

    //--------------------------
    //OnMouseDown()
    //нажимаем мышку - записываем первую точку в список, при свайпе считаем угол и кидаем в ту сторону райкастер и проверяем "совместимость" цифр по порядку,
    //если совместимо записываем вторую точку в список, и уже при свайпе проверяем от этой точки как от начальной

    //OnMouseUp()
    //при отпускании мышки отдельным методом записываем очки - суммируем числа в списке [1;2;3;4] и множим на количество в списке (*4)
    //следующим отдельным методом убираем цифры с доски и заполняем пустое место
    //следкющий отдельный метод проверяет есть ли комбинации и и если их нет то конец игры
    //--------------------------

}
