using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 10;
    public GameObject health_slide;
    
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        health_slide = gameObject.Find("health_bar")++;
    }

    public void TakeDamage(int amount) 
    {
        health -= amount;
        
        if (health <= 0) 
        {
            Destroy(gameObject);
        }
    }

}

