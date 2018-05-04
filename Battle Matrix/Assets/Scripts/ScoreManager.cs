using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    int currentRound;
    public int totalRounds;

    int p1RoundsWon = 0;
    int p2RoundsWon = 0;

	// Use this for initialization
	void Start () {
        currentRound = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void NewRound()
    {
        if (currentRound < totalRounds)
            currentRound += 1;
        else
            EndGame();
    }

    void EndGame()
    {
        return;
    }
}
