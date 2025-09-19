using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefab para spawn")]
    public GameObject prefab;       // o prefab que você quer gerar
    public Transform spawnPoint;    // posição base para spawn
    public Vector3 offset;          // offset adicional (ex: 0,3,0)

    [Header("Controle")]
    public float spawnInterval = 2f; // tempo entre spawns
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnPrefab();
            timer = 0f;
        }
    }

    void SpawnPrefab()
    {
        if (prefab != null && spawnPoint != null)
        {
            Vector3 spawnPos = spawnPoint.position + offset;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
