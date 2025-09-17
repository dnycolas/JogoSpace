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
    
    private Vector2 direcaoTiro = Vector2.right;

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

        Debug.Log("VIDA INICIAL: " + hp.Vida);
        Debug.Log("NUM CORACOES: " + hp.NumCoracoes);
    }

    void Update()
    {
        if (currentState == State.Die) return; // não faz nada morto

        float move = Input.GetAxisRaw("Horizontal");

        if (onWall && move != 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (hp.Vida <= 0)
        {
            Morrer();
            return;
        }

        // Direção do tiro (WASD)
        if (Input.GetKey(KeyCode.W)) direcaoTiro = Vector2.up;
        else if (Input.GetKey(KeyCode.S)) direcaoTiro = Vector2.down;
        else if (Input.GetKey(KeyCode.A)) direcaoTiro = Vector2.left;
        else if (Input.GetKey(KeyCode.D)) direcaoTiro = Vector2.right;

        float angle = Mathf.Atan2(direcaoTiro.y, direcaoTiro.x) * Mathf.Rad2Deg;
        PontoDeTiro.rotation = Quaternion.Euler(0, 0, angle);

        // Atirar
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(MunicaoPlayer, PontoDeTiro.position, PontoDeTiro.rotation);
            SetState(State.Shoot);
        }

        // === Atualiza animação ===
        if (currentState != State.Shoot) // tiro não é interrompido até acabar
        {
            if (!isGrounded)
                SetState(State.Jump);
            else if (move != 0)
            {
                SetState(State.Walk);
                sr.flipX = move < 0;

                // Espelha o ponto de tiro junto com o sprite
                Vector3 pos = PontoDeTiro.localPosition;
                pos.x = Mathf.Abs(pos.x) * (sr.flipX ? -1 : 1);
                PontoDeTiro.localPosition = pos;
            }
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

            // animação de tiro volta para Idle no fim
            if (currentState == State.Shoot && currentFrame >= currentAnimation.Length)
            {
                SetState(State.Idle);
                return;
            }

            // animação de morte para no último frame
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
        Invoke(nameof(RespawnPlayer), 1.5f); // respawn depois de 1.5s (ou troca cena)
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
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("Wall"))
            GetComponent<Collider2D>().sharedMaterial = new PhysicsMaterial2D() { friction = 0, bounciness = 0 };
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
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
            else if (other.CompareTag("Trap")) // 👈 dano vindo da Trap
            {
                hp.Vida--;
                lastDamageTime = Time.time;

                // Knockback (empurra pra trás + um pulinho)
                Vector2 knockbackDir = (transform.position - other.transform.position).normalized;
                rb.velocity = new Vector2(knockbackDir.x * 5f, 5f);
                // ajuste os valores (5f, 5f) conforme achar melhor
            }
        }
    }

}
