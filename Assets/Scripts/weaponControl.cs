using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class weaponControl : MonoBehaviour
{

    bool collide;
    private int damageText;
    private int quantity;
    private ui ui; //скрипт всего УИ
    public Board board; //объект поля

    private void Start()
    {
        ui = FindObjectOfType<ui>(); //присваиваем скрипт к переменным
        board = FindObjectOfType<Board>();
        collide = false;
        StartCoroutine(MoveKnife()); //запускаем движения ноля к стартовой позиции
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "boss")
        {
            collide = true;
            FindObjectOfType<AudioManager>().Play("attack");
            gameObject.GetComponent<Animator>().SetTrigger("destroy");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            ui.BossHealth(board.damage, board.level); //передаем в ую метод информацию про урон, для изменения шкалы хп босса

            //ниже показ урона над боссом
            Vector3 temp3 = ui.damagePic.transform.position;
            ui.damagePic.transform.position = new Vector3(transform.position.x + 1f, transform.position.y + 1f, transform.position.z);
            ui.damagePic.gameObject.SetActive(true);
            ui.damageText.text = Convert.ToString(damageText);

            //изменение цвета фона в зависимости от размера поля и собранных цифр
            if(board.width == 5)
            {
                if(quantity <=3)
                    ui.damagePic.GetComponent<Image>().color = new Color(0.9921569f, 0.8470588f, 0.2941177f, 1f);
                else if(quantity == 4)
                    ui.damagePic.GetComponent<Image>().color = new Color(1f, 0.4392157f, 0.2627451f, 1f);
                else if (quantity == 5)
                    ui.damagePic.GetComponent<Image>().color = new Color(0.9372549f, 0.3254902f, 0.3137255f, 1f);
            }
            else if (board.width == 7)
            {
                if (quantity <= 3)
                    ui.damagePic.GetComponent<Image>().color = new Color(0.9921569f, 0.8470588f, 0.2941177f, 1f);
                else if (quantity == 4 || quantity == 5)
                    ui.damagePic.GetComponent<Image>().color = new Color(1f, 0.4392157f, 0.2627451f, 1f);
                else if (quantity == 6 || quantity == 7)
                    ui.damagePic.GetComponent<Image>().color = new Color(0.9372549f, 0.3254902f, 0.3137255f, 1f);
            }
            else if (board.width == 9)
            {
                if (quantity <= 3)
                    ui.damagePic.GetComponent<Image>().color = new Color(0.9921569f, 0.8470588f, 0.2941177f, 1f);
                else if (quantity >= 4 && quantity <= 6)
                    ui.damagePic.GetComponent<Image>().color = new Color(1f, 0.4392157f, 0.2627451f, 1f);
                else if (quantity >= 7 && quantity <= 9)
                    ui.damagePic.GetComponent<Image>().color = new Color(0.9372549f, 0.3254902f, 0.3137255f, 1f);
            }
        }
    }

    public void Damage_text(int damage) //показываем урон над боссом
    {
        damageText = damage;
    }

    public void Quantity (int quantity_send) //показываем урон над боссом
    {
        quantity = quantity_send;
    }

    private IEnumerator MoveKnife() 
    {
        
        float step; //количество шагов, зависит от растояния
        float moveTime = 0; //не помню зачем, но нужно
        float speed = 2;  //скорость движения
        Vector3 startPosition = gameObject.transform.position;
        Vector3 endPosition = new Vector3(gameObject.transform.position.x + 10f, gameObject.transform.position.y, gameObject.transform.position.z); //вектор3 стартовой и конечной позиции оружия

        step = (speed / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
        //ниже формула для плавного движения
        while (moveTime <= 1.0f & collide == false)
        {
            moveTime += step;
            gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(0.25f);
        ui.damagePic.gameObject.SetActive(false);
        Destroy(gameObject, 2f);

    }

}
