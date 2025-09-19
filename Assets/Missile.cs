using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public GameObject target;       // Player
    public float speed = 5f;        // velocidade do míssil
    public float lifeTime = 5f;     // destrói sozinho após X segundos

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target == null) return;

        Vector2 dir = (target.transform.position - transform.position).normalized;
        rb.velocity = dir * speed;
    }
}
