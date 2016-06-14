using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public float curSanity = 100f;

    public InteractiveObject objectsTurn;

    public InteractiveObject selectedObject;
    public bool mouseOverButton = false;
    public bool turnOver = false;

    public List<InteractiveObject> objectList = new List<InteractiveObject>();

    public List<Transform> npcCells;
    public List<Transform> partyCells;

    public List<InteractiveObject> party = new List<InteractiveObject>();
    public List<int> partyHealth = new List<int>();

    public List<GameObject> skillsCurrent = new List<GameObject>();

    public List<GameObject> skills_1 = new List<GameObject>();
    public List<GameObject> skills_2 = new List<GameObject>();
    public List<GameObject> skills_3 = new List<GameObject>();

    public SkillList skillList;

    [SerializeField]
    private Text actionTextFeedback;
    [SerializeField]
    private Animator actionTextFeedbackAnimator;
    [SerializeField]
    private RawImage clickToSkip;

    public bool inDialog = false;
    public bool canSkipTurn = false;
    public bool blockSkillIcons = false;
    public bool canSkipDialog = false;

    public ObjectsInfoController objInfoController;

    public Animator tradeWindow;
    public bool tradeActive = false;

    public Animator inventory;
    public bool inventoryActive = false;
    public InventoryController inventoryController;

    public TradeWindowController tradeController;
    public StageRandomController stageRandomController;

    private NpcController curTrader = null;

    public bool choiceActive = false;
    public ChoiceController choiceController;

    int objectsTurnIndex = 0;

    public Animator skipTurnAnim;
    public Animator goFurtherAnim;
    public Animator sanityMeterAnim;

    private bool changeScene = false;

    void Awake()
    {
        // First we check if there are any other instances conflicting
        if (Instance != null && Instance != this)
        {
            // If that is the case, we destroy other instances
            Destroy(gameObject);
        }
        else
        {
            // Here we save our singleton instance
            Instance = this;

            // Furthermore we make sure that we don't destroy between scenes (this is optional)
            DontDestroyOnLoad(gameObject);

            transform.FindChild("Canvas").gameObject.SetActive(true);

            GameObject.FindGameObjectWithTag("Ally").GetComponent<InteractiveObject>().inParty = true;
            party[0] = GameObject.FindGameObjectWithTag("Ally").GetComponent<InteractiveObject>();
            party[0].inParty = true;

            GetRandomSkills(skills_1);
            skillsCurrent = skills_1;
        }
    }

    void Start()
    {
        NewStage();
    }

    void NewStage()
    {
        print("Build stage");
        stageRandomController.BuildStage();

        SortObjects();

        objectsTurn = party[0];
        clickToSkip.raycastTarget = false;
        
        foreach (InteractiveObject obj in objectList)
        {
            //print(obj.name);
            obj.ToggleTurnFeedback();
        }
        CheckSkipAndGo();
    }

    void GetRandomSkills(List<GameObject> skills)
    {
        List<GameObject> tempList = new List<GameObject>(skillList.allSkills);
        
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

    void SetPartySkills()
    {
        if (party.Count > 1)
            skills_2 = new List<GameObject>(party[1].npcControl.skills);
        if (party.Count > 2)
            skills_3 = new List<GameObject>(party[2].npcControl.skills);

        if (objectsTurn == party[0])
            skillsCurrent = skills_1;
        else if (party.Count > 1 && objectsTurn == party[1])
            skillsCurrent = skills_2;
        else if (party.Count > 2 && objectsTurn == party[2])
            skillsCurrent = skills_3;
    }

    void SortObjects()
    {
        objectList = objectList.OrderByDescending(w => w.speed).ToList();
        //SetTurn();
    }

    public void SetSelectedObject(InteractiveObject curSelected)
    {
        // SET TARGET
        selectedObject = curSelected;

        foreach(InteractiveObject obj in objectList)
        {
            if (obj != null)
                obj.ToggleSelectedFeedback();
        }

        // UPDATE STATUS WINDOWS
        objInfoController.ShowWindows(objectsTurn, curSelected, false);
        InventoryClosed();
    }

    public void ClearSelectedObject()
    {
        if (!inDialog)
        {
            selectedObject = null;

            foreach (InteractiveObject obj in objectList)
            {
                if (obj != null)
                    obj.ToggleSelectedFeedback();
            }
            if (!turnOver)
                objInfoController.HideWindows();

            if (!mouseOverButton && !tradeActive)
                InventoryClosed();
        }
    }

    public void UnitSkipsTurn()
    {
        PrintActionFeedback(objectsTurn._name, null, null, false, false, false);
        SetTurn();
    }

    public void FrenzyDamage(float baseFrenzyDmg)
    {
        curSanity -= baseFrenzyDmg;
        // FRENZY FEEDBACK
        sanityMeterAnim.SetFloat("Sanity", curSanity);

        // FRENZY FEEDBACK
    }

    public void UseSkill(GameObject skill, InteractiveObject target)
    {

        skipTurnAnim.SetBool("Active", false);
        goFurtherAnim.SetBool("Active", false);

        foreach (InteractiveObject npc in objectList)
        {
            if (npc == objectsTurn)
            {
                objectsTurnIndex = objectList.IndexOf(npc);
                break;
            }
        }

        objInfoController.ShowWindows(objectsTurn, target, false);

        turnOver = true;
        //print(objectsTurn._name + " uses " + skill.GetComponent<SkillController>().name + " on " + target._name);
        GameObject skillInstance = Instantiate(skill.GetComponent<SkillController>().AttackParticle, target.transform.position, target.transform.rotation) as GameObject;
        skillInstance.transform.parent = target.transform;
        skill.GetComponent<SkillController>().SetTargets(objectsTurn, target);
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
                        generatedString = caster + " uses " + skill + " against " + target + ". " + caster + " thinks it was clever.";
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
                        generatedString = caster + " uses " + skill + " against self for some reason.";
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
        actionTextFeedbackAnimator.SetBool("Active", true);
        actionTextFeedbackAnimator.SetBool("Update", true);
        StartCoroutine("AnimatorSetUpdateFalse");
    }

    IEnumerator AnimatorSetUpdateFalse()
    {
        yield return new WaitForSeconds(0.1F);
        actionTextFeedbackAnimator.SetBool("Update", false);
    }

    public void HideTextManually()
    {
        StartCoroutine("HideTextIfMouseExit");
    }

    IEnumerator HideTextIfMouseExit()
    {
        yield return new WaitForSeconds(0.1f);
        if (!mouseOverButton)
            actionTextFeedbackAnimator.SetBool("Active", false);
    }

    public void SetTurn()
    {
        blockSkillIcons = true;
        mouseOverButton = false;
        ClearSelectedObject();
        //print(objectsTurn._name + " finished turn");
        InventoryInactive();
        StartCoroutine("TurnCooldown");
    }


    IEnumerator TurnCooldown()
    {
        turnOver = false;
        yield return new WaitForSeconds(0.3f);
        canSkipTurn = true;
        clickToSkip.raycastTarget = true;
    }

    public void SkipTurn()
    {
        if (canSkipTurn)
        {
            objInfoController.HideWindows();
            SortObjects(); ////////////////////////////////////////////////////////////
            actionTextFeedbackAnimator.SetBool("Active", false);
            StartCoroutine("NewTurn");
            clickToSkip.raycastTarget = false;
            canSkipTurn = false;
        }
        else if (canSkipDialog)
        {
            DialogUpdate();
            canSkipDialog = false;
        }
    }

    IEnumerator NewTurn()
    {

        for (int i = objectList.Count - 1; i >= 0; i--)
        {
            if (objectList[i].health <= 0)
                objectList[i].Death();
        }

        if (objectsTurn != null)
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
        }
        else
        {

            if (objectList.Count > 1)
                objectsTurn = objectList[objectsTurnIndex];
            else
                objectsTurn = objectList[0];
        }

        yield return new WaitForSeconds(0.1f);

        foreach (InteractiveObject obj in objectList)
        {
            obj.ToggleTurnFeedback();
        }

        if (objectsTurn.inParty)
            InventoryActive();

        SetPartySkills();

        blockSkillIcons = false;

        CheckSkipAndGo();
    }

    void CheckSkipAndGo()
    {
        if (objectsTurn.inParty && !inDialog && !tradeActive && !inventoryActive && !choiceActive && !blockSkillIcons)
        {
            skipTurnAnim.SetBool("Active", true);
            goFurtherAnim.SetBool("Active", true);
        }
        else
        {
            skipTurnAnim.SetBool("Active", false);
            goFurtherAnim.SetBool("Active", false);
        }
    }

    public void LeaveLevel(int enemyAmount)
    {
        if (!changeScene)
        {
            if (enemyAmount > 0)
            {
                ChoiceActive(null, true);
            }
            else
            {
                StartCoroutine("LoadScene");
            }
        }
    }

    IEnumerator LoadScene ()
    {
        changeScene = true;
        turnOver = true;
        yield return new WaitForSeconds(0.5F);
        print("Load Day");
        changeScene = false;
        turnOver = false;
    }

    public void DialogStart(InteractiveObject speaker)
    {
        if (selectedObject.npcControl != null)
            curTrader = selectedObject.npcControl;

        inDialog = true;
        mouseOverButton = false;
        HideTextManually();
        selectedObject = speaker;
        DialogSetText();
        InventoryInactive();
        clickToSkip.raycastTarget = true;
        StartCoroutine("DialogCooldown");

        CheckSkipAndGo();
    }

    void DialogUpdate()
    {
        if (selectedObject.activePhrase < selectedObject.dialogues[selectedObject.activeDialog].stringList.Count - 1)
        {
            selectedObject.activePhrase += 1;
            DialogSetText();
            StartCoroutine("DialogCooldown");
        }
        else
        {
            DialogOver();
        }
    }

    IEnumerator DialogCooldown()
    {
        yield return new WaitForSeconds(0.3F);
        canSkipDialog = true;
    }

    void DialogSetText()
    {
        objInfoController.ShowWindows(objectsTurn, selectedObject, true);
    }

    public void DialogOver()
    {
        clickToSkip.raycastTarget = false;

        inDialog = false;

        bool specialDialog = false;

        if (selectedObject.npcControl != null && selectedObject.npcControl.agressiveTo != NpcController.Target.everyone)
        {
            if (selectedObject.actionOnDialog == InteractiveObject.DialogAction.trade)
            {
                TradeActive(); // OPEN SHOP
                specialDialog = true;
            }
            else if (selectedObject.activeDialog == 1) // CHECK FOR TEAM UP
            {
                ChoiceActive(selectedObject, false);
                specialDialog = true;
            }
        }

        else if (selectedObject.npcControl != null && selectedObject.npcControl.agressiveTo == NpcController.Target.everyone)
        {
            if (selectedObject.activeDialog == 5 && selectedObject.npcControl.skills.Count < 5) // CHECK FOR CALM
            {
                ChoiceActive(selectedObject, false);
                specialDialog = true;
            }
        }

        if (!specialDialog) //Clear selection if no choise or trade
        {
            ClearSelectedObject();
            CheckSkipAndGo();
        }

        InventoryActive();

        objInfoController.HideWindows();

        mouseOverButton = false;
        HideTextManually();
        objInfoController.HideDialogBackground();
    }

    void ChoiceActive(InteractiveObject npc, bool sleepNearEnemy)
    {
        if (!sleepNearEnemy)
            InventoryToggle();

        choiceController.ShowWindow(npc, sleepNearEnemy);
        choiceActive = true;
    }

    public void ChoiceInactive(bool sleep)
    {
        choiceActive = false;
        InventoryToggle();

        ClearSelectedObject();

        CheckSkipAndGo();

        if (sleep)
        {
            StartCoroutine("LoadScene");
        }
    }

    void TradeActive()
    {
        InventoryToggle();
        tradeWindow.SetBool("Active", true);
        tradeActive = true;
        tradeController.npc = curTrader;
        tradeController.OpenTradeWindow();
    }

    public void TradeOver()
    {
        mouseOverButton = false;
        HideTextManually();
        tradeActive = false;
        InventoryToggle();
        StartCoroutine("SetTradeInactive");

        CheckSkipAndGo();
    }

    IEnumerator SetTradeInactive()
    {
        tradeWindow.SetBool("Active", false);
        yield return new WaitForSeconds(0.2f);
        mouseOverButton = false;
        objInfoController.HideWindows();
    }

    public void InventoryToggle()
    {
        if (objectsTurn.inParty && !tradeActive && !choiceActive)
        {
            if (!inventoryActive)
            {
                inventoryController.SetMoneyFeedback();
                inventory.SetBool("Active", true);
                inventoryActive = true;
                inventoryController.SortSlots();
            }
            else if (inventoryActive)
            {
                inventory.SetBool("Active", false);
                inventoryActive = false;
            }

            //CheckSkipAndGo();
        }
    }

    void InventoryClosed()
    {
        inventory.SetBool("Active", false);
        inventoryActive = false;

        CheckSkipAndGo();
    }
    
    void InventoryInactive()
    {
        inventory.SetBool("Inactive", true);
        inventoryActive = false;

        CheckSkipAndGo();
    }

    void InventoryActive()
    {
        if (objectsTurn == party[0])
        {
            inventory.SetBool("Inactive", false);
        }
    }

    public void MouseEnterButton()
    {
        if (!tradeActive)
            mouseOverButton = true;
    }

    public void MouseExitButton()
    {
        if (!tradeActive)
            mouseOverButton = false;
    }
}