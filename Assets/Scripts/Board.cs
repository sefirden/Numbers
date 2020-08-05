using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class Board : MonoBehaviour, IPointerClickHandler
{
    public int width;
    public int height;

    public GameObject[] dots;

    public Text scoreText;
    public Text HighscoreText;
    public Text hintcount;
    public Text refillcount;
    public Text refillcountLayer;
    public GameObject EndGameLayer;
    public GameObject NoMatchLayer;
    private bool countStep;
    private int[,] numbers;
    public GameObject[,] allDots;
    public GameObject[] CollectedNumbers;

    public int[] TagForRandomRefill;
    public GameObject[] HintNumbers;

    private Vector2 startPosition, endPosition;
    private GameObject tempObject;
    private int index;
    
    private bool hint;
       
    private LineRenderer ChainLine;

    private void Awake()
    {
        width = PlayerResource.Instance.width;
        height = PlayerResource.Instance.height;
    }

    // Start is called before the first frame update
    void Start()
    {
        allDots = new GameObject[width, height];
        numbers = new int[width, height];
        CollectedNumbers = new GameObject[width]; //максимальная длина цепочки - 9
        TagForRandomRefill = new int[width*height];

        HintNumbers = new GameObject[width];

        ChainLine = GetComponent<LineRenderer>();
        ChainLine.enabled = false;
        hint = false;

        index = 0;
        PlayerResource.Instance.score = 0;
        Shuffle();
        SetUp();
    }

    void Update()
    {
        scoreText.text = "Score: " + Convert.ToString(PlayerResource.Instance.score);
        hintcount.text = Convert.ToString(PlayerResource.Instance.hint);
        refillcount.text = Convert.ToString(PlayerResource.Instance.refill);
        refillcountLayer.text = Convert.ToString(PlayerResource.Instance.refill);
        HighscoreText.text = "High Score: " + Convert.ToString(PlayerResource.Instance.hiScore);


        endPosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y); //при каждом кадре считает последнюю позицию мышки

        if (Input.GetMouseButtonDown(0) && PlayerResource.Instance.GameIsPaused !=true && PlayerResource.Instance.EndGame != true)//клик кнопки мышки вниз //&& !EventSystem.current.IsPointerOverGameObject()
        {

            ClickSelect(); //ищем стартовую точку
            
           // Draw(true); //выключаем подсказку

        }
        else if (Input.GetMouseButton(0) && PlayerResource.Instance.GameIsPaused != true && PlayerResource.Instance.EndGame != true) //когда мышь зажата // && !EventSystem.current.IsPointerOverGameObject()
        {

            RaycastHit2D hit2 = Physics2D.Linecast(startPosition, endPosition); //кидаем лайнкаст каждый раз по апдейту из предыдущего тайла по положению курсора

            if (hit2) //если что-то поймали лайнкастом
            {
                if (tempObject != null)
                {
                    if (Convert.ToInt32(tempObject.transform.tag) - Convert.ToInt32(hit2.transform.tag) == -1) //если текущая цифра больше предыдущей на 1
                    {

                        tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем у предыдущего тайла колайдер
                        tempObject.transform.name = "owned";

                        Debug.Log(hit2.transform.tag);

                        hit2.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false; //выключаем у текущего тайла колайдер, чтобы лайнкаст его не цеплял
                        tempObject = hit2.transform.gameObject;//записываем последний тайл в темп, чтобы потом включить там колайдер

                        endPosition = hit2.transform.position; //последнюю позицию ставим по центру тайла
                        startPosition = endPosition; //начинаем новые лайнкасты с последнего положения мышки

                        CollectedNumbers[index] = hit2.transform.gameObject; //записываем в массив
                                                                             //visual
                        CollectedNumbers[index].transform.localScale *= 1.25f;
                        CollectedNumbers[index].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                        CollectedNumbers[index].transform.name = "owned";

                        ChainLine.enabled = true;
                        ChainLine.positionCount = index + 1;
                        //ChainLine.SetPosition(0, CollectedNumbers[0].transform.position);
                        ChainLine.SetPosition(index, CollectedNumbers[index].transform.position);

                        index++;
                    }
                    else if (Convert.ToInt32(tempObject.transform.tag) - Convert.ToInt32(hit2.transform.tag) == 1 && hit2.transform.name == "owned")
                    {
                        tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем у предыдущего тайла колайдер

                        Debug.Log(hit2.transform.tag);

                        hit2.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false; //выключаем у текущего тайла колайдер, чтобы лайнкаст его не цеплял
                        tempObject = hit2.transform.gameObject;//записываем последний тайл в темп, чтобы потом включить там колайдер

                        endPosition = hit2.transform.position; //последнюю позицию ставим по центру тайла
                        startPosition = endPosition; //начинаем новые лайнкасты с последнего положения мышки
                        
                        index--;

                        CollectedNumbers[index].transform.localScale = Vector3.one;
                        CollectedNumbers[index].GetComponent<BoxCollider2D>().size = new Vector2(0.76f, 0.76f);
                        CollectedNumbers[index].transform.name = "ok";
                        CollectedNumbers[index] = null;

                        ChainLine.positionCount = index;



                    }
                    else
                    {
                        Debug.LogWarning("wrong number");
                    }
                }
            }

        }
        else if (Input.GetMouseButtonUp(0) && PlayerResource.Instance.GameIsPaused != true && PlayerResource.Instance.EndGame != true)//отпускаем кнопку мышки
        {
            if (tempObject != null)
            {
                tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем коллайдер у последнего тайла
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
            Draw(false);
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
            if (CollectedNumbers[i] != null)
            {
                tempScore += width;
                quantity++;
            }
        }
        if (quantity > 1) //если выбрана больше чем 1 цифра
        {
            PlayerResource.Instance.score += tempScore * quantity;

            if (PlayerResource.Instance.score > PlayerResource.Instance.hiScore)
            {
                PlayerResource.Instance.hiScore = PlayerResource.Instance.score;
            }

            if (PlayerResource.Instance.gameMode == "timetrial")
                {
                PlayerResource.Instance.time += quantity * (1f + width / 10f); //в зависимости от сложности уровня добавляет за каждую собранную цифру время от 1,5 до 1,9 сек
                }
            
            Debug.LogWarning("Score: " + PlayerResource.Instance.score);
            Destroy(); //удаляем собранные цифры

            if (ChainLine.enabled == true)
            {
                ChainLine.enabled = false;
                ChainLine.positionCount = 1;
                ChainLine.SetPosition(0, Vector3.zero);
            }
        }
        else
        {
            if (CollectedNumbers[0] != null)
            { 
                CollectedNumbers[0].transform.localScale = Vector3.one;
                CollectedNumbers[0].GetComponent<BoxCollider2D>().size = new Vector2(0.76f, 0.76f);
                CollectedNumbers[0].transform.name = "ok";
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

    private int[] Scan()
    {

        int[] temp = new int [width];
        int count = 0;
        int indx  = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    TagForRandomRefill[count] = 0;
                    count++;
                }
                else
                {
                    TagForRandomRefill[count] = Convert.ToInt32(allDots[i, j].transform.tag);
                    count++;
                }
            }
        }

        Array.Sort(TagForRandomRefill);

        var g = TagForRandomRefill.GroupBy(i => i);

        foreach (var k in g)
        {

            if (k.Count() <= width+1 && k.Key != 0)
            {
               // Debug.Log("цифр количеством меньше " + (width+1) + " - "  + k.Key);
                temp[indx] = k.Key;
                indx++;
            }                
        }


            int[] Board = new int[width];

            for (int i = 0; i < width; i++)
            {
                Board[i] = i + 1;                
            }

            var tag = TagForRandomRefill.Distinct();

            var result = Board.Except(tag);

            foreach (var k in result)
            {
                temp[indx] = k;
                //Debug.LogError("temp[indx] = " + temp[indx]);
                indx++;

            }
            //тут подставлять цифру, которой нет на поле
            Array.Resize(ref temp, indx);

        return temp;
    }

    private void Refilling()
    {
        int dotToUse;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    //каждый раз цикла проверяем количество цифр на поле и те цифры, которые втречаються на поле width+2 и больше раз не попадают в выборку для рандома
                    

                    var temp = Scan();

                    Vector2 tempPosition = new Vector2(i, j);
                    dotToUse = temp[UnityEngine.Random.Range(0, temp.Length)]-1; //тут переписать

                   // Debug.LogError("Добавлена цифра - " + (dotToUse+1));

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";

                    allDots[i, j] = dot;
                }
            }

        }
        Array.Clear(TagForRandomRefill, 0, TagForRandomRefill.Length); //обнуляем собранные цифры

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
            if (PlayerResource.Instance.refill == 0)
            {
                EndGame();
            }
            else if (PlayerResource.Instance.refill > 0)
            {
                NoMatch();
            }
        }

    }

    private void NoMatch()
    {
        //AdMob_baner.Instance.Show(Settings.Instance.ad_top_down);
        Time.timeScale = 0f;
        NoMatchLayer.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;
    }

    public void EndGame()
    {

        Time.timeScale = 0f;
        NoMatchLayer.SetActive(false);
        EndGameLayer.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;
        PlayerResource.Instance.EndGame = true;

        // PlayServicesGoogle.Instance.CollectData(); //собираем данные
        // PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
        // PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

        // AdMob_baner.Instance.Show(Settings.Instance.ad_top_down);
    }

    public void Hint()
    {

        int count = 0;
        if (PlayerResource.Instance.hint > 0 && PlayerResource.Instance.GameIsPaused != true)
        {
            Debug.LogWarning("Hint");
            Draw(false);

            hint = false;

            for (int i = UnityEngine.Random.Range(0, width); i < width; i++)
            {
                if (hint != true)
                {
                    for (int j = UnityEngine.Random.Range(0, height); j < height; j++)
                    {
                        if (hint != true)
                        {
                            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[i, j].transform.position, 1.2f);

                            for (var k = 0; k < hitColliders.Length; k++)
                            {
                                if (Convert.ToInt32(allDots[i, j].transform.tag) - Convert.ToInt32(hitColliders[k].transform.tag) == 1)
                                {
                                    Debug.LogWarning("есть возможный ход: " + allDots[i, j].transform.tag + ">" + hitColliders[k].transform.tag);
                                    HintNumbers[count] = allDots[i, j];
                                    count++;
                                    HintNumbers[count] = hitColliders[k].transform.gameObject;
                                    count++;
                                    hint = true;

                                    HintSearchMinus(count, hitColliders[k].transform.gameObject, hint);
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
                                    if (Convert.ToInt32(allDots[i, j].transform.tag) - Convert.ToInt32(hitColliders[k].transform.tag) == 1)
                                    {
                                        Debug.LogWarning("есть возможный ход: " + allDots[i, j].transform.tag + ">" + hitColliders[k].transform.tag);
                                        HintNumbers[count] = allDots[i, j];
                                        count++;
                                        HintNumbers[count] = hitColliders[k].transform.gameObject;
                                        count++;
                                        hint = true;

                                        HintSearchMinus(count, hitColliders[k].transform.gameObject, hint);
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
                };
            }

            PlayerResource.Instance.hint--;
        }

    }

    private void HintSearchPlus(int count, GameObject TempHintItem, bool hint)
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

            HintSearchPlus(count, HintNumbers[count-1], hint);
            Array.Clear(hitColliders, 0, hitColliders.Length);
        }
        else if (hint == false)
        {
            Debug.LogWarning("Конец подсказки");
            Draw(true);
        }


    }

    private void HintSearchMinus(int count, GameObject TempHintItem, bool hint)
    {
        if (hint == true)
        {
            hint = false;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TempHintItem.transform.position, 1.2f);

            for (var k = 0; k < hitColliders.Length; k++)
            {
                if (Convert.ToInt32(TempHintItem.transform.tag) - Convert.ToInt32(hitColliders[k].transform.tag) == 1)
                {
                    hint = true;

                    Debug.LogWarning("есть возможный ход: " + TempHintItem.transform.tag + ">" + hitColliders[k].transform.tag);

                    HintNumbers[count] = hitColliders[k].transform.gameObject;
                    count++;

                    break;
                }

            }

            HintSearchMinus(count, HintNumbers[count - 1], hint);
            Array.Clear(hitColliders, 0, hitColliders.Length);
        }
        else if (hint == false)
        {


            Array.Reverse(HintNumbers, 0, count);

            for(int i = 0; i < count; i++)
            {
                Debug.LogWarning(HintNumbers[i].transform.tag);
            }

            Debug.LogWarning("Запускаем в +");
            hint = true;
            HintSearchPlus(count, HintNumbers[count - 1], hint);
        }


    }

    private void Draw(bool draw)
    {
        int count = 0;
        if (draw == true) //рисуем
        {
            for (int i = 0; i < HintNumbers.Length; i++)
            {
                if (HintNumbers[i] != null)
                {
                    HintNumbers[i].transform.localScale *= 1.25f;
                    HintNumbers[i].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f);
                    count++;
                }
            }

            ChainLine.enabled = true;
            ChainLine.positionCount = count;
            for (int j = 0; j < count; j++)
            {
                ChainLine.SetPosition(j, HintNumbers[j].transform.position);
            }

        }
        else if (draw == false) //убираем нарисованное
        {
            for (int i = 0; i < HintNumbers.Length; i++)
            {
                if (HintNumbers[i] != null)
                {
                    HintNumbers[i].transform.localScale = Vector3.one;
                    HintNumbers[i].GetComponent<BoxCollider2D>().size = new Vector2(0.76f, 0.76f);
                    count++;
                }
            }

            Array.Clear(HintNumbers, 0, HintNumbers.Length);

            if (ChainLine.enabled == true)
            {
                ChainLine.enabled = false;
                ChainLine.positionCount = 1;
                ChainLine.SetPosition(0, Vector3.zero);
            }
        }
    }

    public void Refill(bool layer)
    {
        if (layer == false)
        {
            if (PlayerResource.Instance.refill > 0 && PlayerResource.Instance.GameIsPaused != true)
            {
                Debug.LogWarning("Refill");

                if (ChainLine.enabled == true)
                {
                    ChainLine.enabled = false;
                    ChainLine.positionCount = 1;
                    ChainLine.SetPosition(0, Vector3.zero);
                }

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
                PlayerResource.Instance.refill--;
            }
        }

        if (layer == true)
        {
            if (PlayerResource.Instance.refill > 0)
            {
                Time.timeScale = 1f;
                NoMatchLayer.SetActive(false);
                PlayerResource.Instance.GameIsPaused = false;

                Debug.LogWarning("Refill");

                if (ChainLine.enabled == true)
                {
                    ChainLine.enabled = false;
                    ChainLine.positionCount = 1;
                    ChainLine.SetPosition(0, Vector3.zero);
                }

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
                PlayerResource.Instance.refill--;
            }
        }

    }
}
