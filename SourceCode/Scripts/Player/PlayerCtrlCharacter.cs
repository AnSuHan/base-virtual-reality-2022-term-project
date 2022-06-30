using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCtrlCharacter : MonoBehaviour
{
    //variable for double click
    private DoubleClick dc;

    //variable for player moving
    public GameObject mainCam;
    private GameObject ship;
    private float speed = 0.1f;

    //variable for controling player statue
    private bool mode = true;   //true : moving, false : stop

    //variable for getting items (using playerpref)
    private string saveAs = null;       //save like "_0_3_2_1_2 ..."
    public Texture[] itemImg = new Texture[10];

    //variable for change scene preparing
    public GameObject stopCanvas;
    private Vector3 lookDir;
    public Button flyBtn;
    public Button invBtn;

    //variable for deviding each scene
    private bool isItemScene = true;

    //variable for change script for moving
    private ScriptCtrl sc;
    private Chmod cm;

    // Start is called before the first frame update
    void Start()
    {
        saveAs = ChsaveAs();
        dc = GetComponent<DoubleClick>();
        ship = GameObject.Find("PlayerUnit01");

        string fly = "FlyingScene";
        if (fly.CompareTo(SceneManager.GetActiveScene().name) == 0)     //https://bit.ly/3yR6cCs
        {
            isItemScene = false;
            sc = GetComponent<ScriptCtrl>();
            cm = GameObject.Find("PlayerUnit01").GetComponent<Chmod>();
        }
        else
        {
            stopCanvas.SetActive(false);
        }
    }

    private string ChsaveAs()       //remove -1
    {
        string itList = PlayerPrefs.GetString("InvItem");
        Debug.Log("itList : " + itList);    //like _0_3_1_2 ...     //work well until this

        int count = -1;
        char[] itChar = itList.ToCharArray();
        int[] itemList = new int[14];
        bool serial = false;
        bool isMinus = false;

        for (int i = 0; (i < itChar.Length) && (count < itemList.Length - 1); i++)      //get item num array
        {
            //Debug.Log("itChar length : " + itChar.Length);      //93???
            //Debug.Log("itChar[i] : " + itChar[i]);

            if (itChar[i] == '_')
            {
                serial = false;
            }
            else if (itChar[i] == '-')
            {
                isMinus = true;
            }
            else if (itChar[i] == '1' && isMinus)    //-1
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

        Debug.Log("end of itemList");
        //Debug.Log("size of count : " + count);      //19

        Array.Sort(itemList);       //-1 -1 0 1 ...
        Array.Reverse(itemList);    //2 1 0 -1 -1 ...


        string temp = "";
        for (int i = 0; i < 14; i++)      //check itemList       //remove -1 clearly
        {
            if(itemList[i] == -1)
            {
                continue;
            }

            temp += "_" + itemList[i];
        }
        Debug.Log("temp : " + temp);        //{//_1_2_0 -> 49 50 48        //_1_3_1_1 -> 49 51 49 49}

        PlayerPrefs.SetString("InvItem", temp);

        return temp;
    }

    // Update is called once per frame
    void Update()
    {
        if(mode && isItemScene)
        {
            this.transform.Translate(mainCam.transform.forward * speed);
        }
        
        if(Input.GetMouseButtonDown(1))     //use only debug
        {
            this.transform.Translate(mainCam.transform.forward * 5);
        }

        if (dc.ClickDouble(Time.deltaTime))     //control moving and stop
        {
            mode = !mode;
            dc.Initialization();
        }

        if (isItemScene)        //use this block at item scene
        {
            if (!mode && dc.LongClick(Time.deltaTime))        //player is stop staute && long click over 1.5f sec
            {                                                   //change scene canvas setActive(true)
                Debug.Log("stop and long click inputted");

                stopCanvas.SetActive(true);
                stopCanvas.transform.position = this.transform.position;

                lookDir = mainCam.transform.position - transform.position;

                stopCanvas.transform.Translate(lookDir * 100);
                stopCanvas.transform.Translate(Vector3.up * 2);
                Debug.Log(lookDir);

                //SceneManager.LoadScene();                 //see inventory
            }
            if (mode)
            {
                stopCanvas.SetActive(false);
            }

            RaycastHit hit;
            Vector3 forward = mainCam.transform.TransformDirection(Vector3.forward) * 1000;
            //Debug.DrawRay(mainCam.transform.position, forward, Color.green);

            if (Physics.Raycast(mainCam.transform.position, forward, out hit))
            {
                if (hit.collider.CompareTag("SceneChange"))
                {
                    Debug.Log("hit");
                    if (hit.collider.name.CompareTo("FlyingBtn") == 0)
                    {
                        flyBtn.onClick.Invoke();
                    }
                    else if (hit.collider.name.CompareTo("InvBtn") == 0)
                    {
                        invBtn.onClick.Invoke();
                    }
                }
            }
        }
        
        
    }


    void OnCollisionEnter(Collision other)
    {
        //Debug.Log("enter trigger Enter");

        if(other.transform.CompareTag("Item"))
        {
            Destroy(other.gameObject);
            //Debug.Log("execute destroy trigg");

            int tempPref = Name2Index(other.transform.name);

            if(tempPref != -1)
            {
                saveAs += "_" + tempPref;
                Debug.Log("SaveAs : " + saveAs);
            }
            else
            {
                Debug.Log("Exception Occurs");
            }
        }
    }

    public int Name2Index(string input)
    {
        //remove "(Clone)"
        if(input.Substring(input.Length - 7, 7).CompareTo("(Clone)") == 0)
            input = input.Substring(0, input.Length - 7);


        Debug.Log("input : " + input);

        for(int i = 0; i < itemImg.Length; i++)
        {
            if(input.CompareTo(itemImg[i].name) == 0)
            {
                return i;
            }
        }

        return -1;
    }

    public void ChangeFlyScene()
    {
        //Debug.Log("change to flying scene");
        PlayerPrefs.SetString("InvItem", saveAs);
        SceneManager.LoadScene(0);
    }
    public void ChangeInvScene()
    {
        //Debug.Log("change to inventory scene");
        PlayerPrefs.SetString("InvItem", saveAs);
        SceneManager.LoadScene(2);
    }
}
