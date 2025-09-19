using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Tempos")]
    public float fallDelay = 4f;     // tempo até cair
    public float respawnDelay = 3f;  // tempo até reaparecer

    private Collider2D col;
    private SpriteRenderer sr;

    private Vector3 startPos;
    private bool isFalling = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isFalling && collision.gameObject.CompareTag("Player"))
        {
            Invoke(nameof(Fall), fallDelay);
        }
    }

    void Fall()
    {
        isFalling = true;
        col.enabled = false;     // desativa colisão
        sr.enabled = false;      // desativa visual
        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        transform.position = startPos; // volta pra posição original
        col.enabled = true;
        sr.enabled = true;
        isFalling = false;
    }
}
