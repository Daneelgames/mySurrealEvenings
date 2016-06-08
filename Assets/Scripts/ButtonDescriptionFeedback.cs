using UnityEngine;
using System.Collections;

public class ButtonDescriptionFeedback : MonoBehaviour {

    public string desctiption = "This is a button.";

    public void MouseEnter()
    {
        if (GameManager.Instance.objectsTurn.inParty && !GameManager.Instance.blockSkillIcons)
            GameManager.Instance.PrintActionFeedback(null, desctiption, null, false, false, true);
    }

    public void MouseExit()
    {
        if (GameManager.Instance.objectsTurn.inParty && !GameManager.Instance.blockSkillIcons)
            GameManager.Instance.HideTextManually();
    }

}
