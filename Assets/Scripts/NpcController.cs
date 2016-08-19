using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{

    public enum Target { none, everyone, enemies, self }

    public float overallDifficulty = 1f;

    public Target agressiveTo = Target.none;

    public InteractiveObject objectController;

    public int skillsAmount = 3;
    public List<GameObject> skillsStaticInclude;
    public List<GameObject> skillsStaticAvoid;

    public List<GameObject> skills = new List<GameObject>(); //dynamic skill list

    public int levelPreffered = 1;


    public void GenerateNpcSkills()
    {
        List<GameObject> allSkills = new List<GameObject>(GameManager.Instance.skillList.allSkills);
        skills.Clear();

        foreach (GameObject skill in skillsStaticInclude)   //remove included skills
        {
            skills.Add(skill);
            allSkills.Remove(skill);
        }
        foreach (GameObject skill in skillsStaticAvoid) //remove unincluded skills
        {
            allSkills.Remove(skill);
        }

        for (int i = 0; i < skillsAmount - skillsStaticInclude.Count; i++) //fill empty slots with random skills
        {
            if (allSkills.Count > 0)
            {
                int random = Random.Range(0, allSkills.Count);
                skills.Add(allSkills[random]);
                allSkills.RemoveAt(random);
            }
        }
    }

    void Start()
    {
        int randomTarget = Random.Range(0, 4);
        switch (randomTarget)
        {
            case 0:
                agressiveTo = Target.none;
                break;
            case 1:
                agressiveTo = Target.everyone;
                break;
            case 2:
                agressiveTo = Target.enemies;
                break;
            case 3:
                agressiveTo = Target.self;
                break;
            default:
                agressiveTo = Target.everyone;
                break;
        }
    }

    public void RemoveSkill(SkillController skillToRemove)
    {
        foreach (GameObject sk in skills)
        {
            if (sk.GetComponent<SkillController>().skillName == skillToRemove.skillName)
            {
                skills.Remove(sk);
                break;
            }
        }
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
    }
}