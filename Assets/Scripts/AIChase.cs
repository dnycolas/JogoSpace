using UnityEngine;

public class AIChase : MonoBehaviour
{
    public GameObject Player;
    public float speed = 3f;
    public float detectionRange = 4f;
    public float attackRange = 1f;
    public float reactionTime = 0.5f;

    private float distance;
    private float attackTimer = 0f;

    public float Vida = 2;

    [Header("Sprites Idle")]
    public Sprite[] idleFrames;
    [Header("Sprites Chase")]
    public Sprite[] chaseFrames;
    [Header("Sprites Attack")]
    public Sprite[] attackFrames;
    [Header("Sprites Death")]
    public Sprite[] deathFrames;

    public float frameRate = 0.15f; // tempo entre frames
    private float frameTimer = 0f;
    private int currentFrame = 0;

    private SpriteRenderer sr;

    private enum State { Idle, Chase, Attack, Death }
    private State currentState = State.Idle;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        SetState(State.Idle);
    }

    void Update()
    {
        if (currentState != State.Death) // só anima se não estiver morto
        {
            distance = Vector2.Distance(transform.position, Player.transform.position);

            if (distance < detectionRange)
            {
                attackTimer += Time.deltaTime;

                if (distance > attackRange)
                {
                    // perseguindo
                    Vector2 direction = (Player.transform.position - transform.position).normalized;
                    transform.position += (Vector3)(direction * speed * Time.deltaTime);
                    SetState(State.Chase);
                }
                else if (attackTimer >= reactionTime)
                {
                    SetState(State.Attack);
                    Debug.Log("Atacou!");
                    attackTimer = 0f;
                }
            }
            else
            {
                attackTimer = 0f;
                SetState(State.Idle);
            }
        }

        Animate();
    }

    void Animate()
    {
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrame++;

            Sprite[] frames = GetCurrentFrames();
            if (frames.Length > 0)
            {
                if (currentFrame >= frames.Length) currentFrame = 0;
                sr.sprite = frames[currentFrame];
            }
        }
    }

    Sprite[] GetCurrentFrames()
    {
        switch (currentState)
        {
            case State.Chase: return chaseFrames;
            case State.Attack: return attackFrames;
            case State.Death: return deathFrames;
            default: return idleFrames;
        }
    }

    void SetState(State newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            currentFrame = -1; // força reset da animação
            frameTimer = frameRate;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ShotPlayer") && currentState != State.Death)
        {
            Vida--;

            if (Vida <= 0)
            {
                SetState(State.Death);
                GameManager.instance.AddKill();
                Destroy(gameObject, 0.5f); // dá tempo da animação de morte
            }
        }
    }
}
