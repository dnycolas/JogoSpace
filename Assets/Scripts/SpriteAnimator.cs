using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] frames;       // sprites da animação
    public float frameRate = 0.15f; // tempo entre os frames

    private SpriteRenderer spriteRenderer;
    private int currentFrame;
    private float timer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        if (timer >= frameRate)
        {
            timer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
                currentFrame = 0;

            spriteRenderer.sprite = frames[currentFrame];
        }
    }
}
