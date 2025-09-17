using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] idleFrames;   // frames da trap parada
    public Sprite[] attackFrames; // frames da trap atacando

    public float frameRate = 0.15f;

    private SpriteRenderer sr;
    private Sprite[] currentAnimation;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private bool isAttacking = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        TrocarAnimacao(idleFrames);
    }

    void Update()
    {
        RodarAnimacao();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isAttacking)
        {
            isAttacking = true;
            TrocarAnimacao(attackFrames, true);
        }
    }

    void TrocarAnimacao(Sprite[] novaAnimacao, bool reiniciar = false)
    {
        if (currentAnimation == novaAnimacao && !reiniciar) return;
        currentAnimation = novaAnimacao;
        currentFrame = 0;
        frameTimer = 0f;
        sr.sprite = currentAnimation[0];
    }

    void RodarAnimacao()
    {
        if (currentAnimation == null || currentAnimation.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame++;

            if (currentFrame >= currentAnimation.Length)
            {
                if (isAttacking)
                {
                    // depois de atacar, volta pro idle
                    isAttacking = false;
                    TrocarAnimacao(idleFrames, true);
                    return;
                }
                else
                {
                    currentFrame = 0; // loop no idle
                }
            }

            sr.sprite = currentAnimation[currentFrame];
        }
    }
}

