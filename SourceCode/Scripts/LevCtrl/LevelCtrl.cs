using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCtrl : MonoBehaviour
{
    //variable for second enter this scene;
    private int shipNum;                    //playerPref

    //variable for ship needed fix
    public GameObject[] shipPrefabs;
    private GameObject destShip;        //instantiate ship
    private GameObject defaultPos;      //ship's parent
    private Vector3 chPos;
    public Texture[] madeItem;            //use 48 ~

    private string HaveItem;
    private int[] haveItemList = new int[14];

    //variable for ship init
    private string fixedParts = "";         //playerPref
    private string[] fixedArray = new string[20];       //Cannon, Cannon, Laser, Gun
    private string[] fixedOrigin = new string[20];      //Cannon, Cannon (1), Laser, Gun            //Big_Gun?????
    private int fixNum;

    //variable for FIRST START GAME
    

    // Start is called before the first frame update
    void Start()
    {
        SpawnShip();

        HaveItem = PlayerPrefs.GetString("InvItem");        //like _0_0_1_1_2_2_0_0_3_3_1_48_49_50
        StrToArr();                                         //like {0, 0, 1, 1} int array           //int[] haveItemList

        Debug.Log("HaveItem in LevelCtrl.cs : " + HaveItem);    //_49_-1_-1_-1_2_0_1_-1_-1_-1_-1_-1_-1_-1_2_2_0
        PrintIntArr(haveItemList);                              //49, -1, -1, -1, 2, 0, 1, -1, -1, -1, -1, -1, -1, -1, 2, 2, 0, 0, 0, 0, 0, 0, 0, 
    }

    public int DoFix(GameObject hit)           
    {
        bool isFixed = false;
        OrderArr(fixedOrigin);              //null만 뒤로 몰아 넣음
        OriginToArray(fixedOrigin);         //fixedOrigin -> fixedArray


        string arrstr = "";
        for(int i = 0; i < fixedArray.Length; i++)
        {
            if (fixedArray[i] == null && true)
                break;

            arrstr += fixedArray[i] + ", ";
        }
        Debug.Log("arrstr : " + arrstr);        //Cannon. Laser

        
        for (int i = 0; i < fixedArray.Length; i++)              //Cannon, Laser, Laser
        {
            if(fixedArray[i] == null)
            {
                continue;
            }

            for(int j = 0; j < haveItemList.Length; j++)        //0 0 1 1 48 49 50
            {
                if (isFixed || (haveItemList[j] == -1))
                    break;

                if(fixedArray[i].CompareTo("Laser") == 0)
                {
                    if(haveItemList[j] == 48)       //65
                    {
                        for(int k = 0; k < hit.transform.childCount; k++)
                        {
                            if(hit.transform.GetChild(k).name.CompareTo(fixedOrigin[i]) == 0)   //Cannon, Laser, Laser (1)
                            {
                                hit.transform.GetChild(k).gameObject.SetActive(true);

                                Debug.Log("Use item : " + haveItemList[j] + ", j : " + j);
                                Debug.Log("fix Laser part setactive true");
                                break;
                            }
                        }
                            
                        fixedArray[i] = null;
                        fixedOrigin[i] = null;
                        haveItemList[j] = -1;
                        isFixed = true;
                    }
                }
                else if(fixedArray[i].CompareTo("Cannon") == 0)
                {
                    if (haveItemList[j] == 49)
                    {
                        for (int k = 0; k < hit.transform.childCount; k++)
                        {
                            if (hit.transform.GetChild(k).name.CompareTo(fixedOrigin[i]) == 0)
                            {
                                hit.transform.GetChild(k).gameObject.SetActive(true);

                                Debug.Log("Use item : " + haveItemList[j] + ", j : " + j);
                                Debug.Log("fix Cannon part setactive true");
                                break;
                            }
                        }

                        fixedArray[i] = null;
                        fixedOrigin[i] = null;
                        haveItemList[j] = -1;
                        isFixed = true;
                    }
                }
                else if (fixedArray[i].CompareTo("Gun") == 0)               //wrong??
                {
                    if (haveItemList[j] == 50)
                    {
                        for (int k = 0; k < hit.transform.childCount; k++)
                        {
                            if (hit.transform.GetChild(k).name.CompareTo(fixedOrigin[i]) == 0)
                            {
                                hit.transform.GetChild(k).gameObject.SetActive(true);

                                Debug.Log("Use item : " + haveItemList[j] + ", j : " + j);
                                Debug.Log("fix Gun part setactive true");
                                break;
                            }
                        }

                        fixedArray[i] = null;
                        fixedOrigin[i] = null;
                        haveItemList[j] = -1;
                        isFixed = true;
                    }
                }
            }   //for haveItemList

            if(isFixed)
            {
                break;
            }
        }       //for fixedArray
        
        if(!isFixed)
        {
            Debug.Log("Nothing Fixed");
        }

        //fixedOrigin[index] set null
        OrderArr(fixedOrigin);              //null만 뒤로 몰아 넣음
        OriginToArray(fixedOrigin);         //fixedOrigin -> fixedArray


        string str = "";
        
        for(int i = 0; i < fixedArray.Length; i++)
        {
            if(fixedArray[i] != null)
                str += fixedArray[i];
        }
        Debug.Log("End of DoFix : " + str);
        //Debug.Log("fixedArray : " + GetFixedParts()); //fixedArray

        Array.Sort(haveItemList);
        Array.Reverse(haveItemList);
        string sv = "";
        for (int i = 0; i < haveItemList.Length; i++)
        {
            sv += "_" + haveItemList[i];
        }
        Debug.Log("haveItemList : " + sv);

        PlayerPrefs.SetString("InvItem", sv);


        if(str == null)     //All Parts Fixed
        {
            PlayerPrefs.SetString("OutParts", null);
            return 2;
        }
        else
        {
            PlayerPrefs.SetString("OutParts", str);     //save for later
        }

        if(isFixed)
        {
            return 1;
        }
        return 0;
    }

    private void OrderArr(string[] arr)     //remove empty index
    {
        for(int i = 19; i >= 0 ; i--)
        {
            if(fixedOrigin[i] != null)
            {
                for(int j = 0; j < i; j++)
                {
                    if(fixedOrigin[j] == null)
                    {
                        fixedOrigin[j] = fixedOrigin[i];
                        fixedOrigin[i] = null;
                        break;
                    }
                }
            }
        }
    }
    private void OriginToArray(string[] arr)
    {
        int index = -1;
        int i;

        for(i = 0; i < 20; i++)
        {
            if(arr[i] == null)
            {
                break;
            }

            if(arr[i].Substring(arr[i].Length - 1) == ")")
            {
                fixedArray[++index] = arr[i].Substring(0, arr[i].Length - 4);
            }
            else
            {
                fixedArray[++index] = arr[i];
            }
        }

        for(int j = i; j < fixedArray.Length; j++)
        {
            fixedArray[++index] = null;
        }
    }
    


    private void StrToArr()     //string HaveItem to int[] haveItemList
    {
        haveItemList = new int[14];
        int index = -1;
        char[] temp = HaveItem.ToCharArray();
        bool isSerial = false;
        bool isMinus = false;

        for (int i = 0; i < temp.Length; i++)
        {
            if (temp[i] == '_')
            {
                isSerial = false;
            }
            else if(temp[i] == '-')
            {
                isMinus = true;
            }
            else if(isMinus && (temp[i] == '1'))        //-1
            {
                haveItemList[++index] = (int)Char.GetNumericValue(temp[i]) * -1;        //idea from : https://bit.ly/3zeuAOu
                isMinus = false;
            }
            else if(Char.IsDigit(temp[i]) && !isSerial && !isMinus)
            {
                haveItemList[++index] = (int)Char.GetNumericValue(temp[i]);

                isSerial = true;
            }
            else if(Char.IsDigit(temp[i]) && isSerial && !isMinus)
            {
                haveItemList[index] *= 10;
                haveItemList[index] += (int)Char.GetNumericValue(temp[i]);
            }
        }
    }

    private int Name2MadeNum(string name)
    {
        for (int i = 0; i < 10; i++)
        {
            if (madeItem[i].name.CompareTo(name) == 0)
            {
                //Debug.Log("return : " + i);
                return i;
            }
        }

        return -1;      //Not Exist Item
    }

    public void SpawnShip()
    {
        bool isNew = false;

        if(destShip != null)
        {
            Destroy(destShip);
            isNew = true;
        }
        Debug.Log("destship : " + destShip);

        chPos = new Vector3(0, -100, 700);

        defaultPos = GameObject.Find("DeckWithShip");
        shipNum = UnityEngine.Random.Range(0, 2);
        destShip = Instantiate(shipPrefabs[shipNum]);
        destShip.transform.position = chPos;

        destShip.transform.parent = defaultPos.transform;

        if(isNew || true)
        {
            InitShip(destShip);
        }
        else
        {
            string destParts = PlayerPrefs.GetString("OutParts");                               //OOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOO


        }
    }

    private void InitShip(GameObject ship)
    {
        fixNum = 0;
        int index = 0;
        int i;

        Transform[] child = new Transform[ship.transform.childCount];

        for(i = 0; i < ship.transform.childCount; i++)
        {
            child[i] = ship.transform.GetChild(i);

            if((child[i].name.CompareTo("Spaceship_Base") != 0) && (child[i].name.CompareTo("Line_004") != 0))      //Not Destroy Ship Body, Particle
            {
                if(UnityEngine.Random.Range(0, 2) == 1)     //destroy parts in 50% chance
                {
                    fixedParts += "@" + child[i].name;
                    fixedOrigin[index] = child[i].name;

                    if (child[i].name.Substring(child[i].name.Length - 1) == ")")
                    {
                        fixedArray[index++] = child[i].name.Substring(0, child[i].name.Length - 4);
                    }
                    else
                    {
                        fixedArray[index++] = child[i].name;
                    }
                    

                    Debug.Log("fixedArray : " + fixedArray[index - 1]);     //Cannon, Laser, Laser
                    child[i].transform.gameObject.SetActive(false);
                    fixNum++;
                }
            }
        }

        for(int j = i; j < fixedArray.Length; j++)
        {
            fixedArray[j] = null;
        }

        if(fixNum == 0)     //don't have destroyed parts
        {
            InitShip(ship);
        }
    }

    public string GetFixedParts()
    {
        string rtfix = "";
        bool isFirst = true;

        for(int i = 0; i < fixedArray.Length; i++)
        {
            if(fixedArray[i] == null)
            {
                break;
            }

            if(!isFirst)
            {
                rtfix += ", ";
            }
            else
            {
                isFirst = false;
            }

            rtfix += fixedArray[i];
        }

        return rtfix;
    }

    public string InvSave()
    {
        string rt = "";
        for(int i = 0; i < haveItemList.Length; i++)
        {
            rt += "_" + haveItemList[i];
        }

        return rt;
    }

    private void PrintIntArr(int[] arr)
    {
        string str = "";

        for(int i = 0; i < arr.Length; i++)
        {
            str += arr[i] + ", ";
        }

        Debug.Log(str);
    }
}
