using System;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using UnityEngine.UI;


// Affichage des scores en fin de partie, le gagnant apparé en haut et les autres joueurs sont à la suite.
public class ScoreDisplay : MonoBehaviour
{
    public static bool gotToLobby = false;
    
     
    public Text scoreText;

    private void ClearText()
    {
        scoreText.text = String.Empty;
    }

    private void PrintScore()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
        Dictionary<GameObject, int> scoreBoard = new Dictionary<GameObject, int>();
        foreach (GameObject gameObject in allObjects)
        {
            if (gameObject.name == "Player(Clone)")
            {
                scoreBoard.Add(gameObject, gameObject.GetComponent<ScoreManager>().GetScore());
            }
        }
        
        KeyValuePair<GameObject, int> winner = scoreBoard.First();
        foreach (KeyValuePair<GameObject, int> kv in scoreBoard)
        {
            if (kv.Value > winner.Value) winner = kv;
        }
        scoreText.text = "The winner is :";
        scoreText.text += $"{winner.Key.GetComponentInChildren<UIHealth>().getPlayerName()} : {winner.Value} !\n ";
        foreach (KeyValuePair<GameObject, int> kv in scoreBoard)
        {
            if (kv.Key !=  winner.Key)
            {
                scoreText.text += $"{kv.Key.GetComponentInChildren<UIHealth>().getPlayerName()} : {kv.Value}\n";
            }
        }
        

    }
    
    // Start is called before the first frame update
    void Start()
    {
        if (!gotToLobby)
        {
            gotToLobby = true;
        }
        else
        {
            PrintScore();
        }
    }
}
