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

    public void PointerEnterButton(int skill) // -2 speech; -1 actions; 0 skill_0; 1 skill_1; 2 skill_2
    {
        if (skill >= 0)
        {
            string sendDescription = GameManager.Instance.skillsCurrent[skill].GetComponent<SkillController>().description;
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);
        }
        GameManager.Instance.mouseOverButton = true;
    }

    public void PointerExitButton()
    {
        GameManager.Instance.mouseOverButton = false;
        GameManager.Instance.HideTextManually();
    }
}