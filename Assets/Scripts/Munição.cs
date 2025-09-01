using UnityEngine;

public class Municao : MonoBehaviour
{
    public float balaVelocidade = 5f;
    public float tempoBala = 2f;

    void Start()
    {
        Invoke("DestroyProjectile", tempoBala);
    }

    void FixedUpdate()
    {
        transform.Translate(transform.right * balaVelocidade * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Wall") )  
        {
            Debug.Log("Bala colidiu com " + other.name);
            Destroy(gameObject);
        }
        
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
