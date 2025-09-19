using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlataformaMover : MonoBehaviour
{
    [Header("Plataforma que vai se mover")]
    public GameObject plataforma; // arrasta a plataforma aqui no inspetor

    [Header("Nova posição da plataforma")]
    public Vector2 novaPosicao = new Vector2(-5.68f, 36.53f);

    private bool ativada = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ativada && other.CompareTag("Player"))
        {
            if (plataforma != null)
            {
                plataforma.transform.position = novaPosicao;
                ativada = true; // só move uma vez
            }
        }
    }
}
