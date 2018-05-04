using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In charge of all game logic.
/// </summary>
public class GameManagerScript : MonoBehaviour {
    public class PlayerBoard {
        private GameManagerScript gameManager;  // "parent" manager to send attacks, etc. through; performs all interaction between players
        private Vector2 boardOffset;  // transform offset (in world space) of the bottom left corner of the board; allows internal code to treat that corner as the origin

        private bool fastDropState;  // does the player currently have fast drop on?


        public PlayerBoard(GameManagerScript manager, Vector2 offset) {
            gameManager = manager;
            boardOffset = offset;
            fastDropState = false;
        }



        /// <summary>
        /// The movement functions, which perform actions that are based directly on player input.  Call these from controls code.
        /// </summary>
        public void RotateClockwise() {

        }
        public void RotateCounterClockwise() {

        }
        public void SlideLeft() {

        }
        public void SlideRight() {

        }
        // Unlike the other movement actions, fast drop is locked to the tick system and therefore must be turned on and off.
        // This also allows players to hold down their fast drop button in order to get continuous fast drop.
        // on: true toggles fast drop on, false toggles fast drop off.
        public void FastDrop(bool on) {
            fastDropState = on;
        }

    }

    public PlayerBoard player1;
    public PlayerBoard player2;

    // Balancing controls for the board height and width, measured in grid squares.
    private static int boardHeight = 20;
    private static int boardWidth = 10;


	// Use this for initialization
	void Start () {
        // Assuming for simplicity that (0, 0) is at the bottom of the boards and centered between them.
        player1 = new PlayerBoard(this, new Vector2(-15, 0));
        player2 = new PlayerBoard(this, new Vector2(5, 0));
    }
	
    
    


	// Update is called once per frame
	void Update () {
		
	}
}
