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
        bool dr2 = Input.GetButtonUp(player.drop);

        bool leftS = Input.GetButton(player.left);
        bool rightS = Input.GetButton(player.right);
        bool rlS = Input.GetButton(player.rotateLeft);
        bool rrS = Input.GetButton(player.rotateRight);
        bool drS = Input.GetButton(player.drop);

        //Debug.Log(pb != null);

        // Check Left
        if (left)
        {
            Debug.Log(player.left);
            pb.SlideLeft();
        }
        // Check Right
        if (right)
        {
            Debug.Log(player.right);
            pb.SlideRight();
        }

        // Check Left Rotation
        if (rl)
        {
            Debug.Log(player.rotateLeft);
            pb.RotateClockwise();
        }
        // Check Right Rotation
        if (rr)
        {
            Debug.Log(player.rotateRight);
            pb.RotateCounterClockwise();
        }

        // Check Drop
        if (dr)
        {
            Debug.Log(player.drop);
            pb.FastDrop(true);
        }
        else if (dr2)
        {
            pb.FastDrop(false);
        }


        // Check Left Sign
        if (leftS)
        {
            player.icons[0].GetComponent<IconActivation>().Activation();
        } else
        {
            player.icons[0].GetComponent<IconActivation>().Deactivation();
        }
        // Check Right Sign
        if (rightS)
        {
            player.icons[1].GetComponent<IconActivation>().Activation();
        }
        else
        {
            player.icons[1].GetComponent<IconActivation>().Deactivation();
        }

        // Check Left Rotation Sign
        if (rlS)
        {
            player.icons[2].GetComponent<IconActivation>().Activation();
        }
        else
        {
            player.icons[2].GetComponent<IconActivation>().Deactivation();
        }
        // Check Right Rotation Sign
        if (rrS)
        {
            player.icons[3].GetComponent<IconActivation>().Activation();
        }
        else
        {
            player.icons[3].GetComponent<IconActivation>().Deactivation();
        }

        // Check Drop
        if (drS)
        {
            player.icons[4].GetComponent<IconActivation>().Activation();
        }
        else
        {
            player.icons[4].GetComponent<IconActivation>().Deactivation();
        }


    }

}
