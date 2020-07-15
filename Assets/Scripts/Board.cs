using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;

    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots; 
    
    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        Shuffle();
        SetUp();
    }

    private void Shuffle()
    {


      /*  for (int t = 0; t < numbers.Length; t++)
        {
            string tmp = numbers[t];
            int r = Random.Range(t, numbers.Length);
            numbers[t] = numbers[r];
            numbers[r] = tmp;
        }*/
    }

    private void SetUp()
    {
        int[,] numbers = new int[width, height];
;
        for (int i = 0; i < width; i++)
        {
             for (int j = 0; j < height; j++)
            {

                numbers[i, j] = j + 1;
                Debug.LogWarning(numbers[i, j]);

                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                int dotToUse = numbers[i,j]; //потом вписать сюда не количество картинок а количество столбцов, тут генерация рандомного заполенния поля
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";

                allDots[i, j] = dot;
            }
        }
    }


}
