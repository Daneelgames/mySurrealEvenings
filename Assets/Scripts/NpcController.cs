﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{

    public enum Target { none, everyone, enemies, self }

    public float overallDifficulty = 1f;

    public Target agressiveTo = Target.none;

    public InteractiveObject objectController;

    public List<GameObject> skills = new List<GameObject>();

    public int levelPreffered = 1;

    public int candyDrop = 3;
    public int pillDrop = 2;

    public GameObject dropFeedback;

    void Start()
    {
        if (agressiveTo == Target.everyone)
            SetAgressiveFeedback();
    }

    public void ChooseAction()
    {
        if (agressiveTo == Target.none)
        {
            Action(-1);
        }
        else
        {
            int chosenSkill = -1;

            int randomSkill = Random.Range(0, skills.Count);

            float randomChance = Random.Range(0f, 1f);

            if (skills.Count > 0)
            {
                switch (Mathf.Abs(skills[randomSkill].GetComponent<SkillController>().skillLevel - levelPreffered))
                {
                    case 0:
                        chosenSkill = randomSkill;
                        break;

                    case 1:
                        if (randomChance > 0.25)
                            chosenSkill = randomSkill;
                        break;

                    case 2:
                        if (randomChance > 0.5)
                            chosenSkill = randomSkill;
                        break;

                    case 3:
                        if (randomChance > 0.75)
                            chosenSkill = randomSkill;

                        break;

                    case 4:
                        if (randomChance > 0.9)
                            chosenSkill = randomSkill;
                        break;

                    default:
                        chosenSkill = -1;
                        break;
                }
            }
            else
                chosenSkill = -1;

            //check other enemies. if no other enemies found, become calm
            if (agressiveTo == Target.enemies)
            {
                bool alone = true;
                foreach (InteractiveObject obj in GameManager.Instance.objectList)
                {
                    if (!obj.inParty && obj != objectController)
                    {
                        alone = false;
                        break;
                    }
                }
                if (alone)
                {
                    agressiveTo = Target.none;
                    chosenSkill = -1;
                }
            }
            StartCoroutine("SetAction", chosenSkill);
        }
    }

    IEnumerator SetAction(int chosenSkill)
    {
        yield return new WaitForSeconds(0.3F);
        Action(chosenSkill);
    }

    void Action(int actionNumber)
    {
        if (actionNumber < 0) // NPC IS LAZY
        {
            GameManager.Instance.UnitSkipsTurn();
        }
        else
        {
            SkillController skill = skills[actionNumber].GetComponent<SkillController>();

            float randomChance = Random.Range(0f, 1f);

            if (skill.skillType == SkillController.Type.offensive) //OFFENSIVE SKILL
            {
                if (agressiveTo == Target.everyone)
                {
                    // target offensive to all
                    if (randomChance > 0.2f)
                    {
                        GameManager.Instance.UseSkill(skills[actionNumber], GameManager.Instance.player);
                    }
                    else
                    {
                        InteractiveObject obj = null;

                        while (obj == null)
                        {
                            int randomObject = Random.Range(0, GameManager.Instance.objectList.Count);

                            if (!GameManager.Instance.objectList[randomObject].inParty)
                            {
                                obj = GameManager.Instance.objectList[randomObject];
                                break;
                            }
                        }

                        GameManager.Instance.UseSkill(skills[actionNumber], obj);
                    }
                }
                else if (agressiveTo == Target.enemies)
                {
                    // target offensive to only enemies
                    InteractiveObject obj = null;

                    while (obj == null)
                    {
                        int randomObject = Random.Range(0, GameManager.Instance.objectList.Count);

                        if (!GameManager.Instance.objectList[randomObject].inParty)
                        {
                            obj = GameManager.Instance.objectList[randomObject];
                            break;
                        }
                    }

                    GameManager.Instance.UseSkill(skills[actionNumber], obj);
                }
                else if (agressiveTo == Target.self)
                {
                    GameManager.Instance.UseSkill(skills[actionNumber], this.objectController);
                }
            }
            else // DEFFENSIVE/RECOVER SKILL
            {
                if (agressiveTo == Target.everyone)
                {
                    //target recover more to NPCs
                    if (randomChance < 0.2f)
                    {
                        GameManager.Instance.UseSkill(skills[actionNumber], GameManager.Instance.player);
                    }
                    else
                    {
                        InteractiveObject obj = null;

                        while (obj == null)
                        {
                            int randomObject = Random.Range(0, GameManager.Instance.objectList.Count);

                            if (!GameManager.Instance.objectList[randomObject].inParty)
                            {
                                obj = GameManager.Instance.objectList[randomObject];
                                break;
                            }
                        }

                        objectController._anim.SetTrigger("Action");
                        GameManager.Instance.UseSkill(skills[actionNumber], obj);
                    }
                }
                else if (agressiveTo == Target.enemies)
                {
                    // target recover to all equally
                    InteractiveObject obj = null;

                    int randomObject = Random.Range(0, GameManager.Instance.objectList.Count);
                    obj = GameManager.Instance.objectList[randomObject];

                    objectController._anim.SetTrigger("Action");
                    GameManager.Instance.UseSkill(skills[actionNumber], obj);
                }
                else if (agressiveTo == Target.self)
                {
                    GameManager.Instance.UseSkill(skills[actionNumber], this.objectController);
                }
            }
        }
    }

    public void SetAgressiveFeedback()
    {
        objectController.localCanvas.AggressiveStart();
    }

    public void RemoveAggressiveFeedback()
    {

        objectController.localCanvas.AggressiveOver();
    }

    public void DropOnDead()
    {
        RemoveAggressiveFeedback();

        float candyChance = Random.Range(0f, 1f);
        float trashChance = Random.Range(0f, 1f);
        float skillChance = Random.Range(0f, 1f);

        int candyValue = candyDrop + Mathf.RoundToInt(Random.Range(-candyDrop / 2, candyDrop / 2));
        int trashValue = pillDrop + Mathf.RoundToInt(Random.Range(-pillDrop / 2, pillDrop / 2));

        if (candyChance > 0.25f) // DROP RANDOM
        {
            GameManager.Instance.inventoryController.CandyGet(candyValue);
        }
        if (trashChance > 0.25f) // DROP RANDOM
        {
            GameManager.Instance.inventoryController.TrashGet(trashValue);
        }
        if (skillChance > 0.5f)
        {
            if (skills.Count > 0)
            {
                GameObject skillDrop = skills[Random.Range(0, skills.Count)];
                Instantiate(skillDrop, transform.position, transform.rotation);
            }
        }

        GameObject newDropFeedback = Instantiate(dropFeedback.gameObject, transform.position, Quaternion.identity) as GameObject;
        newDropFeedback.GetComponent<NpcDropFeedbackController>().SetValues(candyValue, trashValue);
    }
}