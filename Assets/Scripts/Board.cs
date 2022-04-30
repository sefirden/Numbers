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
    public string hints; //количество подсказок, закодировать
    public string refill; //количество перемешиваний, закодировать
    public string score; //очки, закодировать
    public string hiScore; //рекорд, закодировать
    public string turnx2; //ходов с х2 урона или очков, закодировать
    public string turnTime; //ходов до отката +1 минута
    public string loadedBoard; //загружаемое поле в виде строки из всех цифр
    public bool endGame; //конец игры или нет
    public int difficult; //сложность, зависит от размера поля, используется в рандоме при заполнении поля новыми цифрами
    public float difficultTime;
    public bool AdReward; //смотрел ли пользователь рекламу
    public int level; //уровень, от 0 до 8, 9 это ендгейм локация, 10 стартовая локация

    public LineRenderer[] lines; //масив с линиями, что бы рисовать линии между соединенными цифрами
    public LineRenderer sampleLine; //тут добавлен префаб линн в инспекторе как образец для содания линий

    private ui ui; //скрипт уи
    private zeroPlayer zero; //скрипт ноля
    private bossPlayer boss; //скрипт босса
    private Level Level; //скрипт уровней
    public Pause pause;

    public GameObject[] dots; //масив с префабами цифр для заполнения поля

    private bool countStep; //учавствуйет в проверке конца игры
    private int[,] numbers; //масив с изначальными цифрами прис тарте игры, содержит одинаковое количество всех цифр по размеру поля
    private List<GameObject[]> collectHint; //масив для сохранения всех подсказок и подальшего их сравнения
    public GameObject[,] allDots; //масив со всеми созданными цифрами на поле, удалять и изменять цифры через обращение к этому масиву
    public GameObject[] CollectedNumbers; //масив с собранными по порядку цифрами

    public int[] TagForRandomRefill; //масив со всеми цифрами, считается их количество и из нужных заполняем рандомно поле
    public List<GameObject> HintNumbers; //масив для работы подсказок, подсказка заполняет этот масив

    private Vector2 startPosition, endPosition; //вектор 2 стартовой и конечной позиции, нужно для отслеживания вектора направления при соединении цифр
    private GameObject tempObject; //записываем сюда выбранную цифру на предыдущем этапе, если соединили 3 цифры, то эта переменная будет второй цифрой, нужно для сравнивания выбранной предыдущей цифрой и текущей выбранной
    private int index; //индекс записанной в масив собраных цифр

    private bool hint; //переменная учавствует в поиске цифр при подсказке
       
    public float scaleBoard; //переменная для увеличения размера цифр в полях 5 и 7

    public int damage; //урон высчитан из очков, когда босс на уровне, от урона меняется босс и уровень

    public bool changelvl; //будем передавать при смене уровня и перемешивания поля


    private void Awake()
    {
        //присваиваем переменным скрипты
        zero = FindObjectOfType<zeroPlayer>();
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
            turnx2 = PlayerResource.Instance.turnx2N;

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
            turnx2 = PlayerResource.Instance.turnx2T;
            turnTime = PlayerResource.Instance.turnTime;
        }
    }

    void Start()
    {
        allDots = new GameObject[width, height]; //размер масива зависит от выбранного размера поля
        numbers = new int[width, height]; //размер масива зависит от выбранного размера поля
        CollectedNumbers = new GameObject[width]; //размер масива зависит от выбранного размера поля, максимальная длина цепочки - 9
        lines = new LineRenderer[width - 1]; //размер масива зависит от выбранного размера поля -1 , так как соединить 5 цифр нужно 4 линии и тд
        TagForRandomRefill = new int[width*height]; //размер масива зависит от размера поля, для 5 это 5*5=25, 49 и 81 для других режимов

        ui.scoreText.text = SaveSystem.Decrypt(score); //очки
        ui.hintcount.text = SaveSystem.Decrypt(hints); //количество подсказок
        ui.refillcount.text = SaveSystem.Decrypt(refill); //количество перемешиваний
        ui.refillcountLayer.text = SaveSystem.Decrypt(refill); //количество перемешиваний в слое конца игры
        ui.HighscoreText.text = SaveSystem.Decrypt(hiScore); //макс очки
        ui.turnLeftText.text = SaveSystem.GetText("turn_left_damage") + " " + SaveSystem.Decrypt(turnx2);

        HintNumbers = new List<GameObject>(); //размер масива зависит от выбранного размера поля
        collectHint = new List<GameObject[]>(); //минимальный размер масива для сравнения подсказок

        hint = false; //ставим что подсказка сейчас не активна, если в старте этого не делать, то подсказки потом не работают

        index = 0; //индекс обнуляем

        if (Convert.ToInt32(SaveSystem.Decrypt(hints)) == 0) //если нет доступных подсказок, переключаем кнопки на рекламу
        {
            ui.HintButton.gameObject.SetActive(false); //выключаем обычную кнопку
            ui.AdHintButton.gameObject.SetActive(true); //включаем кнопку рекламы и +3 подсказки
        }

        if (Convert.ToInt32(SaveSystem.Decrypt(refill)) == 0) //если нет доступных перемешиваний, переключаем кнопки в уи и в слое конца игры
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
                difficultTime = 0.2f;
                scaleBoard = 2f;
                break;

            case 7:
                difficult = 8;
                difficultTime = 0.1f;
                scaleBoard = 1.34f;
                break;

            case 9:
                difficult = 10;
                difficultTime = 0.0f;
                scaleBoard = 1f;
                break;

            default:
                difficult = width * 2;
                scaleBoard = 1f;
                break;
        }

        zero.ChangeZero(level); //загружаем нолю нужный аниматор (по сути грузим с новыми анимациями и оружием)
        boss.ChangeBoss(level); //загружаем боссу нужный спрайт (по сути грузим нужного по порядку босса)
        ui.BossHealth(damage, level); //в зависимости от урона и уровня грузи лайфбар босса с нужными данными

        FindObjectOfType<AudioManager>().Stop("music_menu");
        FindObjectOfType<AudioManager>().played = true;
        StartCoroutine(FindObjectOfType<AudioManager>().ShufflePlay());

        if (PlayerResource.Instance.isLoaded == true) //если игра была загружена
        {
            Level.LoadLevel(level); //грузим нужный уровень
            SetUpLoaded(); //заполняем поле из загруженных цифр
            PlayerResource.Instance.isLoaded = false; //говорим что уже не загружено
            gameObject.SetActive(true); //включаем видимость поля
            StartCoroutine(ui.LightsOnOff(true)); //вклюяаем свет
            ui.HintButton.interactable = true; //делаем активной кнопку подсказок
            ui.RefillButton.interactable = true; //делаем активной кнопку перемешать
            ui.Tutorial.interactable = true; //делаем активной кнопку подсказка
            ui.DamageX2Button.interactable = true; //делаем активной кнопку x2 урона
            ui.PlusTimeButton.interactable = true; //делаем активной кнопку плюс время

            if (PlayerResource.Instance.gameMode == "timetrial") //если режим игры на время, то показываем таймер
            {
                PlayerResource.Instance.starttimer = true;
                ui.timerimg.SetActive(true);
            }

            if (level == PlayerResource.Instance.scoreToNextLevel.Length)
            {
                ui.DamageX2Button.gameObject.SetActive(false);
                ui.ScoreX2Button.gameObject.SetActive(true);
            }

        }
        else //если была выбрана новая игра
        {
            PlayerResource.Instance.zeroMove = true; //говорим что ноль двигается
            zero.GetComponent<Animator>().SetBool("run", true);
            StartCoroutine(ui.LightsOnOff(false)); //выключаем свет
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.PauseClick();
        }

        endPosition = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y); //при каждом кадре считает последнюю позицию мышки

        if (Input.GetMouseButtonDown(0) && PlayerResource.Instance.GameIsPaused !=true)//клик кнопки мышки вниз если не на паузе
        {
            ClickSelect(); //ищем стартовую точку методом
        }
        else if (Input.GetMouseButton(0) && PlayerResource.Instance.GameIsPaused != true) //когда мышь зажата и нет паузы
        {
            RaycastHit2D hit2 = Physics2D.Linecast(startPosition, endPosition); //кидаем лайнкаст каждый раз по апдейту из предыдущего тайла по положению курсора

            if (hit2) //если что-то поймали лайнкастом
            {
                if (tempObject != null) //если при первом клике была выбрана цифра, а не просто клик в пустоту был
                {
                    if (Convert.ToInt32(tempObject.transform.tag) - Convert.ToInt32(hit2.transform.tag) == -1) //если тег временной(первой цифры) отнять тег пойманой райкастом цифры равно -1 (по сути следущая цифра больше предыдущей)
                    {

                        tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем у предыдущего тайла колайдер, выключался в другом методе
                        tempObject.transform.name = "owned"; //меняем имя, надо для отмены выбора цифры, см дальше

                        hit2.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false; //выключаем у текущего тайла колайдер, чтобы лайнкаст его не цеплял
                        tempObject = hit2.transform.gameObject;//записываем последний тайл в темп, чтобы потом включить там колайдер и сравнивать следующую цифру с текущей

                        endPosition = hit2.transform.position; //последнюю позицию ставим по центру тайла
                        startPosition = endPosition; //начинаем новые лайнкасты с последнего положения мышки

                        CollectedNumbers[index] = hit2.transform.gameObject; //записываем пойманную райкастом цифру в массив пойманных цифр

                        //visual
                        CollectedNumbers[index].transform.localScale *= 1.25f; //увеличиваем объект пойманной цифры на 25%
                        CollectedNumbers[index].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f); //уменьшаем колайдер цифры до размеров до увеличения, иначе райкаст цепляет не те цифры иногда
                        CollectedNumbers[index].transform.name = "owned"; //меняем имя, надо для отмены выбора цифры, см дальше
                        
                        lines[index - 1].SetPosition(0, CollectedNumbers[index - 1].transform.position); //берем линию из масива с линиями по индексу, устанавливаем первую точку линии по предыдущей цифре
                        lines[index - 1].SetPosition(1, CollectedNumbers[index].transform.position); //берем линию из масива с линиями по индексу, устанавливаем вторую точку линии по пойманной райкастом цифре
                        lines[index - 1].gameObject.SetActive(true); //включаем эту линию

                        FindObjectOfType<AudioManager>().Play("number_select_"+ Convert.ToString(index - 1)); 

                        index++; //увеличиваем индекс, для след цифр
                    }
                    else if (Convert.ToInt32(tempObject.transform.tag) - Convert.ToInt32(hit2.transform.tag) == 1 && hit2.transform.name == "owned") //если тег временной(первой цифры) отнять тег пойманой райкастом цифры равно 1 (по сути следущая цифра меньше предыдущей) и имя пойманной имеет имя овнед (значит она уже была выбрана ранее типа первая 4 а выбрана 3)
                    {
                        tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем у предыдущего тайла колайдер, выключался в другом методе

                        hit2.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false; //выключаем у текущего тайла колайдер, чтобы лайнкаст его не цеплял
                        tempObject = hit2.transform.gameObject;//записываем последний тайл в темп, чтобы потом включить там колайдер и сравнивать следующую цифру с текущей

                        endPosition = hit2.transform.position; //последнюю позицию ставим по центру тайла
                        startPosition = endPosition; //начинаем новые лайнкасты с последнего положения мышки
                        
                        index--; //уменьшаем индекс

                        CollectedNumbers[index].transform.localScale = Vector3.one * scaleBoard; //для этой цифры ставим стандартный размер
                        CollectedNumbers[index].GetComponent<BoxCollider2D>().size = new Vector2(0.76f, 0.76f); //ставим стандартный размер колайдера
                        CollectedNumbers[index].transform.name = "ok"; //меняем имя с овнед на ок
                        CollectedNumbers[index] = null; //удаляем эту цифру из списка пойманных цифр

                        lines[index-1].gameObject.SetActive(false); //выключаем линию, которая соединяла эти цифры
                    }
                    else
                    {
                        Debug.LogWarning("wrong number"); //если выбрана неверная цифра типа певая 5, а райкастом выбрали 7
                    }
                }
            }

        }
        else if (Input.GetMouseButtonUp(0) && PlayerResource.Instance.GameIsPaused != true)//отпускаем кнопку мышки и нет паузы 
        {
            if (tempObject != null) //если была выбрана хотя бы одна цифра ранее
            {
                tempObject.GetComponent<BoxCollider2D>().enabled = true; //включаем коллайдер у последнего тайла
            }

            if (index >= 1)
            {
                Score(CollectedNumbers, index); //считаем очки

                Array.Clear(CollectedNumbers, 0, CollectedNumbers.Length); //обнуляем массив с собранными цифрами
                index = 0; //ставим индекс 0, иначе масив собранных цифр будет заполнятся неверно
            }

        }
    }

    public void OnPointerClick(PointerEventData eventData) //чтобы работало UI
    {
        //Debug.LogWarning("Refill");
    }

    private void SetUpLoaded() //заполняем доску при продолжении игры
    {
        int indx = 0;
        string[] a = loadedBoard.Split(new char[] { '*' }); //берем строку с цифрами записанными через * и создаем из нее массив


        for (int i = 0; i < width; i++) //в зависимости от ширины и высоты
        {
            for (int j = 0; j < height; j++)
            {
                float x = (float)i * scaleBoard; //координаты множим на переменную по размеру поля см выше
                float y = (float)j * scaleBoard;

                Vector3 tempPosition = new Vector3(x, y, 1f); //позиция цифры

                int dotToUse = Convert.ToInt32(a[indx]) - 1; //цифра из масива сохраненных цифр
                
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity); //создаем объект цифры, которая берет префаб из списка дотс и нужными координатами
                dot.transform.parent = this.transform; //присваиваем позицию
                dot.name = "t ( " + i + ", " + j + " )"; //присваиваем имя
                dot.transform.localScale *= scaleBoard; //увиличиваем по размеру поля
                allDots[i, j] = dot; //записываем в масив всех цфир поля
                indx++; //увиличиваем индекс
            }
        }
    }

    private void Shuffle() //перемешиваем доску при старте новой игры
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                numbers[i, j] = j; //создаем масив цифр для перемешивания
            }
        }

        //перемешивание массива случайным образом, способ из интернета
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
                float x = (float)i * scaleBoard; //координаты множим на переменную по размеру поля см выше
                float y = (float)j * scaleBoard;

                Vector3 tempPosition = new Vector3(x, y, 1f); //позиция цифры

                int dotToUse = numbers[i, j]; //заполняем поле из масива, который предварительно был перемешан и заполнен, см выше метод шафл
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity); //создаем объект цифры, которая берет префаб из списка дотс и нужными координатами
                dot.transform.parent = this.transform; //присваиваем позицию
                dot.name = "t ( " + i + ", " + j + " )"; //присваиваем имя
                dot.transform.localScale *= scaleBoard; //увиличиваем по размеру поля

                allDots[i, j] = dot; //записываем в масив всех цфир поля
            }
        }
    }

    private void ClickSelect() //обработчик клика по нажатию на кнопку, ищет райкастом цифры
    {
        //Debug.LogError("ClickSelect");
        CheckEndGame(); //проверка на конец игры, есть ли возможные варианты ходов

        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y); //получаем координаты клика, переводим в нужные координаты
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f); //кидаем райкаст по координатам см выше

        if (hit && hit.transform.tag != "boss" && hit.transform.tag != "zero") //если райкастом что-то поймали
        {
            PlayerResource.Instance.TurnIsOn = true;
            Debug.Log("TurnIsOn = " + PlayerResource.Instance.TurnIsOn);

            Draw(false); //убираем нарисованные ранее линии, соединяющие цифры

            startPosition = hit.transform.position; //говорим что стартовая позиция это наши координаты каста


            hit.transform.gameObject.GetComponent<BoxCollider2D>().enabled = false; //выключаем уоллайдер, чтобы не ловить этот же обьект следующим райкастом
            tempObject = hit.transform.gameObject; //говорим что временный объект это наш пойманный кастом

            //ебань чтобы во время анимации удаления не напихать в метод еще цифр кликая на все попало

                CollectedNumbers[index] = tempObject.transform.gameObject; //записываем первое значение в массив собрыннх цифр

                CollectedNumbers[index].transform.localScale *= 1.25f; //увеличиваем размер цифры
                CollectedNumbers[index].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f); //делаем размер колайдера стандартного размера

                index++; //увеличиваем индекс, для заполнения масива по порядку
        }
        else //если райкастом ничего не поймали
        {
            Debug.Log("not item"); //просто выводим в лог

        }
    }

    private void Score(GameObject[] CollectedNumbers, int index) //считает очки, содержит запуск смены босса, уровня и изменение хп босса
    {
        //Debug.LogError("Score");

        int quantity = 0; //количество собранных цифр
        int tempScore = 0; //временное количество очков


        //декодируем переменные для расчетов
        int scoreI = Convert.ToInt32(SaveSystem.Decrypt(score));
        int hiScoreI = Convert.ToInt32(SaveSystem.Decrypt(hiScore));

        for (int i = 0; i < CollectedNumbers.Length; i++) //считаем количество собранных цифр
        {
            if (CollectedNumbers[i] != null) //если елемент масива не нуль
            {
                tempScore += width; //увеличиваем временные очки на размер поля (для поля 5 получаем 5 очков за цифру, для поля 9 получаем 9)
                quantity++; //увеличиваем количество
            }
        }
        if (quantity > 1) //если собрана больше чем 1 цифра
        {
            scoreI += tempScore * quantity; //увеличиваем очки по формуле временные очки множим на количество

            ScoreToAchieve(scoreI);

            if (scoreI > hiScoreI) //если количество очков больше чем максимальное
            {
                hiScoreI = scoreI; //приравниваем максимальное к текущему значению
                hiScore = SaveSystem.Encrypt(Convert.ToString(hiScoreI)); //кодируем хайскор
                ToPlayerResources("hiScore"); //передаем в интерфейс и в playerresources

            }

            if (PlayerResource.Instance.zeroMove == false && PlayerResource.Instance.bossMove == false) //если ноль не двигается и босс не двигается, наносим урон
            {
                //тут добавить проверку на конец уровня, на последнем уровне играть другую анимацию или вместо боса кидать ножи в мишень
                damage += tempScore * quantity; //считаем урон по формуле как и очки см выше
                zero.Attack(level, tempScore * quantity, quantity); //передает уровень и урон, урон для цифр над головой босса                
            }

            if (PlayerResource.Instance.gameMode == "timetrial") //если у нас режим игры на время
            {
                PlayerResource.Instance.time += quantity * (difficultTime + width / 10f); //в зависимости от сложности уровня добавляет за каждую собранную цифру время от 0.5 + 0,5 до 0.5 + 0,9 сек 
            }

            StartCoroutine(ChangeLevel()); //проверяем надо ли менять уровень
            AnimDestroy(CollectedNumbers, index); //удаляем собранные цифры
        }
        else //если собрана всего одна цифра и мы отпустили клик
        {
            if (CollectedNumbers[0] != null) //если первый элемент не нуль
            { 
                CollectedNumbers[0].transform.localScale = Vector3.one * scaleBoard; //ставим первой цифре стандартный размер
                CollectedNumbers[0].GetComponent<BoxCollider2D>().size = new Vector2(0.76f, 0.76f); //делаем размер колайдера тоже стандартным
                CollectedNumbers[0].transform.name = "ok"; //меняем имя, надо для возможности отмены хода движением в обратном порядке по цифрам
            }
            Array.Clear(CollectedNumbers, 0, CollectedNumbers.Length); //обнуляем массив с собранными цифрами
            index = 0; //ставим индекс 0, иначе масив собранных цифр будет заполнятся неверно

            PlayerResource.Instance.TurnIsOn = false;
            Debug.Log("TurnIsOn = " + PlayerResource.Instance.TurnIsOn);
        }

        score = SaveSystem.Encrypt(Convert.ToString(scoreI));
        ToPlayerResources("score");

        ToPlayerResources("damage");
        ToPlayerResources("level");
    }

    private void ScoreToAchieve(int score)
    {
        if (score >= 50000 && score < 51000)
            PlayServicesGoogle.UnlockAchievement(GPGSIds.achievement_score_50k);
        else if (score >= 100000 && score < 101000)
            PlayServicesGoogle.UnlockAchievement(GPGSIds.achievement_score_100k);
        else if (score >= 250000 && score < 251000)
            PlayServicesGoogle.UnlockAchievement(GPGSIds.achievement_score_250k);
        else if (score >= 500000 && score < 501000)
            PlayServicesGoogle.UnlockAchievement(GPGSIds.achievement_score_500k);
        else if (score >= 1000000 && score < 1001000)
            PlayServicesGoogle.UnlockAchievement(GPGSIds.achievement_score_1000k);
    }

    private IEnumerator ChangeLevel()
    {
        int scoreToNextLevel = 0; //количество очков для перехода на след уровень

        if (level != PlayerResource.Instance.scoreToNextLevel.Length) //если у нас не последний уровень
        {
            for (int j = 0; j <= level; j++) //каждый раз считаем количество очков для следующего уровня
            {
                scoreToNextLevel += PlayerResource.Instance.scoreToNextLevel[j]; //плюсуем все значения очков для перехода на след уровень вплоть до теккущего уровня
            }
        }

        if (damage >= scoreToNextLevel && level < PlayerResource.Instance.scoreToNextLevel.Length - 1) //если сумарного урона больше чем количество урона нужное для смены уровня и уровень меньше максимального количества
        {
            changelvl = true; //говорим что смена уровня
            level++; //увеличиваем уровень 
            ui.LifeBarBackground.SetActive(false); //прячем лайфбар босса
            ui.turnLeft.SetActive(false);

            StartCoroutine(zero.KillTheBoss()); //анимация убийства босса, там будут все анимации ноля и босса

            damage = scoreToNextLevel; //уравниваем нанесенный урон до уровня нужного для смены, что бы босс появлялся с ровным количеством хп, а не без нескольких пунктов
            ui.BossHealth(damage, level); //передаем в ую метод информацию про урон, для изменения шкалы хп босса

            Firebase.Analytics.Parameter[] ChangeLevel =
{
            new Firebase.Analytics.Parameter("To_level", Convert.ToString(level)),
            new Firebase.Analytics.Parameter("GameMode", PlayerResource.Instance.gameMode)
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ChangeLevel", ChangeLevel);

        }
        else if (damage >= scoreToNextLevel && level == PlayerResource.Instance.scoreToNextLevel.Length - 1) //переход с последнего уровня с боссом на урвоень конца игры, если не писать -1 то не сработает
        {

            changelvl = true; //говорим что смена уровня
            level++; //увеличиваем уровень 
            ui.LifeBarBackground.SetActive(false); //прячем лайфбар босса
            ui.DamageX2Button.gameObject.SetActive(false);
            ui.ScoreX2Button.gameObject.SetActive(true);
            ui.turnLeft.SetActive(false);


            StartCoroutine(zero.KillTheBoss()); //анимация убийства босса, там будут все анимации ноля и босса
            Firebase.Analytics.Parameter[] ChangeLevel =
{
            new Firebase.Analytics.Parameter("To_level", Convert.ToString(level)),
            new Firebase.Analytics.Parameter("GameMode", PlayerResource.Instance.gameMode)
            };
            Firebase.Analytics.FirebaseAnalytics.LogEvent("ChangeLevel", ChangeLevel);
        }
        yield return new WaitForFixedUpdate();
    }

    private void ShuffleBoardChangeLevel() //метод, который перемешивает поле при смене босса
    {
        //собираем текущее поле в строку

            int[] board = new int[width * width]; //обнуляем предыдущую строку из цифр
            int ind = 0;
            for (int i = 0; i < width; i++) //столбцы
            {
                for (int j = 0; j < width; j++) //рядки
                {
                    board [ind] = Convert.ToInt32(allDots[i, j].transform.tag); //сохраняем теги всех объектов в строку через *
                    ind++;
                }
            }
        //перемешать собранное поле, метод из интернета
            for (int t = 0; t < width * width; t++)
            {
                int tmp = board[t];
                int r = UnityEngine.Random.Range(t, width*width);
                board[t] = board[r];
                board[r] = tmp;
            }

        //заполняем поле собранными цифрами, перемешаными
        int ind2 = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = (float)i * scaleBoard; //координаты множим на переменную по размеру поля см выше
                float y = (float)j * scaleBoard;

                Vector3 tempPosition = new Vector3(x, y, 1f); //позиция цифры

                int dotToUse = board[ind2]; //заполняем поле из масива, который предварительно был перемешан и заполнен, см выше метод шафл
                GameObject dot = Instantiate(dots[dotToUse-1], tempPosition, Quaternion.identity); //создаем объект цифры, которая берет префаб из списка дотс и нужными координатами
                dot.transform.parent = this.transform; //присваиваем позицию
                dot.name = "t ( " + i + ", " + j + " )"; //присваиваем имя
                dot.transform.localScale *= scaleBoard; //увиличиваем по размеру поля

                //удалить текущее цифру перед записью новой
                Destroy(allDots[i, j]);
                allDots[i, j] = null;
                allDots[i, j] = dot; //записываем в масив всех цфир поля
                ind2++;
            }
        }
    }

    private void AnimDestroy(GameObject[] CollectedNumbers, int index)
    {
        //Debug.LogError("AnimDestroy");
        for (int i = 0; i < index - 1; i++)
        {
            lines[i].gameObject.SetActive(false); //выключаем все включенные линии по этой хитрой схеме
        }

        //анимация удаления
        for (int i = 0; i < index; i++)
        {
            float x = CollectedNumbers[i].transform.position.x; //координаты множим на переменную по размеру поля см выше
            float y = CollectedNumbers[i].transform.position.y;

            Vector3 tempPosition = new Vector3(x, y, 1f); //позиция цифры
            GameObject dot = Instantiate(CollectedNumbers[i], tempPosition, Quaternion.identity);
            dot.name = "anim"; //присваиваем имя
            dot.tag = "anim";
            dot.transform.localScale = new Vector3(scaleBoard, scaleBoard, scaleBoard); //увеличиваем по размеру поля
            dot.GetComponent<BoxCollider2D>().enabled = false; //выключаем коллайдер иначе пока идет анимация можно выбрать еще цифру
            dot.GetComponent<Animator>().SetTrigger("destroy");
        }
        
        Destroy(CollectedNumbers, index);
    }
       
    private void Destroy(GameObject[] CollectedNumbers, int index) //удаляем собранные элементы
    {
       //Debug.LogError("Destroy");
        //удаляем собранные
        for (int i = 0; i < index; i++)
        {

            Destroy(allDots[Convert.ToInt32(CollectedNumbers[i].transform.position.x / scaleBoard), Convert.ToInt32(CollectedNumbers[i].transform.position.y / scaleBoard)]); //удаляем все собранные объекты
            allDots[Convert.ToInt32(CollectedNumbers[i].transform.position.x / scaleBoard), Convert.ToInt32(CollectedNumbers[i].transform.position.y / scaleBoard)] = null; //обнуляем нужные элементы массива всех цифр            
        }

        Array.Clear(CollectedNumbers, 0, CollectedNumbers.Length); //обнуляем массив с собранными цифрами
        index = 0; //ставим индекс 0, иначе масив собранных цифр будет заполнятся неверно

       //двигаем ряды вниз
       StartCoroutine(DecreaseRow());
    }
    
    private IEnumerator DecreaseRow()//короутина, которая двигает цифры вниз, на место собранных ранее
    {
        PlayerResource.Instance.anim_board_destroy = true;
        yield return new WaitForSeconds(0.32f); //время анимации удаления

        DeleteAnim();

        int nullCount = 0; //количество пустых ячеек на поле

        for (int i = 0; i < width; i++)//столбцы
        {
            for (int j = 0; j < height; j++) //рядки
            {
                if (allDots[i, j] == null) //если есть пустая ячейка
                {
                    nullCount++; //увеличиваем счетчик пустых ячеек
                }
                else if (nullCount > 0) //если пустых ячеек больше 0
                {

                    allDots[i, j].transform.Translate(transform.position.x, transform.position.y - nullCount * scaleBoard, transform.position.z, Space.World); //двигаем полную ячейку по вертикали вниз на количество пустых ячеек
                    allDots[i, j - nullCount] = allDots[i, j]; //подставляем в пустую ячейку цифру сверху

                    allDots[i, j] = null; //обнуляем ячейку, которая стала пустой
                }
            }
            nullCount = 0; //обнуляем количество пустх ячеек и переходим к проверке следующего столбца
        }
       // yield return new WaitForSeconds(0.1f); //ожидание 0,4с, хрен пойми на что влияет, потестить, теоретически скорость сдвига ячеек вниз

        //запускаем заполнение пустых ячеек на поле

        Refilling();
    }

    private void DeleteAnim() //удяляем все обьекты созданные для анимации
    {
        GameObject[] Wally = GameObject.FindGameObjectsWithTag("anim");

        foreach (var item in Wally)
        {
            Destroy(item);
        }
    }

    private int[] Scan() //метод, который проверяет количество всех цифр на поле и возвращает масив, который содержит только те цифры, которых мало на поле
    {
        //смысл метода, что в итоге мы получим массив цифр которых не хватает на поле по правилам, и из этого массива будет выбрана случайная цифра
        //собираем все цифры, вместо пустых ячеек ставим 0, потом сортируем и создаем словарь с количеством каждой цифры, во временный метод вписываем цифры, которых мало на поле и также вписываем те, которых на поле не осталось вообще
       
        int[] temp = new int [width]; //временный масив размером в ширину поля
        int count = 0; //счетчик цифр
        int indx  = 0; //индекс масива

        for (int i = 0; i < width; i++) //столбцы
        {
            for (int j = 0; j < height; j++) //рядки
            {
                if (allDots[i, j] == null) //если на доске есть пустое значение
                {
                    TagForRandomRefill[count] = 0; //то пишем в массив 0
                    count++; //увеличиваем счетчик
                }
                else //если не пустое значение
                {
                    TagForRandomRefill[count] = Convert.ToInt32(allDots[i, j].transform.tag); //пишем в массив тег текущего объекта
                    count++; //увеличиваем счетчик
                }
            }
        }

        Array.Sort(TagForRandomRefill); //сортируем все собранные цифры в массиве

        var g = TagForRandomRefill.GroupBy(i => i); //собираем словарь из массива всех цифр, где ключ к словарю это цифра, а значение это количество этих цифр

        foreach (var k in g) //для каждоый цифры в словаре
        {
            //собираем временный масив из цифр, которых не хватает на поле по правилам что ниже
            if (k.Count() <= (width + difficult) && k.Key != 0) //если количество цифр меньше равно ширине поля + модификатор сложности (для каждого размера поля свой см выше) и это не цифра 0 (типа цифра 2 встречается 5 раз, что меньше ширины поля + сложность, значит цифра 2 попадет в временный массив для заполнения поля)
            {
                temp[indx] = k.Key; //то пишем ее во временный массив 
                indx++; //увеличиваем индекс, для заполенния массива
            }                
        }


            int[] Board = new int[width]; //создаем массив, который содержит все цифры по порядку, зависит от размера поля 

            for (int i = 0; i < width; i++)
            {
                Board[i] = i + 1;  //для поля размером 5, массив будет 1,2,3,4,5              
            }

            var tag = TagForRandomRefill.Distinct(); //массив, который из всех собранных цифр оставляет только по одному варианту (типа было 1,1,2,3,3, станет 1,2,3)

            var result = Board.Except(tag); //массив который оставляет из массива боард, только те цифры, которых нет в массиве тег (без этого куска возможен вариант, когда ты собрал все цифры и они больше не смогут появится при заполнении)

            foreach (var k in result) //для каждой цифры в масиве результ
            {
                temp[indx] = k; //записываем в временный масив
                indx++; //увиличиваем индекс

            }

            Array.Resize(ref temp, indx); //меняем размер масива в зависимости от количества цифр в нем, по умолчанию размер как ширина поля

        return temp; //возвращаем временный массив
    }

    private void Refilling() //заполнение поля, на место пустых ячеек
    {
        //Debug.LogError("Refilling");
        int dotToUse; //номер цифры из списка префабов для заполнения ячейки поля

        for (int i = 0; i < width; i++) //столбцы
        {
            for (int j = 0; j < height; j++) //рядки
            {
                if (allDots[i, j] == null) //если находим пустуя ячейку на поле
                {
           
                    var temp = Scan(); //получаем массив с цифрами для заполнения, которые соответствуют правилам, см метод скан выше

                    float x = (float)i * scaleBoard; //координаты множим на переменную по размеру поля см выше
                    float y = (float)j * scaleBoard;

                    Vector3 tempPosition = new Vector3(x, y, 1f); //позиция цифры

                    dotToUse = temp[UnityEngine.Random.Range(0, temp.Length)]-1; //выбираем цифру из массива случайным способом и отнимаем 1, иначе вместо 2 будем заполнять 3 и тд.

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity); //создаем объект цифры, которая берет префаб из списка дотс и нужными координатами
                    dot.transform.parent = this.transform; //присваиваем позицию
                    dot.name = "( " + i + ", " + j + " )"; //присваиваем имя
                    dot.transform.localScale *= scaleBoard; //увеличиваем по размеру поля

                    allDots[i, j] = dot; //записываем в масив всех цфир поля
                }
            }

        }
        Array.Clear(TagForRandomRefill, 0, TagForRandomRefill.Length); //обнуляем собранные цифры, не помню почему именно тут а не в методе скан, лучше не трогать

        if(changelvl == true) //если смена уровня, то мешаем поле
        {
            ShuffleBoardChangeLevel(); //мешаем поле
            changelvl = false; //говорим что смена уровня уже не тру
        }
        CollectBoardToSave(); //сохранение всех цифр на поле по порядку в строку, через *
    }

    private void CollectBoardToSave() //сохранение всех цифр на поле по порядку в строку, через *
    {
        //Debug.LogError("CollectBoardToSave");
        loadedBoard = null; //обнуляем предыдущую строку из цифр

        for (int i = 0; i < width; i++) //столбцы
        {
            for (int j = 0; j < height; j++) //рядки
            {
                loadedBoard += allDots[i, j].transform.tag + "*"; //сохраняем теги всех объектов в строку через *
            }
        }

        ToPlayerResources("loadedBoard");

        PlayerResource.Instance.TurnIsOn = false;
        Debug.Log("TurnIsOn = " + PlayerResource.Instance.TurnIsOn);
    }

    private void CheckEndGame() //проверка на конец игры, есть ли возможные варианты ходов
    {
        countStep = false; //шаг при проверке конца игры, фелс если нет возможных ходов 

        for (int i = 0; i < width; i++) //столбцы
        {
            if (countStep != true) //если нет возможных ходов, если это не сделать будет в холостую проверять все столбцы даже если найдет возможный ход в первом столбце
            {
                for (int j = 0; j < height; j++) //рядки
                {
                    if (countStep != true) //если нет возможных ходов
                    {
                        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[i, j].transform.position, 1.2f * scaleBoard); //создаем массив с коллайдерами, записываем все колайдеры в радиусе 1,2*скейл от точки проверки

                        for (var k = 0; k < hitColliders.Length; k++) //для всех элементов массива с колайдерами
                        {
                            if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(allDots[i, j].transform.tag) == 1) //если текущий тег колайдера - текущая точка проверки равно 1 (образно если точка проверки равно 1 и рядом мы нашли цифру 2)
                            {
                                countStep = true; //говорим что есть возможный ход
                                break; //выходим из цикла
                            }
                            else if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(allDots[i, j].transform.tag) == -1) //если текущий тег колайдера - текущая точка проверки равно -1 (образно если точка проверки равно 2 и мы нашли цифру 1) это позволяет делать меньше итераций для поиска хода, когда он есть
                            {
                                countStep = true; //говорим что есть возможный ход
                                break; //выходим из цикла
                            }
                        }
                        Array.Clear(hitColliders, 0, hitColliders.Length); //очищаем массив с колайдерами в радиусе от точки проверки
                    }
                    else
                        break; //выходим из цикла
                }
            }
            else
                break; //выходим из цикла
        }
        if(countStep == false) //если после всех проверок нет возможных ходов
        {
            int refillI = Convert.ToInt32(SaveSystem.Decrypt(refill));
            if (refillI == 0 && AdReward == true) //если количество перемешиваний поля равно 0 и реклама просмотрена была
            {
                EndGame(); //показываем всплывающий слой конца игры
            }
            else if (refillI > 0) //если количество перемешиваний больше 0
            {
                NoMatch(); //показываем всплывающий слой нет ходов с предложением перемешать поле
            }
            else if (refillI == 0 && AdReward == false) //если количество перемешиваний поля равно 0, но человек еще не смотрел видео рекламу для перемешивания в этой игре
            {
                NoMatch(); //показываем всплывающий слой нет ходов с предложением перемешать поле за просмотр видео рекламы
            }
        }

    }

    private void NoMatch() //запускаем когда нет возможных ходов
    {
        AdMob_baner.Instance.Show(); //показываем рекламный банер
        Time.timeScale = 0f; //ставим паузу в игре
        ui.NoMatchLayer.SetActive(true); //показываем слой нет возможных ходов
        PlayerResource.Instance.GameIsPaused = true; //говорим что игра на паузе
    }

    public void EndGame() //запускаем когда нет возможных ходов и вариантов перемешать поле
    {
        endGame = true; //говорим что конец игры, должно быть перед паузой иначе апдейт не передаст в плеерресоурс что игра окончена, это позволяло при проигрыше выйти в меню и потом нажать продолжить игру даже когда проиграл
        //если не сработает передать в коде с режимами ниже руками в плеерресоурсес конец игры
        ToPlayerResources("endGame");

        Firebase.Analytics.Parameter[] EndGame =
{
            new Firebase.Analytics.Parameter("width", Convert.ToString(width)),
            new Firebase.Analytics.Parameter("Why", "no match"),
            new Firebase.Analytics.Parameter("level", Convert.ToString(level)),
            new Firebase.Analytics.Parameter("GameMode", PlayerResource.Instance.gameMode),
            new Firebase.Analytics.Parameter("score", Convert.ToInt32(SaveSystem.Decrypt(score))),
            };
        Firebase.Analytics.FirebaseAnalytics.LogEvent("EndGame", EndGame);


        Time.timeScale = 0f; //ставим паузу в игре
        ui.NoMatchLayer.SetActive(false); //выключаем слой нет ходов (надо когда из слоя нет ходов мы отказываемся перемешивать поле)
        ui.EndGameLayer.SetActive(true); //показываем слой конца игры
        PlayerResource.Instance.GameIsPaused = true; //говорим что игра на паузе

        string scoreS = SaveSystem.Decrypt(score);
        string hiScoreS = SaveSystem.Decrypt(hiScore);
        ui.EndGameScore.text = scoreS; //показываем на слое конца иры очки
        ui.EndGameHiScore.text = hiScoreS; //показываем максимальные очки

        if (PlayerResource.Instance.gameMode == "normal") //если режим игры нормальный
        {
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__normal_mode, Convert.ToInt32(hiScoreS)); //отправляем лучшие очки в Google Play
            PlayerResource.Instance.EndGameN = true; 

        }
        else if (PlayerResource.Instance.gameMode == "timetrial") //если режим игры на время
        {
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_play_time_time_limit_mode, Convert.ToInt64(PlayerResource.Instance.playedTime * 1000)); //отправляем лучшее время в Google Play
            PlayServicesGoogle.AddScoreToLeaderboard(GPGSIds.leaderboard_top_score__time_limit_mode, Convert.ToInt32(hiScoreS)); //отправляем лучшие очки в Google Play
            PlayerResource.Instance.EndGameT = true;
        }


        PlayServicesGoogle.Instance.CollectData(); //собираем данные
        PlayServicesGoogle.Instance.SaveToJson(); //пишем в JSON
        PlayServicesGoogle.Instance.SaveToCloud(); //пишем в облако

        AdMob_baner.Instance.Show(); //показываем рекламный банер
    }

    public void HintButton() //вешаем на кнопку поиска
    {
        Draw(false); //убираем все нарисованные линии
        int hintsI = Convert.ToInt32(SaveSystem.Decrypt(hints));

        if (hintsI > 0 && PlayerResource.Instance.GameIsPaused != true) //если доступных подсказок больше 0 и игра не на паузе
        {
            Hint(0, 0);
            hintsI--; //отнимаем одну подсказку из доступных
            Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "Hint");
        }

        if (hintsI == 0) //если подсказок больше не осталось
        {
            ui.HintButton.gameObject.SetActive(false); //выключаем кнопку подсказок
            ui.AdHintButton.gameObject.SetActive(true); //включаем кнопку +3 подсказки за видео рекламу
        }
            hints = SaveSystem.Encrypt(Convert.ToString(hintsI));
            ToPlayerResources("hints");
    }

    public void Hint(int a, int b) //стартовый метод подсказок, ищет последовательность цифр для соединения от случайной цифры на поле
    {
        hint = false;
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(allDots[a, b].transform.position, 1.2f * scaleBoard); //создаем массив с коллайдерами, записываем все колайдеры в радиусе 1,2*скейл от последней цифры в подсказке

            for (int k = 0; k < hitColliders.Length; k++) //для всех элементов массива с колайдерами
            {
                if (Convert.ToInt32(allDots[a, b].transform.tag) - Convert.ToInt32(hitColliders[k].transform.tag) == 1) //если последняя цифра в подсказке - текущий тег колайдера равно 1 (образно если последняя цифра равно 2 и рядом мы нашли цифру 1)
                {
                    hint = true; //говорим что есть возможный ход, чтобы остановить цикл
                    HintNumbers.Add(allDots[a, b].transform.gameObject); //записываем найденную колайдером цифру как следующий элемент массива
                    HintNumbers.Add(hitColliders[k].transform.gameObject); //записываем найденную колайдером цифру как следующий элемент массива

                    HintSearchMinus(HintNumbers[HintNumbers.Count() - 1], hint, a, b); //запускаем метод, который будет искать возможные цифры, которые меньше чем точка старта проверки (передаем счетчик, найденная колайдером цифра, ну и была подсказка или нет)
                    Array.Clear(hitColliders, 0, hitColliders.Length); //очищаем массив с колайдерами в радиусе от точки проверки 
                    return; //выходим из цикла
                }
                else if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(allDots[a, b].transform.tag) == 1)
                {                
                    hint = true; //говорим что есть возможный ход, чтобы остановить цикл
                    HintNumbers.Add(allDots[a, b].transform.gameObject); //записываем найденную колайдером цифру как следующий элемент массива
                    HintNumbers.Add(hitColliders[k].transform.gameObject); //записываем найденную колайдером цифру как следующий элемент массива

                    HintSearchPlus(HintNumbers[HintNumbers.Count() - 1], hint, a, b); //запускаем метод, который будет искать возможные цифры, которые меньше чем точка старта проверки (передаем счетчик, найденная колайдером цифра, ну и была подсказка или нет)
                    Array.Clear(hitColliders, 0, hitColliders.Length); //очищаем массив с колайдерами в радиусе от точки проверки 
                    return;
                }
            }

        Array.Clear(hitColliders, 0, hitColliders.Length); //очищаем массив с колайдерами в радиусе от точки проверки 

        if (b == width - 1 && a == width - 1)
        {
            Comparer();
        }
        else if (b == width - 1 && a < width - 1)
        {
            a++;
            b = 0;
            Hint(a, b);
        }
        else if (b < width - 1 && a <= width - 1)
        {
            b++;
            Hint(a, b);
        }
    }

    private void HintSearchMinus(GameObject TempHintItem, bool hint, int a, int b) //запускаем поиск следующей цифры для подсказок по убыванию от предыдущей минимальной цифры
    {
        if (hint == true) //если мы продолжаем поиск в минус
        {
            hint = false; //это мы типа говорим что первая последовательность еще не найдена, эта ебала нужна для выхода из последовательности и запуска поиска в плюс
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TempHintItem.transform.position, 1.2f * scaleBoard); //создаем массив с коллайдерами, записываем все колайдеры в радиусе 1,2*скейл от последней цифры в подсказке

            for (var k = 0; k < hitColliders.Length; k++) //для всех элементов массива с колайдерами
            {
                if (Convert.ToInt32(TempHintItem.transform.tag) - Convert.ToInt32(hitColliders[k].transform.tag) == 1) //если последняя цифра в подсказке - текущий тег колайдера равно 1 (образно если последняя цифра равно 2 и рядом мы нашли цифру 1)
                {
                    hint = true; //говорим что есть возможный ход, чтобы остановить цикл
                    HintNumbers.Add(hitColliders[k].transform.gameObject); //записываем найденную колайдером цифру как следующий элемент массива

                    break; //выходим из цикла
                }
            }

            HintSearchMinus(HintNumbers[HintNumbers.Count() - 1], hint, a, b); //запускаем метод, который будет искать возможные цифры, которые меньше чем точка старта проверки (передаем счетчик, найденная колайдером цифра, ну и была подсказка или нет)
            Array.Clear(hitColliders, 0, hitColliders.Length); //очищаем массив с колайдерами в радиусе от точки проверки
        }
        else if (hint == false) //если поиск в минус больше не нашел вариантов
        {
            HintNumbers.Reverse(); //реверсим масив с собранными цифрами для подсказки (типа было 3,2,1 стало 1,2,3)
            hint = true; //говорим что есть возможный ход
            HintSearchPlus(HintNumbers[HintNumbers.Count()-1], hint, a, b); //запускаем метод, который будет искать возможные цифры, которые больше чем последняя точка проверки (передаем счетчик, найденная колайдером цифра, ну и была подсказка или нет)
        }
    }

    private void HintSearchPlus(GameObject TempHintItem, bool hint, int a, int b) //запускаем поиск следующей цифры для подсказок по возростанию от предыдущей максимальной цифры
    {

        if (hint == true) //если мы продолжаем поиск в плюс
        {
            hint = false; //это мы типа говорим что первая последовательность еще не найдена, эта ебала нужна для выхода из последовательности и конца поиска подсказок
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(TempHintItem.transform.position, 1.2f * scaleBoard); //создаем массив с коллайдерами, записываем все колайдеры в радиусе 1,2*скейл от последней цифры в подсказке

            for (var k = 0; k < hitColliders.Length; k++) //для всех элементов массива с колайдерами
            {
                if (Convert.ToInt32(hitColliders[k].transform.tag) - Convert.ToInt32(TempHintItem.transform.tag) == 1) //если текущий тег колайдера - последняя цифра в подсказке равно 1 (образно если последняя цифра равно 2 и рядом мы нашли цифру 3)
                {
                    hint = true; //говорим что есть возможный ход, чтобы остановить цикл
                    HintNumbers.Add(hitColliders[k].transform.gameObject); //записываем найденную колайдером цифру как следующий элемент массива

                    break; //выходим из цикла
                }
            }

            HintSearchPlus(HintNumbers[HintNumbers.Count() - 1], hint, a, b); //запускаем метод, который будет искать возможные цифры, которые больше чем точка старта проверки (передаем счетчик, найденная колайдером цифра, ну и была подсказка или нет)
            Array.Clear(hitColliders, 0, hitColliders.Length); //очищаем массив с колайдерами в радиусе от точки проверки
        }
        else if (hint == false) //если поиск в плюс больше не нашел вариантов
        {
            //вот тут записываем полученный вариант в отдельный масив
            GameObject[] output = HintNumbers.GetRange(0, HintNumbers.Count()).ToArray();
            collectHint.Add(output); //сохраняем теги всех объектов в строку через *

            HintNumbers.Clear(); //очищаем масив с цифрами

            if (b == width - 1 && a == width - 1)
            {
                Comparer();
            }
            else if (b == width - 1 && a < width - 1)
            {
                a++;
                b = 0;
                Hint(a, b);

            }
            else if (b < width-1 && a <= width - 1)
            {
                b++;
                Hint(a, b);
            }

        }
    }

    private void Comparer()
    {
        List<int> compare = new List<int>();

        //тут сравниваем все элементы массива collecthint
        foreach (var k in collectHint)
        {
            compare.Add(k.Count());
        }

        var indices = new List<int>();
        int max = int.MinValue;

        for (int i = 0; i < compare.Count(); i++)
        {
            if (compare[i] > max)
            {
                max = compare[i];
                indices.Clear();
            }

            if (compare[i] == max)
            {
                indices.Add(i);
            }
        }

        int rndm = indices[UnityEngine.Random.Range(0, indices.Count())];

        HintNumbers.Clear();

        for (int i = 0; i < collectHint[rndm].Count(); i++)
        {
            HintNumbers.Add(collectHint[rndm][i]);
        }

        //Debug.LogError(collectHint.Count());
        collectHint.Clear();
        Draw(true); //рисуем линии между цифрами подсказок и увеличиваем их размеры
    }
       
    public void Draw(bool draw) //метод по рисованию линий между цифрами при подсказках
    {
        int count = -1; //счетчик равно -1, иначе будет рисовать не верно

        if (draw == true) //если драв = тру то рисуем
        {
            for (int i = 0; i < HintNumbers.Count(); i++) //для всех цифр в массиве с собранными подсказками
            {
                if (HintNumbers[i] != null) //если элемент массива не равно нуль
                {
                    HintNumbers[i].transform.localScale *= 1.25f; //увеличиваем размер цифры
                    HintNumbers[i].GetComponent<BoxCollider2D>().size = new Vector2(0.6f, 0.6f); //делаем коллайдер стандартного размера
                    count++; //увеличиваем счетчик
                }
            }
            for (int j = 0; j < count; j++) //для всех линий в рамках счетчика цифр
            {
                lines[j].SetPosition(0, HintNumbers[j].transform.position); //ставим стартовую точку линии
                lines[j].SetPosition(1, HintNumbers[j+1].transform.position); //ставим конечную точку линии
                lines[j].gameObject.SetActive(true); //показываем линию
            }

        }
        else if (draw == false) //если фелс то убираем нарисованное
        {
            for (int i = 0; i < HintNumbers.Count(); i++) //для всех цифр в массиве с собранными подсказками
            {
                if (HintNumbers[i] != null) //если элемент массива не равно нуль
                {
                    HintNumbers[i].transform.localScale = Vector3.one * scaleBoard; //делаем размер цифр стандартным
                    HintNumbers[i].GetComponent<BoxCollider2D>().size = new Vector2(0.76f, 0.76f); //делаем коллайдер стандартного размера
                }
            }

            HintNumbers.Clear(); //очищаем масив с собранными цифрами в подсказке
            
            for(int j = 0; j < lines.Length; j++) //для всех линий
            {
                lines[j].gameObject.SetActive(false); //выключаем линию
            }

        }
    }

    public void Refill(bool layer) //метод по перемешиванию поля, ну а если точнее, то не перемешивание а заполнение новыми цифрами
    {
        int refillI = Convert.ToInt32(SaveSystem.Decrypt(refill));

        if (layer == false) //если метод был запущен не со слоя когда нет возможных ходов
        {
            if (refillI > 0 && PlayerResource.Instance.GameIsPaused != true) //если количество перемешиваний больше 0 и игра не на паузе
            {
                Draw(false); //выключаем все нарисованные линии между цифрами

                for (int i = 0; i < width; i++) //столбцы
                {
                    for (int j = 0; j < height; j++) //рядки
                    {
                        if (allDots[i, j] != null) //если нужная ячейка не нуль, содержит цифры
                        {
                            Destroy(allDots[i, j]); //удаляем все собранные объекты
                            allDots[i, j] = null; //гвоорим что ячейка равна нуль
                        }
                    }
                }

                Shuffle(); //перемешиваем доску 
                SetUp(); //ставим новые цифры на поле
                CollectBoardToSave(); //сохраняем новые цифры с строку для сохранения
                Firebase.Analytics.FirebaseAnalytics.LogEvent("Button_click", "Button", "Refill");
                refillI--; //отнимаем количество доступных перемешиваний

                if (refillI == 0) //если внезапно количество подсказок стало равно 0
                {
                    ui.RefillButton.gameObject.SetActive(false); //выключаем кнопку перемешать поле
                    ui.AdRefillButton.gameObject.SetActive(true); //включаем кнопку просмотра рекламы для перемешивания поля

                    ui.RefillButtonLayer.gameObject.SetActive(false); //выключаем кнопку перемешать поле на слое нет ходов
                    ui.AdRefillButtonLayer.gameObject.SetActive(true); //включаем кнопку просмотра рекламы для перемешивания поля на слое нет ходов
                }
            }
        }

        if (layer == true) //если метод был запущен со слоя когда нет возможных ходов
        {
            if (refillI > 0) //если количество перемешиваний больше 0
            {
                Time.timeScale = 1f; //выключаем паузу в игре
                ui.NoMatchLayer.SetActive(false); //выключаем слой нет возможных ходов
                PlayerResource.Instance.GameIsPaused = false; //говорим что уже не пауза
                AdMob_baner.Instance.Hide(); //прячем рекламный банер

                Draw(false); //выключаем все нарисованные линии между цифрами

                for (int i = 0; i < width; i++) //столбцы
                {
                    for (int j = 0; j < height; j++) //рядки
                    {
                        if (allDots[i, j] != null) //если нужная ячейка не нуль, содержит цифры
                        {
                            Destroy(allDots[i, j]); //удаляем все собранные объекты
                            allDots[i, j] = null; //говорим что ячейка равна нуль
                        }
                    }
                }

                Shuffle(); //перемешиваем доску 
                SetUp(); //ставим новые цифры на поле
                CollectBoardToSave(); //сохраняем новые цифры с строку для сохранения

                refillI--; //отнимаем количество доступных перемешиваний

                if (refillI == 0) //если внезапно количество подсказок стало равно 0
                {
                    ui.RefillButton.gameObject.SetActive(false); //выключаем кнопку перемешать поле
                    ui.AdRefillButton.gameObject.SetActive(true); //включаем кнопку просмотра рекламы для перемешивания поля

                    ui.RefillButtonLayer.gameObject.SetActive(false); //выключаем кнопку перемешать поле на слое нет ходов
                    ui.AdRefillButtonLayer.gameObject.SetActive(true); //включаем кнопку просмотра рекламы для перемешивания поля на слое нет ходов
                }
            }
        }

        refill = SaveSystem.Encrypt(Convert.ToString(refillI));
        ToPlayerResources("refill");
    }

    public void ToPlayerResources(string data) //убрал обновление данных из апдейта, вызывать этот метод с параметром стринг, когда нужен обновить данные в PlayerResources и в полях на экране, тут потом мы их будем кодировать/декодировать
    {
        if (PlayerResource.Instance.gameMode == "normal") //если нормальный режим, грузим данные для поля из переменных нормального режима, описание переменных см выше
        {

            switch (data) //вешаем на элементы ui текст
            {
                case "turnx2":
                    ui.turnLeftText.text = SaveSystem.Decrypt(turnx2); //количество подсказок
                    PlayerResource.Instance.turnx2N = turnx2;
                    break;

                case "hints":
                    ui.hintcount.text = SaveSystem.Decrypt(hints); //количество подсказок
                    PlayerResource.Instance.hintN = hints;
                    break;

                case "refill":
                    ui.refillcount.text = SaveSystem.Decrypt(refill); //количество перемешиваний
                    ui.refillcountLayer.text = SaveSystem.Decrypt(refill); //количество перемешиваний в слое конца игры
                    PlayerResource.Instance.refillN = refill;

                    break;

                case "score":
                    ui.scoreText.text = SaveSystem.Decrypt(score); //очки
                    PlayerResource.Instance.scoreN = score;
                    break;

                case "hiScore":
                    ui.HighscoreText.text = SaveSystem.Decrypt(hiScore); //макс очки
                    PlayerResource.Instance.hiScoreN = hiScore;
                    break;

                case "loadedBoard":
                    PlayerResource.Instance.loadedBoardN = loadedBoard;
                    break;

                case "endGame":
                    PlayerResource.Instance.EndGameN = endGame;
                    break;

                case "AdReward":
                    PlayerResource.Instance.AdRewardN = AdReward;
                    break;

                case "level":
                    PlayerResource.Instance.levelN = level;
                    break;

                case "damage":
                    PlayerResource.Instance.damageN = damage;

                    break;

                default:
                    Debug.Log("default");
                    break;
            }
        }

        else if (PlayerResource.Instance.gameMode == "timetrial") //если режим на время, грузим данные из переменных режима на время
        {
            switch (data) //в зависимости от размера поля меняем сложность (для рандома цифр) и размер обьектов поля
            {
                case "turnx2":
                    ui.turnLeftText.text = SaveSystem.Decrypt(turnx2); //количество подсказок
                    PlayerResource.Instance.turnx2T = turnx2;
                    break;

                case "turnTime":
                    PlayerResource.Instance.turnTime = turnTime;
                    break;

                case "hints":
                    ui.hintcount.text = SaveSystem.Decrypt(hints); //количество подсказок
                    PlayerResource.Instance.hintT = hints;
                    break;

                case "refill":
                    ui.refillcount.text = SaveSystem.Decrypt(refill); //количество перемешиваний
                    ui.refillcountLayer.text = SaveSystem.Decrypt(refill); //количество перемешиваний в слое конца игры
                    PlayerResource.Instance.refillT = refill;

                    break;

                case "score":
                    ui.scoreText.text = SaveSystem.Decrypt(score); //очки
                    PlayerResource.Instance.scoreT = score;
                    break;

                case "hiScore":
                    ui.HighscoreText.text = SaveSystem.Decrypt(hiScore); //макс очки
                    PlayerResource.Instance.hiScoreT = hiScore;
                    break;

                case "loadedBoard":
                    PlayerResource.Instance.loadedBoardT = loadedBoard;
                    break;

                case "endGame":
                    PlayerResource.Instance.EndGameT = endGame;
                    break;

                case "AdReward":
                    PlayerResource.Instance.AdRewardT = AdReward;
                    break;

                case "level":
                    PlayerResource.Instance.levelT = level;
                    break;

                case "damage":
                    PlayerResource.Instance.damageT = damage;

                    break;

                default:
                    Debug.Log("default");
                    break;
            }
        }
    }

    public void AdTurnX2()
    {
        ui.DamageX2Button.interactable = false; //выключаем интерактивность кнопки получение доп подсказок
        ui.ScoreX2Button.interactable = false;
        ui.DamageX2.gameObject.SetActive(false); //выключаем картинку просмотреть видео рекламу за +3 подсказки
        ui.ScoreX2.gameObject.SetActive(false);
        ui.DamageX2Loading.gameObject.SetActive(true); //включаем анимацию загрузки рекламы
        ui.ScoreX2Loading.gameObject.SetActive(true);

        AdMob_baner.Instance.OnGetMoreTurnX2Clicked(); //запускаем просмотр видео рекламы для подсказок в скрипте адмоб
    }


    public void AdTurnX2Recieve() //если видео реклама была просмотрена, получение доп подсказок
    {
        int turnX2I = 30;
        turnx2 = SaveSystem.Encrypt(Convert.ToString(turnX2I)); ; //ставим количество подсказок равным 3
                ui.turnLeft.gameObject.SetActive(true);
        ToPlayerResources("turnx2");

        ui.DamageX2.gameObject.SetActive(true); //включаем картинку что реклама доступна
        ui.DamageX2Loading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы
        ui.ScoreX2.gameObject.SetActive(true); //включаем картинку что реклама доступна
        ui.ScoreX2Loading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы


        pause.Resume();
    }

    public void AdTurnX2Close() //если была закрыта реклама для получения доп подсказок
    {
        ui.DamageX2Button.interactable = true; //выключаем интерактивность кнопки получение доп подсказок
        ui.ScoreX2Button.interactable = true;
        ui.DamageX2.gameObject.SetActive(true); //включаем картинку что реклама доступна
        ui.ScoreX2.gameObject.SetActive(true); //включаем картинку что реклама доступна
        ui.ScoreX2Loading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы
        ui.DamageX2Loading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы

        pause.Resume();
    }


    public void AdHint() //если была нажата кнопка просмотреть виде рекламу для получения доп подсказок
    {
        ui.AdHintButton.interactable = false; //выключаем интерактивность кнопки получение доп подсказок
        ui.AdsHint.gameObject.SetActive(false); //выключаем картинку просмотреть видео рекламу за +3 подсказки
        ui.AdsHintLoading.gameObject.SetActive(true); //включаем анимацию загрузки рекламы
        ui.AdHintButton.GetComponentInChildren<Text>().text = ""; //меняем количество доступных доп подсказок за просмотр рекламы на счетчике

        AdMob_baner.Instance.OnGetMoreHintClicked(); //запускаем просмотр видео рекламы для подсказок в скрипте адмоб
    }

    public void AdHintRecieve() //если видео реклама была просмотрена, получение доп подсказок
    {
        int hintsI = 3;
        hints = SaveSystem.Encrypt(Convert.ToString(hintsI)); ; //ставим количество подсказок равным 3
        ToPlayerResources("hints");

        ui.HintButton.gameObject.SetActive(true); //включаем кнопку посмотреть подсказку
        ui.AdHintButton.gameObject.SetActive(false); //выключаем кнопку просмотреть видео рекламу за +3 подсказки

        ui.AdsHint.gameObject.SetActive(true); //включаем картинку что реклама доступна
        ui.AdHintButton.GetComponentInChildren<Text>().text = "+3"; //меняем количество получаемых подсказок за просмотр рекламы на счетчике
        ui.AdsHintLoading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы

        ui.AdHintButton.interactable = true; //включаем интерактивность кнопки посмотреть рекламу за подсказки

        pause.Resume();
    }

    public void AdHintClose() //если была закрыта реклама для получения доп подсказок
    {
        ui.AdHintButton.interactable = true; //включаем интерактивность кнопки посмотреть рекламу за подсказки
        ui.AdsHint.gameObject.SetActive(true); //включаем картинку что реклама доступна
        ui.AdHintButton.GetComponentInChildren<Text>().text = "+3"; //меняем количество получаемых подсказок за просмотр рекламы на счетчике
        ui.AdsHintLoading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы

        pause.Resume();
    }

    public void AdRefill() //если была нажата кнопка просмотреть виде рекламу для перемешивания поля
    {
        ui.AdRefillButton.interactable = false; //выключаем интерактивность кнопки
        ui.AdRefillButtonLayer.interactable = false; //выключаем интерактивность кнопки на слое нет ходов

        ui.AdsRefillOn.gameObject.SetActive(false); //выключаем картинку что реклама доступна
        ui.AdsRefillOff.gameObject.SetActive(false); //выключаем картинку что реклама не доступна
        ui.AdsRefillLoading.gameObject.SetActive(true); //включаем анимацию загрузки рекламы

        ui.AdsRefillOnLayer.gameObject.SetActive(false); //выключаем картинку что реклама доступна на слое нет ходов
        ui.AdsRefillOffLayer.gameObject.SetActive(false); //выключаем картинку что реклама не доступна на слое нет ходов
        ui.AdsRefillLoadingLayer.gameObject.SetActive(true); //включаем анимацию загрузки рекламы на слое нет ходов


        ui.AdRefillButton.GetComponentInChildren<Text>().text = ""; //меняем количество доступных перемешиваний за просмотр рекламы на счетчике
        ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = ""; //меняем количество доступных перемешиваний за просмотр рекламы на счетчике на слое нет ходов

        AdMob_baner.Instance.OnGetMoreRefillClicked(); //запускаем просмотр видео рекламы для перемешивания поля в скрипте адмоб
    }

    public void AdRefillRecieve() //если видео реклама была просмотрена, перемешивание поля
    {
        AdReward = true; //говорим, что реклама просмотрена, чтобы нельзя было смотреть рекламу несколько раз и перемешивать поле больше чем 1 раз
        ToPlayerResources("AdReward");

        ui.AdsRefillOn.gameObject.SetActive(false); //выключаем картинку что реклама доступна
        ui.AdsRefillOff.gameObject.SetActive(true); //включаем картинку что реклама не доступна
        ui.AdsRefillLoading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы

        ui.AdsRefillOnLayer.gameObject.SetActive(false); //выключаем картинку что реклама доступна на слое нет ходов
        ui.AdsRefillOffLayer.gameObject.SetActive(true); //включаем картинку что реклама не доступна на слое нет ходов
        ui.AdsRefillLoadingLayer.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы на слое нет ходов

        //4 строки ниже нужны для случая когда рекламу стали смотреть со слоя нет ходов, а не из основного интерфейса игры
        Time.timeScale = 1f; //выключаем паузу
        ui.NoMatchLayer.SetActive(false); //выключаем слой что нет ходов
        PlayerResource.Instance.GameIsPaused = false; //гоыорим что уже не пауза
        AdMob_baner.Instance.Hide(); //прячем рекламный банер

        Draw(false); //выключаем все нарисованные линии между цифрами

        for (int i = 0; i < width; i++) //столбцы
        {
            for (int j = 0; j < height; j++) //рядки
            {
                if (allDots[i, j] != null) //если нужная ячейка не нуль, содержит цифры
                {
                    Destroy(allDots[i, j]); //удаляем все собранные объекты
                    allDots[i, j] = null; //говорим что ячейка равна нуль
                }
            }
        }

        Shuffle(); //перемешиваем доску 
        SetUp(); //ставим новые цифры на поле
        CollectBoardToSave(); //сохраняем новые цифры с строку для сохранения

        pause.Resume();
    }

    public void AdRefillClose() //если была закрыта реклама для перемешивания поля
    {
        if (AdReward == false) //если награда не была получена
        {
            ui.AdRefillButton.interactable = true; //включаем интерактивность кнопки
            ui.AdRefillButtonLayer.interactable = true; //включаем интерактивность кнопки на слое нет ходов

            ui.AdsRefillOn.gameObject.SetActive(true); //включаем картинку что реклама доступна
            ui.AdsRefillOff.gameObject.SetActive(false); //выключаем картинку что реклама не доступна
            ui.AdsRefillLoading.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы

            ui.AdsRefillOnLayer.gameObject.SetActive(true); //включаем картинку что реклама доступна на слое нет ходов
            ui.AdsRefillOffLayer.gameObject.SetActive(false); //выключаем картинку что реклама не доступна на слое нет ходов
            ui.AdsRefillLoadingLayer.gameObject.SetActive(false); //выключаем анимацию загрузки рекламы на слое нет ходов


            ui.AdRefillButton.GetComponentInChildren<Text>().text = "1"; //меняем количество доступных перемешиваний за просмотр рекламы на счетчике
            ui.AdRefillButtonLayer.GetComponentInChildren<Text>().text = "1"; //меняем количество доступных перемешиваний за просмотр рекламы на счетчике на слое нет ходов
        }

        pause.Resume();
    }

}