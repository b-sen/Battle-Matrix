using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowScoreScript : MonoBehaviour {

    public struct PlayerScore
    {
        public int[] attacks;
        public List<Text> rows;
        
        public int roundsWon;
        public int multiplier;
        public Text roundsText;
        public Text multiText;

    }


    public GameManagerScript gm;

    GameManagerScript.PlayerBoard[] pb;

    PlayerScore[] ps;

    // Use this for initialization
    void Start () {

        ps[0] = new PlayerScore();
        ps[1] = new PlayerScore();

        if (gm)
        {
            //Debug.Log("test");
            pb[0] = gm.player1;
            //Debug.Log("test 1:" + (pb1 != null) + "-" + (gm.player1 != null));
            pb[1] = gm.player2;
            //Debug.Log("test 2:" + (pb2 != null) + "-" + (gm.player2 != null));
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (pb[0] == null)
            pb[0] = gm.player1;
        if (pb[1] == null)
            pb[1] = gm.player2;



        for(int i = 0; i < 2; i++)
        {
            ps[i].attacks = pb[i].GetAttackTotals();
            ps[i].multiplier = pb[i].GetRoundMultiplier();
            ps[i].roundsWon = gm.GetPlayerRoundsWon(i + 1);

            ps[i].multiText.text = "";
            ps[i].roundsText.text = "";

            for (int j = 0; j < ps[i].attacks.Length; j++)
            {
                if(ps[i].rows[j])
                {
                    if (ps[i].attacks[j] == 0)
                        ps[i].rows[j].text = "";
                    else
                        ps[i].rows[j].text = ps[i].attacks[j].ToString();
                }
                
            }
        }

    }
}
