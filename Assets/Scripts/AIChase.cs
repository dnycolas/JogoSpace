using UnityEngine;

public class AIChase : MonoBehaviour
{
    public GameObject Player;
    public float speed = 3f;
    public float detectionRange = 4f;
    public float attackRange = 1f;
    public float reactionTime = 0.5f; // tempo antes de atacar

    private float distance;
    private float attackTimer = 0f;

    void Update()
    {
        distance = Vector2.Distance(transform.position, Player.transform.position);

        if (distance < detectionRange)
        {
            attackTimer += Time.deltaTime;

            if (distance > attackRange)
            {
                // Move em direção ao player
                Vector2 direction = (Player.transform.position - transform.position).normalized;
                transform.position += (Vector3)(direction * speed * Time.deltaTime);
            }
            else if (attackTimer >= reactionTime)
            {
                // Aqui você coloca o ataque (ex: dano, animação)
                Debug.Log("Atacou!");
                attackTimer = 0f; // reseta o timer
            }
        }
        else
        {
            // Se o player sair da detecção, resetar timer
            attackTimer = 0f;
        }
    }
}
