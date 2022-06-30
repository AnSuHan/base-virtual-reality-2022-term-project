using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCollider : MonoBehaviour
{
    private bool isColl;

    private void OnCollisionEnter(Collision collision)
    {
        isColl = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isColl = false;
    }

    public bool GetColl()
    {
        return isColl;
    }
}
