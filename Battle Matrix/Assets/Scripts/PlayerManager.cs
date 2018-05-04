using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    struct PlayerControls
    {
        string horizontal;
        string rotate;
        string drop;

        public PlayerControls(string hz, string rt, string dr)
        {
            this.horizontal = hz;
            this.rotate = rt;
            this.drop = dr;
        }
    }

    PlayerControls p1;
    PlayerControls p2;

	// Use this for initialization
	void Start () {
        p1 = new PlayerControls("P1Horizontal", "P1Rotate", "P1Drop");
        p2 = new PlayerControls("P2Horizontal", "P2Rotate", "P2Drop");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
