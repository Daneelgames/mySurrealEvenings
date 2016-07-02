using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectsInfoController : MonoBehaviour {

    public Animator casterGO;

    public Image unitSpriteCaster;
    public Text _nameCaster;

    public List<Image> effectsCaster = new List<Image>();

    public Image agressiveIconCaster;

    public Animator targetGO;

    public Image unitSpriteTarget;
    public Text _nameTarget;

    public List<Image> effectsTarget = new List<Image>();

    public Image agressiveIconTarget;

    private InteractiveObject caster;
    private InteractiveObject target;

    private bool windowsVisible = false;

    public Animator dialogBackgroundAnimator;
    public Text dialogText;

    //private float casterMaxHealth = 1;
    //private float targetMaxHealth = 1;

    public void ShowWindows(InteractiveObject curCaster, InteractiveObject curTarget, bool isDialog)
    {
        if (!isDialog)
            StartCoroutine("SetAnimatorBools");
        else
        {
            StartCoroutine("DialogStartAnimatorBools");
            SetDialogText(curTarget);
        }

        caster = curCaster;
        target = curTarget;

        if (curCaster != null)
        {
            _nameCaster.text = caster._name;
            unitSpriteCaster.sprite = curCaster.facepic;

            if (caster.npcControl != null && caster.npcControl.agressiveTo == NpcController.Target.everyone)
                agressiveIconCaster.enabled = true;
            else
                agressiveIconCaster.enabled = false;
        }

        _nameTarget.text = target._name;
        unitSpriteTarget.sprite = curTarget.facepic;

        if (target.npcControl != null && target.npcControl.agressiveTo == NpcController.Target.everyone)
            agressiveIconTarget.enabled = true;
        else
            agressiveIconTarget.enabled = false;


        windowsVisible = true;
    }

    public void DialogOver()
    {
        HideWindows();
    }

    IEnumerator SetAnimatorBools()
    {
        casterGO.SetBool("Update", true);
        casterGO.SetBool("Active", true);
        targetGO.SetBool("Update", true);
        targetGO.SetBool("Active", true);

        yield return new WaitForSeconds(0.1F);

        casterGO.SetBool("Update", false);
        targetGO.SetBool("Update", false);
    }

    IEnumerator DialogStartAnimatorBools()
    {
        casterGO.SetBool("Active", false);
        targetGO.SetBool("Update", true);
        targetGO.SetBool("Active", true);
        dialogBackgroundAnimator.SetBool("Update", true);
        dialogBackgroundAnimator.SetBool("Active", true);

        yield return new WaitForSeconds(0.1F);

        casterGO.SetBool("Update", false);
        targetGO.SetBool("Update", false);
        dialogBackgroundAnimator.SetBool("Update", false);
    }

    public void HideWindows()
    {
        windowsVisible = false;

        casterGO.SetBool("Active", false);
        targetGO.SetBool("Active", false);
        dialogBackgroundAnimator.SetBool("Active", false);
    }

    public void HideDialogBackground()
    {
        dialogBackgroundAnimator.SetBool("Active", false);
    }

    void SetDialogText(InteractiveObject speaker)
    {
        dialogText.text = speaker.dialogues[speaker.activeDialog].stringList[speaker.activePhrase];
    }

    public void AggressiveFeedback(int unit) // 0 - caster; 1 - target
    {
        if (GameManager.Instance.objectsTurn.inParty && !GameManager.Instance.blockSkillIcons)
        {
            string name = "";
            if (unit == 0)
                name = caster._name;
            else
                name = target._name;

            GameManager.Instance.PrintActionFeedback(null, name + " is aggressive to your crew.", null, false, false, true);
            GameManager.Instance.mouseOverButton = true;
        }
    }

    public void AgressiveFeedbackOff()
    {

        if (GameManager.Instance.objectsTurn.inParty && !GameManager.Instance.blockSkillIcons)
        {
            GameManager.Instance.HideTextManually();
            GameManager.Instance.mouseOverButton = false;
        }
    }
}