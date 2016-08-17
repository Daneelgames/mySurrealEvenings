
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public enum State { Night, Day };
    public State gameState = State.Night;

    public static GameManager Instance { get; private set; }
    public int roomsMinimum = 10;
    public float escapeChance = 0.75f;
    public float curSanity = 100f;

    public InteractiveObject objectsTurn;

    public GameObject activeSkill;
    public InteractiveObject selectedObject;
    public bool mouseOverButton = false;
    public bool turnOver = false;

    public InteractiveObject attackTarget;

    public List<InteractiveObject> objectList = new List<InteractiveObject>();
    public List<InteractiveObject> allyList;
    public List<InteractiveObject> enemyList;
    public List<InteractiveObject> activeTeamList;
    public bool allyTurn = true;
    public int pressTurns = 0;
    public Text pressTurnsCounter;
    public Animator pressTurnsAnim;
    public Image pressTurnsBack;

    public List<Transform> npcCells;
    public List<Transform> partyCells;

    public InteractiveObject player;
    public int partyHealth;

    public List<GameObject> skillsCurrent = new List<GameObject>();

    public List<GameObject> skills = new List<GameObject>();

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


    public Animator inventory;
    public bool inventoryActive = false;
    public InventoryController inventoryController;

    public StageRandomController stageRandomController;


    public bool choiceActive = false;
    public ChoiceController choiceController;

    int objectsTurnIndex = 0;

    public Animator skipTurnAnim;
    public Animator goFurtherAnim;
    public Animator sanityMeterAnim;

    private bool changeScene = false;

    public Image fader;
    private bool fade = false;


    public Animator cameraAnim;
    public Animator clockAnim;

    public BattleBarController battleBar;

    public SkillController skillInAction;
    public CameraHolderController camHolder;


    public SkillRelationController _skillRelationcontroller;

    public List<SkillController> skillsOnGround;

    public GameObject nightEnvironment;
    public GameObject nightBorderParticles;
    public LevelMapGenerator _levelMapGenerator;
    public LevelMovementController levelMovementController;
    public RewardWindowController rewardController;
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

            NpcDatabase.ClearLists();

            transform.FindChild("Canvas").gameObject.SetActive(true);

            GameObject.FindGameObjectWithTag("Ally").GetComponent<InteractiveObject>().inParty = true;
            player = GameObject.FindGameObjectWithTag("Ally").GetComponent<InteractiveObject>();
            player.inParty = true;

            GetRandomSkills(skills);
            skillsCurrent = skills;

            foreach (NpcController mob in stageRandomController.npcList) // generate mobs stats on start of new game
            {
                mob.objectController.GenerateDynamicStats();
            }
            player.GenerateDynamicStats();
            _levelMapGenerator.GenerateMap(roomsMinimum);
        }
    }

    void Update()
    {
        if (fade)
        {
            fader.color = Color.Lerp(fader.color, Color.black, 0.1f);
        }
        else
        {
            fader.color = Color.Lerp(fader.color, Color.clear, 0.1f);
        }
    }

    public void NewStage()
    {
        player.PlayerNight();

        nightEnvironment.SetActive(true);
        nightBorderParticles.SetActive(true);

        stageRandomController.BuildStage();

        foreach (InteractiveObject obj in objectList)
        {
            switch (obj.tag)
            {
                case "Ally":
                    allyList.Add(obj);
                    print("ally");
                    break;
                case "Enemy":
                    enemyList.Add(obj);
                    break;
            }
        }

        SortObjects();

        SetActiveTeamList(allyList);

        objectsTurn = player;
        clickToSkip.raycastTarget = false;

        foreach (InteractiveObject obj in objectList)
        {
            //print(obj.name);
            obj.ToggleTurnFeedback();
        }
        sanityMeterAnim.SetFloat("Sanity", curSanity);
        gameState = State.Night;
        CheckSkipAndGo();
    }

    void SetActiveTeamList(List<InteractiveObject> list)
    {
        activeTeamList.Clear();
        activeTeamList = new List<InteractiveObject>(list);

        UpdatePressTurns(activeTeamList.Count);
    }

    public void UpdatePressTurns(int newAmount)
    {
        if (newAmount >= 0)
            pressTurns = newAmount;
        else
            pressTurns = 0;
        pressTurnsCounter.text = "" + pressTurns;
        pressTurnsAnim.SetTrigger("Update");

        switch (allyTurn)
        {
            case true: // SET ALLY TEAM STEP TURNS
                pressTurnsBack.color = Color.green;
                break;
            case false: // SET ENEMY TEAM STEP TURNS
                pressTurnsBack.color = Color.red;
                break;
        }
    }
    void GetRandomSkills(List<GameObject> skills)
    {
        List<GameObject> tempList = new List<GameObject>(skillList.allSkills);

        while (skills.Count < 8)
        {
            float random = Random.Range(0f, 1f);
            int randomSkill = Random.Range(0, tempList.Count);

            SkillController skill = tempList[randomSkill].GetComponent<SkillController>();

            if (skill.skillLevel == 1)
            {
                skills.Add(tempList[randomSkill]);
                //tempList.RemoveAt(randomSkill);
            }
            else if (skill.skillLevel == 2 && random > 0.25)
            {
                skills.Add(tempList[randomSkill]);
                //tempList.RemoveAt(randomSkill);
            }
            else if (skill.skillLevel == 3 && random > 0.5)
            {
                skills.Add(tempList[randomSkill]);
                //tempList.RemoveAt(randomSkill);
            }
            else if (skill.skillLevel == 4 && random > 0.75)
            {
                skills.Add(tempList[randomSkill]);
                //tempList.RemoveAt(randomSkill);
            }
            else if (skill.skillLevel == 5 && random > 0.9)
            {
                skills.Add(tempList[randomSkill]);
                //tempList.RemoveAt(randomSkill);
            }
        }
    }

    void SetPartySkills()
    {
        if (objectsTurn == player)
            skillsCurrent = skills;
    }

    void SortObjects()
    {
        objectList = objectList.OrderByDescending(w => w.speed).ToList();
        allyList = allyList.OrderByDescending(w => w.speed).ToList();
        enemyList = enemyList.OrderByDescending(w => w.speed).ToList();
        activeTeamList = activeTeamList.OrderByDescending(w => w.speed).ToList();
        //SetTurn();
    }

    public void SetSelectedObject(InteractiveObject curSelected)
    {
        // SET TARGET
        selectedObject = curSelected;

        foreach (InteractiveObject obj in objectList)
        {
            if (obj != null)
                obj.ToggleSelectedFeedback();
        }

        CheckSkipAndGo();

        // UPDATE STATUS WINDOWS
        //objInfoController.ShowWindows(objectsTurn, curSelected, false);
        //InventoryClosed();
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

            if (!mouseOverButton)
                InventoryClosed();
        }
    }

    public void UnitSkipsTurn()
    {
        PrintActionFeedback(objectsTurn._name, null, null, false, false, false);
        UpdatePressTurns(pressTurns - 1);
        SetTurn();
    }

    public void FrenzyDamage(float baseFrenzyDmg)
    {
        curSanity -= baseFrenzyDmg;
        // FRENZY FEEDBACK
        sanityMeterAnim.SetFloat("Sanity", curSanity);

        // FRENZY FEEDBACK
    }

    public void RecoverSanity(float recoverAmount)
    {
        curSanity += recoverAmount;
        // FRENZY FEEDBACK
        sanityMeterAnim.SetFloat("Sanity", curSanity);

        // FRENZY FEEDBACK
    }

    public void UseSkill(GameObject skill, InteractiveObject target)
    {
        if (skill != null)
            activeSkill = skill;

        camHolder.TargetFocus(target.transform.position);

        attackTarget = target;
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

        //objInfoController.ShowWindows(objectsTurn, target, false);

        turnOver = true;
        //print(objectsTurn._name + " uses " + skill.GetComponent<SkillController>().name + " on " + target._name);
        GameObject skillInstance = Instantiate(skill.GetComponent<SkillController>().AttackParticle, target.transform.position, target.transform.rotation) as GameObject;
        skillInstance.transform.parent = target.transform;

        SkillController _sc = skill.GetComponent<SkillController>(); // SET CURRENT ACTIVE SKILL
        _sc.SetTargets(objectsTurn, target);
        skillInAction = _sc;

        bool hitself = false;
        bool offensive = false;
        if (objectsTurn == target)
            hitself = true;
        if (skill.GetComponent<SkillController>().skillType == SkillController.Type.offensive)
            offensive = true;

        PrintActionFeedback(objectsTurn._name, skill.name, target._name, hitself, offensive, false);
        SetTurn();
        int skillIndex = 0;

        for (int i = 0; i < skillsCurrent.Count; i++)
        {
            if (skill == skillsCurrent[i])
            {
                skillIndex = i;
                break;
            }
        }

        //if (objectsTurn.inParty)
        //    inventoryController.DeleteItem(skillIndex);
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
        yield return new WaitForSeconds(1.5f);
        attackTarget = null;
        canSkipTurn = true;
        clickToSkip.raycastTarget = true;

        activeSkill = null;
        SkipTurn();
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
            {
                if (objectList[i] != player)
                    objectList[i].Death();
            }
        }
        SortObjects();
        // CHECK PRESS TURNS HERE 
        // IF <=, CHANGE ACTIVE TEAM
        //        UpdatePressTurns(activeTeamList.Count);
        CheckPressTurns();

        if (objectsTurn != null)
        {
            bool foundNewObj = false;
            foreach (InteractiveObject obj in activeTeamList)
            {

                if (obj == objectsTurn)
                {
                    int objInt = activeTeamList.IndexOf(obj);

                    if (objInt < activeTeamList.Count - 1)
                    {
                        objectsTurn = activeTeamList[objInt + 1];
                        foundNewObj = true;
                        break;
                    }
                    else
                    {
                        objectsTurn = activeTeamList[0];
                        foundNewObj = true;
                        break;
                    }
                }
            }
            if (!foundNewObj)// if team hasn't objectsTurn member
            {
                if (activeTeamList.Count > 0)
                    objectsTurn = activeTeamList[0];
                else
                {
                    SetActiveTeamList(allyList);
                    objectsTurn = activeTeamList[0];
                }
            }
        }
        else
        {

            if (activeTeamList.Count > 1)
                objectsTurn = activeTeamList[objectsTurnIndex];
            else
            {
                if (activeTeamList.Count > 0)
                    objectsTurn = activeTeamList[0];
                else
                {
                    SetActiveTeamList(allyList);
                    objectsTurn = activeTeamList[0];
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        foreach (InteractiveObject obj in activeTeamList)
        {
            obj.ToggleTurnFeedback();
        }

        if (objectsTurn.inParty)
            InventoryActive();

        SetPartySkills();

        blockSkillIcons = false;

        CheckSkipAndGo();

        CheckRemainingMonsters();
    }

    void CheckPressTurns()
    {
        if (pressTurns <= 0)
        {
            SortObjects();
            switch (allyTurn)
            {
                case true:
                    SetActiveTeamList(enemyList);
                    allyTurn = false;
                    break;
                case false:
                    SetActiveTeamList(allyList);
                    allyTurn = true;
                    break;
            }
            foreach (InteractiveObject i in objectList)
            {
                i.SetGotExtraPress(false);
            }
        }
        UpdatePressTurns(pressTurns);
    }

    void CheckRemainingMonsters()
    {
        bool monsters = false;

        foreach (InteractiveObject i in objectList)
        {
            if (!i.inParty)
            {
                monsters = true;
                break;
            }
        }
        if (!monsters && !levelMovementController.activeRoom.roomCleared && !levelMovementController.activeRoom.safeRoom) // REDUCE ROOM DIFF AND SPAWN RATE IF MONSTERS ARE DEAD
        {
            levelMovementController.activeRoom.SetSpawnRate(0.33f);
            levelMovementController.activeRoom.SetRoomDiffuculty(levelMovementController.activeRoom.roomDifficulty / 2);
            levelMovementController.activeRoom.SetRoomCleared(true);
        }
        else if (levelMovementController.activeRoom.safeRoom)
        {
            levelMovementController.activeRoom.GiveReward();
        }
    }

    void CheckSkipAndGo()
    {
        if (objectsTurn.inParty && !inDialog && !inventoryActive && !choiceActive && !blockSkillIcons && gameState == State.Night && selectedObject == null) // check trulala
        {
            bool monsters = false;
            foreach (InteractiveObject character in objectList)
            {
                if (!character.inParty)
                {
                    monsters = true;
                    break;
                }
            }
            if (monsters)
            {
                skipTurnAnim.SetBool("Active", true);
                goFurtherAnim.SetBool("Active", true);

                levelMovementController.ToggleMapTraverseIcons(false);
                UpdatePressTurns(pressTurns);
                pressTurnsAnim.SetBool("Active", true);
            }
            else
            {
                skipTurnAnim.SetBool("Active", false);
                goFurtherAnim.SetBool("Active", false);

                levelMovementController.ToggleMapTraverseIcons(true);
                pressTurnsAnim.SetBool("Active", false);
            }
        }
        else
        {
            skipTurnAnim.SetBool("Active", false);
            goFurtherAnim.SetBool("Active", false);
        }
    }

    public void ChangeRoom()
    {
        StartCoroutine("LoadNight");
    }

    IEnumerator LoadNight()
    {
        changeScene = true;
        turnOver = true;

        fade = true;

        yield return new WaitForSeconds(0.75F);
        fader.color = Color.black;
        //        print("Load Night");
        yield return new WaitForSeconds(1F);
        levelMovementController.ChangeRoom();
        levelMovementController.ToggleMapTraverseIcons(false);
        //screen is black
        allyList.Clear();
        enemyList.Clear();

        NewStage(); // generate new stage

        fade = false;
        yield return new WaitForSeconds(0.75F);
        CheckRemainingMonsters();
        fader.color = Color.clear;

        changeScene = false;
        turnOver = false;
    }

    public void DialogStart(InteractiveObject speaker)
    {
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

        if (selectedObject.npcControl != null)
        {
            if (selectedObject.activeDialog == 0) // CHECK FOR TRADE
            {
                ChoiceActive(selectedObject, false, false, true, false);
                specialDialog = true;
            }
            else if (selectedObject.activeDialog == 1) // CHECK FOR REPEl
            {
                ChoiceActive(selectedObject, false, false, false, true);
                specialDialog = true;
            }
            else if (selectedObject.activeDialog == 6) // refused to trade, agry
            {
                specialDialog = true;

                actionTextFeedback.text = objectsTurn._name + " doing nothing. " + selectedObject._name + " is angry!";
                actionTextFeedbackAnimator.SetBool("Active", true);
                actionTextFeedbackAnimator.SetBool("Update", true);
                StartCoroutine("AnimatorSetUpdateFalse");

                objInfoController.HideWindows();

                mouseOverButton = false;
                objInfoController.HideDialogBackground();

                SetTurn();
            }
            else if (selectedObject.activeDialog == 7) // mob is happy
            {
                specialDialog = true;

                actionTextFeedback.text = selectedObject._name + " walks away.";
                actionTextFeedbackAnimator.SetBool("Active", true);
                actionTextFeedbackAnimator.SetBool("Update", true);
                StartCoroutine("AnimatorSetUpdateFalse");

                objInfoController.HideWindows();

                mouseOverButton = false;
                objInfoController.HideDialogBackground();

                selectedObject.WalkAway();
                SetTurn();
            }
        }

        if (!specialDialog) //Clear selection if no repel or trade
        {
            ClearSelectedObject();
            CheckSkipAndGo();
            InventoryActive();
            HideTextManually();
        }


        objInfoController.HideWindows();

        mouseOverButton = false;
        objInfoController.HideDialogBackground();
    }

    void ChoiceActive(InteractiveObject npc, bool sleepNearEnemy, bool outOfPills, bool trade, bool repel)
    {
        //InventoryClosed();

        ClearSelectedObject();

        inventoryController.SetResourcesFeedback();
        inventory.SetBool("Active", true);
        inventoryActive = true;
        inventoryController.SortSlots();

        choiceController.ShowWindow(npc, sleepNearEnemy, outOfPills, trade, repel);
        goFurtherAnim.SetBool("Active", false);
        skipTurnAnim.SetBool("Active", false);
        choiceActive = true;
    }

    public void ChoiceInactive(ChoiceController.ChoiceType choice, bool yes, InteractiveObject choiceNpc)
    {
        choiceActive = false;
        InventoryToggle();

        ClearSelectedObject();

        CheckSkipAndGo();
    }

    public void InventoryToggle()
    {
        if (objectsTurn.inParty && !choiceActive)
        {
            if (!inventoryActive)
            {
                inventoryController.SetResourcesFeedback();
                inventory.SetBool("Active", true);
                inventoryActive = true;
                inventoryController.SortSlots();
            }
            else if (inventoryActive)
            {
                inventory.SetBool("Active", false);
                inventoryActive = false;
            }
            if (selectedObject != null)
            {
                ClearSelectedObject();
            }
        }
    }

    public void InventoryClosed()
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
        if (objectsTurn == player)
        {
            inventory.SetBool("Inactive", false);
        }
    }

    public void MouseEnterButton()
    {
        mouseOverButton = true;
    }

    public void MouseExitButton()
    {
        mouseOverButton = false;
    }

    public void CameraShake(float waitTime)
    {
        StartCoroutine("ShakeCam", waitTime);
    }

    IEnumerator ShakeCam(float waitTime)
    {
        cameraAnim.SetBool("Shake", true);
        yield return new WaitForSeconds(waitTime);
        cameraAnim.SetBool("Shake", false);
    }

    public void SkillRelationDiscoverFeedback(GameObject skillFound, InteractiveObject npcRelative, bool weak)
    {
        string skillName = skillFound.GetComponent<SkillController>().skillName;
        string relationText = "";
        if (weak)
        {
            relationText = npcRelative._name + " is weak against " + skillName + "!";
            if (!objectsTurn.gotExtraPress)
            {
                UpdatePressTurns(pressTurns);
                objectsTurn.SetGotExtraPress(true);
            }
            else
                UpdatePressTurns(pressTurns - 1); // character loses pressTurn if he already got it

        }
        else
        {
            relationText = npcRelative._name + " is immune to " + skillName + "!";
            UpdatePressTurns(pressTurns - 1);
        }
        _skillRelationcontroller.SetFeedback(relationText);
        CheckPressTurns();
    }

    public void AddEscapeChance(float amount)
    {
        escapeChance += amount;

        if (escapeChance > 0.75)
            escapeChance = 0.75f;
        else if (escapeChance < 0.25)
            escapeChance = 0.25f;
    }

    public void EscapeFromBattle()
    {
        float random = Random.value;

        if (escapeChance >= random)
        {
            AddEscapeChance(-0.25f);

            levelMovementController.RunFromBattle();
        }
        else // escape failed
        {
            UpdatePressTurns(0);
            actionTextFeedback.text = "Escape failed!";
            actionTextFeedbackAnimator.SetBool("Active", true);
            actionTextFeedbackAnimator.SetBool("Update", true);
            StartCoroutine("AnimatorSetUpdateFalse");
            SetTurn();
        }
    }

    public void HitWall(bool breakWall)
    {
        if (!breakWall)
        {
            _skillRelationcontroller.SetFeedback("You hit the wall and got damaged!");
            player.health -= player.maxHealth / 10;
        }
        else
        {
            _skillRelationcontroller.SetFeedback("You found a room behind a false wall!");
        }
    }
    public void ShowRewardWindow(int moneyDrop, bool keyDrop, GameObject treasure)
    {
        rewardController.SetReward(moneyDrop, keyDrop, treasure);
        rewardController.ToggleWindow(true);
    }
}