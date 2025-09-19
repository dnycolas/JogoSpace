using UnityEngine;

public class CheckPointCleaner : MonoBehaviour
{
    [Tooltip("Ponto de respawn que será aplicado ao player quando tocar o checkpoint")]
    public Vector2 respawnPoint = new Vector2(-2.6f, 43.8f);

    [Tooltip("Se true, destrói os inimigos. Se false, apenas desativa (SetActive(false)).")]
    public bool destroyEnemies = true;

    [Tooltip("Tag usada pelos inimigos (ex: 'Enemie')")]
    public string enemyTag = "Enemie";

    [Tooltip("Tag do player (deve ser 'Player')")]
    public string playerTag = "Player";

    [Tooltip("Desativa o checkpoint depois de usado")]
    public bool oneUseOnly = true;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used) return;

        if (!other.CompareTag(playerTag)) return; // só reage ao player

        // tenta setar o respawn no Player (método SetRespawnPoint)
        var playerScript = other.GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.SetRespawnPoint(respawnPoint);
        }
        else
        {
            Debug.LogWarning("[CheckPointCleaner] Player entrou no checkpoint, mas o componente Player não foi encontrado no GameObject.");
        }

        // encontra todos inimigos pela tag e remove/desativa
        GameObject[] inimigos = GameObject.FindGameObjectsWithTag(enemyTag);
        for (int i = 0; i < inimigos.Length; i++)
        {
            if (destroyEnemies)
                Destroy(inimigos[i]);
            else
                inimigos[i].SetActive(false);
        }

        used = true;
        if (oneUseOnly)
        {
            // desativa o collider/objeto pra não disparar de novo
            var col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;
            // opcional: desativa a própria GameObject
            // gameObject.SetActive(false);
        }

        Debug.Log("[CheckPointCleaner] Checkpoint ativado. Inimigos limpos e respawn setado em " + respawnPoint);
    }

    // gizmo opcional para visualizar respawn no editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(new Vector3(respawnPoint.x, respawnPoint.y, 0f), 0.15f);
    }
}
