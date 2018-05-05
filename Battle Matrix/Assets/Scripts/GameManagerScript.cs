using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure to use the block states in the same way as other scripts.
[RequireComponent(typeof(BlockStateEnum))]

// Make sure to use the polyominoes in the same way as other scripts.
[RequireComponent(typeof(PolyominoShapeEnum))]

/// <summary>
/// In charge of all game logic.
/// </summary>
public class GameManagerScript : MonoBehaviour {
    public class PlayerBoard {
        private GameManagerScript gameManager;  // "parent" manager to send attacks, etc. through; performs all interaction between players
        private Vector2 boardOffset;  // transform offset (in world space) of the bottom left corner of the board; allows internal code to treat that corner as the origin

        private bool fastDropState;  // does the player currently have fast drop on?

        private BlockScript[,] grid;  // the board of blocks
        private Queue<PolyominoShapeEnum.PolyominoShape> upcomingPolyominoes;  // the shapes of all upcoming polyominoes (not generated onto the board yet)
        private PolyominoScript controllablePolyomino;  // current polyomino that is falling under player control (not due to attack)


        public PlayerBoard(GameManagerScript manager, Vector2 offset) {
            gameManager = manager;
            boardOffset = offset;

            fastDropState = false;

            grid = new BlockScript[boardHeight, boardWidth];  // automatically initializes to null
            upcomingPolyominoes = new Queue<PolyominoShapeEnum.PolyominoShape>();
            while (upcomingPolyominoes.Count < numUpcomingPolyominoes) {  // fully populate queue
                upcomingPolyominoes.Enqueue(gameManager.ChoosePolyomino());
            }
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

        /// <summary>
        /// Use this to preview the upcoming polyominoes in the UI.
        /// </summary>
        /// <returns>A list containing all this player's upcoming polyominoes in queue order.</returns>
        public List<PolyominoShapeEnum.PolyominoShape> GetUpcomingPolyominoesAsList() {
            return new List<PolyominoShapeEnum.PolyominoShape>(upcomingPolyominoes.ToArray());
        }



        private void GenerateNextControllablePolyomino() {
            PolyominoShapeEnum.PolyominoShape shape = upcomingPolyominoes.Dequeue();  // shape of the polyomino to generate
            upcomingPolyominoes.Enqueue(gameManager.ChoosePolyomino());  // immediately bring the queue up to full size

            controllablePolyomino = ((GameObject)Instantiate(gameManager.polyominoPrefab)).GetComponent<PolyominoScript>();  // new empty polyomino to fill
            List<Vector2Int> newBlockLocations;

            // Specifies the blocks to place and spawning points for the given polyomino shape.
            switch (shape) { // all shapes arranged in their visual order on the board for ease of use
                case PolyominoShapeEnum.PolyominoShape.O_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] {    new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
                                                                                new Vector2Int((boardWidth / 2) - 1, boardHeight - 2),  new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.I_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] {    new Vector2Int((boardWidth / 2) - 2, boardHeight - 1),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),  new Vector2Int((boardWidth / 2) + 1, boardHeight - 1)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.T_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] {    new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),  new Vector2Int((boardWidth / 2) + 1, boardHeight - 1),
                                                                                                                                        new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.S_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] {    new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
                        new Vector2Int((boardWidth / 2) - 2, boardHeight - 2),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.Z_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] {    new Vector2Int((boardWidth / 2) - 2, boardHeight - 1),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),
                                                                                                                                        new Vector2Int((boardWidth / 2) - 1, boardHeight - 2),  new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.J_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] {    new Vector2Int((boardWidth / 2) - 2, boardHeight - 1),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
                                                                                                                                                                                                new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.L_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] {    new Vector2Int((boardWidth / 2) - 2, boardHeight - 2),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
                                                                                new Vector2Int((boardWidth / 2) - 2, boardHeight - 2)});
                    break;
                default:  // should never occur
                    Debug.Log("Could not generate controllable polyomino due to unknown shape.");
                    return;
            }

            // Check that the spawning points are actually free; if not, round over!
            bool canSpawn = true;
            foreach (Vector2Int gridLocation in newBlockLocations) {
                if (grid[gridLocation.y, gridLocation.x] != null) canSpawn = false;  // block already in spawn location
            }

            if (canSpawn) {  // generate and register blocks in place
                foreach (Vector2Int gridLocation in newBlockLocations) {
                    BlockScript block = GenerateBlockInPlace(gridLocation);
                    grid[gridLocation.y, gridLocation.x] = block;  // register to player's board
                    controllablePolyomino.memberBlocks.Add(block);  // register to polyomino
                }
            } else {
                // fill in generation of offending blocks anyway if desired
                LoseRound();
            }
        }




        /// <summary>
        /// Helper functions.
        /// </summary>

        // Handles all the details of converting from a 2D integer grid location to a 3D floating point world position for block generation.
        private BlockScript GenerateBlockInPlace(Vector2Int gridLocation) {
            float epsilon = 0.000001F;  // to ensure that floor casting will return the correct grid location
            return ((GameObject)Instantiate(gameManager.blockPrefab, new Vector3(gridLocation.x + boardOffset.x + epsilon, gridLocation.y + boardOffset.y + epsilon, blockZLevel), Quaternion.identity)).GetComponent<BlockScript>();
        }

        // Call only when losing a round.
        private void LoseRound() {

        }
    }

    /// <summary>
    /// UI / appearance controls and related variables.
    /// </summary>
    
    // Set appropriate Prefabs in the Inspector.
    public GameObject blockPrefab;
    public GameObject polyominoPrefab;

    public static int blockZLevel = 1;


    /// <summary>
    /// Meant for use by other scripts as needed.
    /// </summary>

    // Handle all player-specific functionality.
    public PlayerBoard player1;
    public PlayerBoard player2;


    /// <summary>
    /// Balancing controls - know about them, but other scripts don't need to use them.
    /// </summary>

    // Balancing control for number of upcoming polyominoes to display per player.
    private static int numUpcomingPolyominoes = 3;
    
    // Balancing controls for the board height and width, measured in grid squares.
    private static int boardHeight = 20;
    private static int boardWidth = 10;


    /// <summary>
    /// For class functionality only.
    /// </summary>

    // PRNG instance to use as needed for attacks and polyomino selection.
    private System.Random prng;


	// Use this for initialization
	void Start () {
        prng = new System.Random();

        // Assuming for simplicity that (0, 0) is at the bottom of the boards and centered between them.
        player1 = new PlayerBoard(this, new Vector2(-15, 0));
        player2 = new PlayerBoard(this, new Vector2(5, 0));
    }
	
    // Choose a new random polyomino to queue.  Here in case we want both players to draw from a shared bag as another point of competition.
    private PolyominoShapeEnum.PolyominoShape ChoosePolyomino() {
        return (PolyominoShapeEnum.PolyominoShape)(prng.Next(System.Enum.GetNames(typeof(PolyominoShapeEnum.PolyominoShape)).Length));  // remains uniform regardless of number of polyominoes to choose from
    }


	// Update is called once per frame
	void Update () {
		
	}

}
