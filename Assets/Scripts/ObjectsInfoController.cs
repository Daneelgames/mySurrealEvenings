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

    public void ShowWindows(InteractiveObject curCaster, InteractiveObject curTarget)
    {
        StartCoroutine("SetAnimatorBools");

        caster = curCaster;
        target = curTarget;

        _nameCaster.text = caster._name;
        //casterMaxHealth = caster.maxHealth;

        _nameTarget.text = target._name;
        //targetMaxHealth = target.maxHealth;

        windowsVisible = true;
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

    public void HideWindows()
    {
        windowsVisible = false;

        casterGO.SetBool("Active", false);
        targetGO.SetBool("Active", false);
    }
}