using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowScoreScript : MonoBehaviour {

    public struct PlayerScore
    {
        public int[] attacks;
        public List<GameObject> rows;
        
        public int roundsWon;
        public int multiplier;
        public GameObject roundsText;
        public GameObject multiText;

    }


    public GameManagerScript gm;

    public List<GameObject> rows1;
    public List<GameObject> rows2;

    public GameObject rounds1;
    public GameObject rounds2;
    public GameObject multi1;
    public GameObject multi2;

    GameManagerScript.PlayerBoard[] pb;

    PlayerScore[] ps;

    // Use this for initialization
    void Start () {

        ps[0] = new PlayerScore();
        ps[1] = new PlayerScore();

        ps[0].rows = rows1;
        ps[0].roundsText = rounds1;
        ps[0].multiText = multi1;

        ps[1].rows = rows2;
        ps[1].roundsText = rounds2;
        ps[1].multiText = multi2;

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < ps[i].rows.Count; j++)
            {
                ps[i].rows[j].GetComponent<Text>().text = "";
            }
        }

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

            ps[i].multiText.GetComponent<Text>().text = "Rounds Won: " + ps[i].multiplier.ToString();
            ps[i].roundsText.GetComponent<Text>().text = "Multiplier: " + ps[i].roundsWon.ToString();

            for (int j = 0; j < ps[i].attacks.Length; j++)
            {
                if(ps[i].rows[j])
                {
                    if (ps[i].attacks[j] == 0)
                        ps[i].rows[j].GetComponent<Text>().text = "";
                    else
                        ps[i].rows[j].GetComponent<Text>().text = ps[i].attacks[j].ToString();
                }
                
            }
        }

    }
}
