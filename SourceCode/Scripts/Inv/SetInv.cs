using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetInv : MonoBehaviour
{
    public GameObject[] Row = new GameObject[2];    //RawImage array
    public GameObject CraftCanvas;

    //exist item list
    public Texture[] ItemImg = new Texture[10];         //saved Image   [bolt, gear, plate, wires]
    private string[] ItemImgName = new string[10];      //saved item name
    
    //player inventory
    private int[] first = new int[7];               //save items as itemNumber
    private int[] second = new int[7];

    private int[,] grp;

    //crafting canvas
    private int[] onItem = new int[4] {-1, -1, -1, -1 };      //selected item
    private int selectedItemNum = 0;
    public GameObject CraftTable;
    private string[,] itemRoute = new string[4, 2];             //where is selected item's image canvas

    //variable for crafted items
    public Texture[] madeItem = new Texture[10];

    //variable for unSelecting
    private int delIndex;

    //variable for unSelecting from selected canvas
    private GameObject[] selObj = new GameObject[4];
    private int selCnt = 0;
    private int selObjDelIndex;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i< 10; i++)              //get item's names
        {
            if(ItemImg[i] == null)
            {
                break;
            }
            ItemImgName[i] = ItemImg[i].name;
            //Debug.Log("PR : " + ItemImgName[i]);    //working
        }

        //for debug
        //PlayerPrefs.SetString("InvItem", "_0_0_1_1_2_2_0_0_3_3_1");

        InitArr(first, second);
        
        //grp = new int[,]{first, second};

        SetImg(Row, first, second);
    }

    private void SetImg(GameObject[] par, int[] firstList, int[] secondList)
    {
        SetImgPart(par[0], firstList);
        SetImgPart(par[1], secondList);
    }

    private void SetImgPart(GameObject par, int[] itemList)
    {
        Transform temp;
        RawImage tempImg;

        for (int j = 0; j < itemList.Length; j++)
        {
            temp = par.transform.GetChild(j);
            tempImg = temp.GetComponent<RawImage>();

            if (itemList[j] == 0)      //item Number와 일치하는 Image 설정(하는 것으로 변경해야 함)
            {
                tempImg.texture = ItemImg[0];
            }
            else if(itemList[j] == 1)
            {
                tempImg.texture = ItemImg[1];
            }
            else if(itemList[j] == 2)
            {
                tempImg.texture = ItemImg[2];
            }
            else if(itemList[j] == 3)
            {
                tempImg.texture = ItemImg[3];
            }
            else if(itemList[j] == -1)
            {
                tempImg.texture = null;
            }
            else if(itemList[j] == 48)  //A             //Gun [laser]
            {
                tempImg.texture = madeItem[0];
            }
            else if (itemList[j] == 49)                 //Cannon
            {
                tempImg.texture = madeItem[1];
            }
            else if (itemList[j] == 50)                 //Engine [Big_Gun]
            {
                tempImg.texture = madeItem[2];
            }
        }
    }

    private void SetCraftImage()
    {
        SetImgPart(CraftTable, onItem);
    }

    public void DestroyItem()
    {
        bool isFirstRow = true;
        GameObject delBaseItem = null;

        for (int i = 0; i < 4; i++)
        {
            if(onItem[i] == -1)
            {
                break;
            }

            
            GameObject temp = GameObject.Find(itemRoute[i, 0]);
            Debug.Log(temp);

            if (temp.name.CompareTo("FirstRow") == 0)
            {
                isFirstRow = true;
            }
            else
            {
                isFirstRow = false;
            }

            for (int j = 0; j < 7; j++) //item's col
            {
                GameObject childtemp = temp.transform.GetChild(j).gameObject;

                if (childtemp.name.CompareTo(itemRoute[i, 1]) != 0)
                {
                    continue;
                }
                else
                {
                    delBaseItem = childtemp;
                    Debug.Log("delBaseItem : " + delBaseItem);

                    //delBaseItem's RawImage texture, color, collider istrigger initialize needed
                    delBaseItem.GetComponent<RawImage>().texture = null;
                    delBaseItem.GetComponent<RawImage>().color = Color.white;
                    delBaseItem.GetComponent<Collider>().isTrigger = false;

                    if (isFirstRow)  //firstRow
                    {
                        first[j] = -1;
                    }
                    else        //secondRow
                    {
                        second[j] = -1;
                    }

                    break;
                }
            }

            onItem[i] = -1;
            itemRoute[i, 0] = null;
            itemRoute[i, 1] = null;
        }

        selectedItemNum = 0;
        selCnt = 0;

        SetCraftImage();                //craft canvas clear
        SortArray();                    //push  -1 to end
        SetImg(Row, first, second);     //init inventory canvas(7*2)

        SaveInvPref();
    }

    public bool SelectItem(string name, RaycastHit hit)     //only use item slot
    {
        //Debug.Log("enter GetItem");
        int itemNum = Name2Num(name);
        //Debug.Log("itemNum " + itemNum);

        if(itemNum != -1)
        {
            //Debug.Log("get Item " + itemNum);

            if(selectedItemNum < 4)         
            {
                onItem[selectedItemNum] = itemNum;

                itemRoute[selectedItemNum, 0] = hit.collider.transform.parent.name;     //save gameobject as parent's and hit's name  //using two dimension array
                itemRoute[selectedItemNum, 1] = hit.collider.name;

                selectedItemNum++;

                //for unselect in selected canvas
                selObj[selCnt++] = hit.transform.gameObject;
                //Debug.Log("selObj : " + selObj[selCnt - 1]);    //can get name [also parent name (use transform.parent)]
            }       

            SetCraftImage();
            ShowOnItem();

            //Debug.Log("selectedItemNum : " + selectedItemNum + ", selCnt : " + selCnt);
            return true;
        }
        else
        {
            Debug.Log("Not Exist Item");
            return false;
        }
    }

    public void UnSelectItem(RaycastHit hit)
    {
        //Debug.Log("do unselect");

        if (hit.collider.CompareTag("Item"))
        {
            for(int i = 0; i < itemRoute.Length / 2 ; i++)
            {
                if( ((itemRoute[i, 0].CompareTo(hit.collider.transform.parent.name)) == 0) && ((itemRoute[i, 1].CompareTo(hit.collider.name)) == 0) )
                {
                    delIndex = i;
                    //Debug.Log("found data");


                    //Debug.Log("par name : " + hit.collider.transform.parent.name);      //ex. FirstRow
                    //Debug.Log("item name : " + hit.collider.name);                      //ex. HasItem2

                    selObjDelIndex = SearchSelObj(selObj, hit.collider.transform.parent.name, hit.collider.name);
                    Debug.Log("selobjdelIndex : " + selObjDelIndex);

                    SearchDelObj(selObj, selObjDelIndex);
                    
                    break;
                }
                else
                {
                    delIndex = -1;
                }

                //Debug.Log("do itemRoute length");   //8
            }

            if (delIndex == -1)
                return;

            //onItem[delIndex] = -1;
            SortCanv(delIndex);
            SetCraftImage();

            selectedItemNum--;
            selCnt--;

            Debug.Log("selectedItemNum : " + selectedItemNum + ", selCnt : " + selCnt);

            ShowOnItem();
        }
        else if(hit.collider.CompareTag("Select"))      
        {
            //Debug.Log(hit.collider.name);                                   //ex. checkedItem1      [Not Valid Value]
            //Debug.Log(hit.collider.transform.parent.name);                  //ex. craftCanvas
            Debug.Log(hit.collider.GetComponent<RawImage>().texture.name);  //ex. bolt-2024571_1280
            string hitTexName = hit.collider.GetComponent<RawImage>().texture.name;


            int k = 0;
            while((k < 4) && (selObj[k] != null))
            {
                //Debug.Log(selObj[k].GetComponent<RawImage>().texture.name);

                if(hitTexName.CompareTo(selObj[k].GetComponent<RawImage>().texture.name) == 0)      //Find texture's name is same in selObj[]  //and delete it {No Consider is it}(correct??)
                {
                    //Debug.Log("selobj's name : " + selObj[k].name);                         //ex. HasItem2
                    //Debug.Log("selobj's name : " + selObj[k].transform.parent.name);        //ex. FirstRow

                    SetInitCanvInvItem(selObj[k].transform.parent.name, selObj[k].name);            //Function for 'inventory canvas's items should be changed "color", "istriggered"'
                    SearchDelObj(selObj, k);                                                        



                    break;
                }

                k++;
            }
            //Debug.Log("===========");

            
            SortCanv(k);
            SetCraftImage();
            selectedItemNum--;
            selCnt--;

            ShowOnItem();
            Debug.Log("selectedItemNum : " + selectedItemNum + ", selCnt : " + selCnt);
        }
    }

    public void GetMadeItem(RaycastHit hit)
    {
        bool isFirstRow = true;
        int getItemNum = Name2MadeNum(hit.transform.GetComponent<RawImage>().texture.name); //made item num
        //Debug.Log("@@@@getItemNum's tex Name : " + hit.transform.GetComponent<RawImage>().texture.name);

        hit.transform.GetComponent<RawImage>().texture = null;
        GameObject delBaseItem = null;
        Debug.Log(Row.Length);  //2

        for (int i = 0; i < 4; i++)      //remove selected Items(cnt == 4) when get made item
        {
            GameObject temp = GameObject.Find(itemRoute[i, 0]);
            Debug.Log(temp);

            if (temp.name.CompareTo("FirstRow") == 0)
            {
                isFirstRow = true;
            }
            else
            {
                isFirstRow = false;
            }

            for (int j = 0; j < 7; j++) //item's col
            {
                GameObject childtemp = temp.transform.GetChild(j).gameObject;

                if (childtemp.name.CompareTo(itemRoute[i, 1]) != 0)
                {
                    continue;
                }
                else
                {
                    delBaseItem = childtemp;
                    Debug.Log("delBaseItem : " + delBaseItem);

                    //delBaseItem's RawImage texture, color, collider istrigger initialize needed
                    delBaseItem.GetComponent<RawImage>().texture = null;
                    delBaseItem.GetComponent<RawImage>().color = Color.white;
                    delBaseItem.GetComponent<Collider>().isTrigger = false;

                    if (isFirstRow)  //firstRow
                    {
                        first[j] = -1;
                    }
                    else        //secondRow
                    {
                        second[j] = -1;
                    }

                    break;
                }
            }

            onItem[i] = -1;
            itemRoute[i, 0] = null;
            itemRoute[i, 1] = null;

        }

        selectedItemNum = 0;
        selCnt = 0;
        Debug.Log("getItemNum : " + getItemNum);    //0->48, 1->49


        int inputtedNum = -1;

        if (getItemNum != -1)
        {
            //get made item
            for (int i = 0; i < 7 * 2; i++)
            {
                if (i < 7)
                {
                    if (first[i] == -1)                        
                    {
                        first[i] = getItemNum + 48;
                        inputtedNum = i;
                        break;
                    }
                }
                else
                {
                    if (second[i - 7] == -1)
                    {
                        second[i - 7] = getItemNum + 48;
                        inputtedNum = i;
                        break;
                    }
                }
            }
        }
        getItemNum = -1;

        SetCraftImage();                //craft canvas clear
        SortArray();                    //push  -1 to end
        SetImg(Row, first, second);     //init inventory canvas(7*2)

        //For Not select made Item with tag     //[item] tag || [editorOnly] tag

        GameObject parTemp;
        GameObject chTemp;
        int ind = inputtedNum % 7;

        //Change Tag Not Interaction
        if (inputtedNum < 7)
        {
            parTemp = GameObject.Find("FirstRow");
            chTemp = parTemp.transform.GetChild(ind).gameObject;
        }
        else
        {
            parTemp = GameObject.Find("SecondRow");
            chTemp = parTemp.transform.GetChild(ind).gameObject;
        }

        chTemp.tag = "NotInteraction";

        //save write through //playerpref
        SaveInvPref();
    }

    private void SortArray()
    {
        int[] temp = new int[14];

        for(int i = 0; i < 14; i++)     //first,second -> temp
        {
            if(i < 7)
            {
                temp[i] = first[i];
            }
            else
            {
                temp[i] = second[i - 7];
            }
        }

        Array.Sort(temp);       //-1 -1 2 3 ...
        Array.Reverse(temp);    //3 2 1 0 -1 -1...

        for(int i = 0; i < 14; i++)     //temp -> first, second
        {
            if(i < 7)
            {
                first[i] = temp[i];
            }
            else
            {
                second[i - 7] = temp[i];
            }
        }
    }
    
    private void SetInitCanvInvItem(string parName, string name)
    {
        GameObject initObj = new GameObject();
        Transform temp;

        int cnt = GameObject.Find(parName).transform.childCount;

        for(int i = 0; i < cnt; i++)
        {
            temp = GameObject.Find(parName).transform.GetChild(i);

            if(temp.name.CompareTo(name) == 0)
            {
                initObj = temp.gameObject;

                break;
            }
        }
        

        Debug.Log("inited GameObject : " + initObj);        //ex. HasItem2
        initObj.transform.GetComponent<RawImage>().color = Color.white;
        initObj.GetComponent<Collider>().isTrigger = false;

    }

    public string PlayerprefArray()
    {
        string str = "";

        for(int i = 0; i < 7; i++)
        {
            str += "_" + first[i];
        }
        for (int i = 0; i < 7; i++)
        {
            str += "_" + second[i];
        }

        return str;
    }

    private void SaveInvPref()
    {
        string sv = PlayerprefArray();

        PlayerPrefs.SetString("InvItem", sv);
    }

    private int SearchSelObj(GameObject[] selObj, string parName, string name)
    {
        for(int i = 0; i < selObj.Length; i++)
        {
            if(name.CompareTo(selObj[i].transform.name) == 0)       //selObj have gameobject(name, parent name)
            {
                if(parName.CompareTo(selObj[i].transform.parent.name) == 0)
                {
                    return i;
                }
            }
        }

        return -1;
    }
    private void SearchDelObj(GameObject[] selObj, int delIndex) 
    {
        selObj[delIndex] = null;

        for(int i = delIndex; i < selObj.Length - 1; i++)
        {
            selObj[i] = selObj[i + 1];
        }

        selObj[selObj.Length - 1] = null;

        ShowSelObj();
    }
    private void ShowSelObj()
    {
        Debug.Log("sel : " + selObj[0] + " " + selObj[1] + " " + selObj[2] + " " + selObj[3]);
    }

    private void SortCanv(int delIndex)   //maybe only use crafting canvas
    {                                       //onItem[delIndex] should be deleted
        int del = delIndex;

        while(del < 3)                      //"itemRoute, onItem" should be changed (error occurs with forgetting itemRoute)
        {
            onItem[del] = onItem[del + 1];
            itemRoute[del, 0] = itemRoute[del + 1, 0];
            itemRoute[del, 1] = itemRoute[del + 1, 1];
            del++;
        }

        onItem[3] = -1;
        itemRoute[3, 0] = null;
        itemRoute[3, 1] = null;
    }

    private int Name2Num(string name)
    {
        for(int i = 0; i < 10; i++)
        {
            if(ItemImgName[i].CompareTo(name) == 0)
            {
                //Debug.Log("return : " + i);
                return i;   
            }
        }

        return -1;      //Not Exist Item
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

    public void ShowOnItem()
    {
        string rt = null;

        for(int i = 0; i < onItem.Length; i++)
        {
            rt += "index[" + i + "] : " + onItem[i] + " ";
        }

        //Debug.Log(rt);
    }

    private void InitArr(int[] arr, int[] arr2)        //playerPref
    {
        MakeEmpty(arr);     //clear array -1
        MakeEmpty(arr2);

        string itList = PlayerPrefs.GetString("InvItem"); 
        Debug.Log("itList : " + itList);    //like _0_3_1_2 ...     //work well until this

        int count = -1;
        char[] itChar = itList.ToCharArray();
        int[] itemList = new int[14];
        bool serial = false;
        bool isMinus = false;

        int i, j;

        for (i = 0; (i < itChar.Length) && (count < itemList.Length - 1); i++)      //get item num array
        {
            //Debug.Log("itChar length : " + itChar.Length);      //93???
            //Debug.Log("itChar[i] : " + itChar[i]);

            if (itChar[i] == '_')
            {
                serial = false;
            }
            else if(itChar[i] == '-')
            {
                isMinus = true;
            }
            else if(itChar[i] == '1' && isMinus)    //-1
            {
                itemList[++count] = -1;
                isMinus = false;
            }
            else if (char.IsDigit(itChar[i]) && !serial && !isMinus)
            {
                itemList[++count] = (int)Char.GetNumericValue(itChar[i]);
                //Debug.Log("count : " + count);
                serial = true;
            }
            else if (char.IsDigit(itChar[i]) && serial && !isMinus)
            {
                //Debug.Log("@count : " + count);
                itemList[count] *= 10;
                itemList[count] += (int)Char.GetNumericValue(itChar[i]);
                //count++;
                serial = true;
            }
        }

        for (j = count + 1; j < 14; j++) {      //make full array[14]
            itemList[j] = -1;
        }
       
        Debug.Log("end of itemList");
        //Debug.Log("size of count : " + count);      //19

        Array.Sort(itemList);       //-1 -1 0 1 ...
        Array.Reverse(itemList);    //2 1 0 -1 -1 ...


        string temp = "";
        for(i = 0; i < 14; i++)      //check itemList       //crop valid index 14
        {
            temp += "_" + itemList[i];
        }
        Debug.Log("temp : " + temp);        //{//_1_2_0 -> 49 50 48        //_1_3_1_1 -> 49 51 49 49}

        for (i = 0; i < 14; i++)
        {
            if(i < 7)
            {
                arr[i] = itemList[i];// - 48;
            }
            else if(i < 14)
            {
                arr2[i - 7] = itemList[i];// - 48;
            }
        }

        PlayerPrefs.SetString("InvItem", temp);
    }

    public int[] OnBoardItem()      //return selected item array [MakeItem.cs]
    {
        return onItem;
    }
    public Texture DoTexture(int madeIndex)
    {
        return madeItem[madeIndex];
    }

    private void MakeEmpty(int[] arr)
    {
        for(int i = 0; i < arr.Length; i++)
        {
            arr[i] = -1;
        }
    }

    
}
