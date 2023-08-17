using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class healthpickup : MonoBehaviour
{
    Player playerscript;
    public int healAmount;
    

    private void Start()
    {
        playerscript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerscript.Heal(healAmount);
          
            Destroy(gameObject);
        }
    }
}
