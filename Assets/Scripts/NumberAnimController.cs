using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberAnimController : MonoBehaviour
{
    private Animator animator;
    string[] clips;

    private void Awake()
    {
        // Get the animator component
        animator = GetComponent<Animator>();

        // Get all available clips
        clips = new string[] {"idle0", "idle1", "idle2" }; 

    }


    void Start()
    {
        Invoke("RandomAnimStart", UnityEngine.Random.Range(1, 10));
    }


    private void RandomAnimStart()
    {
        StartCoroutine(PlayRandomly());
    }


    private IEnumerator PlayRandomly()
    {
        while (true)
        {
            animator.SetTrigger(clips[UnityEngine.Random.Range(0, clips.Length)]);

            yield return new WaitForSeconds(UnityEngine.Random.Range(5,10));
        }
    }
}
