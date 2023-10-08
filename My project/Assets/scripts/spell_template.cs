using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spell_template : MonoBehaviour
{
    public enum spell_type {
        NULL_ATTACK,
        BASIC_ATTACK,
        BASIC_DEFENSE,
        BASIC_REGEN_VIDA,
        BASIC_REGEN_PODER,
    };

    public hero_controller hero_controller;
    public LineRenderer debug_spell_trail;

    public ParticleSystem particles;
    public spell_type type;
    
    // public Vector3[] spell_list;
    // public KeyCode key;

    void Start() {
        hero_controller = GameObject.Find("hero_controller").GetComponent<hero_controller>();
        debug_spell_trail = GetComponent<LineRenderer>();
        particles = GetComponent<ParticleSystem>();
    }

    /*
    void Update() {
        if(Input.GetKeyDown(key)) {
            Vector3[] temp_spell_list = hero_controller.draw_spell_list.ToArray();
            
            float min_x = temp_spell_list[0].x;
            float min_y = temp_spell_list[0].y;
            float max_x = temp_spell_list[0].x;
            float max_y = temp_spell_list[0].y;
            
            for(int i = 0; i < temp_spell_list.Length; ++i) {
                Vector3 it = temp_spell_list[i];
                
                if(min_x > it.x) min_x = it.x;
                if(min_y > it.y) min_y = it.y;
                if(max_x < it.x) max_x = it.x;
                if(max_y < it.y) max_y = it.y;
            }
            
            float dim_x = max_x - min_x;
            float dim_y = max_y - min_y;

            spell_list = new Vector3[temp_spell_list.Length];
            for(int i = 0; i < temp_spell_list.Length; ++i) {
                Vector3 it = temp_spell_list[i];
                float x = it.x - min_x; 
                float y = it.y - min_y;

                spell_list[i] = new Vector3(x, y, it.z);
            }

            debug_spell_trail.positionCount = spell_list.Length;
            for(int i = 0; i < spell_list.Length; ++i) {
                Vector3 it = spell_list[i];
                
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(it.x, it.y, Camera.main.nearClipPlane));
                debug_spell_trail.SetPosition(i, point);
            }
        }
    }
    */
}
