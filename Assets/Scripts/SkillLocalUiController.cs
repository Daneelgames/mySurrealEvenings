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
        childicon = GetComponentInChildren<Image>();
    }

    public void SetWeak()
    {
        childicon.sprite = relationsIcons[0];
    }
    public void SetImmune()
    {
        childicon.sprite = relationsIcons[1];
    }

}
