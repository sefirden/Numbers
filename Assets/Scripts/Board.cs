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
    private int[,] numbers;
    public GameObject[,] allDots; 
    
    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        numbers = new int[width, height];
        Shuffle();
        SetUp();
    }

    private void Shuffle()
    {

    for (int i = 0; i < width; i++)
        { 
            for (int j = 0; j < height; j++)
            { 
                numbers[i, j] = j;
        
            }
        }
       
    for (int tp = 0; tp < width; tp++)
        {  
            for (int t = 0; t < height; t++)
            { 
                int tmp = numbers[tp,t]; 
                int r = UnityEngine.Random.Range(t, height);
                int rp = UnityEngine.Random.Range(tp, width);
                numbers[tp,t] = numbers[rp,r];
                numbers[rp,r] = tmp;
  
            }
        } 
    }

    private void SetUp()
    {

        for (int i = 0; i < width; i++)
        {
             for (int j = 0; j < height; j++)
            {
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
