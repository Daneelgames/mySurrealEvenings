using UnityEngine;
using System.Collections;

public enum Effect { none, poison, curse, insanity }

public class SkillController : MonoBehaviour {

    public string skillName;

    public int price = 1;

    public enum Type {offensive, recover}
    public enum Range {one, all, allParty, allAgressive}

    public Type skillType = Type.offensive; 

    public Sprite skillSprite;

    public string description;

    public int skillLevel = 1;

    public float frenzy = 5;

    public float damageTarget = 0;
    public float recoverTarget = 0;
    public Effect effectTarget = Effect.none;

    public float damageCaster = 0;
    public float recoverCaster = 0;
    public Effect effectCaster = Effect.none;

    public float missRate = 0; // from 0 to 1
    public float critRate = 0; // from 0 to 1

    public Range skillRange = Range.one;

    public GameObject AttackParticle;

    private bool gathered = false;

    public void SetTargets(InteractiveObject caster, InteractiveObject target)
    {
        //print(caster.name);
        if (skillRange == Range.one)
        {
            float targetRandom = Random.Range(0f, 1f);
            if (targetRandom >= missRate)
            {
                target.Damage(damageTarget + targetRandom, caster);
                target.Recover(recoverTarget + targetRandom);


                if (effectTarget != Effect.none)
                {
                    bool alreadyHasEffect = false;
                    foreach (Effect j in target.unitEffect)
                    {
                        if (j == effectTarget)
                            alreadyHasEffect = true;
                    }
                    if (!alreadyHasEffect)
                        target.unitEffect.Add(effectCaster);
                }
            }

            float casterRandom = Random.Range(0f, 1f);
            if (casterRandom >= missRate)
            {
                caster.Damage(damageCaster + casterRandom, caster);
                caster.Recover(recoverCaster + casterRandom);
                

                if (effectCaster != Effect.none)
                {
                    bool alreadyHasEffect = false;
                    foreach (Effect j in caster.unitEffect)
                    {
                        if (j == effectCaster)
                            alreadyHasEffect = true;
                    }
                    if (!alreadyHasEffect)
                        caster.unitEffect.Add(effectCaster);
                }
            }
        }
        
        // FRENZY DMG
        if (target == GameManager.Instance.party[0] || caster == GameManager.Instance.party[0])
            GameManager.Instance.FrenzyDamage(frenzy);
    }

    void OnMouseDown()
    {
        if (!gathered && GameManager.Instance.objectsTurn.inParty && !GameManager.Instance.blockSkillIcons)
        {
            bool emptySlot = false;
            foreach (InventorySlotController slot in GameManager.Instance.inventoryController.slots)
            {
                if (slot.itemInSlot == null)
                {
                    emptySlot = true;
                    break;
                }
            }

            if (emptySlot)
            {
                gathered = true;
                GameObject newSkill = null;
                foreach (GameObject go in GameManager.Instance.skillList.allSkills)
                {
                    if (go.name == skillName)
                    {
                        newSkill = go;
                        break;
                    }
                }
                GameManager.Instance.inventoryController.ItemGet(newSkill.GetComponent<SkillController>());
                GameManager.Instance.skills_1.Add(newSkill);

                GameManager.Instance.HideTextManually();
                GameManager.Instance.mouseOverButton = false;
            }
            else
                PrintNoSpace();
        }
    }

    void Update()
    {
        if (gathered)
            FlyToInventory();
    }

    void FlyToInventory()
    {
        if (Vector3.Distance(transform.position, GameManager.Instance.party[0].transform.position) > 3f)
            transform.position = Vector3.Lerp(transform.position, GameManager.Instance.party[0].transform.position, 5f * Time.deltaTime);
        else
            Destroy(gameObject);
    }

    public void OnMouseEnter()
    {
        if (GameManager.Instance.objectsTurn.inParty)
        {
            GameManager.Instance.PrintActionFeedback(null, description, null, false, false, true);
            GameManager.Instance.mouseOverButton = true;
        }
    }

    public void OnMouseExit()
    {
        if (GameManager.Instance.objectsTurn.inParty)
        {
            GameManager.Instance.HideTextManually();
            GameManager.Instance.mouseOverButton = false;
        }
    }

    void PrintNoSpace()
    {
        GameManager.Instance.PrintActionFeedback(null, "Inventory is full!", null, false, false, true);
    }
}