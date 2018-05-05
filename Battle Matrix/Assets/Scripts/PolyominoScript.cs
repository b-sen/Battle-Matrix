using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure to use the polyominoes in the same way as other scripts.
[RequireComponent(typeof(PolyominoShapeEnum))]

public class PolyominoScript : MonoBehaviour {
    public List<BlockScript> memberBlocks;

	// Use this for initialization
	void Start () {
        memberBlocks = new List<BlockScript>();
	}

    // Update is called once per frame
    void Update() {

    }

    // Allow for de-linking behaviour.
    private void OnDestroy() {
        foreach (BlockScript block in memberBlocks) {
            block.SetPolyomino(null);
        }
    }
}
