using UnityEngine;
using TMPro; // ← Importante pra usar TextMeshPro

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool onWall;

    public HpPoint hp; // referência ao script que controla os corações

    public int vidasExtra = 3;        // vidas extras (o contador)
    public TMP_Text vidasText;        // UI do contador de vidas (TMP)

    private Vector2 Respawn;          // posição inicial pra respawn

    public GameObject MunicaoPlayer;  // prefab da munição
    public Transform PontoDeTiro;     // ponto de onde a bala nasce
    public float offset = 1f;         // distância do ponto de tiro do player

    private Vector2 direcaoTiro = Vector2.right; // direção inicial do tiro = direita

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Respawn = transform.position;

        AtualizarVidasUI(); // mostra logo as vidas na tela
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal");

        // Movimento
        if (onWall && move != 0)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(move * speed, rb.velocity.y);
        }

        // Pulo
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        // Respawn se zerar a vida
        if (hp.Vida <= 0)
        {
            RespawnPlayer();
        }

        // ====== DIREÇÃO DO TIRO (WASD) ======
        if (Input.GetKey(KeyCode.W))
            direcaoTiro = Vector2.up * 1;
        else if (Input.GetKey(KeyCode.S))
            direcaoTiro = Vector2.down;
        else if (Input.GetKey(KeyCode.A))
            direcaoTiro = Vector2.left * 1;
        else if (Input.GetKey(KeyCode.D))
            direcaoTiro = Vector2.right * 1;

        // Atualiza posição e rotação do ponto de tiro
        PontoDeTiro.localPosition = direcaoTiro * offset;
        float angle = Mathf.Atan2(direcaoTiro.y, direcaoTiro.x) * Mathf.Rad2Deg ;
        PontoDeTiro.rotation = Quaternion.Euler(0, 0, angle);

        // ====== ATIRAR ======
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(MunicaoPlayer, PontoDeTiro.position, PontoDeTiro.rotation);
        }
    }

    public void RespawnPlayer()
    {
        transform.position = Respawn;      // volta posição
        rb.velocity = Vector2.zero;       // zera velocidade
        hp.Vida = hp.NumCoracoes;         // recupera corações

        vidasExtra--;                     // perde uma vida extra
        AtualizarVidasUI();

        if (vidasExtra < 0)
        {
            Destroy(gameObject);          // game over
        }
    }

    void AtualizarVidasUI()
    {
        if (vidasExtra != -1 && vidasText != null)
        {
            vidasText.text = "x " + vidasExtra;
        }
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
        if (other.CompareTag("Shot") || other.CompareTag("EnemieShotter"))
        {
            hp.Vida--;
            Destroy(other.gameObject); // bala some
        }
        else if (other.CompareTag("Enemie"))
        {
            hp.Vida--; // só perde vida, inimigo continua vivo
        }
    }

}
