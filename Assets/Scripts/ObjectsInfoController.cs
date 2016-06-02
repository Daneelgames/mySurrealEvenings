using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectsInfoController : MonoBehaviour {

    public Animator casterGO;

    public Image unitSpriteCaster;
    public Text _nameCaster;
    public Image healthbarCaster;

    public List<Image> effectsCaster = new List<Image>();

    public Image agressiveIconCaster;

    public Animator targetGO;

    public Image unitSpriteTarget;
    public Text _nameTarget;
    public Image healthbarTarget;

    public List<Image> effectsTarget = new List<Image>();

    public Image agressiveIconTarget;

    private InteractiveObject caster;
    private InteractiveObject target;

    private bool windowsVisible = false;

    public Animator dialogBackgroundAnimator;
    public Text dialogText;

    //private float casterMaxHealth = 1;
    //private float targetMaxHealth = 1;

    void Update()
    {
        if (windowsVisible)
        {
            healthbarCaster.fillAmount = Mathf.Lerp(caster.health / caster.maxHealth, healthbarCaster.fillAmount, 0.5f);
            healthbarTarget.fillAmount = Mathf.Lerp(target.health / target.maxHealth, healthbarTarget.fillAmount, 0.5f);
        }
    }

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
            if (caster.npcControl != null && caster.npcControl.agressiveTo == NpcController.Target.everyone)
                agressiveIconCaster.enabled = true;
            else
                agressiveIconCaster.enabled = false;
        }

        _nameTarget.text = target._name;
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
}