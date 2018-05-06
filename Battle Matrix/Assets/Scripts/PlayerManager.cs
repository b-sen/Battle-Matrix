using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    // Exists to contain the names of each player's input axis
    public struct PlayerControls
    {
        public string left;
        public string right;
        public string rotateLeft;
        public string rotateRight;
        public string drop;

        
        public List<GameObject> icons;

        public PlayerControls(string l, string r, string rl, string rr, string dr, List<GameObject> ic)
        {
            this.left = l;
            this.right = r;
            this.rotateLeft = rl;
            this.rotateRight = rr;
            this.drop = dr;
            this.icons = ic;
        }
    }

    public PlayerControls p1;
    public PlayerControls p2;

    // Index order is: left, right, rotate left, rotate right, drop
    public List<GameObject> p1Icons;
    public List<GameObject> p2Icons;

    public float delay = 0.5f;
    float nextTime = 0.5f;
    float currTime = 0f;

    public GameManagerScript gm;

    GameManagerScript.PlayerBoard pb1;
    GameManagerScript.PlayerBoard pb2;

    // Use this for initialization
    void Start () {
        
        p1 = new PlayerControls("P1Left", "P1Right", "P1RotateLeft", "P1RotateRight", "P1Drop", p1Icons);
        p2 = new PlayerControls("P2Left", "P2Right", "P2RotateLeft", "P2RotateRight", "P2Drop", p2Icons);

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

        currTime += Time.deltaTime;

        if (pb1 == null)
            pb1 = gm.player1;
        if (pb2 == null)
            pb2 = gm.player2;

        CheckInputs(p1, pb1);
        CheckInputs(p2, pb2);

    }

    void CheckInputs(PlayerControls player, GameManagerScript.PlayerBoard pb)
    {
        if(pb == null)
        {
            return;
        }

        bool left = Input.GetButtonDown(player.left);
        bool right = Input.GetButtonDown(player.right);
        bool rl = Input.GetButtonDown(player.rotateLeft);
        bool rr = Input.GetButtonDown(player.rotateRight);
        bool dr = Input.GetButtonDown(player.drop);

        //Debug.Log(pb != null);

        // Check Left
        if (left)
        {
            player.icons[0].GetComponent<IconActivation>().Activation();
            pb.SlideLeft();
        } else
        {
            player.icons[0].GetComponent<IconActivation>().Deactivation();
        }
        // Check Right
        if (right)
        {
            player.icons[1].GetComponent<IconActivation>().Activation();
            pb.SlideRight();
        }
        else
        {
            player.icons[1].GetComponent<IconActivation>().Deactivation();
        }

        // Check Left Rotation
        if (rl)
        {
            player.icons[2].GetComponent<IconActivation>().Activation();
            pb.RotateClockwise();
        }
        else
        {
            player.icons[2].GetComponent<IconActivation>().Deactivation();
        }
        // Check Right Rotation
        if (rr)
        {
            player.icons[3].GetComponent<IconActivation>().Activation();
            pb.RotateCounterClockwise();
        }
        else
        {
            player.icons[3].GetComponent<IconActivation>().Deactivation();
        }

        // Check Drop
        if (dr)
        {
            player.icons[4].GetComponent<IconActivation>().Activation();
            pb.FastDrop(true);
        }
        else
        {
            player.icons[4].GetComponent<IconActivation>().Deactivation();
            pb.FastDrop(false);
        }


    }

}
