using UnityEngine;
using TMPro; // ← Importante pra usar TextMeshPro

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool onWall;

    public HpPoint hp;

    public int vidasExtra = 3;
    public TMP_Text vidasText;

    private Vector2 Respawn;

    public GameObject MunicaoPlayer;
    public Transform PontoDeTiro;
    public float offset = 1f;

    private Vector2 direcaoTiro = Vector2.right;

    // === TIMER DE DANO ===
    public float damageCooldown = 1f; // tempo de espera (1s)
    private float lastDamageTime = -10f;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Respawn = transform.position;
        AtualizarVidasUI();

        Debug.Log("VIDA INICIAL: " + hp.Vida);
        Debug.Log("NUM CORACOES: " + hp.NumCoracoes);
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        if (onWall && move != 0)
            rb.velocity = new Vector2(0, rb.velocity.y);
        else
            rb.velocity = new Vector2(move * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        if (hp.Vida <= 0)
            RespawnPlayer();

        // Direção do tiro (WASD)
        if (Input.GetKey(KeyCode.W)) direcaoTiro = Vector2.up;
        else if (Input.GetKey(KeyCode.S)) direcaoTiro = Vector2.down;
        else if (Input.GetKey(KeyCode.A)) direcaoTiro = Vector2.left;
        else if (Input.GetKey(KeyCode.D)) direcaoTiro = Vector2.right;

        PontoDeTiro.localPosition = direcaoTiro * offset;
        float angle = Mathf.Atan2(direcaoTiro.y, direcaoTiro.x) * Mathf.Rad2Deg;
        PontoDeTiro.rotation = Quaternion.Euler(0, 0, angle);

        // Atirar
        if (Input.GetMouseButtonDown(0))
            Instantiate(MunicaoPlayer, PontoDeTiro.position, PontoDeTiro.rotation);
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
        // Só toma dano se passou o cooldown
        if (Time.time - lastDamageTime >= damageCooldown)
        {
            if (other.CompareTag("Shot") || other.CompareTag("EnemieShotter"))
            {
                hp.Vida--;
                Destroy(other.gameObject); // destrói a bala
                lastDamageTime = Time.time; // registra o último hit
            }
            else if (other.CompareTag("Enemie"))
            {
                hp.Vida--;
                lastDamageTime = Time.time; // registra o último hit
            }
        }
    }


    
}
