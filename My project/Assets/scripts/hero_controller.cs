using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class hero_controller : MonoBehaviour {

    // @NOTE: global...
    public Slider vida_slide;
    public Slider poder_slide;
    public LineRenderer spell_trail;

    public ParticleSystem smoke_effect;
    public List<spell_template> magic_spell_list;
    public Light orb_light;
    public Animator hand_animator;
    
    // @NOTE: pvp stuff
    public GameObject this_spell;
    public Transform  target;

    // @NOTE: Constants
    public float MI_LIMITE_DE_VIDA  = 100.0f;
    public float MI_LIMITE_DE_PODER = 100.0f;
    public float MINIMO_DE_PODER_PARA_DISPARAR = 25.0f;
    // @NOTE: in seconds!
    public float DURACION_REGENERACION_DE_PODER = 0.5f;
    public float RESTABLECIMIENTO_DE_PODER = 5.0f;
    public float CONSUMO_DE_PODER          = 2.5f;

    // @NOTE: do reset!
    public float VIDA;
    public float PODER;

    public int active_spell;
    public int[] spell_draw_miss_count_array;
    public int[] spell_ready_amount_array;
    public List<Vector3> draw_spell_list;
    
    // @NOTE: active effects!!!
    public float regenerating_PODER;
    public bool shooting = false;
    
    void reset_hero() {
        active_spell = -1;

        VIDA  = MI_LIMITE_DE_VIDA;
        PODER = 0;

        regenerating_PODER = 0;
        for(int i = 0; i < spell_draw_miss_count_array.Length; ++i) {
            spell_draw_miss_count_array[i] = 0;
        }
    }

    void Start() {
        vida_slide   = GameObject.Find("health_bar").GetComponent<Slider>();
        poder_slide  = GameObject.Find("magic_bar").GetComponent<Slider>();
        smoke_effect = GameObject.Find("smoke_effect").GetComponent<ParticleSystem>();
        target       = GameObject.Find("enemy_controller").GetComponent<Transform>();
        this_spell   = GameObject.Find("spell");
        orb_light    = GameObject.Find("orb_light").GetComponent<Light>();

        spell_draw_miss_count_array = new int[magic_spell_list.Count];
        spell_ready_amount_array    = new int[magic_spell_list.Count];
        reset_hero();
    }
    
    void Update() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position         = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results               = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        float spell_damage = 0.0f;
        
        bool immediate_shoot = false;
        bool casting         = false;
        bool start_casting   = false;
        bool end_casting     = false;

        bool block_spell_casting = (shooting && PODER < MINIMO_DE_PODER_PARA_DISPARAR);
        if(shooting) {
            shooting = (PODER < MINIMO_DE_PODER_PARA_DISPARAR);
        }

        if(!block_spell_casting) {
            if (Input.GetButtonDown("Fire1")) {
                casting = true;
                start_casting = true;
            } else if (Input.GetButton("Fire1")) {
                casting = true;
            } 
        }
        
        if(Input.GetButtonUp("Fire1") || PODER <= 0) {
            casting = false;
            end_casting = true;
        }
        
        if(casting) {
            if(PODER > 0) {
                PODER -= Time.deltaTime * CONSUMO_DE_PODER;
            }
        } else {
            // regenerating_PODER -= Time.deltaTime;
            PODER += Time.deltaTime * RESTABLECIMIENTO_DE_PODER;

            if(PODER > MI_LIMITE_DE_PODER) PODER = MI_LIMITE_DE_PODER;
        }

        poder_slide.value = PODER / MI_LIMITE_DE_PODER;
        vida_slide.value  = VIDA  / MI_LIMITE_DE_VIDA;

        if(start_casting && active_spell == -1) {
            for(int index = 0; index < magic_spell_list.Count; ++index) {
                spell_template spell = magic_spell_list[index];
                for(int i = 0; i < results.Count; ++i) {
                    RaycastResult it = results[i];

                    if(it.gameObject.tag.Equals(spell.transform.tag + "_begin")) {
                        active_spell = index;
                        break;
                    }
                }
            }
        }
        
        bool valid_spell_end = false;
        if(end_casting) {
            bool is_valid = false;
            for(int index = 0; index < magic_spell_list.Count; ++index) {
                spell_template spell = magic_spell_list[index];
                for(int i = 0; i < results.Count; ++i) {
                    RaycastResult it = results[i];

                    if(active_spell == index && it.gameObject.tag.Equals(spell.transform.tag + "_end")) {
                        is_valid = true;
                        break;
                    }
                }
            }
            if(is_valid) {
                valid_spell_end = true;
            }
        }
        
        if(!shooting && casting) {
            Vector3 mouse_pos = Input.mousePosition;

            bool add_new  = false;
            int add_index = 0;
            if(draw_spell_list.Count > 0) {
                add_new = true;
                for(int i = 0; i < draw_spell_list.Count; ++i) {
                    Vector3 it = draw_spell_list[i]; 

                    if(Vector3.Distance(it, mouse_pos) < 5.0f) {
                        add_new = false;
                        break;
                    }
                }
                add_index = draw_spell_list.Count;
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

       // ParticleSystem.EmissionModule smoke_emission = smoke_effect.emission;
       // smoke_effect.emission.enabled = (draw_spell_list.Count > 0);
        var em = smoke_effect.emission;
		em.enabled = (draw_spell_list.Count > 0);
        //
        // @NOTE: Where the MAGIC ocurres...
        //
        if(draw_spell_list.Count > 0) {
            for(int index = 0; index < magic_spell_list.Count; ++index) {
                spell_template spell = magic_spell_list[index];
                int count = 1;

                for(int i = 0; i < results.Count; ++i) {
                    RaycastResult it = results[i];

                    if(it.gameObject.tag.Contains(spell.transform.tag)) {
                        count = 0;
                        break;
                    }
                }

                spell_draw_miss_count_array[index] += count;
            }
        }

        for(int i = 0; i < magic_spell_list.Count; ++i) {
            spell_template it_spell    = magic_spell_list[i];
            ParticleSystem it_particle = it_spell.particles;
            int          miss_count    = spell_draw_miss_count_array[i];

            ParticleSystem.EmissionModule emission = it_particle.emission;
            ParticleSystem.MainModule     settings = it_particle.main;
            if(miss_count < 25 && draw_spell_list.Count > 25) {
                LineRenderer debug_spell_trail = it_spell.debug_spell_trail;

                orb_light.color     = debug_spell_trail.startColor;
                settings.startColor = new ParticleSystem.MinMaxGradient( debug_spell_trail.endColor );

                spell_trail.startColor = debug_spell_trail.startColor;
                spell_trail.endColor   = debug_spell_trail.endColor;

                if(!emission.enabled) {
                    emission.enabled = true;
                    emission.rateOverTime = 3.0f;
                }
            }
        }

        if(valid_spell_end) {
            for(int i = 0; i < spell_draw_miss_count_array.Length; ++i) {
                int miss_count = spell_draw_miss_count_array[i];
                if(miss_count < 25 && draw_spell_list.Count > 25) {
                    spell_ready_amount_array[i] += 1;
                    // keep_maging = true;
                }

                spell_draw_miss_count_array[i] = 0;
            }
        }

        if(end_casting) {
            for(int i = 0; i < spell_ready_amount_array.Length; ++i) {
                int it = spell_ready_amount_array[i];
                spell_template it_spell    = magic_spell_list[i];
                ParticleSystem it_particle = it_spell.particles;
            
                ParticleSystem.EmissionModule emission = it_particle.emission;
                ParticleSystem.MainModule     settings = it_particle.main;

                if(it == 0) {
                    if(emission.enabled) {
                        emission.enabled = false;
                        it_particle.Clear();
                        orb_light.color = Color.gray;
                    }
                } else {
                    emission.rateOverTime = 10.0f;
                    spell_damage = 20.0f;

                    switch(it_spell.type) {
                        case spell_template.spell_type.BASIC_ATTACK: {
                            spell_damage += 15.0f;
                        } break;
                        case spell_template.spell_type.BASIC_DEFENSE: {
                            VIDA += 25.0f;
                        } break;
                        case spell_template.spell_type.BASIC_REGEN_VIDA: {
                            VIDA += 15.0f;
                        } break;
                        case spell_template.spell_type.BASIC_REGEN_PODER: {
                            PODER += 25.0f;
                        } break;
                    };
                }
            }

            valid_spell_end = false;
            draw_spell_list.Clear();
                
            for(int i = 0; i < spell_draw_miss_count_array.Length; ++i) {
                spell_draw_miss_count_array[i] = 0;
            }
            
            if(shooting == false) {
                shooting = true;
                casting = false;

                GameObject proj_obj = GameObject.Instantiate(this_spell);
                projectile proj     = proj_obj.AddComponent<projectile>();
                proj_obj.tag = "hero_projectile";

                proj.transform.position = this_spell.transform.position;
                proj.origin = this_spell.transform;
                proj.target = target;
                proj.damage = spell_damage;
                
                proj_obj.transform.LookAt(target);
                proj.is_active = true;
                
                for(int i = 0; i < magic_spell_list.Count; ++i) {
                    spell_template it_spell    = magic_spell_list[i];
                    ParticleSystem it_particle = it_spell.particles;

                    ParticleSystem.EmissionModule emission = it_particle.emission;
                    ParticleSystem.MainModule     settings = it_particle.main;

                    emission.enabled = false; 
                    it_particle.Clear();
                }
                spell_trail.startColor = Color.white;
                spell_trail.endColor   = Color.gray;

                draw_spell_list.Clear();
                spell_trail.positionCount = 0;
            }
                
            for(int i = 0; i < spell_ready_amount_array.Length; ++i) {
                spell_ready_amount_array[i] = 0;
            }

            active_spell = -1;
        }

        hand_animator.SetBool("casting", casting);
        hand_animator.SetBool("shoot", shooting);    
    }
}

        /*
        int draw_spell_point_count = draw_spell_list.Count;
        if(draw_spell_point_count > 0) {
            float min_x = draw_spell_list[0].x;
            float min_y = draw_spell_list[0].y;
            float max_x = draw_spell_list[0].x;
            float max_y = draw_spell_list[0].y;

            for(int i = 0; i < draw_spell_point_count; ++i) {
                Vector3 it = draw_spell_list[i];
                
                if(min_x > it.x) min_x = it.x;
                if(min_y > it.y) min_y = it.y;
                if(max_x < it.x) max_x = it.x;
                if(max_y < it.y) max_y = it.y;
            }
            
            float dim_x = max_x - min_x;
            float dim_y = max_y - min_y;

            for(int index = 0; index < spell_match_percent_array.Length; ++index) {
                spell_template spell = magic_spell_list[index];

                Vector3 start_spell_point = spell.spell_list[0];
                Vector3 end_spell_point = spell.spell_list[spell.spell_list.Length - 1];

                Vector3 start_draw_point = draw_spell_list[0];
                Vector3 end_draw_point = draw_spell_list[draw_spell_point_count - 1];
                start_draw_point.x  = start_draw_point.x - min_x;
                start_draw_point.y  = start_draw_point.y - min_y;
                end_draw_point.x    = end_draw_point.x - min_x;
                end_draw_point.y    = end_draw_point.y - min_y;
                
                for(int i = 0; i < draw_spell_point_count; ++i) {
                    Vector3 it_draw = draw_spell_list[i];

                    // @NOTE: Incomplete
                    float x = it_draw.x - min_x; 
                    float y = it_draw.y - min_y;
                    Vector3 fixed_it_draw = new Vector3(x, y, it_draw.z);
                    
                    float closest_distance = 10000.0f; // @NOTE: Temporary...
                    for(int j = 0; j < spell.spell_list.Length; ++j) {
                        Vector3 it_spell = spell.spell_list[j];     
                        float _x = it_spell.x;
                        float _y = it_spell.y;
                        
                        float d = (float) Vector3.Distance(fixed_it_draw, it_spell);
                        if(d < closest_distance) {
                            closest_distance = d;
                        }
                    }
                    
                    if(closest_distance > 10.0f) {
                        spell_match_percent_array[index] += closest_distance;
                    }
                }

                Debug.Log("start: "+Vector3.Distance(start_draw_point, start_spell_point) );
                Debug.Log("end: "+Vector3.Distance(end_draw_point, end_spell_point));
                if(Vector3.Distance(start_draw_point, start_spell_point) < 20.0f 
                    && Vector3.Distance(end_draw_point, end_spell_point) < 20.0f) {
                    float it_percent = spell_match_percent_array[index];
                    spell_match_percent_array[index] = ((3500.0f - it_percent) / 3500.0f);
                } else {
                    spell_match_percent_array[index] = 0.0f;
                }

            }
        }
        */
