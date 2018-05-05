using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make sure to use the polyominoes in the same way as other scripts.
[RequireComponent(typeof(PolyominoShapeEnum))]

public class PolyominoScript : MonoBehaviour {
    public List<BlockScript> memberBlocks;  // MUST be initially provided BY INSTANTIATOR!

	// Use this for initialization
	void Start () {
        // Cannot actually initialize here, as this function can get called AFTER the code that both instantiates the object and populates the list!
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
