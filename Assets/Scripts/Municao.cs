using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Munição : MonoBehaviour
{
    public float balaVelocidade;
    public float tempoBala;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyProjectile", tempoBala); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(transform.right * balaVelocidade * Time.deltaTime,Space.World);
    }

    void DestroyProjectile() 
    {
    
        Destroy(gameObject);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemie") || other.CompareTag("EnemieShotter") || other.CompareTag("Wall") || other.CompareTag("Ground"))
        {
            Debug.Log("Bala colidiu com " + other.name);
            Destroy(gameObject);
        }
    }
}
