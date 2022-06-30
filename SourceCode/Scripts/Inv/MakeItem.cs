using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeItem : MonoBehaviour
{
    //variable for figuring out what item is selected
    public GameObject RtCanv;
    private SetInv si;
    private int[] onItem;

    //variable for recipe (how to make final items)
    private int[,] recipe = new int[5, 2];

    private float timer;
    private int madeItem;

    // Start is called before the first frame update
    void Start()
    {                       //Laser, Cannon, Gun
        recipe = new int[,]{ { 0, 0, 1, 1, 65 }, { 0, 1, 2, 3, 66 }, { 1, 1, 2, 3, 67} };        //65 ~ 90 (A ~ Z) is for MadeItem     //**Add Recipe in order
        si = GameObject.Find("Head").GetComponent<SetInv>();
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 1.0f)       //check per 1.0f
        {
            onItem = si.OnBoardItem();
            /*
            if(false)    //working
            {
                Debug.Log(onItem[0]);
                Debug.Log(onItem[1]);
                Debug.Log(onItem[2]);
                Debug.Log(onItem[3]);
                Debug.Log("@@@@@@");
            }
            */
            if(onItem[3] != -1)     //Do checkRecipe when onItem array is full
            {
                madeItem = CheckRecipe(onItem);     
                //Debug.Log("madeItem : " + madeItem);
                //Debug.Log("#####");

                if (madeItem != -1)      //recipe is correct
                {
                    RtCanv.transform.GetComponent<RawImage>().texture = si.DoTexture(madeItem - 65);    //throw made item Number
                }
                else
                {
                    RtCanv.transform.GetComponent<RawImage>().texture = null;
                }

            }
            else
            {
                RtCanv.transform.GetComponent<RawImage>().texture = null;
            }
            

            timer = 0.0f;
        }

    }

    private int CheckRecipe(int[] on)
    {
        //Debug.Log(recipe.Length);   //10

        int corCnt;

        int[] sortedOn = new int[4];
        
        //sortedOn = on;    [Not Soft Copy]
        for(int i = 0; i < 4; i++)  // [Do Hard Copy] [To let "int[] on" for not changing in game scene]
        {
            sortedOn[i] = on[i];
        }

        Array.Sort(sortedOn);
        //Debug.Log("&&&&"+sortedOn[0] + sortedOn[1] + sortedOn[2] + sortedOn[3]);    //save like 0123      [-1 : don't mind] (because always four items needed)

        for(int i = 0; i < recipe.Length / 5; i++)
        {
            corCnt = 0;

            for(int j = 0; j < 4; j++)
            {
                //Debug.Log("J : " + j + " recipe : " + recipe[i, j] + ", sortedOn : " + sortedOn[j]);

                if (sortedOn[j] == recipe[i, j])
                {
                    corCnt++;
                }
                else
                {
                    //break;
                }
            }

            if(corCnt == 4)
            {
                return recipe[i, 4];    //return made item number
            }
        }

        return -1;
    }
}
