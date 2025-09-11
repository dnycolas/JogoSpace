using UnityEngine;

public class InimigoBasico : MonoBehaviour
{
    public float speed = 2f;       // velocidade do inimigo
    public float moveTime = 2f;    // tempo que ele anda em cada dire��o

    private int direction = 1;     // 1 = direita, -1 = esquerda
    private float timer;

    public float Vida = 2;

    [Header("Sprites Andando")]
    public Sprite[] walkFrames;
    [Header("Sprites Morte")]
    public Sprite[] deathFrames;

    public float frameRate = 0.15f; // tempo entre cada frame
    private float frameTimer = 0f;
    private int currentFrame = 0;

    private SpriteRenderer sr;
    private bool isDead = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        timer = moveTime;
    }

    void Update()
    {
        if (isDead)
        {
            Animate(deathFrames, false); // toca a anima��o de morte s� uma vez
            return;
        }

        // movimento para os lados
        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);

        // diminui o timer
        timer -= Time.deltaTime;

        // quando o timer acaba, inverte a dire��o
        if (timer <= 0f)
        {
            direction *= -1;
            timer = moveTime;

            // vira o sprite tamb�m
            sr.flipX = direction < 0;
        }

        // anima��o de andar
        Animate(walkFrames, true);
    }

    void Animate(Sprite[] frames, bool loop)
    {
        if (frames.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop) currentFrame = 0;
                else currentFrame = frames.Length - 1; // trava no �ltimo frame (ex: morte)
            }

            sr.sprite = frames[currentFrame];
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ShotPlayer") && !isDead)
        {
            Vida--;

            if (Vida <= 0)
            {
                isDead = true;
                currentFrame = -1; // for�a reset da anima��o
                GameManager.instance.AddKill();
                Destroy(gameObject, 0.5f); // tempo pra mostrar anima��o de morte
            }
        }
    }
}
