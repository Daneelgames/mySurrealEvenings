using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour {

    public enum Target {none, everyone, enemies}

    public Target agressiveTo = Target.none;

    [SerializeField]
    InteractiveObject objectController;

    public List<GameObject> skills = new List<GameObject>();

    public int levelPreffered = 1;
    
    public void ChooseAction()
    {
        if (agressiveTo == Target.none)
        {
            Action(-1);
        }
        else
        {
            int chosenSkill = -1;

            int randomSkill = Random.Range(0, 3);

            float randomChance = Random.Range(0f, 1f);

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

            Action(chosenSkill);
        }
    }

    void Action(int actionNumber)
    {
        if (actionNumber < 0)
        {
            // NPC IS LAZY
            //print(objectController._name + " is lazy.");
            GameManager.Instance.PrintActionFeedback(objectController._name, null, null, false, false);
            GameManager.Instance.SetTurn();
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
                        int randomParty = Random.Range(0, GameManager.Instance.party.Count);

                        GameManager.Instance.UseSkill(skills[actionNumber], GameManager.Instance.party[randomParty]);
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
            }
            else // DEFFENSIVE SKILL
            {
                if (agressiveTo == Target.everyone)
                {
                    //target recover more to NPCs
                    if (randomChance < 0.2f)
                    {
                        int randomParty = Random.Range(0, GameManager.Instance.party.Count);

                        GameManager.Instance.UseSkill(skills[actionNumber], GameManager.Instance.party[randomParty]);
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
            }
        }
    }
}