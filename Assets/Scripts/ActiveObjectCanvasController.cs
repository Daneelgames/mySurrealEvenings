using UnityEngine;
using System.Collections;

public class ActiveObjectCanvasController : MonoBehaviour {

    [SerializeField]
    private GameObject[] buttonIcons;
    [SerializeField]
    private GameObject[] skillIcons;

    [SerializeField]
    Canvas _canvas;

    public bool skillsVisible = false;

    void Awake()
    {
        _canvas.worldCamera = Camera.main;
    }

    public void ShowIcons()
    {
        foreach (GameObject go in buttonIcons)
            go.SetActive(true);
    }

    public void HideIcons()
    {
        foreach (GameObject go in buttonIcons)
            go.SetActive(false);

        HideSkills();
    }

    public void ShowSkills()
    {
        foreach (GameObject go in skillIcons)
            go.SetActive(true);

        skillsVisible = true;
    }

    public void HideSkills()
    {
        foreach (GameObject go in skillIcons)
            go.SetActive(false);

        skillsVisible = false;
    }
}
