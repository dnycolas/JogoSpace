using System.Collections;
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
    public float frameRate = 0.12f;

    [Header("Patrulha")]
    public float patrolRange = 3f;
    public float speed = 1.5f;
    public float pauseDuration = 1f;

    [Header("Detecção / Tiro")]
    public float rangedVisionEnemie = 6f;
    public float verticalTolerance = 0.5f;
    public float AlcanceTiro = 4f;
    public float StartTimeBtwnShots = 1.0f;
    public float projectileSpeed = 10f;

    // estado patrulha interna
    private float startX;
    private bool patrolRight = true;
    private float pauseTimer = 0f;
    private float timeBtwnShots = 0f;

    // componentes
    private SpriteRenderer sr;

    // estados
    private enum State { Idle, Walk, Shoot, Die }
    private State currentState = State.Idle;

    // animação
    private int currentFrameIndex = 0;
    private float frameTimer = 0f;
    private Coroutine oneShotCoroutine = null;

    void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) Debug.LogError("[InimigoAtirador] Precisa de SpriteRenderer.");

        startX = transform.position.x;

        // começa com o primeiro frame do idle
        SetState(State.Idle);
    }

    void Update()
    {
        if (player == null) return;

        // timers
        if (timeBtwnShots > 0f) timeBtwnShots -= Time.deltaTime;
        if (pauseTimer > 0f) pauseTimer -= Time.deltaTime;

        // checa visão
        float distanceX = Mathf.Abs(player.position.x - transform.position.x);
        float distanceY = Mathf.Abs(player.position.y - transform.position.y);
        bool inVision = (distanceX <= rangedVisionEnemie && distanceY <= verticalTolerance);

        // lógica principal
        if (currentState != State.Die)
        {
            if (inVision && Vector2.Distance(transform.position, player.position) <= AlcanceTiro)
            {
                FaceTowards(player.position.x);
                // se pode atirar agora, dispara e entra em Shoot
                if (timeBtwnShots <= 0f)
                {
                    ShootNow();
                    timeBtwnShots = StartTimeBtwnShots;
                }
                else
                {
                    // está no range mas recarregando: mostra Idle (ou Walk se preferir)
                    SetState(State.Idle);
                }
            }
            else
            {
                // patrulha normal
                Patrol();
            }
        }

        // atualiza animação looping apenas para Idle/Walk
        if (currentState == State.Idle || currentState == State.Walk)
            UpdateLoopingAnimation();
    }

    void Patrol()
    {
        if (pauseTimer > 0f)
        {
            SetState(State.Idle);
            return;
        }

        SetState(State.Walk);

        float dir = patrolRight ? 1f : -1f;
        transform.Translate(Vector2.right * dir * speed * Time.deltaTime);

        if (transform.position.x >= startX + patrolRange && patrolRight)
        {
            patrolRight = false;
            pauseTimer = pauseDuration;
            SetState(State.Idle);
            FaceTowards(transform.position.x - 1f);
        }
        else if (transform.position.x <= startX - patrolRange && !patrolRight)
        {
            patrolRight = true;
            pauseTimer = pauseDuration;
            SetState(State.Idle);
            FaceTowards(transform.position.x + 1f);
        }
    }

    void ShootNow()
    {
        // instantiate projectile
        if (TiroEnemie == null || ShotPoint == null)
        {
            Debug.LogWarning("[InimigoAtirador] TiroEnemie ou ShotPoint não atribuídos.");
            return;
        }

        Vector2 dir = (player.position - ShotPoint.position).normalized;
        GameObject bala = Instantiate(TiroEnemie, ShotPoint.position, Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg));
        Rigidbody2D rb = bala.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = dir * projectileSpeed;

        // toca animação de tiro *uma vez* e volta pra Idle quando acabar
        if (oneShotCoroutine != null) StopCoroutine(oneShotCoroutine);
        oneShotCoroutine = StartCoroutine(PlayOneShotAnimation(shootFrames, () => {
            SetState(State.Idle);
            oneShotCoroutine = null;
        }));
    }

    // animação looping (Idle/Walk)
    void UpdateLoopingAnimation()
    {
        Sprite[] frames = (currentState == State.Walk ? walkFrames : idleFrames);
        if (frames == null || frames.Length == 0) return;

        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            currentFrameIndex++;
            if (currentFrameIndex >= frames.Length) currentFrameIndex = 0;
            sr.sprite = frames[currentFrameIndex];
        }
    }

    // play frames one-shot
    IEnumerator PlayOneShotAnimation(Sprite[] frames, System.Action onComplete)
    {
        if (frames == null || frames.Length == 0)
        {
            onComplete?.Invoke();
            yield break;
        }

        // força estado de shoot (impede UpdateLoopingAnimation)
        currentState = State.Shoot;

        for (int i = 0; i < frames.Length; i++)
        {
            sr.sprite = frames[i];
            yield return new WaitForSeconds(frameRate);
        }

        onComplete?.Invoke();
    }

    void SetState(State newState)
    {
        if (currentState == newState) return;

        // se havia uma one-shot em andamento, cancela
        if (oneShotCoroutine != null)
        {
            StopCoroutine(oneShotCoroutine);
            oneShotCoroutine = null;
        }

        currentState = newState;
        currentFrameIndex = 0;
        frameTimer = 0f;

        // mostra primeiro frame imediato
        Sprite[] frames = null;
        switch (newState)
        {
            case State.Idle: frames = idleFrames; break;
            case State.Walk: frames = walkFrames; break;
            case State.Shoot: frames = shootFrames; break;
            case State.Die: frames = dieFrames; break;
        }
        if (frames != null && frames.Length > 0) sr.sprite = frames[0];
    }

    void FaceTowards(float targetX)
    {
        Vector3 s = transform.localScale;
        s.x = (targetX > transform.position.x) ? Mathf.Abs(s.x) : -Mathf.Abs(s.x);
        transform.localScale = s;
    }

    // função pública pra matar o inimigo (se quiser usar)
    public void Die()
    {
        if (oneShotCoroutine != null) StopCoroutine(oneShotCoroutine);
        oneShotCoroutine = StartCoroutine(PlayOneShotAnimation(dieFrames, () => {
            Destroy(gameObject);
        }));
        currentState = State.Die;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangedVisionEnemie);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AlcanceTiro);
    }
}
