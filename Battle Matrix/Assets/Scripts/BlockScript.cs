using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure to use the block states in the same way as other scripts.
[RequireComponent(typeof(BlockStateEnum))]

public class BlockScript : MonoBehaviour {
    private BlockStateEnum.BlockState state;  // current state
    private PolyominoScript polyomino;  // polyomino this block is a member of, if any - null otherwise

    public Sprite fallingSprite;
    public Sprite lockedSprite;
    public Sprite matchedSprite;

    float size_x;
    float size_y;

    // Use this for initialization
    void Start () {
        this.SetState(BlockStateEnum.BlockState.Falling);  // default state for newly created blocks
	}

    public BlockStateEnum.BlockState GetState() {
        return state;
    }
    public void SetState(BlockStateEnum.BlockState newState) {
        state = newState;

        switch (state)
        {
            case BlockStateEnum.BlockState.Falling:
                transform.GetComponent<SpriteRenderer>().sprite = fallingSprite;
                break;
            case BlockStateEnum.BlockState.Locked:
                transform.GetComponent<SpriteRenderer>().sprite = lockedSprite;
                break;
            default:
                transform.GetComponent<SpriteRenderer>().sprite = matchedSprite;
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
	
	// Update is called once per frame
	void Update () {
        

    }
}
