using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RealInitGame : MonoBehaviour
{
    private float timer = 0.0f;
    private string initstr;

    // Start is called before the first frame update
    void Start()
    {
        initstr = "";
        for(int i = 0; i < 14; i++)
        {
            initstr += "_-1"; 
        }

        PlayerPrefs.SetString("InvItem", initstr);    
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer >= 3.0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
