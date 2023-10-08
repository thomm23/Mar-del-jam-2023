using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_sky : MonoBehaviour
{
    public float turn_speed = 20.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, turn_speed * Time.deltaTime);
    }
}
