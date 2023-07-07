using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempHealth : MonoBehaviour, IDamageable
{
    public float health = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Damage(float damage)
    {
        health -= damage;

        if(health <= 0)
            Destroy(gameObject);
    }
}
