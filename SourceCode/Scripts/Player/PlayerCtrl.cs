using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCtrl : MonoBehaviour     //only use flying scene
{
    public GameObject mainCam;
    private GameObject ship;
    private bool isMove;

    //variable for changing mode
    private Chmod cm;
    private Vector3 savedPos = new Vector3();
    //private bool mode = false;

    //variable for checking double click
    private DoubleClick dc;

    //variable for interpolate moving
    private float cur_angle;
    private float prev_angle;
    private float delta_angle;

    //variable for existing collision 
    private ShipCollider cs;
    //private bool isColl = false;
    private float playerY;

    //variable for change script for moving
    private ScriptCtrl sc;
    private bool isItemScene = true;

    // Start is called before the first frame update
    void Start()
    {
        ship = GameObject.Find("PlayerUnit01");
        cm = GameObject.Find("PlayerUnit01").GetComponent<Chmod>();
        dc = GetComponent<DoubleClick>();
        cs = ship.GetComponent<ShipCollider>();
        sc = GetComponent<ScriptCtrl>();

        string fly = "FlyingScene";
        if (fly.CompareTo(SceneManager.GetActiveScene().name) == 0)     //https://bit.ly/3yR6cCs
        {
            isItemScene = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("PlayerCtrl : " + cm.GetShouldChange());

        if (isMove)
        {
            MoveForward();
        }

        if (!isMove && dc.LongClick(Time.deltaTime))  //mode change
        {
            dc.Initialization();
            cm.NeedChange();
            Debug.Log("call needChange with dc in PlayerCtrl");
           
            sc.ChangeScript();
        }

        if(isMove && dc.LongClick(Time.deltaTime))  //scene change
        {
            Debug.Log("Change Scene");

            SceneManager.LoadScene(2);
        }

        if (dc.ClickDouble(Time.deltaTime))  //stop / moving
        {
            isMove = !isMove;
            dc.Initialization();
        }

        
    }

    public void SavePlayerPos()
    {
        savedPos = transform.position;
    }
    public void ReturnPos()
    {
        transform.position = savedPos;
    }



    void MoveForward()
    {
        this.transform.Translate(mainCam.transform.forward);

        cur_angle = mainCam.transform.eulerAngles.y;
        delta_angle = cur_angle - prev_angle;
        prev_angle = cur_angle;

        //ship과 camera가 붙어있을 때만 실행되도록
        if (delta_angle < 0)
        {
            ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation,
                Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, 45), Time.deltaTime);
        }
        else if (delta_angle > 0)
        {
            ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation,
                Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, -45), Time.deltaTime);
        }
        else
        {
            ship.transform.rotation = Quaternion.Lerp(ship.transform.rotation,
                Quaternion.Euler(ship.transform.eulerAngles.x, ship.transform.eulerAngles.y, 0), Time.deltaTime);
        }
    }

    private Vector3 multVec(Vector3 vec, float num)
    {
        Vector3 nVec = new Vector3(vec.x * num, vec.y * num, vec.z * num);

        return nVec;
    }
}