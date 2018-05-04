using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconActivation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Activation()
    {
        Text txt = gameObject.GetComponent<Text>();
        txt.color = Color.white;
    }

    public void Deactivation()
    {
        Text txt = gameObject.GetComponent<Text>();
        txt.color = Color.black;
    }
}
