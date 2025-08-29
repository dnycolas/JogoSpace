using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InimigoAtirador : InimigoBasico
{
    public GameObject Player;
    public float rangedVisionEnemie = 4f; // Alcance horizontal de detecção

    public Transform player;
    public Transform ShotPoint;
    public Transform gun;

    public GameObject TiroEnemie;

    public bool Alcance;

    public float AlcanceTiro;

    public float distanceX;

    public float StartTimeBtwnShots;
    private float timeBtwnShots;

    void Update()
    {
        // distância horizontal entre inimigo e player
        distanceX = Mathf.Abs(Player.transform.position.x - transform.position.x);

        speed = 1f;

        // se estiver dentro do alcance horizontal e quase na mesma altura
        if (distanceX <= rangedVisionEnemie && Mathf.Abs(Player.transform.position.y - transform.position.y) < 0.5f)
        {
            moveTime = 0f;
            Alcance = true;
        }
        else
        {
            Alcance = false;
            moveTime = 2f;
            base.Update();
        }

        if (Player.transform.position.x > transform.position.x)
        {
            ShotPoint.rotation = Quaternion.Euler(0, 0, 0);   // direita
        }
        else
        {
            ShotPoint.rotation = Quaternion.Euler(0, 180, 0); // esquerda
        }

        // Atira se estiver no alcance
        if (Vector2.Distance(transform.position, player.position) <= AlcanceTiro)
        {
            if (timeBtwnShots <= 0f)
            {
                Instantiate(TiroEnemie, ShotPoint.position, ShotPoint.rotation);
                timeBtwnShots = StartTimeBtwnShots;
            }
            else
            {
                timeBtwnShots -= Time.deltaTime;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangedVisionEnemie);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, AlcanceTiro);
    }
}