using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetanguloMover : MonoBehaviour
{
    public float velocidade = 2f;       // velocidade do movimento
    public float distancia = 3f;        // até onde ele sobe/desce

    private Vector3 posInicial;
    private bool subindo = true;

    void Start()
    {
        posInicial = transform.position;
    }

    void Update()
    {
        if (subindo)
        {
            transform.position += Vector3.up * velocidade * Time.deltaTime;
            if (transform.position.y >= posInicial.y + distancia)
                subindo = false;
        }
        else
        {
            transform.position += Vector3.down * velocidade * Time.deltaTime;
            if (transform.position.y <= posInicial.y - distancia)
                subindo = true;
        }
    }
}
