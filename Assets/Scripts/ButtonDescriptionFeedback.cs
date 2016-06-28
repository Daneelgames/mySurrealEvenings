using UnityEngine;
using System.Collections;

public class ButtonDescriptionFeedback : MonoBehaviour
{

    public enum iconType { sanity, goFurther, skipTurn }

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

                if (curSanity >= 90)
                    sanityDescription = "Child is perfectly fine!";
                else if (curSanity < 90 && curSanity >= 75)
                    sanityDescription = "Child feels almost great!";
                else if (curSanity < 75 && curSanity >= 50)
                    sanityDescription = "Child is OK.";
                else if (curSanity < 50 && curSanity >= 25)
                    sanityDescription = "Child is scared...";
                else if (curSanity < 25 && curSanity >= 10)
                    sanityDescription = "Child is trembling!";
                else if (curSanity < 10)
                    sanityDescription = "CHILD IS TERRIFIED";


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

                escapeDescription = "Go to sleep.";
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