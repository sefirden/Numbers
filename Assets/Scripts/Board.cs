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
    public int hints;
    public int refill;
    public int score;
    public int hiScore;
    public string loadedBoard;
    public bool endGame;
    public int difficult;
    public bool AdReward;
    public int level;

    private ui ui;
    private Level Level;

    public GameObject[] dots;

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

    public float scaleBoard;

    private void Awake()
    {
        ui = FindObjectOfType<ui>();
        Level = FindObjectOfType<Level>();


        if (PlayerResource.Instance.gameMode == "normal")
        {
            width = PlayerResource.Instance.widthN;
            height = PlayerResource.Instance.heightN;
            hints = PlayerResource.Instance.hintN;
            refill = PlayerResource.Instance.refillN;
            score = PlayerResource.Instance.scoreN;
            hiScore = PlayerResource.Instance.hiScoreN;
            loadedBoard = PlayerResource.Instance.loadedBoardN;
            endGame = PlayerResource.Instance.EndGameN;
            AdReward = PlayerResource.Instance.AdRewardN;
            level = PlayerResource.Instance.levelN;

        }
        else if(PlayerResource.Instance.gameMode == "timetrial")
        {
            width = PlayerResource.Instance.widthT;
            height = PlayerResource.Instance.heightT;
            hints = PlayerResource.Instance.hintT;
            refill = PlayerResource.Instance.refillT;
            score = PlayerResource.Instance.scoreT;
            hiScore = PlayerResource.Instance.hiScoreT;
            loadedBoard = PlayerResource.Instance.loadedBoardT;
            endGame = PlayerResource.Instance.EndGameT;
            AdReward = PlayerResource.Instance.AdRewardT;
            level = PlayerResource.Instance.levelT;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Level.LoadLevel(level);
        allDots = new GameObject[width, height];
        numbers = new int[width, height];
        CollectedNumbers = new GameObject[width]; //максимальная длина цепочки - 9
        TagForRandomRefill = new int[width*height];

        HintNumbers = new GameObject[width];

        ChainLine = GetComponent<LineRenderer>();
        ChainLine.enabled = false;
        hint = false;

        index = 0;

        if (hints == 0)
        {
            ui.HintButton.gameObject.SetActive(false);
            ui.AdHintButton.gameObject.SetActive(true);
        }

        if (refill == 0)
        {
            ui.RefillButton.gameObject.SetActive(false);
            ui.AdRefillButton.gameObject.SetActive(true);
            ui.RefillButtonLayer.gameObject.SetActive(false);
            ui.AdRefillButtonLayer.gameObject.SetActive(true);
        }

        if (AdReward == true)
        {
            ui.AdRefillButton.interactable = false;
            ui.AdRefillButtonLayer.interactable = false;
            ui.AdRefillButton.GetComponentInChildren<Text>().text = "no more";
            ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "no more";
            ui.Adrefillcount.text = "0";
            ui.AdrefillcountLayer.text = "0";
        }

        switch (width)
        {
            case 5:
                difficult = 3;
                scaleBoard = 2f;
                Debug.LogWarning("case 1 " + difficult);
                break;
            case 6:
                difficult = 5;
                scaleBoard = 1.6f;
                Debug.LogWarning("case 2 " + difficult);
                break;
            case 7:
                difficult = 5;
                scaleBoard = 1.34f;
                Debug.LogWarning("case 3 " + difficult);
                break;
            case 8:
                difficult = 7;
                scaleBoard = 1.13f;
                Debug.LogWarning("case 4 " + difficult);
                break;
            case 9:
                difficult = 7;
                scaleBoard = 1f;
                Debug.LogWarning("case 5 " + difficult);
                break;
            default:
                difficult = width * 2;
                scaleBoard = 1f;
                Debug.LogWarning("case 5 " + difficult);
                break;
        }

        ui.BossHealth(score, level);

        if (PlayerResource.Instance.isLoaded == true)
        {
            SetUpLoaded();
            PlayerResource.Instance.isLoaded = false;
        }
        else
        {
            Shuffle();
            SetUp();
        }
    }

    void Update()
    {

        if (PlayerResource.Instance.gameMode == "normal")
        {
            PlayerResource.Instance.hintN = hints;
            PlayerResource.Instance.refillN = refill;
            PlayerResource.Instance.scoreN = score;
            PlayerResource.Instance.hiScoreN = hiScore;
            PlayerResource.Instance.loadedBoardN = loadedBoard;
            PlayerResource.Instance.EndGameN = endGame;
            PlayerResource.Instance.AdRewardN = AdReward;
            PlayerResource.Instance.levelN = level;

        }
        else if (PlayerResource.Instance.gameMode == "timetrial")
        {
            PlayerResource.Instance.hintT = hints;
            PlayerResource.Instance.refillT = refill;
            PlayerResource.Instance.scoreT = score;
            PlayerResource.Instance.hiScoreT = hiScore;
            PlayerResource.Instance.loadedBoardT = loadedBoard;
            PlayerResource.Instance.EndGameT = endGame;
            PlayerResource.Instance.AdRewardT = AdReward;
            PlayerResource.Instance.levelT = level;
        }

        ui.scoreText.text = Convert.ToString(score);
        ui.hintcount.text = Convert.ToString(hints);
        ui.refillcount.text = Convert.ToString(refill);
        ui.refillcountLayer.text = Convert.ToString(refill);
        ui.HighscoreText.text = Convert.ToString(hiScore);


        endPosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y); //при каждом кадре считает последнюю позицию мышки

        if (Input.GetMouseButtonDown(0) && PlayerResource.Instance.GameIsPaused !=true)//клик кнопки мышки вниз //&& !EventSystem.current.IsPointerOverGameObject()
        {

            ClickSelect(); //ищем стартовую точку
            
           // Draw(true); //выключаем подсказку

        }
        else if (Input.GetMouseButton(0) && PlayerResource.Instance.GameIsPaused != true) //когда мышь зажата // && !EventSystem.current.IsPointerOverGameObject()
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

                        CollectedNumbers[index].transform.localScale = Vector3.one * scaleBoard;
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
        else if (Input.GetMouseButtonUp(0) && PlayerResource.Instance.GameIsPaused != true)//отпускаем кнопку мышки
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

    private void SetUpLoaded()
    {
        int indx = 0;
        string[] a = loadedBoard.Split(new char[] { '*' });


        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = (float)i * scaleBoard;
                float y = (float)j * scaleBoard;

                Vector3 tempPosition = new Vector3(x, y, 1f);

                int dotToUse = Convert.ToInt32(a[indx]) - 1; //потом вписать сюда не количество картинок а количество столбцов, тут генерация рандомного заполенния поля
                
                //Debug.LogError(loadedBoard + " and " + dotToUse);

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "t ( " + i + ", " + j + " )";
                dot.transform.localScale *= scaleBoard;
                allDots[i, j] = dot;
                indx++;
            }
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
                float x = (float)i * scaleBoard;
                float y = (float)j * scaleBoard;

                Vector3 tempPosition = new Vector3(x, y, 1f);

                int dotToUse = numbers[i, j]; //потом вписать сюда не количество картинок а количество столбцов, тут генерация рандомного заполенния поля
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "t ( " + i + ", " + j + " )";
                dot.transform.localScale *= scaleBoard;

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
        int scoreToNextLevel = 0;

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
            score += tempScore * quantity;

            if (score > hiScore)
            {
                hiScore = score;
            }

            for(int j = 0; j <= level; j++)
            {
                scoreToNextLevel += PlayerResource.Instance.scoreToNextLevel[j];
            }


            if (score >= scoreToNextLevel && level < PlayerResource.Instance.scoreToNextLevel.Length)
            {
                level++;

                Level.ChangeLevel(level);
               // Level.LoadLevel(level);
            }

            if (PlayerResource.Instance.gameMode == "timetrial")
            {
                PlayerResource.Instance.time += quantity * (1f + width / 10f); //в зависимости от сложности уровня добавляет за каждую собранную цифру время от 1,5 до 1,9 сек
            }
            
            Debug.LogWarning("Score: " + score);

            ui.BossHealth(score, level);

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
                CollectedNumbers[0].transform.localScale = Vector3.one * scaleBoard;
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
            Destroy(allDots[Convert.ToInt32(CollectedNumbers[i].transform.position.x / scaleBoard), Convert.ToInt32(CollectedNumbers[i].transform.position.y / scaleBoard)]); //удаляем все собранные объекты
            allDots[Convert.ToInt32(CollectedNumbers[i].transform.position.x / scaleBoard), Convert.ToInt32(CollectedNumbers[i].transform.position.y / scaleBoard)] = null;
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

                    allDots[i, j].transform.Translate(transform.position.x, transform.position.y - nullCount * scaleBoard, transform.position.z, Space.World);
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

            if (k.Count() <= (width + difficult) && k.Key != 0) //тут крутим сложность
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

                    float x = (float)i * scaleBoard;
                    float y = (float)j * scaleBoard;

                    Vector3 tempPosition = new Vector3(x, y, 1f);
                    dotToUse = temp[UnityEngine.Random.Range(0, temp.Length)]-1; //тут переписать

                   // Debug.LogError("Добавлена цифра - " + (dotToUse+1));

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    dot.transform.localScale *= scaleBoard;
                    allDots[i, j] = dot;
                }
            }

        }
        Array.Clear(TagForRandomRefill, 0, TagForRandomRefill.Length); //обнуляем собранные цифры

        CollectBoardToSave();
        CheckEndGame();

    }

    private void CollectBoardToSave()
    {
        loadedBoard = null;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                loadedBoard += allDots[i, j].transform.tag + "*";
            }
        }
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
                        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[i, j].transform.position, 1.2f * scaleBoard);

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
            if (refill == 0)
            {
                EndGame();
            }
            else if (refill > 0)
            {
                NoMatch();
            }
        }

    }

    private void NoMatch()
    {
        AdMob_baner.Instance.Show();
        Time.timeScale = 0f;
        ui.NoMatchLayer.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;
    }

    public void EndGame()
    {

        Time.timeScale = 0f;
        ui.NoMatchLayer.SetActive(false);
        ui.EndGameLayer.SetActive(true);
        PlayerResource.Instance.GameIsPaused = true;
        endGame = true;



        PlayServicesGoogle.UnlockAchievement(GPGSIds.achievement_end_game); //ачивка прошел игру получена

        if (PlayerResource.Instance.gameMode == "normal")
        {
              PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__normal_mode, hiScore); //отправляем лучшее время в Google Play

        }
        else if (PlayerResource.Instance.gameMode == "timetrial")
        {
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_play_time_time_limit_mode, Convert.ToInt64(PlayerResource.Instance.playedTime * 1000)); //отправляем лучшее время в Google Play
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__time_limit_mode, hiScore); //отправляем лучшее время в Google Play
        }


        PlayServicesGoogle.Instance.CollectData(); //собираем данные
        PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
        PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

        AdMob_baner.Instance.Show();
    }

    public void Hint()
    {

        int count = 0;
        if (hints > 0 && PlayerResource.Instance.GameIsPaused != true)
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
                            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[i, j].transform.position, 1.2f * scaleBoard);

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
                                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[i, j].transform.position, 1.2f * scaleBoard);

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

            hints--;

            if(hints == 0)
            {
                ui.HintButton.gameObject.SetActive(false);
                ui.AdHintButton.gameObject.SetActive(true);
            }
        }

    }

    private void HintSearchPlus(int count, GameObject TempHintItem, bool hint)
    {
        if (hint == true)
        {
            hint = false;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TempHintItem.transform.position, 1.2f * scaleBoard);

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
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TempHintItem.transform.position, 1.2f * scaleBoard);

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
                    HintNumbers[i].transform.localScale = Vector3.one * scaleBoard;
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
            if (refill > 0 && PlayerResource.Instance.GameIsPaused != true)
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
                CollectBoardToSave();
                refill--;

                if (refill == 0)
                {
                    ui.RefillButton.gameObject.SetActive(false);
                    ui.AdRefillButton.gameObject.SetActive(true);

                    ui.RefillButtonLayer.gameObject.SetActive(false);
                    ui.AdRefillButtonLayer.gameObject.SetActive(true);
                }
            }
        }

        if (layer == true)
        {
            if (refill > 0)
            {
                Time.timeScale = 1f;
                ui.NoMatchLayer.SetActive(false);
                PlayerResource.Instance.GameIsPaused = false;
                AdMob_baner.Instance.Hide();

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
                CollectBoardToSave();
                refill--;

                if (refill == 0)
                {
                    ui.RefillButton.gameObject.SetActive(false);
                    ui.AdRefillButton.gameObject.SetActive(true);

                    ui.RefillButtonLayer.gameObject.SetActive(false);
                    ui.AdRefillButtonLayer.gameObject.SetActive(true);
                }
            }
        }

    }

    public void AdHint()
    {
        ui.AdHintButton.interactable = false;
        ui.AdHintButton.GetComponentInChildren<Text>().text = "Loading...";
        AdMob_baner.Instance.OnGetMoreHintClicked();
    }

    public void AdHintRecieve()
    {
        hints = 3;
        ui.HintButton.gameObject.SetActive(true);
        ui.AdHintButton.gameObject.SetActive(false);
        ui.AdHintButton.interactable = true;
    }

    public void AdHintClose()
    {
        ui.AdHintButton.interactable = true;
        ui.AdHintButton.GetComponentInChildren<Text>().text = "More Hints";
    }

    public void AdRefill()
    {
        ui.AdRefillButton.interactable = false;
        ui.AdRefillButtonLayer.interactable = false;

        ui.AdRefillButton.GetComponentInChildren<Text>().text = "Loading...";
        ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "Loading...";

        AdMob_baner.Instance.OnGetMoreRefillClicked();
    }

    public void AdRefillRecieve()
    {
        AdReward = true;

        ui.AdRefillButton.GetComponentInChildren<Text>().text = "no more";
        ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "no more";
        ui.Adrefillcount.text = "0";
        ui.AdrefillcountLayer.text = "0";

        Time.timeScale = 1f;
        ui.NoMatchLayer.SetActive(false);
        PlayerResource.Instance.GameIsPaused = false;
        AdMob_baner.Instance.Hide();

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
        CollectBoardToSave();

    }

    public void AdRefillClose()
    {
        if (AdReward == false)
        {
            ui.AdRefillButton.interactable = true;
            ui.AdRefillButtonLayer.interactable = true;

            ui.AdRefillButton.GetComponentInChildren<Text>().text = "More refill";
            ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "More refill";
        }
    }
}
