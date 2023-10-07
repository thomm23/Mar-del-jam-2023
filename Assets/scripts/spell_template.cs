using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spell_template : MonoBehaviour
{
    public enum spell_type {
        BASIC_ATTACK,
        BASIC_DEFENSE,
        BASIC_REGEN_VIDA,
        BASIC_REGEN_PODER,
    };

    public hero_controller hero_controller;
    public Vector3[] spell_list;
    public LineRenderer debug_spell_trail;
    public KeyCode key;
    public spell_type type;

    void Start() {
        hero_controller = GameObject.Find("hero_controller").GetComponent<hero_controller>();
        debug_spell_trail = GetComponent<LineRenderer>();
    }

    void Update() {
        if(Input.GetKeyDown(key)) {
            spell_list = hero_controller.draw_spell_list.ToArray();

            debug_spell_trail.positionCount = spell_list.Length;
            for(int i = 0; i < spell_list.Length; ++i) {
                Vector3 it = spell_list[i];
                
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(it.x, it.y, Camera.main.nearClipPlane));
                debug_spell_trail.SetPosition(i, point);
            }
        }
    }
}
