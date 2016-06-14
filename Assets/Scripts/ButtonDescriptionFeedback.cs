using UnityEngine;
using System.Collections;

public class ButtonDescriptionFeedback : MonoBehaviour {

    public enum iconType { sanity, goFurther, skipTurn}

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
            else if (type == iconType.sanity)
            {
                float curSanity = GameManager.Instance.curSanity;
                string sanityDescription = "";

                if (curSanity >= 75)
                    sanityDescription = "Sanity is high.";
                else if (curSanity < 75 && curSanity >= 50)
                    sanityDescription = "Sanity is normal.";
                else if (curSanity < 50 && curSanity >= 25)
                    sanityDescription = "Sanity is low.";
                else if (curSanity < 25 )
                    sanityDescription = "Sanity is critical!";

                GameManager.Instance.PrintActionFeedback(null, sanityDescription, null, false, false, true);
            }

            else if (type == iconType.goFurther)
            {
                enemyAmount = 0;
                enemyLvlAmount = 0;
                string escapeDescription = "";
                foreach (InteractiveObject npc in GameManager.Instance.objectList)
                {
                    if (npc.npcControl != null && npc.npcControl.agressiveTo == NpcController.Target.everyone)
                    {
                        enemyAmount += 1;
                        enemyLvlAmount += npc.npcControl.overallDifficulty;
                    }
                }

                enemyAmount += Mathf.RoundToInt(enemyLvlAmount);

                switch (enemyAmount)
                {
                    case 0:
                        escapeDescription = "Go further.";
                        break;
                    case 1:
                        escapeDescription = "90% chance to escape.";
                        break;
                    case 2:
                        escapeDescription = "80% chance to escape.";
                        break;
                    case 3:
                        escapeDescription = "70% chance to escape.";
                        break;
                    case 4:
                        escapeDescription = "60% chance to escape.";
                        break;
                    case 5:
                        escapeDescription = "50% chance to escape.";
                        break;
                    case 6:
                        escapeDescription = "40% chance to escape.";
                        break;
                    case 7:
                        escapeDescription = "30% chance to escape.";
                        break;
                    case 8:
                        escapeDescription = "20% chance to escape.";
                        break;
                    case 9:
                        escapeDescription = "10% chance to escape.";
                        break;
                    default:
                        escapeDescription = "Go further.";
                        break;
                }

                GameManager.Instance.PrintActionFeedback(null, escapeDescription, null, false, false, true);
            }
        }
    }

    public void MouseClick()
    {
        if (type == iconType.goFurther)
        {
            GameManager.Instance.LeaveLevel(enemyAmount);
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