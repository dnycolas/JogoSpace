using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InimigoAtirador : InimigoBasico
{
    [Header("Referências")]
    public Transform player;
    public Transform ShotPoint;
    public GameObject TiroEnemie;

    [Header("Spritesheet Animations")]
    public Sprite[] idleFrames;
    public Sprite[] walkFrames;
    public Sprite[] shootFrames;
    public Sprite[] dieFrames;
    public float frameRate = 0.15f;

    [Header("Patrulha")]
    public float patrolRange = 3f;
    public float speed = 1.5f;
    public float pauseDuration = 1f;

    [Header("Detecção / Tiro")]
    public float rangedVisionEnemie = 6f;
    public float verticalTolerance = 0.5f;
    public float AlcanceTiro = 4f;
    public float StartTimeBtwnShots = 1.2f;
    public float projectileSpeed = 10f;

    private float startX;
    private bool patrolRight = true;
    private float pauseTimer = 0f;
    private float timeBtwnShots = 0f;
    private SpriteRenderer sr;

    private enum State { Idle, Walk, Shoot, Die }
    private State currentState = State.Idle;

    private Sprite[] currentFrames;
    private int currentFrameIndex = 0;
    private float frameTimer = 0f;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        startX = transform.position.x;
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);

        bool inRange = (distanceX <= rangedVisionEnemie && distanceY <= verticalTolerance);

        if (currentState != State.Die)
        {
            if (inRange)
            {
                currentState = State.Shoot;
                FaceTowards(player.position.x);
                Atirar();
            }
            else
            {
                Patrulhar();
            }
        }

        if (timeBtwnShots > 0f) timeBtwnShots -= Time.deltaTime;
        if (pauseTimer > 0f) pauseTimer -= Time.deltaTime;

        UpdateAnimation();
    }

    void FaceTowards(float targetX)
    {
        if (targetX > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void Patrulhar()
    {
        if (pauseTimer > 0f)
        {
            currentState = State.Idle;
            return;
        }

        currentState = State.Walk;

        float dir = patrolRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        if (transform.position.x >= startX + patrolRange && patrolRight)
        {
            patrolRight = false;
            pauseTimer = pauseDuration;
            currentState = State.Idle;
            FaceTowards(transform.position.x - 1f);
        }
        else if (transform.position.x <= startX - patrolRange && !patrolRight)
        {
            patrolRight = true;
            pauseTimer = pauseDuration;
            currentState = State.Idle;
            FaceTowards(transform.position.x + 1f);
        }
    }

    void Atirar()
    {
        if (Vector2.Distance(transform.position, player.position) <= AlcanceTiro)
        {
            if (timeBtwnShots <= 0f)
            {
                GameObject bala = Instantiate(TiroEnemie, ShotPoint.position, Quaternion.identity);

                Rigidbody2D rb = bala.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 direcao = (player.position - ShotPoint.position).normalized;
                    rb.velocity = direcao * projectileSpeed;

                    float angle = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
                    bala.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                }

                timeBtwnShots = StartTimeBtwnShots;
            }
        }
    }

    void UpdateAnimation()
    {
        switch (currentState)
        {
            case State.Idle: currentFrames = idleFrames; break;
            case State.Walk: currentFrames = walkFrames; break;
            case State.Shoot: currentFrames = shootFrames; break;
            case State.Die: currentFrames = dieFrames; break;
        }

        if (currentFrames != null && currentFrames.Length > 0)
        {
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                currentFrameIndex = (currentFrameIndex + 1) % currentFrames.Length;
                sr.sprite = currentFrames[currentFrameIndex];
            }
        }
    }
}