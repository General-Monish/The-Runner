using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowprojectile : MonoBehaviour
{
    public float speed;
    public float lifetime;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);

    }

    // Update is called once per frame
    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void DestroyProjectile()
    {

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            collision.GetComponent<Enemy>().TakeDamage(damage);
            DestroyProjectile();
        }



    }

}
