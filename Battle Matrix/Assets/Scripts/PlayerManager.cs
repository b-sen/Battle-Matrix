using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    // Exists to contain the names of each player's input axis
    public struct PlayerControls
    {
        public string horizontal;
        public string rotate;
        public string drop;

        public List<GameObject> icons;

        public PlayerControls(string hz, string rt, string dr, List<GameObject> ic)
        {
            this.horizontal = hz;
            this.rotate = rt;
            this.drop = dr;
            this.icons = ic;
        }
    }

    public PlayerControls p1;
    public PlayerControls p2;

    // Index order is: left, right, rotate left, rotate right, drop
    public List<GameObject> p1Icons;
    public List<GameObject> p2Icons;

    GameManagerScript.PlayerBoard pb;

    // Use this for initialization
    void Start () {
        p1 = new PlayerControls("P1Horizontal", "P1Rotate", "P1Drop", p1Icons);
        p2 = new PlayerControls("P2Horizontal", "P2Rotate", "P2Drop", p2Icons);
    }
	
	// Update is called once per frame
	void Update () {
        CheckInputs(p1);
        CheckInputs(p2);
    }

    void CheckInputs(PlayerControls player)
    {
        float hz = Input.GetAxis(player.horizontal);
        float rt = Input.GetAxis(player.rotate);
        float dr = Input.GetAxis(player.drop);

        // Check Horizontal
        if (hz < 0)
        {
            player.icons[0].GetComponent<IconActivation>().Activation();
            player.icons[1].GetComponent<IconActivation>().Deactivation();
            //pb.SlideLeft();
        }
        else if (hz > 0)
        {
            player.icons[0].GetComponent<IconActivation>().Deactivation();
            player.icons[1].GetComponent<IconActivation>().Activation();
            //pb.SlideRight();
        }
        else
        {
            player.icons[0].GetComponent<IconActivation>().Deactivation();
            player.icons[1].GetComponent<IconActivation>().Deactivation();
        }

        // Check Rotation
        if (rt < 0)
        {
            player.icons[2].GetComponent<IconActivation>().Activation();
            player.icons[3].GetComponent<IconActivation>().Deactivation();
            //pb.RotateClockwise();
        }
        else if (rt > 0)
        {
            player.icons[2].GetComponent<IconActivation>().Deactivation();
            player.icons[3].GetComponent<IconActivation>().Activation();
            //pb.RotateCounterClockwise();
        }
        else
        {
            player.icons[2].GetComponent<IconActivation>().Deactivation();
            player.icons[3].GetComponent<IconActivation>().Deactivation();
        }

        // Check Drop
        if (dr != 0)
        {
            player.icons[4].GetComponent<IconActivation>().Activation();
            //pb.FastDrop(true);
        }
        else
        {
            player.icons[4].GetComponent<IconActivation>().Deactivation();
            //pb.FastDrop(false);
        }


    }

}
