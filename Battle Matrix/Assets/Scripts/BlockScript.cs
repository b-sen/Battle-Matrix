using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure to use the block states in the same way as other scripts.
[RequireComponent(typeof(BlockStateEnum))]

public class BlockScript : MonoBehaviour {
    private BlockStateEnum.BlockState state;  // current state
    private PolyominoScript polyomino;  // polyomino this block is a member of, if any - null otherwise

    // Use this for initialization
    void Start () {
        this.SetState(BlockStateEnum.BlockState.Falling);  // default state for newly created blocks
	}

    public BlockStateEnum.BlockState GetState() {
        return state;
    }
    public void SetState(BlockStateEnum.BlockState newState) {
        state = newState;
        // change sprite here
    }

    public PolyominoScript GetPolyomino() {
        return polyomino;
    }
    public void SetPolyomino(PolyominoScript newPolyomino) {
        polyomino = newPolyomino;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
