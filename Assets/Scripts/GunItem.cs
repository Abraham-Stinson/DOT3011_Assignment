using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunItem : MonoBehaviour,IInteractable
{
    public void Interact()
    {
        Debug.Log("Picked up a gun");
        Destroy(gameObject);
    }
}