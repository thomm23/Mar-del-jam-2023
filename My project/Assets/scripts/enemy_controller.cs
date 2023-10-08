using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy_controller : MonoBehaviour
{
    public float MI_LIMITE_DE_VIDA  = 200.0f;
    public float VIDA;
    public float shoot_t;

    public hero_controller hero;
    public GameObject a_spell;
    public Transform target;
    public Slider vida_slide;

    public void Start() {
        shoot_t = 0;
        VIDA = MI_LIMITE_DE_VIDA;
    }

    public void Update() {
        shoot_t += Time.deltaTime;

        vida_slide.value  = VIDA  / MI_LIMITE_DE_VIDA;

        if(shoot_t > 5.0f) {
            shoot_t = Random.Range(-3.0f, 0.0f);
                GameObject proj_obj = GameObject.Instantiate(a_spell);
                projectile proj     = proj_obj.AddComponent<projectile>();
                proj_obj.tag = "enemy_projectile";
                proj.transform.position = a_spell.transform.position;
                proj.origin = a_spell.transform;
                proj.target = target;
                
                proj_obj.transform.LookAt(target);
                proj.is_active = true;
        }

    }


}
