using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In charge of all game logic.
/// </summary>
public class GameManagerScript : MonoBehaviour {
    public class PlayerBoard {






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
            
        }

    }

    public PlayerBoard player1;
    public PlayerBoard player2;


	// Use this for initialization
	void Start () {
		
	}
	
    
    


	// Update is called once per frame
	void Update () {
		
	}
}
