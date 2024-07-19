using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCheckpoint : MonoBehaviour
{
   public GameObject checkpoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "PlayerObj")
        {
            checkpoint.SetActive(true);
        }
    }
}
