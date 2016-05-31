using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public InteractiveObject objectsTurn;

    public InteractiveObject SelectedObject;
    public bool mouseOverButton = false;
    public bool turnOver = false;

    public List<InteractiveObject> objectList = new List<InteractiveObject>();

    public List<InteractiveObject> party = new List<InteractiveObject>();
    public List<int> partyHealth = new List<int>();

    public List<GameObject> skillsCurrent = new List<GameObject>();

    public List<GameObject> skills_1 = new List<GameObject>();
    public List<GameObject> skills_2 = new List<GameObject>();
    public List<GameObject> skills_3 = new List<GameObject>();

    private SkillList skillList;

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }

        // Here we save our singleton instance
        Instance = this;

        // Furthermore we make sure that we don't destroy between scenes (this is optional)
        DontDestroyOnLoad(gameObject);

        skillList = GetComponent<SkillList>();

        party[0] = GameObject.FindGameObjectWithTag("Player").GetComponent<InteractiveObject>();
        objectsTurn = party[0];

        GetRandomSkills(skills_1);
        skillsCurrent = skills_1;
    }
    
    void Start()
    {
        SortObjects();
        //SetTurnObject();
    }

    void GetRandomSkills(List<GameObject> skills)
    {
        List<GameObject> tempList = skillList.allSkills;
        
        //for (int i = 0; i < 3; i ++)
        while (skills.Count < 3)
        {
            float random = Random.Range(0f, 1f);
            int randomSkill = Random.Range(0, tempList.Count);

            SkillController skill = tempList[randomSkill].GetComponent<SkillController>();

            if (skill.skillLevel == 1)
            {
                skills.Add(tempList[randomSkill]);
                tempList.RemoveAt(randomSkill);
            }
            else if(skill.skillLevel == 2 && random > 0.25)
            {
                skills.Add(tempList[randomSkill]);
                tempList.RemoveAt(randomSkill);
            }
            else if (skill.skillLevel == 3 && random > 0.5)
            {
                skills.Add(tempList[randomSkill]);
                tempList.RemoveAt(randomSkill);
            }
            else if (skill.skillLevel == 4 && random > 0.75)
            {
                skills.Add(tempList[randomSkill]);
                tempList.RemoveAt(randomSkill);
            }
            else if (skill.skillLevel == 5 && random > 0.9)
            {
                skills.Add(tempList[randomSkill]);
                tempList.RemoveAt(randomSkill);
            }

        }
    }

    void SortObjects()
    {
        objectList = objectList.OrderByDescending(w => w.speed).ToList();
        SetTurn();
    }

    public void SetSelectedObject(InteractiveObject curSelected)
    {
        SelectedObject = curSelected;

        foreach(InteractiveObject obj in objectList)
        {
            obj.ToggleSelectedFeedback();
        }
    }

    public void ClearSelectedObject()
    {
        SelectedObject = null;

        foreach (InteractiveObject obj in objectList)
        {
            obj.ToggleSelectedFeedback();
        }
    }

    public void SetTurn()
    {
        turnOver = true;
        mouseOverButton = false;
        ClearSelectedObject();
        //print(objectsTurn._name + " finished turn");
        StartCoroutine("NewTurn");
    }

    IEnumerator NewTurn()
    {
        yield return new WaitForSeconds(1f);
        turnOver = false;

        foreach (InteractiveObject obj in objectList)
        {
            if (obj == objectsTurn)
            {
                int objInt = objectList.IndexOf(obj);

                if (objInt < objectList.Count - 1)
                {
                    objectsTurn = objectList[objInt + 1];
                    break;
                }
                else
                {
                    objectsTurn = objectList[0];
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (InteractiveObject obj in objectList)
        {
            obj.ToggleTurnFeedback();
        }
    }

    /*
    void SetTurnObject()
    {
        foreach (InteractiveObject obj in objectList)
        {
            if (obj == objectsTurn)
            {
                int objInt = objectList.IndexOf(obj);

                if (objInt < objectList.Count - 1)
                {
                    objectsTurn = objectList[objInt + 1];
                    break;
                }
                    else
                {
                    objectsTurn = objectList[0];
                    break;
                }
            }
        }

        foreach (InteractiveObject obj in objectList)
        {
            obj.ToggleTurnFeedback();
        }
    } */
}