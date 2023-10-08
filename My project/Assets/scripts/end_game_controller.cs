using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class end_game_controller : MonoBehaviour
{
    public bool game_ended = false;
    public GameObject win_text;
    public GameObject lose_text;
    
    void Update() {
        if(game_ended) {
            if(Input.GetKeyDown(KeyCode.Space)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void end(int reason) {
        game_ended = true;

        if(reason == 1) {
            win_text.SetActive(true);
        } else if(reason == 0) {
            lose_text.SetActive(true);
        }
    }

}
