using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float Damage;
    public float Speed;
    public GameObject hitEffectPrefab; // The hit effect prefab to instantiate on collision
    PlayerController refplayer;

    // Private Variables
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //Rotate towards Mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        Vector2 myPos = transform.position;
        Vector2 dir = mousePos - myPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if(angle > -90 && angle <= 90)
        {
            transform.rotation = Quaternion.AngleAxis(angle, transform.forward);
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(angle, -transform.forward);
        }

        rb.velocity = transform.right * Speed * Time.fixedDeltaTime;
        refplayer = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            DestroyBullet();
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().GetDamage(Damage);
            //Put particle effect here
            //Sound ?

            //Si désactivé enchaine les ennemis, maybe après certains combo ça le désactive
            if (refplayer.Passed10Kills == false)
            {
            Destroy(gameObject);
            }
        }
    }

    void DestroyBullet()
    {
        // Destroy the bullet object
        Instantiate(hitEffectPrefab, transform.position, Quaternion.identity); // Instantiate the hit effect
        Destroy(gameObject);
    }
}
