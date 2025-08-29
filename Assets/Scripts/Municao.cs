using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Munição : MonoBehaviour
{
    public float balaVelocidade;
    public float tempoBala;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyProjectile", tempoBala); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(transform.right * balaVelocidade * Time.deltaTime,Space.World);
    }

    void DestroyProjectile() 
    {
    
        Destroy(gameObject);

    }
}
