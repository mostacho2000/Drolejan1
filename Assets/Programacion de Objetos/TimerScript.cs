using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerScript : MonoBehaviour
{
    public float timer = 0;
    public TMP_Text timertext;
    public Action timercallback;
    void Start()
    {

    }

    public void Settimer(float timeparameter, Action callbackparameter)
    {
        timer = timeparameter;
        timercallback = callbackparameter;
    }


    void Update()
    {

        if (timer > 0)
        {


            timer -= Time.deltaTime;
            timertext.text = timer.ToString();
            if (timeisup())
            {
                Debug.Log("Termino el tiempo");
                timertext.text = "Termino el tiempo";
                timercallback?.Invoke();
            }
        }
    }

    private bool timeisup()
    {
        if (timer <= 0)
        {
            return true;
        }
        else return false;
    }

    public void addtimer(float addtime)
    {
        timer += addtime;
    }
}
