using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
  public float speed;
    private Rigidbody2D rb;
    private Vector2 MoveAmount;
    public int health;

    private Animator anim;

    public Image[] Hearts;
    public Sprite fulheart;
    public Sprite emptyheart;
    public Animator hurtAnim;
    private scenetransition sceneTransitions;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sceneTransitions = FindObjectOfType<scenetransition>();

    }

    private void Update()
    {
        Vector2 moveinput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        MoveAmount = moveinput.normalized * speed;
        if (moveinput != Vector2.zero)
        {
            anim.SetBool("run", true);
        }
        else
        {
            anim.SetBool("run", false);

        }
        // Rotate the player by 180 degrees along the Y axis
        if (moveinput.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (moveinput.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }


    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + MoveAmount*Time.fixedDeltaTime);
    }

    public void TakeDamage(int damageAmount)
    {
        health -= damageAmount;
        UpdateHealthUI(health);
        hurtAnim.SetTrigger("hurt");
        if (health <= 0)
        {
            Destroy(this.gameObject);
            sceneTransitions.LoadScene("lose");

        }
    }

    public void ChangeWeapon(weapon weaponToequip)
    {
        Destroy(GameObject.FindGameObjectWithTag("weapon"));
        Instantiate(weaponToequip, transform.position, transform.rotation, transform);
    }

    void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < Hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                Hearts[i].sprite = fulheart;
            }
            else
            {
                Hearts[i].sprite = emptyheart;
            }
        }
    }

    public void Heal(int healAmount)
    {
        if (health + healAmount > 5)
        {
            health = 5;
        }
        else
        {
            health += healAmount;
        }
        UpdateHealthUI(health);
    }

}
