using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaFundo : MonoBehaviour
{
    public AudioSource Au;
    public AudioClip Aclip;


    // Start is called before the first frame update
    void Start()
    {
        
        Au.clip = Aclip;
        Au.Play();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
