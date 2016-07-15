using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class NpcDatabase
{
    // ONEYE ////////////////////////
    public static List<GameObject> foundSkillsRelationsOneye;
    public static List<GameObject> foundWeakRelationsOneye;
    public static List<GameObject> foundImmuneRelationsOneye;

    // MRTRUNKLE ////////////////////////
    public static List<GameObject> foundSkillsRelationsMrTrunkle;
    public static List<GameObject> foundWeakRelationsMrTrunkle;
    public static List<GameObject> foundImmuneRelationsMrTrunkle;

    // DADSWIG ////////////////////////
    public static List<GameObject> foundSkillsRelationsDadsWig;
    public static List<GameObject> foundWeakRelationsDadsWig;
    public static List<GameObject> foundImmuneRelationsDadsWig;

    // BROCCOLI ////////////////////////
    public static List<GameObject> foundSkillsRelationsBroccoli;
    public static List<GameObject> foundWeakRelationsBroccoli;
    public static List<GameObject> foundImmuneRelationsBroccoli;

    public static void ClearLists() // CLEAR ALL LISTS ON START OF SESSION
    {
        // ONEYE ////////////////////////
        foundSkillsRelationsOneye = new List<GameObject>();
        foundWeakRelationsOneye = new List<GameObject>();
        foundImmuneRelationsOneye = new List<GameObject>();
        // MRTRUNKLE ////////////////////////
        foundSkillsRelationsMrTrunkle = new List<GameObject>();
        foundWeakRelationsMrTrunkle = new List<GameObject>();
        foundImmuneRelationsMrTrunkle = new List<GameObject>();
        // DADSWIG ////////////////////////
        foundSkillsRelationsDadsWig = new List<GameObject>();
        foundWeakRelationsDadsWig = new List<GameObject>();
        foundImmuneRelationsDadsWig = new List<GameObject>();
        // BROCCOLI ////////////////////////
        foundSkillsRelationsBroccoli = new List<GameObject>();
        foundWeakRelationsBroccoli = new List<GameObject>();
        foundImmuneRelationsBroccoli = new List<GameObject>();
    }

    public static List<GameObject> GetSkillRelations(InteractiveObject npc)
    {
        switch (npc._name)
        {
            case "Oneye":
                return foundSkillsRelationsOneye;
            case "Mr. Trunkle":
                return foundSkillsRelationsMrTrunkle;
            case "Dad's wig":
                return foundSkillsRelationsDadsWig;
            case "Broccoli":
                return foundSkillsRelationsBroccoli;


            default:
                return foundSkillsRelationsOneye;
        }
    }

    public static List<GameObject> GetSkillRelationsWeak(InteractiveObject npc)
    {
        switch (npc._name)
        {
            case "Oneye":
                return foundWeakRelationsOneye;
            case "Mr. Trunkle":
                return foundWeakRelationsMrTrunkle;
            case "Dad's wig":
                return foundWeakRelationsDadsWig;
            case "Broccoli":
                return foundWeakRelationsBroccoli;


            default:
                return foundWeakRelationsOneye;
        }
    }
    public static List<GameObject> GetSkillRelationsImmune(InteractiveObject npc)
    {
        switch (npc._name)
        {
            case "Oneye":
                return foundImmuneRelationsOneye;
            case "Mr. Trunkle":
                return foundImmuneRelationsMrTrunkle;
            case "Dad's wig":
                return foundImmuneRelationsDadsWig;
            case "Broccoli":
                return foundImmuneRelationsBroccoli;


            default:
                return foundImmuneRelationsOneye;
        }
    }

    public static void CheckSkillRelation(bool weak, InteractiveObject npc)
    {
        List<GameObject> foundSkillsRelations = GetSkillRelations(npc);
        List<GameObject> foundWeakRelations = GetSkillRelationsWeak(npc);
        List<GameObject> foundImmuneRelations = GetSkillRelationsImmune(npc);

        GameObject activeSkill = GameManager.Instance.activeSkill;

        if (foundSkillsRelations.Count > 0)
        {
            bool alreadyFound = false;
            foreach (GameObject skill in foundSkillsRelations)
            {
                if (activeSkill == skill)
                {
                    alreadyFound = true;
                    break;
                }
            }

            if (!alreadyFound)
            {
                foundSkillsRelations.Add(GameManager.Instance.activeSkill);

                if (weak)
                {
                    foundWeakRelations.Add(GameManager.Instance.activeSkill);
                }
                else
                {
                    foundImmuneRelations.Add(GameManager.Instance.activeSkill);
                }
            }
        }
        else
        {
            foundSkillsRelations.Add(GameManager.Instance.activeSkill);

            if (weak)
            {
                foundWeakRelations.Add(GameManager.Instance.activeSkill);
            }
            else
            {
                foundImmuneRelations.Add(GameManager.Instance.activeSkill);
            }
        }

        npc.StartRelationCoroutine();
    }
}