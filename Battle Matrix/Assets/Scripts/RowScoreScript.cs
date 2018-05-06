using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowScoreScript : MonoBehaviour {

    public GameManagerScript gm;

    public List<GameObject> rows1;
    public List<GameObject> rows2;

    public GameObject rounds1;
    public GameObject rounds2;
    public GameObject multi1;
    public GameObject multi2;

    GameManagerScript.PlayerBoard[] pb = new GameManagerScript.PlayerBoard[2];

    int[] p1Attack;
    int[] p2Attack;

    //PlayerScore[] ps;

    // Use this for initialization
    void Start () {

       if (gm)
        {
            //Debug.Log("test");
            pb[0] = gm.player1;
            //Debug.Log("test 1:" + (pb1 != null) + "-" + (gm.player1 != null));
            pb[1] = gm.player2;
            //Debug.Log("test 2:" + (pb2 != null) + "-" + (gm.player2 != null));
        }

        for (int i = 0; i < rows1.Count; i++)
        {
            rows1[i].GetComponent<Text>().text = "";
        }
        for (int i = 0; i < rows2.Count; i++)
        {
            rows2[i].GetComponent<Text>().text = "";
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (pb[0] == null)
            pb[0] = gm.player1;
        if (pb[1] == null)
            pb[1] = gm.player2;

        if(pb[0] != null)
        {
            rounds1.GetComponent<Text>().text = "Rounds Won: " + pb[0].GetRoundMultiplier().ToString();
            multi1.GetComponent<Text>().text = "Multiplier: " + pb[0].GetRoundMultiplier().ToString();

            for (int i = 0; i < pb[0].GetAttackTotals().Length; i++)
            {
                if (rows1[i] != null || pb[0].GetAttackTotals()[i] == 0)
                    rows1[i].GetComponent<Text>().text = "";
                else
                    rows1[i].GetComponent<Text>().text = pb[0].GetAttackTotals()[i].ToString();
            }
        }
        if (pb[1] != null)
        {
            rounds2.GetComponent<Text>().text = "Rounds Won: " + pb[1].GetRoundMultiplier().ToString();
            multi2.GetComponent<Text>().text = "Multiplier: " + pb[1].GetRoundMultiplier().ToString();

            for (int i = 0; i < pb[1].GetAttackTotals().Length; i++)
            {
                if (rows2[i] != null || pb[0].GetAttackTotals()[i] == 0)
                    rows2[i].GetComponent<Text>().text = "";
                else
                    rows2[i].GetComponent<Text>().text = pb[0].GetAttackTotals()[i].ToString();
            }
        }

        /*for(int i = 0; i < 2; i++)
        {

            if (pb[i] != null)
            {
                ps[i].attacks = pb[i].GetAttackTotals();
                ps[i].multiplier = pb[i].GetRoundMultiplier();
                ps[i].roundsWon = gm.GetPlayerRoundsWon(i + 1);

                ps[i].multiText.GetComponent<Text>().text = "Rounds Won: " + ps[i].multiplier.ToString();
                ps[i].roundsText.GetComponent<Text>().text = "Multiplier: " + ps[i].roundsWon.ToString();

                for (int j = 0; j < ps[i].attacks.Length; j++)
                {
                    if (ps[i].rows[j])
                    {
                        if (ps[i].attacks[j] == 0)
                            ps[i].rows[j].GetComponent<Text>().text = "";
                        else
                            ps[i].rows[j].GetComponent<Text>().text = ps[i].attacks[j].ToString();
                    }

                }
            }

            
        }*/

    }
}
