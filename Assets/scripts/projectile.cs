using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {
    public Transform origin;
    public Transform target;
    
    public bool is_active = false;
    public float distance_to_explode;

    void Update() {
        if(is_active) {
            float speed = 5.0f;
            transform.position += transform.forward * Time.deltaTime * speed;
            
            if(Vector3.Distance(origin.position, transform.position) > distance_to_explode) {
                Destroy(gameObject);
            }
        }
    }
}
