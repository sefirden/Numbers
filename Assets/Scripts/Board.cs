using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

//основной игровой срипт с кучей методов, тут ебаный пиздец, править аккуратно

public class Board : MonoBehaviour, IPointerClickHandler //вот вотета фигня в конце нужна чтобы срабатывалм клики на уи
{
    public int width; //ширина поля
    public int height; //высота поля
    public int hints; //количество подсказок, закодировать
    public int refill; //количество перемешиваний, закодировать
    public int score; //очки, закодировать
    public int hiScore; //рекорд, закодировать
    public string loadedBoard; //загружаемое поле в виде строки из всех цифр
    public bool endGame; //конец игры или нет
    public int difficult; //сложность, зависит от размера поля, используется в рандоме при заполнении поля новыми цифрами
    public bool AdReward; //смотрел ли пользователь рекламу
    public int level; //уровень, от 0 до 8, 9 это ендгейм локация, 10 стартовая локация

    public LineRenderer[] lines; //масив с линиями, что бы рисовать линии между соединенными цифрами
    public LineRenderer sampleLine; //тут добавлен префаб линн в инспекторе как образец для содания линий

    private ui ui; //скрипт уи
    private bossPlayer boss; //скрипт босса
    private Level Level; //скрипт уровней

    public GameObject[] dots; //масив с префабами цифр для заполнения поля

    private bool countStep; //учавствуйет в проверке конца игры
    private int[,] numbers; //масив с изначальными цифрами прис тарте игры, содержит одинаковое количество всех цифр по размеру поля
    public GameObject[,] allDots; //масив со всеми созданными цифрами на поле, удалять и изменять цифры через обращение к этому масиву
    public GameObject[] CollectedNumbers; //масив с собранными по порядку цифрами

    public int[] TagForRandomRefill; //масив со всеми цифрами, считается их количество и из нужных заполняем рандомно поле
    public GameObject[] HintNumbers; //масив для работы подсказок, подсказка заполняет этот масив

    private Vector2 startPosition, endPosition; //вектор 2 стартовой и конечной позиции, нужно для отслеживания вектора направления при соединении цифр
    private GameObject tempObject; //записываем сюда выбранную цифру на предыдущем этапе, если соединили 3 цифры, то эта переменная будет второй цифрой, нужно для сравнивания выбранной предыдущей цифрой и текущей выбранной
    private int index; //индекс записанной в масив собраных цифр
    
    private bool hint; //переменная учавствует в поиске цифр при подсказке
       
    public float scaleBoard; //переменная для увеличения размера цифр в полях 5 и 7

    public int damage; //урон высчитан из очков, когда босс на уровне, от урона меняется босс и уровень

    private void Awake()
    {
        //присваиваем переменным скрипты
        ui = FindObjectOfType<ui>();
        Level = FindObjectOfType<Level>();
        boss = FindObjectOfType<bossPlayer>();

        //если нормальный режим игры, присваиваем переменные из плеерресурсес, которые загружаются из сейва
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
            damage = PlayerResource.Instance.damageN;

        }
        else if(PlayerResource.Instance.gameMode == "timetrial") //для режима на время
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
            damage = PlayerResource.Instance.damageT;
        }
    }

    void Start()
    {
        allDots = new GameObject[width, height]; //размер масива зависит от выбранного размера поля
        numbers = new int[width, height]; //размер масива зависит от выбранного размера поля
        CollectedNumbers = new GameObject[width]; //размер масива зависит от выбранного размера поля, максимальная длина цепочки - 9
        lines = new LineRenderer[width - 1]; //размер масива зависит от выбранного размера поля -1 , так как соединить 5 цифр нужно 4 линии и тд
        TagForRandomRefill = new int[width*height]; //размер масива зависит от размера поля, для 5 это 5*5=25, 49 и 81 для других режимов

        HintNumbers = new GameObject[width]; //размер масива зависит от выбранного размера поля

        hint = false; //ставим что подсказка сейчас не активна, если в старте этого не делать, то подсказки потом не работают

        index = 0; //индекс обнуляем

        if (hints == 0) //если нет доступных подсказок, переключаем кнопки на рекламу
        {
            ui.HintButton.gameObject.SetActive(false); //выключаем обычную кнопку
            ui.AdHintButton.gameObject.SetActive(true); //включаем кнопку рекламы и +3 подсказки
        }

        if (refill == 0) //если нет доступных перемешиваний, переключаем кнопки в уи и в слое конца игры
        {
            ui.RefillButton.gameObject.SetActive(false); //выключаем обычную кнопку
            ui.AdRefillButton.gameObject.SetActive(true); //включаем кнопку рекламы и +1 перемешивание
            ui.RefillButtonLayer.gameObject.SetActive(false); //выключаем обычную кнопку в слое конца игры
            ui.AdRefillButtonLayer.gameObject.SetActive(true); //включаем кнопку рекламы и +1 перемешивание в слое конца игры
        }

        if (AdReward == true) //если реклама была просмотрена для перемешивания поля
        {
            //делаем не активными кнопки рекламы и в слое конца игры
            ui.AdRefillButton.interactable = false;
            ui.AdRefillButtonLayer.interactable = false;

            //меняем картинки на кнопках
            ui.AdsRefillOn.gameObject.SetActive(false); //выключаем картинку доступна реклама
            ui.AdsRefillOff.gameObject.SetActive(true); //включаем картинку реклама НЕ доступна
            ui.AdsRefillLoading.gameObject.SetActive(false); //выключаем картинку загрузки рекламы

            //см выше, но для кнопки в слоее конца игры
            ui.AdsRefillOnLayer.gameObject.SetActive(false);
            ui.AdsRefillOffLayer.gameObject.SetActive(true);
            ui.AdsRefillLoadingLayer.gameObject.SetActive(false);

            //счетчик доступных просмотров рекламы на кнопках ставим без текста, можно ставить 0, но не красиво
            ui.AdRefillButton.GetComponentInChildren<Text>().text = "";
            ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "";
        }

        switch (width) //в зависимости от размера поля меняем сложность (для рандома цифр) и размер обьектов поля
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

        boss.ChangeBoss(level); //загружаем боссу нужный спрайт (по сути грузим нужного по порядку босса)
        ui.BossHealth(damage, level); //в зависимости от урона и уровня грузи лайфбар босса с нужными данными

        if (PlayerResource.Instance.isLoaded == true) //если игра была загружена
        {
            Level.LoadLevel(level); //грузим нужный уровень
            SetUpLoaded(); //заполняем поле из загруженных цифр
            PlayerResource.Instance.isLoaded = false; //говорим что уже не загружено
            gameObject.SetActive(true); //включаем видимость поля
        }
        else //если была выбрана новая игра
        {
            gameObject.SetActive(false); //выключаем поле
            Level.LoadLevel(10); //загрузка уровня новой игры
            Shuffle(); //перемешиваем стандартный набор цифр при новой игре
            SetUp(); //заполняем поле перемешенными цифрами
        }

        for (int i = 0; i < width-1; i++) //создаем обьекты линий для соединения цифр, количество линий размер поля -1
        {
            LineRenderer line = Instantiate(sampleLine, sampleLine.transform.position, Quaternion.identity);
            line.name = "line " + i;
            lines[i] = line;
            lines[i].gameObject.SetActive(false); //скрываем созданные линии
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
            PlayerResource.Instance.damageN = damage;

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
            PlayerResource.Instance.damageT = damage;
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


                        lines[index - 1].SetPosition(0, CollectedNumbers[index - 1].transform.position);
                        lines[index - 1].SetPosition(1, CollectedNumbers[index].transform.position);
                        lines[index - 1].gameObject.SetActive(true);

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

                        lines[index-1].gameObject.SetActive(false);



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

            if (PlayerResource.Instance.zeroMove == false && PlayerResource.Instance.bossMove == false)
            {
                damage += tempScore * quantity;
                ui.BossHealth(damage, level);
            }

            for (int j = 0; j <= level; j++)
            {
                scoreToNextLevel += PlayerResource.Instance.scoreToNextLevel[j];
            }


            if (damage >= scoreToNextLevel && level < PlayerResource.Instance.scoreToNextLevel.Length)
            {
                level++;

                Level.ChangeLevel(level);
                boss.ChangeBoss(level);
                PlayerResource.Instance.zeroMove = true;
                ui.LifeBarBackground.SetActive(false);
                damage = scoreToNextLevel;
                ui.BossHealth(damage, level);
            }

            if (PlayerResource.Instance.gameMode == "timetrial")
            {
                PlayerResource.Instance.time += quantity * (1f + width / 10f); //в зависимости от сложности уровня добавляет за каждую собранную цифру время от 1,5 до 1,9 сек
            }
            
            Debug.LogWarning("Score: " + score);


            Destroy(); //удаляем собранные цифры

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

        for (int i = 0; i < index-1; i++)
        {
            lines[i].gameObject.SetActive(false);
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
            else if (refill == 0 && AdReward == false)
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

        ui.EndGameScore.text = Convert.ToString(score);
        ui.EndGameHiScore.text = Convert.ToString(hiScore);



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
        int count = -1;

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
            for (int j = 0; j < count; j++)
            {
                lines[j].SetPosition(0, HintNumbers[j].transform.position);
                lines[j].SetPosition(1, HintNumbers[j+1].transform.position);
                lines[j].gameObject.SetActive(true);
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

                }
            }

            Array.Clear(HintNumbers, 0, HintNumbers.Length);
            
            for(int j = 0; j < lines.Length; j++)
            {
                lines[j].gameObject.SetActive(false);
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

                Draw(false);

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

                Draw(false);

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
        ui.AdsHint.gameObject.SetActive(false);
        ui.AdsHintLoading.gameObject.SetActive(true);
        ui.AdHintButton.GetComponentInChildren<Text>().text = "";

        AdMob_baner.Instance.OnGetMoreHintClicked();
    }

    public void AdHintRecieve()
    {
        hints = 3;
        ui.HintButton.gameObject.SetActive(true);
        ui.AdHintButton.gameObject.SetActive(false);

        ui.AdsHint.gameObject.SetActive(true);
        ui.AdHintButton.GetComponentInChildren<Text>().text = "+3";
        ui.AdsHintLoading.gameObject.SetActive(false);

        ui.AdHintButton.interactable = true;
    }

    public void AdHintClose()
    {
        ui.AdHintButton.interactable = true;
        ui.AdsHint.gameObject.SetActive(true);
        ui.AdHintButton.GetComponentInChildren<Text>().text = "+3";
        ui.AdsHintLoading.gameObject.SetActive(false);
    }

    public void AdRefill()
    {
        ui.AdRefillButton.interactable = false;
        ui.AdRefillButtonLayer.interactable = false;

        ui.AdsRefillOn.gameObject.SetActive(false);
        ui.AdsRefillOff.gameObject.SetActive(false);
        ui.AdsRefillLoading.gameObject.SetActive(true);

        ui.AdsRefillOnLayer.gameObject.SetActive(false);
        ui.AdsRefillOffLayer.gameObject.SetActive(false);
        ui.AdsRefillLoadingLayer.gameObject.SetActive(true);


        ui.AdRefillButton.GetComponentInChildren<Text>().text = "";
        ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "";

        AdMob_baner.Instance.OnGetMoreRefillClicked();
    }

    public void AdRefillRecieve()
    {
        AdReward = true;

        ui.AdsRefillOn.gameObject.SetActive(false);
        ui.AdsRefillOff.gameObject.SetActive(true);
        ui.AdsRefillLoading.gameObject.SetActive(false);

        ui.AdsRefillOnLayer.gameObject.SetActive(false);
        ui.AdsRefillOffLayer.gameObject.SetActive(true);
        ui.AdsRefillLoadingLayer.gameObject.SetActive(false);

        Time.timeScale = 1f;
        ui.NoMatchLayer.SetActive(false);
        PlayerResource.Instance.GameIsPaused = false;
        AdMob_baner.Instance.Hide();

        Debug.LogWarning("Refill");

        Draw(false);
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

            ui.AdsRefillOn.gameObject.SetActive(true);
            ui.AdsRefillOff.gameObject.SetActive(false);
            ui.AdsRefillLoading.gameObject.SetActive(false);

            ui.AdsRefillOnLayer.gameObject.SetActive(true);
            ui.AdsRefillOffLayer.gameObject.SetActive(false);
            ui.AdsRefillLoadingLayer.gameObject.SetActive(false);


            ui.AdRefillButton.GetComponentInChildren<Text>().text = "1";
            ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "1";
        }
    }
}
