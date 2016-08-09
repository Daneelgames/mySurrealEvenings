using UnityEngine;
using System.Collections;

public class ButtonDescriptionFeedback : MonoBehaviour
{

    public enum iconType { sanity, escape, skipTurn, backpack }

    public string desctiption = "This is a button.";
    public iconType type = iconType.sanity;

    private int enemyAmount = 0;
    private float enemyLvlAmount = 0;


    public void MouseEnter()
    {
        if (GameManager.Instance.objectsTurn.inParty && !GameManager.Instance.blockSkillIcons)
        {
            GameManager.Instance.mouseOverButton = true;
            if (type == iconType.skipTurn)
                GameManager.Instance.PrintActionFeedback(null, desctiption, null, false, false, true);
            else if (type == iconType.backpack)
                GameManager.Instance.PrintActionFeedback(null, desctiption, null, false, false, true);
            else if (type == iconType.sanity)
            {
                float curSanity = GameManager.Instance.curSanity;
                string sanityDescription = "";

                if (curSanity >= 90)
                    sanityDescription = "I'm perfectly fine!";
                else if (curSanity < 90 && curSanity >= 75)
                    sanityDescription = "I feel almost great!";
                else if (curSanity < 75 && curSanity >= 50)
                    sanityDescription = "I'm ok.";
                else if (curSanity < 50 && curSanity >= 25)
                    sanityDescription = "I'm scared...";
                else if (curSanity < 25 && curSanity >= 10)
                    sanityDescription = "I'm trembling!";
                else if (curSanity < 10)
                    sanityDescription = "I AM TERRIFIED";

                GameManager.Instance.PrintActionFeedback(null, sanityDescription, null, false, false, true);
            }

            else if (type == iconType.escape)
            {
                float curChance = GameManager.Instance.escapeChance * 100;
                string text = "";

                if (curChance > 50)
                    text = "75% chance to escape";
                else if (curChance <= 50 && curChance > 25)
                    text = "50% chance to escape";
                else if (curChance <= 25)
                    text = "25% chance to escape";

                GameManager.Instance.PrintActionFeedback(null, text, null, false, false, true);
            }
        }
    }


    public void MouseExit()
    {
        if (GameManager.Instance.objectsTurn.inParty && !GameManager.Instance.blockSkillIcons)
        {
            GameManager.Instance.HideTextManually();
            GameManager.Instance.mouseOverButton = false;
        }
    }
}