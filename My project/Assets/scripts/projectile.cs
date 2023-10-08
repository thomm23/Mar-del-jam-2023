using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {
    public Transform origin;
    public Transform target;
    
    public bool is_active = false;
    public float damage;
    
    public hero_controller  hero;
    public enemy_controller enemy;
    public end_game_controller end_game;

    void Start() {
        hero  = GameObject.Find("hero_controller").GetComponent<hero_controller>();
        enemy = GameObject.Find("enemy_controller").GetComponent<enemy_controller>();
        end_game = GameObject.Find("end_game_handler").GetComponent<end_game_controller>();
    }

    void Update() {
        if(is_active) {
            float speed = 5.0f;
            transform.position += transform.forward * Time.deltaTime * speed;
        }
    }

    void check_if_end_the_game() {
        if(hero.VIDA < 0) {
            end_game.end(0);
        } else if(enemy.VIDA < 0) {
            end_game.end(1);
        }
    }

    void OnTriggerEnter(Collider other) {
        if(tag.Equals("enemy_projectile") && other.tag.Equals("Player")) {
            hero.VIDA -= damage; 
            check_if_end_the_game();
            Destroy(gameObject);
        } else if(tag.Equals("hero_projectile") && other.tag.Equals("Enemy")) {
            enemy.VIDA -= damage; 
            check_if_end_the_game();
            Destroy(gameObject);
        } else if(tag.Equals("enemy_projectile") && other.tag.Equals("hero_projectile")) {
            //Destroy(other.gameObject);
            //Destroy(gameObject);
        } else if(tag.Equals("enemy_projectile") && other.tag.Equals("hero_projectile")) {
            //Destroy(other.gameObject);
            //Destroy(gameObject);
        }
    }
}
