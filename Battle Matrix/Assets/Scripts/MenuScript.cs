using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    
    public List<GameObject> menuItems;
    public List<GameObject> creditsItems;
    public Button creditsButton;

    bool isCredits;

    // Use this for initialization
    void Start () {
        SetMenu();
        isCredits = false;
        creditsButton.onClick.AddListener(SwitchMenu);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SwitchMenu()
    {
        if (isCredits)
            SetMenu();
        else
            SetCredits();

        isCredits = !isCredits;

    }

    void SetMenu()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].SetActive(true);
        }

        for (int i = 0; i < creditsItems.Count; i++)
        {
            creditsItems[i].SetActive(false);
        }
    }

    void SetCredits()
    {
        for (int i = 0; i < menuItems.Count; i++)
        {
            menuItems[i].SetActive(false);
        }

        for (int i = 0; i < creditsItems.Count; i++)
        {
            creditsItems[i].SetActive(true);
        }
    }
}
