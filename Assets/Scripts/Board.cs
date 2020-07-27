using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour, IPointerClickHandler
{
    public int width;
    public int height;

    public GameObject[] dots;

    public Text scoreText;
    public Text endGame;
    public Text hintcount;
    public Text refillcount;
    private bool countStep;
    private int[,] numbers;
    public GameObject[,] allDots;
    public GameObject[] CollectedNumbers;

    public GameObject[] HintNumbers;

    private Vector2 startPosition, endPosition;
    private GameObject tempObject;
    private int index;
    private int score;

    public int hintCount;
    private bool hint;
    public int refillCount;


    private LineRenderer ChainLine;

    private bool move;


    // Start is called before the first frame update
    void Start()
    {

        allDots = new GameObject[width, height];
        numbers = new int[width, height];
        CollectedNumbers = new GameObject[width]; //максимальная длина цепочки - 9
        HintNumbers = new GameObject[width];

        ChainLine = GetComponent<LineRenderer>();
        ChainLine.enabled = false;
        hint = false;

        index = 0;
        score = 0;
        Shuffle();
        SetUp();
    }

    void Update()
    {
        scoreText.text = Convert.ToString(score);
        hintcount.text = Convert.ToString(hintCount);
        refillcount.text = Convert.ToString(refillCount);


        endPosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y); //при каждом кадре считает последнюю позицию мышки

        if (Input.GetMouseButtonDown(0))//клик кнопки мышки вниз //&& !EventSystem.current.IsPointerOverGameObject()
        {

            ClickSelect(); //ищем стартовую точку
            
           // Draw(true); //выключаем подсказку

        }
        else if (Input.GetMouseButton(0)) //когда мышь зажата // && !EventSystem.current.IsPointerOverGameObject()
        {

            RaycastHit2D hit2 = Physics2D.Linecast(startPosition, endPosition); //кидаем лайнкаст каждый раз по апдейту из предыдущего тайла по положению курсора

            if (hit2) //если что-то поймали лайнкастом
            {
                if (tempObject != null)
                {
                    if (Convert.ToInt32(tempObject.transform.tag) - Convert.ToInt32(hit2.transform.tag) == -1) //если текущая цифра больше предыдущей на 1
                    {

                        tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем у предыдущего тайла колайдер

                        Debug.Log(hit2.transform.tag);

                        hit2.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false; //выключаем у текущего тайла колайдер, чтобы лайнкаст его не цеплял
                        tempObject = hit2.transform.gameObject;//записываем последний тайл в темп, чтобы потом включить там колайдер

                        endPosition = hit2.transform.position; //последнюю позицию ставим по центру тайла
                        startPosition = endPosition; //начинаем новые лайнкасты с последнего положения мышки

                        CollectedNumbers[index] = hit2.transform.gameObject; //записываем в массив
                                                                             //visual
                        CollectedNumbers[index].transform.localScale *= 1.25f;
                        CollectedNumbers[index].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);

                        ChainLine.enabled = true;
                        ChainLine.positionCount = index + 1;
                        //ChainLine.SetPosition(0, CollectedNumbers[0].transform.position);
                        ChainLine.SetPosition(index, CollectedNumbers[index].transform.position);

                        index++;
                    }
                    else
                    {
                        Debug.LogWarning("wrong number");
                    }
                }
            }

        }
        else if (Input.GetMouseButtonUp(0))//отпускаем кнопку мышки
        {
            if (tempObject != null)
            {
                tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем коллайдер у последнего тайла
            }

            if (ChainLine.enabled == true)
            {
                ChainLine.enabled = false;
                ChainLine.positionCount = 1;
                ChainLine.SetPosition(0, Vector3.zero);
            }

            Score(); //считаем очки
        }

    }

    public void OnPointerClick(PointerEventData eventData) //чтобы работало UI
    {
        //Debug.LogWarning("Refill");
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
                int tmp = numbers[tp, t];
                int r = UnityEngine.Random.Range(t, height);
                int rp = UnityEngine.Random.Range(tp, width);
                numbers[tp, t] = numbers[rp, r];
                numbers[rp, r] = tmp;

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

                int dotToUse = numbers[i, j]; //потом вписать сюда не количество картинок а количество столбцов, тут генерация рандомного заполенния поля
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "t ( " + i + ", " + j + " )";

                allDots[i, j] = dot;
            }
        }
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

            CollectedNumbers[index].transform.localScale *= 1.25f;
            CollectedNumbers[index].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
            ChainLine.SetPosition(index, CollectedNumbers[index].transform.position);

            index++;

        }
        else
        {
            Debug.Log("not item");
        }
    }

    private void Score() //считает очки
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
        if (quantity > 1) //если выбрана больше чем 1 цифра
        {
            score += tempScore * quantity;
            Debug.LogWarning("Score: " + score);
            Destroy(); //удаляем собранные цифры
        }
        else
        {
            if (CollectedNumbers[0] != null)
            { 
                CollectedNumbers[0].transform.localScale = Vector3.one;
            }
            Array.Clear(CollectedNumbers, 0, CollectedNumbers.Length); //обнуляем собранные цифры
            index = 0;
        }

    }
    
    private void Destroy() //удаляем собранные элементы
    {
        //удаляем собранные--------------------------------------------
        for (int i = 0; i < index; i++)
        {
            Destroy(allDots[Convert.ToInt32(CollectedNumbers[i].transform.position.x), Convert.ToInt32(CollectedNumbers[i].transform.position.y)]); //удаляем все собранные объекты
            allDots[Convert.ToInt32(CollectedNumbers[i].transform.position.x), Convert.ToInt32(CollectedNumbers[i].transform.position.y)] = null;
        }
        Array.Clear(CollectedNumbers, 0, CollectedNumbers.Length); //обнуляем собранные цифры
        index = 0;

        //двигаем ряды вниз--------------------------------------------
        StartCoroutine(DecreaseRow());
    }
    
    private IEnumerator DecreaseRow() //private IEnumerator DecreaseRow()
    {
        Debug.LogWarning("START COROUTINE");
        int nullCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {

                    allDots[i, j].transform.Translate(transform.position.x, transform.position.y - nullCount, transform.position.z, Space.World);
                    allDots[i, j - nullCount] = allDots[i, j];

                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        Debug.LogWarning("END COROUTINE");

        Refilling();
    }

    private void Refilling()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j);

                    int dotToUse = UnityEngine.Random.Range(0, width);
                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";

                    allDots[i, j] = dot;
                }
            }

        }
        CheckEndGame();
    }

    private void CheckEndGame()
    {
        countStep = false;

        for (int i = 0; i < width; i++)
        {
            if (countStep != true)
            {
                for (int j = 0; j < height; j++)
                {
                    if (countStep != true)
                    {
                        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[i, j].transform.position, 1.2f);

                        for (var k = 0; k < hitColliders.Length; k++)
                        {
                            if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(allDots[i, j].transform.tag) == 1)
                            {
                                Debug.LogWarning("есть возможный ход: " + allDots[i, j].transform.tag + ">" + hitColliders[k].transform.tag);
                                countStep = true;
                                break;
                            }
                            else if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(allDots[i, j].transform.tag) == -1)
                            {
                                Debug.LogWarning("есть возможный ход: " + hitColliders[k].transform.tag + ">" + allDots[i, j].transform.tag);
                                countStep = true;
                                break;
                            }
                        }
                        Array.Clear(hitColliders, 0, hitColliders.Length);
                    }
                    else
                        break;
                }
            }
            else
                break;
        }
        if(countStep == false)
        {
            endGame.text = "GAME OVER";
        }

    }

    public void Hint()
    {
        int count = 0;
        if (hintCount > 0)
        {
            Debug.LogWarning("Hint");

            hint = false;

            for (int i = 0; i < width; i++)
            {
                if (hint != true)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (hint != true)
                        {
                            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[i, j].transform.position, 1.2f);

                            for (var k = 0; k < hitColliders.Length; k++)
                            {
                                if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(allDots[i, j].transform.tag) == 1)
                                {
                                    Debug.LogWarning("есть возможный ход: " + allDots[i, j].transform.tag + ">" + hitColliders[k].transform.tag);
                                    HintNumbers[count] = allDots[i, j];
                                    count++;
                                    HintNumbers[count] = hitColliders[k].transform.gameObject;
                                    count++;
                                    hint = true;

                                    HintSearch(count, hitColliders[k].transform.gameObject, hint);
                                    break;
                                }
                            }
                            Array.Clear(hitColliders, 0, hitColliders.Length);
                        }
                        else
                            break;
                    }
                }
                else
                    break;
            }

            if (hint == false)
            {
                endGame.text = "GAME OVER";
            }

            hintCount--;
        }

    }

    private void HintSearch(int count, GameObject TempHintItem, bool hint)
    {
        if (hint == true)
        {
            hint = false;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TempHintItem.transform.position, 1.2f);

            for (var k = 0; k < hitColliders.Length; k++)
            {
                if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(TempHintItem.transform.tag) == 1)
                {
                    hint = true;

                    Debug.LogWarning("есть возможный ход: " + TempHintItem.transform.tag + ">" + hitColliders[k].transform.tag);

                    HintNumbers[count] = hitColliders[k].transform.gameObject;
                    count++;

                    break;
                }

            }

            HintSearch(count, HintNumbers[count-1], hint);
            Array.Clear(hitColliders, 0, hitColliders.Length);
        }
        else if (hint == false)
        {
            Debug.LogWarning("Конец подсказки");

            Draw(hint);

        }


    }

    private void Draw(bool hint)
    {
        if (hint == false) //рисуем
        {
            for (int i = 0; i < HintNumbers.Length; i++)
            {
                if (HintNumbers[i] != null)
                {
                    HintNumbers[i].transform.localScale *= 1.25f;
                    HintNumbers[i].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                }
            }


        }
    }

    public void Refill()
    {
        if (refillCount > 0)
        {
            Debug.LogWarning("Refill");

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (allDots[i, j] != null)
                    {
                        Destroy(allDots[i, j]); //удаляем все собранные объекты
                        allDots[i, j] = null;
                    }
                }

            }
            Shuffle();
            SetUp();
            refillCount--;
        }
    }
}
