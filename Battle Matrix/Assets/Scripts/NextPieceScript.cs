using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolyominoShapeEnum))]

public class NextPieceScript : MonoBehaviour {

    // Index corresponds to polyomino enum
    public List<Sprite> iconList;
    public bool isP1 = true;
    public GameManagerScript gm;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isP1)
        {
            SetNextPiece(gm.player1.GetUpcomingPolyominoesAsList()[0]);
        }
        else
        {
            SetNextPiece(gm.player2.GetUpcomingPolyominoesAsList()[0]);
        }
	}

    void SetNextPiece(PolyominoShapeEnum.PolyominoShape shape)
    {
        transform.GetComponent<SpriteRenderer>().sprite = iconList[(int)shape];
    }
}
