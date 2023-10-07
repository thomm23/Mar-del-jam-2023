using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hero_controller : MonoBehaviour {

    // @NOTE: global...
    public Slider vida_slide;
    public Slider poder_slide;
    public List<spell_template> magic_spell_list;
    public LineRenderer spell_trail;

    public ParticleSystem smoke_effect;
    public ParticleSystem magic_effect;
    
    public GameObject this_spell;
    public Transform  target;

    public float MI_LIMITE_DE_VIDA         = 100.0f;
    public float MI_LIMITE_DE_PODER        = 100.0f;
    public float MINIMO_DE_PODER_PARA_DISPARAR = 25.0f;

    // @NOTE: in seconds!
    public float DURACION_REGENERACION_DE_PODER = 0.5f;
    public float RESTABLECIMIENTO_DE_PODER = 5.0f;
    public float CONSUMO_DE_PODER          = 2.5f;

    // @NOTE: do reset!
    public bool  casting;
    public float VIDA;
    public float PODER;
    public List<Vector3> draw_spell_list;

    // @NOTE: active effects!!!
    bool shooting = false;

    public float regenerating_PODER;
    
    void reset_hero() {
        VIDA  = MI_LIMITE_DE_VIDA;
        PODER = MI_LIMITE_DE_PODER;

        regenerating_PODER = 0;
    }

    void Start() {
        vida_slide   = GameObject.Find("health_bar").GetComponent<Slider>();
        poder_slide  = GameObject.Find("magic_bar").GetComponent<Slider>();
        smoke_effect = GameObject.Find("smoke_effect").GetComponent<ParticleSystem>();
        magic_effect = GameObject.Find("magic_effect").GetComponent<ParticleSystem>();
        target       = GameObject.Find("enemy").GetComponent<Transform>();
        this_spell   = GameObject.Find("spell");

        reset_hero();
    }

    void Update() {
        bool block_spell_casting = (shooting && PODER < MINIMO_DE_PODER_PARA_DISPARAR);

        if(block_spell_casting) {
            casting = false;
        } else {
            if (Input.GetButtonDown("Fire1")) {
                casting = true;
            } 
        }

        if(Input.GetButtonUp("Fire1")) {
            draw_spell_list.Clear();
            casting = false;
        }
        
        if(casting) {
            if(PODER > 0) {
                PODER -= Time.deltaTime * CONSUMO_DE_PODER;
            }
        } else {
            if(PODER < MI_LIMITE_DE_PODER) {
                regenerating_PODER -= Time.deltaTime;
                PODER += Time.deltaTime * RESTABLECIMIENTO_DE_PODER;
            } else {
                PODER = MI_LIMITE_DE_PODER;
            }
        }

        if(PODER <= 0) {
            if(shooting == false) {
                shooting = true;

                GameObject proj_obj = GameObject.Instantiate(this_spell);
                projectile proj     = proj_obj.AddComponent<projectile>();

                proj.transform.position = this_spell.transform.position;
                proj.origin = this_spell.transform;
                proj.target = target;
                proj.distance_to_explode = Vector3.Distance(proj.origin.position, proj.target.position);
                
                proj_obj.transform.LookAt(target);
                proj.is_active = true;
                
                draw_spell_list.Clear();
                spell_trail.positionCount = 0;
            }
        }

        if(shooting) {
            shooting = (PODER < MINIMO_DE_PODER_PARA_DISPARAR);
        }

        poder_slide.value = PODER / MI_LIMITE_DE_PODER;
        vida_slide.value  = VIDA  / MI_LIMITE_DE_VIDA;

        if(casting && PODER > 0) {
            Vector3 mouse_pos = Input.mousePosition;

            bool add_new = false;
            int add_index = 0;
            if(draw_spell_list.Count > 0) {
                int index    = draw_spell_list.Count - 1;
                Vector3 last = draw_spell_list[index]; 

                if(Vector3.Distance(last, mouse_pos) > 5.0f) {
                    add_new = true;
                    add_index = draw_spell_list.Count;
                }
            } else {
                add_new = true;
            }

            if(add_new) {
                draw_spell_list.Add(mouse_pos);
                Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3(mouse_pos.x, mouse_pos.y, Camera.main.nearClipPlane));
                spell_trail.positionCount = draw_spell_list.Count;
                spell_trail.SetPosition(add_index, point);
            }
        }

        //
        // @NOTE: Where the MAGIC ocurres...
        //
        int spell_point_count = draw_spell_list.Count;
        if(spell_point_count > 0) {
            float min_x = draw_spell_list[0].x;
            float min_y = draw_spell_list[0].y;
            float max_x = draw_spell_list[0].x;
            float max_y = draw_spell_list[0].y;
            
            for(int i = 0; i < spell_point_count; ++i) {
                Vector3 it = draw_spell_list[i];
                
                if(min_x > it.x) min_x = it.x;
                if(min_y > it.y) min_y = it.y;
                if(max_x < it.x) max_x = it.x;
                if(max_y < it.y) max_y = it.y;
            }
            
            float dim_x = max_x - min_x;
            float dim_y = max_y - min_y;

            float[] spell_match_percent_array = new float[magic_spell_list.Count];
            for(int index = 0; index < spell_match_percent_array.Length; ++index) {
                spell_template spell = magic_spell_list[index];

                for(int i = 0; i < spell_point_count; ++i) {
                    Vector3 it_draw = draw_spell_list[i];

                    // @NOTE: Incomplete
                    float x = it_draw.x; // - min_x; 
                    float y = it_draw.y; // - min_y;
                    
                    float closest_distance = 10000.0f; // @NOTE: Temporary...
                    for(int j = 0; j < spell.spell_list.Length; ++j) {
                        Vector3 it_spell = spell.spell_list[j];     
                        // float _x = it_spell.x;
                        // float _y = it_spell.y;
                        
                        float d = (float) Vector3.Distance(it_draw, it_spell);
                        if(d < closest_distance) {
                            closest_distance = d;
                        }
                    }
                    
                    if(closest_distance > 10.0f) {
                        spell_match_percent_array[index] += closest_distance;
                    }
                }
            }

            int spell_index = -1;
            for(int i = 0; i < spell_match_percent_array.Length; ++i) {
                float it_percent = spell_match_percent_array[i];

                if(it_percent < 3500.0f) {
                    // @NOTE: Temporary
                    if(spell_index == -1 || (spell_match_percent_array[i] > it_percent)) {
                        spell_index = i;
                    }
                }
            }
            
            ParticleSystem.EmissionModule smoke_emission = smoke_effect.emission;
            smoke_emission.enabled = (draw_spell_list.Count > 0);
            ParticleSystem.EmissionModule magic_emission = magic_effect.emission;
            magic_emission.enabled = (spell_index != -1);
            
            ParticleSystem.MainModule settings = magic_effect.main;

            if(spell_index != -1) {
                LineRenderer debug_spell_trail = magic_spell_list[spell_index].debug_spell_trail;

                spell_trail.startColor = debug_spell_trail.startColor;
                spell_trail.endColor   = debug_spell_trail.endColor;
                settings.startColor = new ParticleSystem.MinMaxGradient( spell_trail.endColor );

                switch(magic_spell_list[spell_index].type) {
                    case spell_template.spell_type.BASIC_ATTACK: {

                    } break;
                    case spell_template.spell_type.BASIC_DEFENSE: {

                    } break;
                    case spell_template.spell_type.BASIC_REGEN_VIDA: {

                    } break;
                    case spell_template.spell_type.BASIC_REGEN_PODER: {
                        regenerating_PODER = DURACION_REGENERACION_DE_PODER;
                    } break;
                };

            } else {
                spell_trail.startColor = Color.white;
                spell_trail.endColor   = Color.white;

                settings.startColor = new ParticleSystem.MinMaxGradient( spell_trail.endColor );
            }
        }
    }
}
