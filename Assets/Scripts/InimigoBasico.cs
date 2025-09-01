using UnityEngine;

public class InimigoBasico : MonoBehaviour
{
    public float speed = 2f;       // velocidade do inimigo
    public float moveTime = 2f;    // tempo que ele anda em cada direção

    private int direction = 1;     // 1 = direita, -1 = esquerda
    private float timer;

    public float Vida = 2; 

    void Start()
    {
        timer = moveTime;          // inicia o timer
    }

    public void Update()
    {
        // move o inimigo no eixo X
        transform.position += new Vector3(direction * speed * Time.deltaTime, 0, 0);

        // diminui o timer
        timer -= Time.deltaTime;

        // quando o timer acaba, inverte a direção e reseta o timer
        if (timer <= 0f)
        {
            direction *= -1;
            timer = moveTime;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
            
        if (collision.CompareTag("ShotPlayer")) 
        {

            Vida--;

            if (Vida == 0) 
            {
            
                Destroy(gameObject);    
            
            }
        
        }

    }
}