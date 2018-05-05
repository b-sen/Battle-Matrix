using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolyominoShapeEnum))]

public class NextPieceScript : MonoBehaviour {

    // Index corresponds to polyomino enum
    public List<Sprite> iconList;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetNextPiece(PolyominoShapeEnum.PolyominoShape shape)
    {
        transform.GetComponent<SpriteRenderer>().sprite = iconList[(int)shape];
    }
}
