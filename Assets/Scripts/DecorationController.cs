using UnityEngine;
using System.Collections;

public class DecorationController : MonoBehaviour
{
    public enum Decoration { chair, couch };
    public Decoration decorType = Decoration.chair;

    public string decorTitle;
    public string decorDescription;
    public Sprite decorIcon;
    public int craftTime;
    public bool clickable = false;

    void Start()
    {
        GameManager.Instance.AddDecoration(this);
    }

    public void ActionOnDay()
    {
        switch (decorType)
        {
            case Decoration.chair:
                ChairEffect();
                break;
        }
    }
    public void ActionOnNight()
    {
        switch (decorType)
        {
            case Decoration.couch:
                CouchEffect();
                break;
        }
    }

    public void ActionOnClick()
    {
        clickable = false;
    }

    void SetClickable()
    {
        clickable = true;
    }

    void ChairEffect()
    {
        print("chair used effect");
    }
    void CouchEffect()
    {
        print("couch used effect");
    }
}