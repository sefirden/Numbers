using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Experimental.Rendering.Universal;

public class LampControl : MonoBehaviour
{
    public float minValue;
    public float maxValue;
    public float blinkTimer;
    private float basicIntensity;


    void Start()
    {
        basicIntensity = gameObject.GetComponent<Light2D>().intensity;
        if (gameObject.activeSelf)
            Invoke("RandomBlinkStart", UnityEngine.Random.Range(minValue, maxValue));
    }

    private void RandomBlinkStart()
    {
        StartCoroutine(BlinkRandomly());
    }


    private IEnumerator BlinkRandomly()
    {
        while (gameObject.activeSelf)
        {
            gameObject.GetComponent<Light2D>().intensity = UnityEngine.Random.Range(basicIntensity - (basicIntensity / 10), basicIntensity + (basicIntensity / 10));

            yield return new WaitForSeconds(blinkTimer);
            gameObject.GetComponent<Light2D>().intensity = basicIntensity;

            yield return new WaitForSeconds(UnityEngine.Random.Range(minValue, maxValue));
        }
    }
}
