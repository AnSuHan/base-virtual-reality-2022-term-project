using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptCtrl : MonoBehaviour
{
    //variable for control script change
    private Chmod cm;
    private GameObject ship;
    private PlayerCtrl pc;
    private SpacePlayer pcc;

    //variable for instantiate
    public GameObject playerPrefab;
    private GameObject newPlayer;
    private DoubleClick dc;
    private GameObject mainCam;
    private GameObject playerEmp;

    //variable for playerpref
    private LevelCtrl lc;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.Find("PlayerUnit01");
        cm = ship.GetComponent<Chmod>();
        pc = this.GetComponent<PlayerCtrl>();       //with    spaceship
        pcc = this.GetComponent<SpacePlayer>();     //without spaceship

        pc.enabled = true;
        pcc.enabled = false;

        dc = this.GetComponent<DoubleClick>();
        mainCam = GameObject.Find("Main Camera");
        playerEmp = GameObject.Find("PlayerEmp");
        lc = GameObject.Find("DeckWithShip").GetComponent<LevelCtrl>();
    }

    // Update is called once per frame
    public void ChangeScript()              //when long click enter in pc or pcc
    {
        if (!cm.GetMode())    //true = Set "moving with airship and camera"       //false = Set "moving only camera without airship"
        {                   //use pc script                                     //use pcc script
            pc.enabled = true;
            pcc.enabled = false;
            ship.GetComponent<CapsuleCollider>().enabled = true;

            Debug.Log("cm.getmod() is true part");

            if(newPlayer != null)
            {
                mainCam.transform.parent = playerEmp.transform;
                cm.SetParent();

                Destroy(newPlayer);
            }
        }
        else
        {
            pc.enabled = false;
            pcc.enabled = true;
            ship.GetComponent<CapsuleCollider>().enabled = false;

            newPlayer = Instantiate(playerPrefab, transform.position, mainCam.transform.rotation);
            newPlayer.transform.parent = playerEmp.transform;
            mainCam.transform.parent = newPlayer.transform;

            Debug.Log("cm.getmod() is false part");
        }

        dc.Initialization();
        //Debug.Log("do init in scriptctrl");
    }

    public void SetPlayer(GameObject play)
    {
        newPlayer = play;
    }
    public GameObject GetPlayer()
    {
        return newPlayer;
    }

    public string SaveInv()
    {
        return lc.InvSave();
    }
}
