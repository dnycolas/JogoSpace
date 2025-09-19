using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointFinal : MonoBehaviour
{
    public Transform platform;         // arrasta a plataforma aqui no Inspector
    public float moveSpeed = 2f;       // velocidade que a plataforma se move
    public Vector2 respawnPoint = new Vector2(-2.6f, 43.8f); // respawn salvo

    private bool activated = false;
    private Vector3 startPosition = new Vector3(-9.78f, 34.15f, 0f);
    private Vector3 closedPosition = new Vector3(-5.07f, 34.15f, 0f);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag("Player"))
        {
            activated = true;

            // 1. Some com todos os inimigos
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemie");
            foreach (GameObject enemy in enemies)
            {
                Destroy(enemy);
            }

            // 2. Define o novo respawn no Player
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.SetRespawnPoint(respawnPoint);
            }

            // 3. Move a plataforma pra fechar a entrada
            if (platform != null)
            {
                StartCoroutine(MovePlatform(startPosition, closedPosition));
            }
        }
    }

    private System.Collections.IEnumerator MovePlatform(Vector3 start, Vector3 end)
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            platform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        platform.position = end;
    }
}

