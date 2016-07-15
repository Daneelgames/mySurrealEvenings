using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SkillLocalUiController : MonoBehaviour
{

    public List<Sprite> relationsIcons;

    private Image childicon;

    void Start()
    {
        childicon = transform.Find("RelationIcon").GetComponentInChildren<Image>();
    }

    public void SetClear()
    {
        childicon.color = Color.clear;
    }

    public void SetWeak()
    {
        childicon.sprite = relationsIcons[0];
        childicon.color = Color.white;
    }

    public void SetImmune()
    {
        childicon.sprite = relationsIcons[1];
        childicon.color = Color.white;
    }
}