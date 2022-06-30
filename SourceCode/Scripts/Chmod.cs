using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chmod : MonoBehaviour  //change parent child relationship (PlayerEmp, PlayerUnit01)
{   //https://docs.unity3d.com/ScriptReference/Transform-parent.html
    private GameObject mainCam;
    private bool shouldChange = false;
    private bool nowState = true;   //true = Set "moving with airship and camera"     //false = Set "moving only camera without airship"
    private Vector3 initPosFromCam;
    private PlayerCtrl pc;

    //variable for "blocking ship rotation" while moving without spaceship
    private GameObject ship;
    private Rigidbody shipRb;

    public GameObject reticlePointer;

    void Start()
    {
        mainCam = GameObject.Find("Main Camera");
        pc = GameObject.Find("PlayerEmp").GetComponent<PlayerCtrl>();
        ship = GameObject.Find("PlayerUnit01");
        shipRb = ship.GetComponent<Rigidbody>();
        initPosFromCam = new Vector3(0, -20, 150);
    }

    void Update()
    {
        if(shouldChange)
        {
            nowState = !nowState;

            if (!nowState)
            {
                DetachParent();
                shipRb.constraints = RigidbodyConstraints.FreezeRotationZ;
                //Debug.Log("Set 'moving only camera without airship'");

                
                reticlePointer.transform.parent = mainCam.transform;
                reticlePointer.transform.position = mainCam.transform.position;
                reticlePointer.transform.rotation = mainCam.transform.rotation;
                
            }
            else
            {
                SetParent();
                shipRb.constraints = RigidbodyConstraints.None;
                //Debug.Log("Set 'moving with airship and camera'");
            }

            
            shouldChange = false;
            //Debug.Log("change state");
        }
    }

    public bool GetShouldChange()
    {
        return shouldChange;
    }

    public void SetParent()
    {
        transform.parent = mainCam.transform;

        pc.ReturnPos();
        mainCam.transform.position = new Vector3(0, 0, 0);
        mainCam.transform.rotation = Quaternion.Euler(0, 0, 0);

        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.position = RtPos(mainCam.transform.position, initPosFromCam);

        //shipRb.constraints = RigidbodyConstraints.None;
    }

    public void DetachParent()      //make not moving airship while detach state
    {
        transform.parent = null;

        pc.SavePlayerPos();

        //shipRb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void NeedChange()
    {
        shouldChange = true;
    }

    public bool GetMode()
    {
        return nowState;
    }

    private Vector3 RtPos(Vector3 mainCam, Vector3 initSetting)
    {
        Vector3 nv = new Vector3(mainCam.x + initSetting.x,
                                mainCam.y + initSetting.y,
                                mainCam.z + initSetting.z);
        
        return nv;
    }
}
