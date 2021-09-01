using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeaponControl : MonoBehaviour
{

    bool collide;
    private void Start()
    {
        collide = false;
        StartCoroutine(MoveKnife()); //запускаем движения ноля к стартовой позиции
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "zero")
        {
            collide = true;
            FindObjectOfType<AudioManager>().Play("attack");
            gameObject.GetComponent<Animator>().SetTrigger("destroy");
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    private IEnumerator MoveKnife()
    {

        float step; //количество шагов, зависит от растояния
        float moveTime = 0; //не помню зачем, но нужно
        float speed = 2;  //скорость движения
        Vector3 startPosition = gameObject.transform.position;
        Vector3 endPosition = new Vector3(-5f, 12.5f, gameObject.transform.position.z); //вектор3 стартовой и конечной позиции оружия

        step = (speed / (startPosition - endPosition).magnitude) * Time.fixedDeltaTime; //считаем количество шагов
        //ниже формула для плавного движения
        while (moveTime <= 1.0f & collide == false)
        {
            moveTime += step;
            gameObject.transform.position = Vector3.Lerp(startPosition, endPosition, moveTime);
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject, 2f);
    }

}
