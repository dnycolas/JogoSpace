using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{

    public HpPoint hp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Shot")) 
        {

            hp.Vida--;
            Destroy(other.gameObject);
        
        }

    }
}
