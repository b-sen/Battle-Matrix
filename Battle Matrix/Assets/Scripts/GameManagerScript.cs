using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private int[] attackTotals;  // the number of blocks that will be sent next tick based on each row's status this tick; 0 means "no attack" and also indicates an unmatched row
        private int roundMultiplier;  // the current attack bonus based on round status
        private int attackReceived;  // the number of blocks to place on the board this tick due to attacks
        private bool hasLost;  // has this player lost the round?

        private Queue<PolyominoShapeEnum.PolyominoShape> upcomingPolyominoes;  // the shapes of all upcoming polyominoes (not generated onto the board yet)
        private PolyominoScript controllablePolyomino;  // current polyomino that is falling under player control (not due to attack)
        private List<PolyominoScript> fallingPolyominos;  // all polyominos that are currently falling outside player control

        private delegate bool MoveAction(PolyominoScript polyomino);


        public PlayerBoard(GameManagerScript manager, Vector2 offset, int multiplier) {

            gameManager = manager;
            boardOffset = offset;
            roundMultiplier = multiplier;

            fastDropState = false;
            hasLost = false;
            fallingPolyominos = new List<PolyominoScript>();
            attackReceived = 0;

            grid = new BlockScript[boardHeight, boardWidth];  // automatically initializes to null
            attackTotals = new int[boardHeight];  // automatically initializes to 0s
            upcomingPolyominoes = new Queue<PolyominoShapeEnum.PolyominoShape>();
            while (upcomingPolyominoes.Count < numUpcomingPolyominoes) {  // fully populate queue
                upcomingPolyominoes.Enqueue(gameManager.ChoosePolyomino());
            }

            GenerateNextControllablePolyomino();
        }



        /// <summary>
        /// The movement functions, which perform actions that are based directly on player input.  Call these from controls code.
        /// </summary>
        public void RotateClockwise() {
            RotatePolyomino(controllablePolyomino, 1);
        }
        public void RotateCounterClockwise() {
            RotatePolyomino(controllablePolyomino, -1);
        }
        public void SlideLeft() {
            MoveAction slider = (poly) => {
                List<Vector2Int> slideLocations = new List<Vector2Int>();

                // "move" each block individually
                foreach (BlockScript block in poly.memberBlocks) {
                    // Where would this block slide to?
                    Vector2Int newGridLocation = FindGridLocationOfBlock(block);
                    newGridLocation.x -= 1;
                    slideLocations.Add(newGridLocation);
                }

                bool slideIsPossible = CanInsertBlocksAtLocations(slideLocations);
                // Actually move the blocks left in the world, setting up for RegisterPolyomino to move them in the grid.
                if (slideIsPossible) {
                    MoveBlocksToGridLocations(poly.memberBlocks, slideLocations);
                }
                return slideIsPossible;
            };

            AttemptWithPolyominoUnregistered(controllablePolyomino, slider);
        }
        public void SlideRight() {
            MoveAction slider = (poly) => {
                List<Vector2Int> slideLocations = new List<Vector2Int>();

                // "move" each block individually
                foreach (BlockScript block in poly.memberBlocks) {
                    // Where would this block slide to?
                    Vector2Int newGridLocation = FindGridLocationOfBlock(block);
                    newGridLocation.x += 1;
                    slideLocations.Add(newGridLocation);
                }

                bool slideIsPossible = CanInsertBlocksAtLocations(slideLocations);
                // Actually move the blocks right in the world, setting up for RegisterPolyomino to move them in the grid.
                if (slideIsPossible) {
                    MoveBlocksToGridLocations(poly.memberBlocks, slideLocations);
                }
                return slideIsPossible;
            };

            AttemptWithPolyominoUnregistered(controllablePolyomino, slider);
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

        /// <summary>
        /// Use this to read blocks about to be sent as attacks.
        /// </summary>
        /// <returns>Integer array of length boardHeight, with each element being the number of blocks to be sent next tick as a result of that row (0 if not matched).</returns>
        public int[] GetAttackTotals() {
            return (int[])(attackTotals.Clone());
        }

        public int GetRoundMultiplier() {
            return roundMultiplier;
        }


        internal void DoTick() {
            /// This ordering of checks ensures that matched rows are always shown for a tick and that polyominoes locked into place can immediately be considered for matching.

            // Clear previously matched rows.
            SweepMatchedRows();

            // Drop any polyominos that are falling outside player control.
            DropAllFallingPolyominos();

            // In case the controllable polyomino was destroyed due to attack, generate a new one once the blocks have stopped falling.
            if (controllablePolyomino == null) {
                if (fallingPolyominos.Count == 0) GenerateNextControllablePolyomino();
            }

            // Always drop the polyomino if it exists, but only lock it if it can't move.
            if ((controllablePolyomino != null) && !(DropPolyomino(controllablePolyomino))) {  // lock it and generate a new one
                LockPolyomino(controllablePolyomino);  // after this the old polyomino is no longer valid
                GenerateNextControllablePolyomino();
            }

            // Generate all blocks from attacks received.
            if (attackReceived != 0) {
                if (controllablePolyomino != null) RemovePolyomino(controllablePolyomino);  // lose current polyomino, both as part of the attack and to keep it from getting in the way
                controllablePolyomino = null;

                GenerateAttackPolyominos();
            }

            // Check for matched rows.
            FindMatchedRows();
        }

        internal void DoFastDrop() {
            // Drop any polyominos that are falling outside player control.
            DropAllFallingPolyominos();

            if (fastDropState) DropPolyomino(controllablePolyomino);  // drop the player's polyomino if needed, but do not lock
        }

        internal void ReceiveAttack(int attack) {
            attackReceived = attack;
        }

        internal bool GetLossStatus() {
            return hasLost;
        }


        /// <summary>
        /// Helper functions.
        /// </summary>

        private void GenerateNextControllablePolyomino() {
            PolyominoShapeEnum.PolyominoShape shape = upcomingPolyominoes.Dequeue();  // shape of the polyomino to generate
            upcomingPolyominoes.Enqueue(gameManager.ChoosePolyomino());  // immediately bring the queue up to full size

            controllablePolyomino = ((GameObject)Instantiate(gameManager.polyominoPrefab)).GetComponent<PolyominoScript>();  // new empty polyomino to fill
            controllablePolyomino.memberBlocks = new List<BlockScript>();
            List<Vector2Int> newBlockLocations;

            // Specifies the blocks to place and spawning points for the given polyomino shape.
            switch (shape) { // all shapes arranged in their visual order on the board for ease of use
                case PolyominoShapeEnum.PolyominoShape.O_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] { new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
                                                                                new Vector2Int((boardWidth / 2) - 1, boardHeight - 2),  new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.I_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] { new Vector2Int((boardWidth / 2) - 2, boardHeight - 1), new Vector2Int((boardWidth / 2) - 1, boardHeight - 1), new Vector2Int((boardWidth / 2), boardHeight - 1), new Vector2Int((boardWidth / 2) + 1, boardHeight - 1) });
                    break;
                case PolyominoShapeEnum.PolyominoShape.T_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] { new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),  new Vector2Int((boardWidth / 2) + 1, boardHeight - 1),
                                                                                                                                        new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.S_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] { new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
                        new Vector2Int((boardWidth / 2) - 2, boardHeight - 2),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.Z_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] { new Vector2Int((boardWidth / 2) - 2, boardHeight - 1),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),
                                                                                                                                        new Vector2Int((boardWidth / 2) - 1, boardHeight - 2),  new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.J_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] { new Vector2Int((boardWidth / 2) - 2, boardHeight - 1),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
                                                                                                                                                                                                new Vector2Int((boardWidth / 2), boardHeight - 2)});
                    break;
                case PolyominoShapeEnum.PolyominoShape.L_Tetromino:
                    newBlockLocations = new List<Vector2Int>(new Vector2Int[] { new Vector2Int((boardWidth / 2) - 2, boardHeight - 1),  new Vector2Int((boardWidth / 2) - 1, boardHeight - 1),  new Vector2Int((boardWidth / 2), boardHeight - 1),
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
                    // register to polyomino
                    controllablePolyomino.memberBlocks.Add(block);
                    block.SetPolyomino(controllablePolyomino);
                }
                RegisterPolyomino(controllablePolyomino);
            } else {
                // fill in generation of offending blocks anyway if desired

                LoseRound();
            }
        }

        // Handle all the details of converting from a 2D integer grid location to a 3D floating point world position for block generation.
        private BlockScript GenerateBlockInPlace(Vector2Int gridLocation) {
            BlockScript newBlock = ((GameObject)Instantiate(gameManager.blockPrefab)).GetComponent<BlockScript>();
            MoveBlockToGridLocation(newBlock, gridLocation);
            return newBlock;
        }
        private void MoveBlockToGridLocation(BlockScript block, Vector2Int gridLocation) {
            float epsilon = 0.000001F;  // to ensure that floor casting will return the correct grid location
            block.transform.position = new Vector3(gridLocation.x + boardOffset.x + epsilon, gridLocation.y + boardOffset.y + epsilon, blockZLevel);
        }
        // Both lists must be the same length!
        private void MoveBlocksToGridLocations(List<BlockScript> blocks, List<Vector2Int> gridLocations) {
            for (int i = 0; i < gridLocations.Count; i++) {
                MoveBlockToGridLocation(blocks[i], gridLocations[i]);
            }
        }

        // Finds the appropriate grid location for a block from its world position.
        private Vector2Int FindGridLocationOfBlock(BlockScript block) {
            return new Vector2Int((int)(block.transform.position.x - boardOffset.x), (int)(block.transform.position.y - boardOffset.y));
        }

        // Call only when losing a round.  Safe to call multiple times in a tick if the round is lost for multiple reasons simultaneously.
        private void LoseRound() {
            hasLost = true;
        }

        // Attempts to drop a polyomino one grid row, respecting collision and leaving the polyomino in place on failure.  Returns true iff successful.
        private bool DropPolyomino(PolyominoScript polyomino) {
            MoveAction dropper = (poly) => {
                bool dropIsPossible = CanDropPolyomino(poly);
                // Actually move the blocks down in the world, setting up for RegisterPolyomino to move them in the grid.
                if (dropIsPossible) {
                    foreach (BlockScript block in poly.memberBlocks) {
                        block.transform.position = new Vector3(block.transform.position.x, block.transform.position.y - 1, block.transform.position.z);  // no, Unity does not allow just setting the y component
                    }
                }
                return dropIsPossible;
            };

            return AttemptWithPolyominoUnregistered(polyomino, dropper);
        }

        // Attempts to rotate a polyomino 90 degrees, respecting collision and leaving the polyomino in place on failure.  Valid directions are 1 for clockwise and -1 for counterclockwise.
        private void RotatePolyomino(PolyominoScript polyomino, int direction) {
            // get bounding box of polyomino
            Vector2Int bottomLeft = FindGridLocationOfBlock(polyomino.memberBlocks[0]);
            Vector2Int topRight = FindGridLocationOfBlock(polyomino.memberBlocks[0]);  // cannot just make another reference to bottomLeft!
            foreach (BlockScript block in polyomino.memberBlocks) {
                Vector2Int location = FindGridLocationOfBlock(block);

                // must check for expansion of each coordinate; Max and Min do two comparisons each
                bottomLeft = Vector2Int.Min(bottomLeft, location);
                topRight = Vector2Int.Max(bottomLeft, location);
            }

            // choose pivot point based on bounding box (bias up and left in case of ties)
            Vector2 pivot = new Vector2(); // floating point because we're going to offset it (so as to rotate blocks based on their centers without offsetting all the block locations)
            pivot.x = (float)(bottomLeft.x + Mathf.FloorToInt((topRight.x + 1 - bottomLeft.x) / 2.0f));  // floor biases left, +1 accounts for size of top-right block
            pivot.y = (float)(bottomLeft.y + Mathf.CeilToInt((topRight.y + 1 - bottomLeft.y) / 2.0f));  // ceiling biases up, +1 accounts for size of top-right block
            pivot -= new Vector2(0.5f, 0.5f);  // offsetting

            // attempt rotation around that pivot point
            MoveAction rotater = (poly) => {
                List<Vector2Int> rotateLocations = new List<Vector2Int>();

                // "move" each block individually
                foreach (BlockScript block in poly.memberBlocks) {
                    // Where would this block rotate to?
                    // Each coordinate is of the form (offset from pivot) + pivot
                    Vector2Int newGridLocation = new Vector2Int(Mathf.RoundToInt(direction * (FindGridLocationOfBlock(block).y - pivot.y) + pivot.x), Mathf.RoundToInt((- direction) * (FindGridLocationOfBlock(block).x - pivot.x) + pivot.y));
                    rotateLocations.Add(newGridLocation);
                }

                bool rotateIsPossible = CanInsertBlocksAtLocations(rotateLocations);
                // Actually move the blocks clockwise in the world, setting up for RegisterPolyomino to move them in the grid.
                if (rotateIsPossible) {
                    MoveBlocksToGridLocations(poly.memberBlocks, rotateLocations);
                }
                return rotateIsPossible;
            };

            AttemptWithPolyominoUnregistered(polyomino, rotater);
        }


        private bool AttemptWithPolyominoUnregistered(PolyominoScript polyomino, MoveAction actionToAttempt) {
            /// Normally blocks in a moving polyomino are tracked in two places: the polyomino and the board.
            /// Here we take advantage of this by temporarily removing those blocks from the board (because they won't collide with each other) to aid collision checking.
            DeregisterPolyomino(polyomino);

            bool actionSuccess = actionToAttempt(polyomino);

            RegisterPolyomino(polyomino);  // regardless of how the action went, return the blocks to the board; will take care of moving the blocks in the grid
            return actionSuccess;
        }

        // Removes all blocks in polyomino from the board; useful largely for temporary changes to perform calculations that want to not consider some polyominoes.
        private void DeregisterPolyomino(PolyominoScript polyomino) {
            foreach (BlockScript block in polyomino.memberBlocks) {
                Vector2Int gridLocation = FindGridLocationOfBlock(block);
                if (!(System.Object.ReferenceEquals(grid[gridLocation.y, gridLocation.x], block))) { // should never occur
                    Debug.Log("Block not found where expected!");
                } else {
                    grid[gridLocation.y, gridLocation.x] = null;
                }
            }
        }

        // Adds all blocks in polyomino to the board.
        private void RegisterPolyomino(PolyominoScript polyomino) {
            foreach (BlockScript block in polyomino.memberBlocks) {
                Vector2Int gridLocation = FindGridLocationOfBlock(block);
                if (grid[gridLocation.y, gridLocation.x] != null) { // should never occur
                    Debug.Log("Two blocks in same grid location!");
                } else {
                    grid[gridLocation.y, gridLocation.x] = block;
                }
            }
        }

        // Checks if all blocks in a polyomino can drop.
        private bool CanDropPolyomino(PolyominoScript polyomino) {
            List<Vector2Int> dropLocations = new List<Vector2Int>();

            // "move" each block individually
            foreach (BlockScript block in polyomino.memberBlocks) {
                // Where would this block drop to?
                Vector2Int newGridLocation = FindGridLocationOfBlock(block);
                newGridLocation.y -= 1;
                dropLocations.Add(newGridLocation);
            }

            return CanInsertBlocksAtLocations(dropLocations);
        }

        // Checks if all new block positions are valid and available given a list.
        private bool CanInsertBlocksAtLocations(List<Vector2Int> gridLocations) {
            bool canInsertBlocks = true;

            // check each block individually
            foreach (Vector2Int location in gridLocations) {
                // Is the location valid (checking bottom / top / left / right in order)?  Would it run into another block?
                if ((location.y < 0) || (location.y >= boardHeight) || (location.x < 0) || (location.x >= boardWidth) || (grid[location.y, location.x] != null)) {
                    canInsertBlocks = false;
                }
            }
            return canInsertBlocks;
        }

        // Changes block states as needed for all blocks in polyomino, and destroys the polyomino as no longer needed.
        private void LockPolyomino(PolyominoScript polyomino) {
            foreach (BlockScript block in polyomino.memberBlocks) {
                block.SetState(BlockStateEnum.BlockState.Locked);
            }
            Destroy(polyomino.gameObject);
        }

        // Removes polyomino and its contents from the world and grid.
        private void RemovePolyomino(PolyominoScript polyomino) {
            DeregisterPolyomino(polyomino);
            foreach (BlockScript block in polyomino.memberBlocks) {
                Destroy(block.gameObject);
            }
            polyomino.memberBlocks.Clear();
            Destroy(polyomino.gameObject);
        }

        // Flags all matched rows and sets block states accordingly.
        private void FindMatchedRows() {
            // check each row individually
            for (int row = 0; row < boardHeight; row++) {
                // to be matched, a row must consist of only locked blocks
                bool isMatched = true;
                for (int column = 0; column < boardWidth; column++) {
                    // Is there a block at this position?  If so, is it locked?
                    if ((grid[row, column] == null) || (grid[row, column].GetState() != BlockStateEnum.BlockState.Locked)) {
                        isMatched = false;
                    }
                }

                if (isMatched) {
                    // change all the block states to reflect the matched row
                    for (int column = 0; column < boardWidth; column++) {
                        grid[row, column].SetState(BlockStateEnum.BlockState.Matched);
                    }

                    attackTotals[row] = attackStrengths[row] * roundMultiplier;  // calculate number of blocks to send and indicate status for clearing next tick
                }
            }
        }

        // Clears out all matched rows.
        private void SweepMatchedRows() {
            int lowestSweptRow = boardHeight + 1;  // obviously invalid as is; for later use to determine if any blocks need to fall as a result of matching

            // check each row individually
            for (int row = 0; row < boardHeight; row++) {
                if (attackTotals[row] != 0) {
                    // destroy all the blocks and remove them from the grid; matched blocks cannot be in polyominoes so this is fine to run
                    for (int column = 0; column < boardWidth; column++) {
                        Destroy(grid[row, column].gameObject);
                        grid[row, column] = null;
                    }

                    // reset attack (and "row is matched") marker
                    attackTotals[row] = 0;

                    if (lowestSweptRow > row) {
                        lowestSweptRow = row;  // log the row we need to check above for falling blocks as a result of matching
                    }
                }
            }

            // set up falling in all rows that have had their support removed
            // "cascade mode" for now, as the simplest algorithm given the polyomino falling code
            if (lowestSweptRow < boardHeight - 1) {  // if only the top row was matched, nothing needs to fall
                // drop each column individually
                for (int column = 0; column < boardWidth; column++) {
                    // within this column, each contiguous group of blocks (disregarding all other columns) that is high enough to fall gets its own polyomino and falls independently
                    PolyominoScript currentPolyomino = null;

                    // ordering is deliberate, so that when the falls are checked for these the lowest will fall first (preventing unwanted collision)
                    for (int row = lowestSweptRow + 1; row < boardHeight; row++) {
                        if ((grid[row, column] != null) && (grid[row, column].GetState() == BlockStateEnum.BlockState.Locked)) {  // this block should fall
                            // make a new polyomino if needed, and add it to the list of ones that should fall
                            if (currentPolyomino == null) {
                                currentPolyomino = ((GameObject)Instantiate(gameManager.polyominoPrefab)).GetComponent<PolyominoScript>();
                                currentPolyomino.memberBlocks = new List<BlockScript>();
                                fallingPolyominos.Add(currentPolyomino);
                            }

                            // this block should be in currentPolyomino, whether it was just created or not
                            currentPolyomino.memberBlocks.Add(grid[row, column]);
                            grid[row, column].SetPolyomino(currentPolyomino);

                            // this block should know that it is now falling
                            grid[row, column].SetState(BlockStateEnum.BlockState.Falling);
                        } else {  // whether there's a block here or not, it ends the current polyomino by separating it from any blocks that should fall above
                            currentPolyomino = null;
                        }
                    }
                }
            }
        }

        // Drops (and locks as needed) all polyominos that are falling outside player control.
        private void DropAllFallingPolyominos() {
            // Using List.ForEach allows modification as we iterate, which is necessary for removing polyominos as they lock.
            System.Action<PolyominoScript> dropper = (poly) => {
                // Always drop the polyomino, but only lock it if it can't move.
                if (!(DropPolyomino(poly))) {  // lock it and generate a new one
                    fallingPolyominos.Remove(poly);  // remove before destruction, while we have the reference passed in
                    LockPolyomino(poly);  // after this the old polyomino is no longer valid
                }
            };
            fallingPolyominos.ForEach(dropper);
        }

        private void GenerateAttackPolyominos() {
            int blocksToGenerate = attackReceived;
            attackReceived = 0;  // prevents call spamming

            int[] nextBlockHeights = Enumerable.Repeat(boardHeight - 1, boardWidth).ToArray();  // next "free" height for each column
            PolyominoScript[] attackPolyominos = Enumerable.Repeat((PolyominoScript)null, boardWidth).ToArray();  // doing this here avoids generating unnecessary extra polyominos

            // generate each block individually to allow per-block processing
            while (blocksToGenerate > 0) {
                int column = gameManager.prng.Next(boardWidth);  // the column to put the block in

                // generate new polyomino if needed
                if (attackPolyominos[column] == null) {
                    attackPolyominos[column] = ((GameObject)Instantiate(gameManager.polyominoPrefab)).GetComponent<PolyominoScript>();
                    attackPolyominos[column].memberBlocks = new List<BlockScript>();
                    fallingPolyominos.Add(attackPolyominos[column]);
                }

                // generate block
                BlockScript block = GenerateBlockInPlace(new Vector2Int(column, nextBlockHeights[column]));
                // register to polyomino
                attackPolyominos[column].memberBlocks.Add(block);
                block.SetPolyomino(attackPolyominos[column]);
                // check grid for colliding block
                if (grid[nextBlockHeights[column], column] == null) {
                    grid[nextBlockHeights[column], column] = block;
                } else {
                    LoseRound();
                }

                nextBlockHeights[column]--;
                blocksToGenerate--;
            }
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
    /// Balancing controls - know about them, but other scripts don't need to use them (at least not directly).
    /// </summary>

    // Balancing control for number of upcoming polyominoes to display per player.
    private static int numUpcomingPolyominoes = 3;
    
    // Balancing controls for the board height and width, measured in grid squares.
    private static int boardHeight = 20;
    private static int boardWidth = 10;

    // Balancing controls for game speed.
    private static double tickLength = 1.0;
    private static int fastDropMultiplier = 4;  // How much faster is fast drop than waiting for the block to fall on its own?

    // Controls how powerful attacks are at each row height (how many blocks they send, before round-based multipliers).
    private static int[] attackStrengths = {10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29};  // set up individually rather than as multipliers for individual tweaking

    // Controls for the round system.
    private static int numRoundsToWin = 3;  // effectively best of numRoundsToWin * 2 - 1
    private static int[] roundDisparityMultipliers = {1, 2, 3};  // multiplier to all attack strengths for the player who is behind


    /// <summary>
    /// For class functionality only.
    /// </summary>

    // PRNG instance to use as needed for attacks and polyomino selection.
    private System.Random prng;

    private double timeSinceLastTick;
    private double timeSinceLastFastDrop;

    // Players' round win counts.
    private int[] roundsWon;
    // Players' attacks to send over to opponents.
    private int[] attacksToSend;


    /// <summary>
    /// Accessors for use by other scripts.
    /// </summary>
    public int GetNumRoundsToWin() {
        return numRoundsToWin;
    }
    public int GetPlayerRoundsWon(int player) {
        return roundsWon[player - 1];
    }


    /// <summary>
    /// Helper functions.
    /// </summary>

    // Use this for initialization
    void Start () {
        prng = new System.Random();
        
        // Assuming for simplicity that (0, 0) is at the bottom of the boards and centered between them.
        player1 = new PlayerBoard(this, new Vector2(-15, 0), roundDisparityMultipliers[0]);
        player2 = new PlayerBoard(this, new Vector2(5, 0), roundDisparityMultipliers[0]);

        timeSinceLastTick = 0.0;
        timeSinceLastFastDrop = 0.0;

        roundsWon = new int[2];  // automatically initializes to 0
        attacksToSend = new int[2];  // automatically initializes to 0
    }
	
    // Choose a new random polyomino to queue.  Here in case we want both players to draw from a shared bag as another point of competition.
    private PolyominoShapeEnum.PolyominoShape ChoosePolyomino() {
        return (PolyominoShapeEnum.PolyominoShape)(prng.Next(System.Enum.GetNames(typeof(PolyominoShapeEnum.PolyominoShape)).Length));  // remains uniform regardless of number of polyominoes to choose from
    }


	// Update is called once per frame
	void Update () {
        timeSinceLastFastDrop += Time.deltaTime;
        timeSinceLastTick += Time.deltaTime;

        if (timeSinceLastTick >= tickLength) {  // time for the next tick
            // reset timer first to avoid repeat ticks from Unity Update() spamming
            timeSinceLastTick -= tickLength;  // carryover-aware timer reset
            timeSinceLastFastDrop -= (tickLength / fastDropMultiplier);  // must also reset fast drop, as its update is included in a tick update (no double dropping)

            // send over attacks from last tick, to be processed this tick
            player1.ReceiveAttack(attacksToSend[1]);
            player2.ReceiveAttack(attacksToSend[0]);

            player1.DoTick();
            player2.DoTick();

            // get attacks from this tick, to be sent over next tick
            attacksToSend[0] = 0;
            attacksToSend[1] = 0;
            for (int i = 0; i < boardHeight; i++) {
                attacksToSend[0] += player1.GetAttackTotals()[i];
                attacksToSend[1] += player2.GetAttackTotals()[i];
            }

            // check for round losses
            if (player1.GetLossStatus() || player2.GetLossStatus()) {

            }
        }

        // implicit else with the exception of extremely slow framerates, which we want
        if (timeSinceLastFastDrop >= (tickLength / fastDropMultiplier)) {  // time for the next fast drop
            player1.DoFastDrop();
            player2.DoFastDrop();
            timeSinceLastFastDrop -= (tickLength / fastDropMultiplier);  // carryover-aware timer reset
        }
	}

}
