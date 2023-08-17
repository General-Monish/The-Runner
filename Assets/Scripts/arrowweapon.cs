using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowweapon : MonoBehaviour
{
    public GameObject projectile;
    public Transform shotpoint;
    public float timeBetweenShots;

    private float shotTime;
    // Start is called before the first frame update
    // Update is called once per frame
    private void Update()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle-90, Vector3.forward);
        transform.rotation = rotation;

        if (Input.GetMouseButton(0))
        {
            if (Time.time >= shotTime)
            {
                Instantiate(projectile, shotpoint.position, transform.rotation);
                shotTime = Time.time + timeBetweenShots;
            }
        }

    }
}
