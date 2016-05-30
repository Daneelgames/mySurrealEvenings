using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public InteractiveObject SelectedObject;
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

        GetRandomSkills(skills_1);
        skillsCurrent = skills_1;
    }
    
    void Start()
    {
        SortObjects();
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
    }

    public void SetSelectedObject(InteractiveObject curSelected)
    {
        SelectedObject = curSelected;

        foreach(InteractiveObject obj in objectList)
        {
            obj.ToggleSelectedFeedback();
        }
    }
}