using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleClick : MonoBehaviour
{
    //variable for checking double click
    private bool isTriggered = false;
    private bool isDoubleClick = false;
    private float acceptTime = 0.5f;        //click twice in acceptTime
    private float timerClick = 0.0f;
    private bool firstEnter = true;

    //variable for checking long click
    private float longTime = 1.5f;
    private bool isFirstClick = false;
    private bool isLongClick = false;
    private float timerLong = 0.0f;


    //variable for debug
    private bool debug = false;

    public bool LongClick(float delta)
    {
        if (isFirstClick)
        {
            if (debug)
                Debug.Log("Long isTrigger Enter");

            if(Input.GetMouseButton(0))         //idea from : https://bit.ly/3PgZATU
            {
                if (debug)
                    Debug.Log("**timerLong is increasing");
                timerLong += delta;

                if (timerLong >= longTime)
                {
                    if (debug)
                        Debug.Log("**Long Set variable true");
                    isLongClick = true;
                    timerLong = 0.0f;
                }
            }
            

            else
            {
                if (debug)
                    Debug.Log("Long Set variable false");
                isLongClick = false;
                isFirstClick = false;
            }
        }
        else
        {
            isFirstClick = Input.GetMouseButtonDown(0);

            if (debug)
                Debug.Log("Long isTrigger false");
            timerLong = 0.0f;
            isLongClick = false;
        }


        //return part
        if (isLongClick)
        {
            if (debug)
                Debug.Log("**Long return true");

            isLongClick = false;

            return true;
        }
        else
        {
            if (debug)
                Debug.Log("Long return false");
            return false;
        }
    }

    public bool ClickDouble(float delta)
    {
        timerClick += delta;

        if (isTriggered)
        {
            if (firstEnter)      //initialize timer for doubleClick
            {
                timerClick = 0.0f;
                firstEnter = false;
            }

            if (timerClick <= acceptTime)     //when clicking twice in acceptTime
            {
                isDoubleClick = Input.GetMouseButtonDown(0);

                if (isDoubleClick)
                {
                    //double click statue
                    isTriggered = false;
                    firstEnter = true;
                    if(debug)
                        Debug.Log("$$double click Entered");
                }
                else
                {
                    if (debug)
                        Debug.Log("@@waiting second click");
                    //waiting statue
                }

            }
            else  //when getting out acceptTime
            {
                isTriggered = false;
                //isDoubleClick = false;
                firstEnter = true;

                if (debug)
                    Debug.Log("$$No double click Entered");
            }

            if (debug)
                Debug.Log("@@Clicked once");
        }
        else
        {
            isTriggered = Input.GetMouseButtonDown(0);
        }


        //return part
        if (isDoubleClick)
        {
            if (debug)
                Debug.Log("Now Double Clicked");
            //isDoubleClick = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Initialization()
    {
        firstEnter = true;
        isTriggered = false;
        isDoubleClick = false;
        isLongClick = false;
        timerClick = 0.0f;
        timerLong = 0.0f;
    }
}
