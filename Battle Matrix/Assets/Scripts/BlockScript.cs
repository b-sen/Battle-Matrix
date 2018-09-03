using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure to use the block states in the same way as other scripts.
[RequireComponent(typeof(BlockStateEnum))]

public class BlockScript : MonoBehaviour {
    private static int ticksOfDelay = 8;  // balancing control - the number of ticks for which an attack block gums up the board as "ticking"

    private BlockStateEnum.BlockState state;  // current state
    private PolyominoScript polyomino;  // polyomino this block is a member of, if any - null otherwise
    private int ticksRemaining;

    public Sprite fallingSprite;
    public Sprite lockedSprite;
    public Sprite matchedSprite;
    public Sprite attackSprite; // also used for ticking blocks

    float size_x;
    float size_y;

    // Use this for initialization
    void Start () {
        // cannot set a default state for newly created blocks here because it will overwrite states given by the generating script - state MUST be set BY INSTANTIATOR!
	}

    public BlockStateEnum.BlockState GetState() {
        return state;
    }
    public void SetState(BlockStateEnum.BlockState newState) {
        // If a block is part of an attack, it must tick down before being available for matches.
        if ((state == BlockStateEnum.BlockState.Attack) && (newState == BlockStateEnum.BlockState.Locked)) {
            state = BlockStateEnum.BlockState.Ticking;
        } else {
            state = newState;
        }

        // Start the tick countdown.
        if (state == BlockStateEnum.BlockState.Ticking) {
            ticksRemaining = ticksOfDelay;
        }

        // Change sprite according to new state.
        switch (state) {
            case BlockStateEnum.BlockState.Falling:
                transform.GetComponent<SpriteRenderer>().sprite = fallingSprite;
                break;
            case BlockStateEnum.BlockState.Locked:
                transform.GetComponent<SpriteRenderer>().sprite = lockedSprite;
                break;
            case BlockStateEnum.BlockState.Matched:
                transform.GetComponent<SpriteRenderer>().sprite = matchedSprite;
                break;
            default:  // from an attack, whether falling as part of it or ticking
                transform.GetComponent<SpriteRenderer>().sprite = attackSprite;
                break;
        }

        size_x = transform.GetComponent<SpriteRenderer>().bounds.size.x;
        size_y = transform.GetComponent<SpriteRenderer>().bounds.size.y;
        Debug.Log(size_x + ", " + size_y);
    }

    public PolyominoScript GetPolyomino() {
        return polyomino;
    }
    public void SetPolyomino(PolyominoScript newPolyomino) {
        polyomino = newPolyomino;
    }

    // to be called by the board this block is on; currently only affects ticking blocks
    public void DoTick() {
        if (state == BlockStateEnum.BlockState.Ticking) {
            ticksRemaining--;  // tick down
            if (ticksRemaining == 0) {  // countdown complete

                // In case the countdown completes while the block is falling due to a cascade - all and only mobile blocks have a polyomino.
                if (polyomino != null) {
                    this.SetState(BlockStateEnum.BlockState.Falling);
                } else {
                    this.SetState(BlockStateEnum.BlockState.Locked);
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        

    }
}
