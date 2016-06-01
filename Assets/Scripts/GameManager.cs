using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField]
    private Text actionTextFeedback;
    [SerializeField]
    private Animator actionTextFeedbackAnimator;
    [SerializeField]
    private RawImage clickToSkip;

    private bool canSkipTurn = false;
    public bool blockSkillIcons = false;

    public ObjectsInfoController objInfoController;


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

        party[0] = GameObject.FindGameObjectWithTag("Ally").GetComponent<InteractiveObject>();
        party[0].inParty = true;

        GetRandomSkills(skills_1);
        skillsCurrent = skills_1;
    }

    void Start()
    {
        StartStage();
    }

    void StartStage()
    {
        SortObjects();

        objectsTurn = party[0];
        clickToSkip.raycastTarget = false;
        
        foreach (InteractiveObject obj in objectList)
        {
            print(obj.name);
            obj.ToggleTurnFeedback();
        }
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
        //SetTurn();
    }

    public void SetSelectedObject(InteractiveObject curSelected)
    {
        // SET TARGET
        SelectedObject = curSelected;

        foreach(InteractiveObject obj in objectList)
        {
            obj.ToggleSelectedFeedback();
        }

        // UPDATE STATUS WINDOWS
        objInfoController.ShowWindows(objectsTurn, curSelected);
    }

    public void ClearSelectedObject()
    {
        SelectedObject = null;

        foreach (InteractiveObject obj in objectList)
        {
            obj.ToggleSelectedFeedback();
        }
        if (!turnOver)
            objInfoController.HideWindows();
    }

    public void UseSkill(GameObject skill, InteractiveObject target)
    {
        // update status windows if it's not the players turn
        if (!objectsTurn.inParty)
        {
            objInfoController.ShowWindows(objectsTurn, target);
        }

        turnOver = true;
        //print(objectsTurn._name + " uses " + skill.GetComponent<SkillController>().name + " on " + target._name);
        GameObject skillInstance = Instantiate(skill, target.transform.position, target.transform.rotation) as GameObject;
        skillInstance.transform.parent = target.transform;
        skillInstance.GetComponent<SkillController>().SetTargets(objectsTurn, target);
        bool hitself = false;
        bool offensive = false;
        if (objectsTurn == target)
            hitself = true;
        if (skill.GetComponent<SkillController>().skillType == SkillController.Type.offensive)
            offensive = true;

        PrintActionFeedback(objectsTurn._name, skill.name, target._name, hitself, offensive, false);
        SetTurn();
    }

    public void PrintActionFeedback(string caster, string skill, string target, bool hitSelf, bool offensive, bool iconDescription)
    {
        string generatedString;

        if (!iconDescription)
        {
            if (skill != null)
            {
                if (!hitSelf)
                {
                    if (offensive)
                    { // CASTER USES OFFENSIVE SKILL ON OTHER
                        generatedString = caster + " uses " + skill + " against " + target + ". " + caster + " thinks it was clever. Is it?";
                    }
                    else
                    { // CASTER USES NON-OFFENSIVE SKILL ON OTHER
                        generatedString = caster + " uses " + skill + " on " + target + ".";
                    }

                }
                else
                {
                    if (offensive)
                    { // CASTER USES OFFENSIVE SKILL ON SELF
                        generatedString = caster + " uses " + skill + " against self. Why " + caster + " doing this???";
                    }
                    else
                    { // CASTER USES NON-OFFENSIVE SKILL ON SELF
                        generatedString = caster + " uses " + skill + " on self. Smart move, " + caster + "!";
                    }
                }
            }
            else // NPC IS LAZY, SKIP HIS MOVE
            {
                generatedString = caster + " is doing nothing!";
            }
        }
        else // THIS ONE PRINTS BUTTON DESCRIPTION
            generatedString = skill;

        actionTextFeedback.text = generatedString;
        actionTextFeedbackAnimator.SetTrigger("UpdateText");
    }

    public void HideTextManually()
    {
        StartCoroutine("HideTextIfMouseExit");
    }

    IEnumerator HideTextIfMouseExit()
    {
        yield return new WaitForSeconds(0.1f);
        if (!mouseOverButton)
            actionTextFeedbackAnimator.SetTrigger("HideText");
    }

    public void SetTurn()
    {
        blockSkillIcons = true;
        mouseOverButton = false;
        ClearSelectedObject();
        //print(objectsTurn._name + " finished turn");
        StartCoroutine("TurnCooldown");
    }


    IEnumerator TurnCooldown()
    {
        turnOver = false;
        yield return new WaitForSeconds(0.5f);
        canSkipTurn = true;
        clickToSkip.raycastTarget = true;
    }

    public void SkipTurn()
    {
        actionTextFeedbackAnimator.SetTrigger("HideText");
        StartCoroutine("NewTurn");
        clickToSkip.raycastTarget = false;
        canSkipTurn = false;
    }

    IEnumerator NewTurn()
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

        yield return new WaitForSeconds(0.1f);

        foreach (InteractiveObject obj in objectList)
        {
            obj.ToggleTurnFeedback();
        }
        
        blockSkillIcons = false;
    }
}