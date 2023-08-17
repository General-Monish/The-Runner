using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    [HideInInspector]
    public Transform player;
    public float speed;
    public float timeBetweenAttack;
    public int damage;

    public int PickupChance;
    public GameObject[] Pickups;

    public int healthpickupchance;
    public GameObject healthpick;
    public GameObject sound;
    public GameObject blood;
    public GameObject effect;
    public virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            int randomnumber = Random.Range(0, 101);
            if (randomnumber < PickupChance)
            {
                GameObject randompickup = Pickups[Random.Range(0, Pickups.Length)];
                Instantiate(randompickup, transform.position, transform.rotation);
            }
            int randomhealth = Random.Range(0, 101);
            if (randomhealth < healthpickupchance)
            {
                Instantiate(healthpick, transform.position, transform.rotation);
            }

            Instantiate(sound, transform.position, transform.rotation);
            Instantiate(effect, transform.position, Quaternion.identity);
            Instantiate(blood, transform.position, transform.rotation);
            Destroy(this.gameObject);

        }
    }



}
