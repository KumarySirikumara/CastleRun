using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelComplete : MonoBehaviour
{
    public Text scoreCount;
    
    public void updateScore(int score){

        scoreCount.text = score.ToString();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Player"){
            SceneManager.LoadScene("LevelComplete");
        }
    }
}
