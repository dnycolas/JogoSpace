using UnityEngine;

public class MunicaoSeguidora : MonoBehaviour
{
    public float velocidade = 5f;
    public float tempoVida = 5f;

    private Transform alvo;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // destrói automaticamente após tempoVida
        Destroy(gameObject, tempoVida);

        // pega o player automaticamente
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            alvo = playerObj.transform;
    }

    void FixedUpdate()
    {
        if (alvo != null)
        {
            Vector2 direcao = (alvo.position - transform.position).normalized;
            rb.velocity = direcao * velocidade;

            // rotaciona a bala para olhar para o player (opcional)
            float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
