﻿using UnityEngine;
using UnityEngine.UI;
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
        for(int i = 0; i < 3; i ++)
        {
            skillIcons[i].SetActive(true);
            if (GameManager.Instance.skillsCurrent[i] != null)
            {
                skillIcons[i].GetComponent<Image>().sprite = GameManager.Instance.skillsCurrent[i].GetComponent<SkillController>().skillSprite;
            }
        }

        skillsVisible = true;
    }

    public void HideSkills()
    {
        foreach (GameObject go in skillIcons)
            go.SetActive(false);

        skillsVisible = false;
    }

    public void PointerEnterButton()
    {
        GameManager.Instance.mouseOverButton = true;
    }

    public void PointerExitButton()
    {
        GameManager.Instance.mouseOverButton = false;
    }
}
