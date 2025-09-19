using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool onWall;

    [Header("Vida")]
    public HpPoint hp;
    public int vidasExtra = 3;
    public TMP_Text vidasText;
    private Vector2 Respawn;

    [Header("Tiro")]
    public GameObject MunicaoPlayer;
    public Transform PontoDeTiro;
    public float shootCooldown = 0.8f;
    private float lastShootTime = -10f;
    private Vector2 direcaoTiro = Vector2.right;
    public float bulletSpeed = 10f; // velocidade da bala
    public float bulletLifeTime = 3f; // tempo pra destruir a bala

    [Header("Cooldown de Dano")]
    public float damageCooldown = 1f;
    private float lastDamageTime = -10f;

    [Header("Sprites de Animação")]
    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] jumpFrames;
    public Sprite[] shootFrames;
    public Sprite[] dieFrames;
    public float frameRate = 0.15f;

    private SpriteRenderer sr;
    private int currentFrame;
    private float frameTimer;
    private Sprite[] currentAnimation;

    private enum State { Idle, Walk, Jump, Shoot, Die }
    private State currentState = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        Respawn = transform.position;
        AtualizarVidasUI();
        TrocarAnimacao(idleFrames);
    }

    void Update()
    {
        if (currentState == State.Die) return;

        float move = Input.GetAxisRaw("Horizontal");

        // movimento
        if (onWall && move != 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(move * speed, rb.velocity.y);

        // pulo
        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // morreu
        if (hp.Vida <= 0)
        {
            Morrer();
            return;
        }

        // atualiza flip, direção do tiro e ponto de tiro
        if (move > 0)
        {
            sr.flipX = false;
            direcaoTiro = Vector2.right;
            PontoDeTiro.localPosition = new Vector3(Mathf.Abs(PontoDeTiro.localPosition.x), PontoDeTiro.localPosition.y, 0);
        }
        else if (move < 0)
        {
            sr.flipX = true;
            direcaoTiro = Vector2.left;
            PontoDeTiro.localPosition = new Vector3(-Mathf.Abs(PontoDeTiro.localPosition.x), PontoDeTiro.localPosition.y, 0);
        }

        // atirar com cooldown
        if (Input.GetMouseButtonDown(0) && Time.time - lastShootTime >= shootCooldown)
        {
            GameObject bala = Instantiate(MunicaoPlayer, PontoDeTiro.position, Quaternion.identity);

            Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
            if (rbBala != null)
            {
                rbBala.velocity = direcaoTiro * bulletSpeed;
            }

            Destroy(bala, bulletLifeTime); // destrói a bala após X segundos

            SetState(State.Shoot);
            lastShootTime = Time.time;
        }

        // === Atualiza animação ===
        if (currentState != State.Shoot)
        {
            if (!isGrounded)
                SetState(State.Jump);
            else if (move != 0)
                SetState(State.Walk);
            else
                SetState(State.Idle);
        }

        RodarAnimacao();
    }

    void SetState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (currentState)
        {
            case State.Idle: TrocarAnimacao(idleFrames); break;
            case State.Walk: TrocarAnimacao(walkFrames); break;
            case State.Jump: TrocarAnimacao(jumpFrames); break;
            case State.Shoot: TrocarAnimacao(shootFrames, true); break;
            case State.Die: TrocarAnimacao(dieFrames, true); break;
        }
    }

    void TrocarAnimacao(Sprite[] novaAnimacao, bool reiniciar = false)
    {
        if (currentAnimation == novaAnimacao && !reiniciar) return;
        currentAnimation = novaAnimacao;
        currentFrame = 0;
        frameTimer = 0f;
        if (currentAnimation.Length > 0)
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

            if (currentState == State.Shoot && currentFrame >= currentAnimation.Length)
            {
                SetState(State.Idle);
                return;
            }

            if (currentState == State.Die && currentFrame >= currentAnimation.Length)
            {
                currentFrame = currentAnimation.Length - 1;
            }

            sr.sprite = currentAnimation[currentFrame % currentAnimation.Length];
        }
    }

    void Morrer()
    {
        rb.velocity = Vector2.zero;
        SetState(State.Die);
        Invoke(nameof(RespawnPlayer), 1.5f);
    }

    public void RespawnPlayer()
    {
        transform.position = Respawn;
        rb.velocity = Vector2.zero;
        hp.Vida = hp.NumCoracoes;

        vidasExtra--;
        AtualizarVidasUI();

        if (vidasExtra < 0)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
        else
        {
            SetState(State.Idle);
        }
    }

    void AtualizarVidasUI()
    {
        if (vidasExtra != -1 && vidasText != null)
            vidasText.text = "x " + vidasExtra;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PlataformaFinal"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("Wall"))
            GetComponent<Collider2D>().sharedMaterial =
                new PhysicsMaterial2D() { friction = 0, bounciness = 0 };
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("PlataformaFinal"))
            isGrounded = false;

        if (collision.gameObject.CompareTag("Wall"))
            GetComponent<Collider2D>().sharedMaterial = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            if (other.CompareTag("Shot") || other.CompareTag("EnemieShotter"))
            {
                hp.Vida--;
                Destroy(other.gameObject);
                lastDamageTime = Time.time;
            }
            else if (other.CompareTag("Enemie"))
            {
                hp.Vida--;
                lastDamageTime = Time.time;
            }
            else if (other.CompareTag("Trap"))
            {
                hp.Vida--;
                lastDamageTime = Time.time;

                Vector2 knockbackDir = (transform.position - other.transform.position).normalized;
                rb.velocity = new Vector2(knockbackDir.x * 5f, 5f);
            }
        }
    }

    public void SetRespawnPoint(Vector2 newRespawn)
    {
        Respawn = newRespawn;
        Debug.Log("[Player] Respawn point set to: " + newRespawn);
    }
}
