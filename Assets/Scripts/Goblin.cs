using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    // Start is called before the first frame update

    public float stopDistance;
    private float attackTime;
    public float attackspeed;

 

    private void Update()
    {
        if (player != null)
        {
            if (Vector2.Distance(transform.position, player.position) > stopDistance)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            }
            else
            {
                if (Time.time >= attackTime)
                {
                    StartCoroutine(Attack());
                    attackTime = Time.time + timeBetweenAttack;
                }
            }


        }

        IEnumerator Attack()
        {
            player.GetComponent<Player>().TakeDamage(damage);
            Vector2 originalposition = transform.position;
            Vector2 targetposition = player.position;

            float percent = 0;
            while (percent <= 1)
            {
                percent += Time.deltaTime * attackspeed;
                float formula = (-Mathf.Pow(percent, 2) + percent) * 4;
                transform.position = Vector2.Lerp(originalposition, targetposition, formula);
                yield return null;
            }
        }

    }
}
