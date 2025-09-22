using UnityEngine;
using UnityEngine.SceneManagement;

public class BossCycle : MonoBehaviour
{
    [Header("Vida")]
    public float Vida = 20; // vida do boss
    public Sprite[] deathFrames;
    private bool isDead = false;

    [Header("Movimento")]
    public float walkSpeed = 2f;
    public float fastSpeed = 4f;
    public float patrolDistance = 5f;
    public float bobAmplitude = 0.5f;
    public float bobSpeed = 2f;

    [Header("Sprites - Sprite Sheets")]
    public Sprite[] walkFrames;
    public Sprite[] shoot1Frames;
    public Sprite[] shoot2Frames;
    public Sprite[] fastFrames;
    public float frameRate = 0.15f;

    [Header("Projéteis")]
    public GameObject MisselTeleguiadoPrefab;
    public GameObject TiroParaBaixoPrefab;
    public Transform spawnPoint;
    public float bulletSpeed = 7f;

    [Header("Cooldown de tiro")]
    public float shoot1Interval = 0.8f;
    public float shoot2Interval = 0.5f;

    private float shoot1Timer = 0f;
    private float shoot2Timer = 0f;

    private SpriteRenderer sr;
    private Vector3 startPos;
    private int direction = 1;

    private Sprite[] currentAnimation;
    private int currentFrame = 0;
    private float frameTimer = 0f;

    private float stateTimer = 0f;
    private int cycleStage = 0;

    private float fastStartY = 49.43f;
    private float fastTargetY = 44.84296f;
    private bool reachedBottom = false;
    private float bottomWaitTimer = 0f;

    private enum State { Alive, Death }
    private State currentState = State.Alive;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        SetStage(0);
    }

    void Update()
    {
        if (currentState == State.Death) // não faz nada se estiver morto
        {
            Animate();
            return;
        }

        stateTimer -= Time.deltaTime;

        // troca de estágio
        if (stateTimer <= 0f && !(cycleStage == 4 && !reachedBottom))
        {
            cycleStage = (cycleStage + 1) % 5;
            SetStage(cycleStage);
        }

        // movimento horizontal
        float currentSpeed = (cycleStage == 4) ? fastSpeed : walkSpeed;
        transform.position += new Vector3(direction * currentSpeed * Time.deltaTime, 0, 0);

        // movimento vertical
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobAmplitude;

        if (cycleStage == 4)
        {
            if (!reachedBottom)
            {
                newY = Mathf.MoveTowards(transform.position.y, fastTargetY, fastSpeed * Time.deltaTime);
                if (Mathf.Approximately(newY, fastTargetY))
                {
                    reachedBottom = true;
                    bottomWaitTimer = 5f;
                }
            }
            else
            {
                bottomWaitTimer -= Time.deltaTime;
                newY = fastTargetY;
                if (bottomWaitTimer <= 0f)
                {
                    newY = Mathf.MoveTowards(transform.position.y, fastStartY, fastSpeed * Time.deltaTime);
                    if (Mathf.Approximately(newY, fastStartY))
                    {
                        reachedBottom = false;
                        stateTimer = 0f;
                    }
                }
            }
        }

        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // inverte direção horizontal
        if (Mathf.Abs(transform.position.x - startPos.x) >= patrolDistance)
        {
            direction *= -1;
            sr.flipX = direction > 0;
        }

        Animate();

        // ataques contínuos durante animação
        if (cycleStage == 1) // shoot1
        {
            shoot1Timer += Time.deltaTime;
            if (shoot1Timer >= shoot1Interval)
            {
                AtacarShoot1();
                shoot1Timer = 0f;
            }
        }
        else if (cycleStage == 3) // shoot2
        {
            shoot2Timer += Time.deltaTime;
            if (shoot2Timer >= shoot2Interval)
            {
                AtacarShoot2();
                shoot2Timer = 0f;
            }
        }
        else
        {
            shoot1Timer = 0f;
            shoot2Timer = 0f;
        }
    }

    void SetStage(int stage)
    {
        switch (stage)
        {
            case 0: currentAnimation = walkFrames; stateTimer = 10f; break;
            case 1: currentAnimation = shoot1Frames; stateTimer = 5f; break;
            case 2: currentAnimation = walkFrames; stateTimer = 10f; break;
            case 3: currentAnimation = shoot2Frames; stateTimer = 5f; break;
            case 4: currentAnimation = fastFrames; stateTimer = 0f; break;
        }

        currentFrame = 0;
        frameTimer = 0f;
        if (currentAnimation.Length > 0)
            sr.sprite = currentAnimation[0];
    }

    void Animate()
    {
        if (currentAnimation == null || currentAnimation.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame++;
            if (currentFrame >= currentAnimation.Length)
                currentFrame = 0;
            sr.sprite = currentAnimation[currentFrame];
        }
    }

    void AtacarShoot1() // míssil teleguiado
    {
        if (MisselTeleguiadoPrefab != null)
        {
            GameObject missile = Instantiate(MisselTeleguiadoPrefab, spawnPoint.position, Quaternion.identity);
        }

        if (MisselTeleguiadoPrefab != null && spawnPoint != null)
        {
            Vector3 spawnPos = spawnPoint.position + new Vector3(0, 3f, 0);
            GameObject missile = Instantiate(MisselTeleguiadoPrefab, spawnPos, Quaternion.identity);
        }
    }

    void AtacarShoot2() // tiros para baixo
    {
        if (TiroParaBaixoPrefab != null)
        {
            GameObject bullet = Instantiate(TiroParaBaixoPrefab, spawnPoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = Vector2.down * bulletSpeed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState == State.Death) return;

        if (other.CompareTag("ShotPlayer"))
        {
            Vida--;

            if (Vida <= 0)
            {
                currentState = State.Death;
                currentAnimation = deathFrames;
                currentFrame = -1;
                frameTimer = frameRate;
                GameManager.instance.AddKill();
                Destroy(gameObject, 1f); // espera 1 segundo pra animação de morte

                SceneManager.LoadScene("GameOver");
            }
        }
    }
}
