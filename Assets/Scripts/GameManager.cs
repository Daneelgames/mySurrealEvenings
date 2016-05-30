using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public InteractiveObject SelectedObject;
    public List<InteractiveObject> objectList = new List<InteractiveObject>();

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        DontDestroyOnLoad(gameObject);
    }

    public void SetSelectedObject(InteractiveObject curSelected)
    {
        SelectedObject = curSelected;

        foreach(InteractiveObject obj in objectList)
        {
            obj.ToggleSelectedFeedback();
        }
    }
}