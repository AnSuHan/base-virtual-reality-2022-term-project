using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnItem : MonoBehaviour
{   //(-10, 10) ~ (40, 170)
    public GameObject[] itemPrefabs;
    private int maxSpawnNum = 30;
    private Vector3 randPos;
    private GameObject parEmp;

    public Texture[] ItemImg = new Texture[10];         //saved Image   [bolt, gear, plate, wires]
    private int itemIndex = 4;
    private int randNum;
    private PlayerCtrlCharacter pcc;

    //variable for apply texture
    public Material[] itemMat = new Material[10];

    // Start is called before the first frame update
    void Start()
    {
        parEmp = GameObject.Find("GrpForInstItem");
        pcc = GameObject.Find("PlayerBody").GetComponent<PlayerCtrlCharacter>();

        for(int i = 0; i < maxSpawnNum; i++)
        {
            randPos = new Vector3(Random.Range(-10, 40), 50, Random.Range(40, 170));
            GameObject ng= Instantiate(itemPrefabs[0], randPos, Quaternion.identity);

            randNum = Random.Range(0, itemIndex);

            //ng.GetComponent<RawImage>().texture = ItemImg[randNum];     //texture is applied but doesn't show the texture
            ng.GetComponent<MeshRenderer>().material = itemMat[randNum];    //change meshRenderer->material [not RawImage->texture]

            //ng.GetComponent<RawImage>().texture.width

            ng.name = ItemImg[randNum].name;

            ng.transform.parent = parEmp.gameObject.transform;
        }
    }
}
