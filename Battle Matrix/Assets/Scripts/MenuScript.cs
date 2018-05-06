using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour {

    
    public List<GameObject> menuItems;
    public List<GameObject> creditsItems;
    public Button creditsButton;
    public Button startButton;

    bool isCredits;

    // Use this for initialization
    void Start () {
        SetMenu();
        isCredits = false;
        startButton.onClick.AddListener(GoToPlay);
        creditsButton.onClick.AddListener(SwitchMenu);
	}
	
	// Update is called once per frame
	void Update () {
        startButton.onClick.AddListener(GoToPlay);
        creditsButton.onClick.AddListener(SwitchMenu);
    }

    void GoToPlay()
    {
        SceneManager.LoadScene("Gameplay");
    }

    void SwitchMenu()
    {
        if (isCredits)
            SetMenu();
        else
            SetCredits();

        isCredits = !isCredits;
        print(isCredits);

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
