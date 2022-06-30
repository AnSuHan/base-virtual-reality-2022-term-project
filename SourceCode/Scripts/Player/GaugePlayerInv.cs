using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GaugePlayerInv : MonoBehaviour
{
    //variable for gauge
    public Image cursorGauge;
    private Vector3 screenCenter;
    public GameObject mainCam;
    private float gaugeTimer = 0.0f;
    private float gazeTime = 2.0f;

    //variable for making items
    private int selCnt = 0;

    //variable for saving items
    private SetInv si;
    private string Tname;

    //variable for changing scene
    public Button flying;
    public Button item;

    void Start()
    {
        screenCenter = new Vector3(Camera.main.pixelWidth / 2, Camera.main.pixelHeight / 2);
        si = GameObject.Find("Head").GetComponent<SetInv>();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 forward = mainCam.transform.TransformDirection(Vector3.forward) * 1000;
        Vector3 forward = mainCam.transform.forward * 1000;
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        RaycastHit hit;
        cursorGauge.fillAmount = gaugeTimer;

        Debug.DrawRay(this.transform.position, forward, Color.green);

        if (Physics.Raycast(this.transform.position, forward, out hit))         //ray, out hit, 100.0f))
        {
            gaugeTimer += 1.0f / gazeTime * Time.deltaTime;

            if(hit.collider.CompareTag("SceneChange"))
            {
                //Debug.Log(hit.collider.name);
                if((hit.collider.name.CompareTo("FlyingButton") == 0) && (gaugeTimer >= 2.0f || Input.GetMouseButtonDown(0)))
                {
                    PlayerPrefs.SetString("InvItem", si.PlayerprefArray());
                    Debug.Log(si.PlayerprefArray());

                    SceneManager.LoadScene(0);
                }
                else if((hit.collider.name.CompareTo("ItemButton") == 0) && (gaugeTimer >= 2.0f || Input.GetMouseButtonDown(0)))
                {
                    PlayerPrefs.SetString("InvItem", si.PlayerprefArray());
                    Debug.Log(si.PlayerprefArray());

                    SceneManager.LoadScene(1);
                }
                    
                        
            }
            else if (hit.collider.CompareTag("Destroy") && (gaugeTimer >= 2.0f || Input.GetMouseButtonDown(0)))
            {
                //Debug.Log("enter Destroy Button");
                //Debug.Log(hit.collider.gameObject.name);        //destroyBtn

                Button btn = hit.transform.gameObject.GetComponent<Button>();
                btn.onClick.Invoke();
            }

            else if ((gaugeTimer >= 2.0f || Input.GetMouseButtonDown(0)) && !(hit.collider.CompareTag("Destroy") && (hit.transform.GetComponent<RawImage>().texture != null)))
            {
                Tname = hit.transform.GetComponent<RawImage>().texture.name;
                //Debug.Log("Tname " + Tname);

                //hit.collider.isTriggered 를 isSelected로 사용
                if ((hit.collider.CompareTag("Item")) && (hit.collider.isTrigger == false) && selCnt < 4)
                {
                    if (si.SelectItem(Tname, hit))         //select item
                    {
                        hit.collider.isTrigger = true;
                        hit.transform.GetComponent<RawImage>().color = Color.red;
                        selCnt++;
                    }
                }
                else if ((hit.collider.CompareTag("Item")) && (hit.collider.isTrigger == true))
                {
                    si.UnSelectItem(hit); //unselect item
                    hit.collider.isTrigger = false;
                    hit.transform.GetComponent<RawImage>().color = Color.white;
                    selCnt--;
                }
                else if(hit.collider.CompareTag("Select"))
                { 
                    si.UnSelectItem(hit); //unselect item
                    selCnt--;
                }
                else if (hit.collider.CompareTag("MadeItem"))
                {
                    si.GetMadeItem(hit);
                    selCnt = 0;

                    Debug.Log("get Crafted item");
                }
                else if(hit.collider.CompareTag("NotInteraction"))
                {
                    //empty
                }

                gaugeTimer = 0.0f;
            }
        }
        else
        {
            gaugeTimer = 0.0f;
        }
    }
}
