using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RowScoreScript : MonoBehaviour {

    public GameManagerScript gm;

    GameManagerScript.PlayerBoard pb1;
    GameManagerScript.PlayerBoard pb2;

    List<Text> p1Rows;
    List<Text> p2Rows;

    int[] p1Attacks;
    int[] p2Attacks;

    // Use this for initialization
    void Start () {

        if (gm)
        {
            //Debug.Log("test");
            pb1 = gm.player1;
            //Debug.Log("test 1:" + (pb1 != null) + "-" + (gm.player1 != null));
            pb2 = gm.player2;
            //Debug.Log("test 2:" + (pb2 != null) + "-" + (gm.player2 != null));
        }

    }
	
	// Update is called once per frame
	void Update () {

        if (pb1 == null)
            pb1 = gm.player1;
        if (pb2 == null)
            pb2 = gm.player2;

        p1Attacks = pb1.GetAttackTotals();
        p2Attacks = pb1.GetAttackTotals();

        for (int i = 0; i < p1Attacks.Length; i++)
        {
            if (p1Rows[i])
            {
                if (p1Attacks[i] == 0)
                    p1Rows[i].text = "";
                else
                    p1Rows[i].text = p1Attacks[i].ToString();
            }
        }
        for (int i = 0; i < p2Attacks.Length; i++)
        {
            if (p2Rows[i])
            {
                if (p2Attacks[i] == 0)
                    p2Rows[i].text = "";
                else
                    p2Rows[i].text = p2Attacks[i].ToString();
            }
        }

    }
}
