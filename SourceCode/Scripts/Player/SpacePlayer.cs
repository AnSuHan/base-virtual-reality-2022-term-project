using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpacePlayer : MonoBehaviour
{
    //variable for double click
    private DoubleClick dc;

    //variable for player moving
    public GameObject mainCam;
    private GameObject ship;
    private float speed = 1.0f;

    //variable for controling player statue
    private bool isMove;   //true : moving, false : stop

    //variable for change script for moving
    private ScriptCtrl sc;
    private Chmod cm;

    //variable for fix ship
    private GameObject needitem;
    private Text itemText;
    private LevelCtrl lc;

    //variable for raycast
    private Vector3 screenCenter;
    private GameObject newPlayer;

    // Start is called before the first frame update
    void Start()
    {
        dc = GetComponent<DoubleClick>();
        ship = GameObject.Find("PlayerUnit01");
        sc = GetComponent<ScriptCtrl>();
        cm = GameObject.Find("PlayerUnit01").GetComponent<Chmod>();

        isMove = true;

        needitem = GameObject.Find("NeedItemCanv");
        itemText = needitem.GetComponent<Canvas>().transform.GetChild(0).GetComponent<Text>();
        needitem.SetActive(false);

        lc = GameObject.Find("DeckWithShip").GetComponent<LevelCtrl>();
    }

    void Update()
    {
        if (isMove)
        {
            this.transform.Translate(mainCam.transform.forward * speed);
        }

        if (dc.ClickDouble(Time.deltaTime))
        {
            isMove = !isMove;
            dc.Initialization();
        }

        if (!isMove && dc.LongClick(Time.deltaTime))
        {
            dc.Initialization();
            cm.NeedChange();
            sc.ChangeScript();
        }

        if(newPlayer != null)
        {
            Vector3 forward = mainCam.transform.forward * 1000;
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);
            RaycastHit hit;
            Vector3 forXZ = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z);

            Debug.DrawRay(newPlayer.transform.position, forward, Color.green);

            if (Physics.Raycast(mainCam.transform.position, forward, out hit))
            {
                if (hit.transform.gameObject.CompareTag("Select"))
                {
                    needitem.SetActive(true);
                    //needitem.transform.parent = newPlayer.transform;
                    needitem.transform.SetParent(newPlayer.transform);
                    needitem.transform.position = newPlayer.transform.position;
                    needitem.transform.rotation = mainCam.transform.rotation;

                    forXZ = new Vector3(mainCam.transform.forward.x, 0, mainCam.transform.forward.z);
                    needitem.transform.Translate(forXZ * 100);
                    

                    itemText.text = lc.GetFixedParts();
                    //Debug.Log("enter rayCast");

                    int opt = -1;

                    if(Input.GetMouseButtonDown(0))
                    {
                        opt = lc.DoFix(hit.transform.gameObject);

                        GameObject particleParPar = GameObject.Find("DeckWithShip");

                        GameObject particlePar = particleParPar.transform.GetChild(1).gameObject;

                        GameObject particle = particlePar.transform.GetChild(particlePar.transform.childCount - 1).gameObject;
                        Debug.Log("particle : " + particle.name);

                        if (opt == 1)     //isFixed = true -> 1, isFixed = false -> 0
                        {                            
                            if (particle.transform.parent.name.Equals(hit.transform.name) == true)
                            {
                                particle.SetActive(true);
                            }
                        }
                        else if(opt == 0)
                        {
                            particle.SetActive(false);
                        }
                        else if(opt == 2)       //Fixed All Parts
                        {
                            lc.SpawnShip();     //destroy and instantiate
                        }
                        //Debug.Log("call dofix");
                    }
                    else if(Input.GetMouseButtonDown(1))    //use only debug
                    {
                        string str = PlayerPrefs.GetString("InvItem");      //invitem debug needed
                        Debug.Log(str);    //inv item
                    }
                }
                else
                {
                    needitem.SetActive(false);
                }
            }
            else
            {
                needitem.SetActive(false);
            }
        }

        newPlayer = sc.GetPlayer();


    }
}