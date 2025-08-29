using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpPoint : MonoBehaviour
{

    public int Vida;
    public int NumCoracoes;

    public Image[] hearts;
    public Sprite Coracao;
    public Sprite CoracaoVazio;

    private void Update()
    {

        if (Vida > NumCoracoes)
        { 
        
            Vida = NumCoracoes;

        }

        for (int i = 0; i < hearts.Length; i++) 
        {

            if (i < Vida) 
            {
            
                hearts[i].sprite = Coracao;


            }else 
            {

                hearts[i].sprite = CoracaoVazio;
                
            }

            if (i < NumCoracoes)
            {

                hearts[i].enabled = true;

            }
            else
            {

                hearts[i].enabled = false;

            }
        
        }
    }

    

}
