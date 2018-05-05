using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

    int currentRound;
    int bestOf;
    public int totalRounds;

    int p1RoundsWon = 0;
    int p2RoundsWon = 0;

	// Use this for initialization
	void Start () {
        currentRound = 1;
        float tr = (float)totalRounds;
        bestOf = Mathf.CeilToInt(tr / 2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void NewRound()
    {
        if (p1RoundsWon < bestOf || p2RoundsWon < bestOf)
            currentRound += 1;
        else
            EndGame();
    }

    void EndGame()
    {
        return;
    }

}
