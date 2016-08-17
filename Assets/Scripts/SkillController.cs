using UnityEngine;
using System.Collections;


public class SkillController : MonoBehaviour
{

    public string skillName;

    public int price = 1;
    public int priceTrash = 1;

    public enum Type { fire, water, stone, wind, electricity, ice, gore, piece }

    public Type skillType = Type.fire;

    public Sprite skillSprite;

    public string description;

    public int skillLevel = 1;

    public float frenzy = 5;

    public float damageTarget = 0;
    public float recoverTarget = 0;

    public float damageCaster = 0;
    public float recoverCaster = 0;
    public int manaCost = 0;

    public GameObject AttackParticle;

    private bool gathered = false;

    public GameObject targets;
    public void SetTargets(InteractiveObject caster, InteractiveObject target)
    {
        //print(caster.name);
        float targetRandom = Random.Range(0f, 1f);
        target.Damage(damageTarget + targetRandom, caster);
        target.Recover(recoverTarget + targetRandom);

        float casterRandom = Random.Range(0f, 1f);
        caster.Damage(damageCaster + casterRandom, caster);
        caster.Recover(recoverCaster + casterRandom);

        // FRENZY DMG
        //if (target == GameManager.Instance.player || caster == GameManager.Instance.player)
        GameManager.Instance.FrenzyDamage(frenzy);

        if (target != caster) // start battle bar
        {
            if (target.inParty)
                GameManager.Instance.battleBar.StartDefence(targets);
            else if (caster.inParty)
                GameManager.Instance.battleBar.StartAttack(targets);
        }
    }

    void Start()
    {
        GameManager.Instance.skillsOnGround.Add(this);
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
                GameManager.Instance.skills.Add(newSkill);

                GameManager.Instance.HideTextManually();
                GameManager.Instance.mouseOverButton = false;
            }
            else
                PrintNoSpace();

            GameManager.Instance.ClearSelectedObject();
        }
    }

    void Update()
    {
        if (gathered)
            FlyToInventory();
    }

    void FlyToInventory()
    {
        if (Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) > 3f)
            transform.position = Vector3.Lerp(transform.position, GameManager.Instance.player.transform.position, 5f * Time.deltaTime);
        else
        {

            GameManager.Instance.skillsOnGround.Remove(this);
            Destroy(gameObject);
        }
    }

    public void OnMouseEnter()
    {
        if (GameManager.Instance.objectsTurn.inParty)
        {
            GameManager.Instance.PrintActionFeedback(null, description, null, false, true);
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
        GameManager.Instance.PrintActionFeedback(null, "Inventory is full!", null, false, true);
    }
}