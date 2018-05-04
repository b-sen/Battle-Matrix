using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    // Exists to contain the names of each player's input axis
    public struct PlayerControls
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

    public PlayerControls p1;
    public PlayerControls p2;

    // Index order is: left, right, rotate left, rotate right, drop
    public List<GameObject> p1Icons;
    public List<GameObject> p2Icons;

    // Use this for initialization
    void Start () {
        p1 = new PlayerControls("P1Horizontal", "P1Rotate", "P1Drop");
        p2 = new PlayerControls("P2Horizontal", "P2Rotate", "P2Drop");
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
