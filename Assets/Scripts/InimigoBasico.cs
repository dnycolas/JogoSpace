using UnityEngine;

public class InimigoBasico : MonoBehaviour
{
    [Header("Movimentação")]
    public float speed = 2f;         // velocidade normal
    public float boostedSpeed = 4f; // velocidade quando "vê" o player
    public float moveTime = 2f;     // tempo que ele anda em cada direção
    public float detectionRange = 3f; // distância para detectar o player

    private int direction = 1;      // 1 = direita, -1 = esquerda
    private float timer;

    [Header("Vida")]
    public float Vida = 2;

    [Header("Sprites Andando")]
    public Sprite[] walkFrames;
    [Header("Sprites Morte")]
    public Sprite[] deathFrames;

    public float frameRate = 0.15f;
    private float frameTimer = 0f;
    private int currentFrame = 0;

    private SpriteRenderer sr;
    private bool isDead = false;

    private Transform player; // referência ao player (busca por tag "Player")

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        timer = moveTime;

        // tenta achar o player por tag; garanta que o player tem tag "Player"
        GameObject obj = GameObject.FindGameObjectWithTag("Player");
        if (obj != null) player = obj.transform;
    }

    void Update()
    {
        if (isDead)
        {
            Animate(deathFrames, false); // animação de morte (só uma vez)
            return;
        }

        // decide velocidade atual: se enxergou o player, usa boostedSpeed
        float currentSpeed = speed;
        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist <= detectionRange)
            {
                currentSpeed = boostedSpeed;
            }
        }

        // movimento lateral (mantém o comportamento de patrulha)
        transform.position += new Vector3(direction * currentSpeed * Time.deltaTime, 0, 0);

        // diminui o timer e inverte direção quando necessário
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            direction *= -1;
            timer = moveTime;
            sr.flipX = direction < 0;
        }

        // animação de andar
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
                else currentFrame = frames.Length - 1;
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
                currentFrame = -1; // força reset da animação (começa do frame 0 depois)
                GameManager.instance.AddKill();
                Destroy(gameObject, 0.5f); // tempo pra mostrar animação de morte
            }
        }
    }
}
