using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;

    public Text scoreText;

    private BackgroundTile[,] allTiles;
    private int[,] numbers;
    public GameObject[,] allDots;
    public GameObject[] CollectedNumbers;

    private Vector2 startPosition, endPosition;
    private GameObject tempObject;
    private int index;
    private int score;

    private bool move;


    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        numbers = new int[width, height];
        CollectedNumbers = new GameObject[9]; //максимальная длина цепочки - 9
        index = 0;
        score = 0;
        Shuffle();
        SetUp();
    }

    void Update()
    {
        scoreText.text = Convert.ToString(score);

        endPosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y); //при каждом кадре считает последнюю позицию мышки

        if (Input.GetMouseButtonDown(0))//клик кнопки мышки вниз
        {
            ClickSelect(); //ищем стартовую точку

            //_lastTile = null;
            // _selectedTiles.Clear();
            //_checkInput = true;
        }
        else if(Input.GetMouseButton(0)) //когда мышь зажата
        {
            Debug.DrawRay(startPosition, endPosition); //для отладки

                RaycastHit2D hit2 = Physics2D.Linecast(startPosition, endPosition); //кидаем лайнкаст каждый раз по апдейту из предыдущего тайла по положению курсора

                if (hit2) //если что-то поймали лайнкастом
                {
                    if(Convert.ToInt32(tempObject.transform.tag) - Convert.ToInt32(hit2.transform.tag) == -1) //если текущая цифра больше предыдущей на 1
                    {

                    tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем у предыдущего тайла колайдер

                    Debug.Log(hit2.transform.tag);
                
                    hit2.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false; //выключаем у текущего тайла колайдер, чтобы лайнкаст его не цеплял
                    tempObject = hit2.transform.gameObject;//записываем последний тайл в темп, чтобы потом включить там колайдер

                    endPosition = hit2.transform.position; //последнюю позицию ставим по центру тайла
                    startPosition = endPosition; //начинаем новые лайнкасты с последнего положения мышки

                    CollectedNumbers[index] = hit2.transform.gameObject; //записываем в массив
                    index++;
                }
                    else
                    {
                    Debug.LogWarning("wrong number");
                    }
                }

        }
        else if (Input.GetMouseButtonUp(0))//отпускаем кнопку мышки
        {
            tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем коллайдер у последнего тайла
            // _checkInput = false;
            Score(); //считаем очки
            Destroy(); //удаляем собранные цифры

           // Invoke("Decrease", 0.4f); //только через инвок срабатывает при первом дестрое

            Invoke("Refilling", 1f); //заполняем пустое место вниз


        }

    }

    private void Refilling()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j].GetComponent<Dot>().transform.tag == "null")
                {
                    GameObject toDelete = allDots[i, j].GetComponent<Dot>().transform.gameObject;
                    Debug.LogWarning("start refilling");
                    Vector2 tempPosition = new Vector2(allDots[i, j].GetComponent<Dot>().transform.position.x, allDots[i, j].GetComponent<Dot>().transform.position.y);

                    int dotToUse = UnityEngine.Random.Range(1, width);
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";

                    Destroy(toDelete);

                    allDots[i, j] = dot;
                }
            }
        }
    }

    private void Score()
    {
        int quantity = 0;
        int tempScore = 0;

        for (int i = 0; i < CollectedNumbers.Length; i++)
        {
            if (CollectedNumbers[i] != null && Convert.ToInt32(CollectedNumbers[i].tag) > 0)
            {
                tempScore += Convert.ToInt32(CollectedNumbers[i].tag);
                quantity++;
            }
        }
        score += tempScore * quantity;
        Debug.LogWarning("Score: " + score);
    } //считает очки

    private void Destroy() //удаляем собранные элементы
    {
        for (int i = 0; i < index; i++)
        {                      
            GameObject dot = Instantiate(dots[9], CollectedNumbers[i].transform.position, Quaternion.identity);
            dot.transform.parent = this.transform;
            dot.name = "new ( " + Convert.ToInt32(CollectedNumbers[i].transform.position.x) + ", " + Convert.ToInt32(CollectedNumbers[i].transform.position.y) + " )";

            allDots[Convert.ToInt32(CollectedNumbers[i].transform.position.x), Convert.ToInt32(CollectedNumbers[i].transform.position.y)] = dot;

            Destroy(CollectedNumbers[i]);
            CollectedNumbers[i] = null;
        }

        Array.Clear(CollectedNumbers, 0, CollectedNumbers.Length); //обнуляем собранные цифры
        index = 0;

    }

    private void Decrease() //запускает короутин
    {
        StartCoroutine(DecreaseRow()); //отпускаем ряды, пока не работает
    }

    private IEnumerator DecreaseRow()
    {
        Debug.LogWarning("START COROUTINE");
        float nullCount = 0;
        float temp = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                if (allDots[i,j].GetComponent<Dot>().transform.tag == "null")
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().transform.Translate(transform.position.x, transform.position.y - nullCount, transform.position.z,Space.World);
                    temp++;
                }
            }

            for (int j = 0; j < height; j++)
            {

                if (allDots[i, j].GetComponent<Dot>().transform.tag == "null")
                {
                    allDots[i, j].GetComponent<Dot>().transform.Translate(transform.position.x, transform.position.y + temp, transform.position.z, Space.World);
                }
            }

            nullCount = 0;
            temp = 0;
        }
        yield return new WaitForSeconds(0.4f);
        Debug.LogWarning("END COROUTINE");
    }

    private void ClickSelect()
    {
        //Converting Mouse Pos to 2D (vector2) World Pos
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

        if (hit)
        {
            Debug.LogWarning("first click tag " + hit.transform.tag);

            startPosition = hit.transform.position;
            Debug.Log("first click position " + startPosition);

            hit.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            tempObject = hit.transform.gameObject;

            CollectedNumbers[index] = tempObject.transform.gameObject; //записываем первое значение в массив
            index++;

        }
        else
        {
            Debug.Log("not item");
        }
    }

    private void Shuffle() //перемешиваем доску
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

    private void SetUp() //заполняем доску
    {

        for (int i = 0; i < width; i++)
        {
             for (int j = 0; j < height; j++)
             {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "n ( " + i + ", " + j + " )";

                int dotToUse = numbers[i,j]; //потом вписать сюда не количество картинок а количество столбцов, тут генерация рандомного заполенния поля
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "t ( " + i + ", " + j + " )";

                allDots[i, j] = dot;
             }
        }
    }


}
