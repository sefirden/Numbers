using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class weaponControl : MonoBehaviour
{

    bool collide;
    public int damageText;
    private ui ui; //скрипт всего УИ

    private void Start()
    {
        ui = FindObjectOfType<ui>(); //присваиваем скрипт к переменным
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
            ui.damageText.gameObject.SetActive(true);
            ui.damageText.text = Convert.ToString(damageText);
        }
    }

    public void Damage_text(int damage) //показываем урон над боссом
    {
        damageText = damage;
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
        yield return new WaitForSeconds(0.5f);
        ui.damageText.gameObject.SetActive(false);
        Destroy(gameObject, 2f);

    }

}
