using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour
{
    public string _name = "Npc";
    public Sprite facepic;
    public float speed = 1;

    public float health = 1;     // dynamic
    public float curMaxHealth = 1;     // dynamic

    public float maxHealth = 1;     // static
    public float minHealth = 1;     // static
    public int mana = 0;
    public int maxMana = 0;

    public bool gotExtraPress = false;

    public InteractiveObject defendAgainst;

    public List<SkillController.Type> weakToStatic;
    public List<SkillController.Type> invToStatic;
    public List<SkillController.Type> weakToDynamic;
    public List<SkillController.Type> invToDynamic;

    public bool inParty = false;
    public ActiveObjectCanvasController localCanvas;
    Animator turnFeedbackAnim;
    public NpcController npcControl;
    public Animator _anim;
    public List<ListWrapper> dialogues = new List<ListWrapper>();
    public int activeDialog = 0;
    public int activePhrase = 0;
    Image healthbar;
    Image manaBar;
    public GameObject deathParticles;
    bool sendWeak = false;

    public GameObject canvasToInstance;

    [System.Serializable]
    public class ListWrapper
    {
        public List<string> stringList;
    }

    public void SetGotExtraPress(bool got)
    {
        gotExtraPress = got;
    }

    public GameObject simpleAttack;

    public void Awake()
    {
        GameObject _canvas = Instantiate(canvasToInstance, transform.position, Quaternion.identity) as GameObject;
        _canvas.name = "ActiveObjectCanvas";
        _canvas.transform.SetParent(transform);

        localCanvas = _canvas.GetComponent<ActiveObjectCanvasController>();
        healthbar = localCanvas.healthbar;
        manaBar = localCanvas.manaBar;

        localCanvas.skillButton_0.onClick.AddListener(delegate { UseSkill(0); });
        localCanvas.skillButton_1.onClick.AddListener(delegate { UseSkill(1); });
        localCanvas.skillButton_2.onClick.AddListener(delegate { UseSkill(2); });
        localCanvas.skillButton_3.onClick.AddListener(delegate { UseSkill(3); });

        localCanvas.defendButton.onClick.AddListener(delegate { Defend(); });
        localCanvas.attackButton.onClick.AddListener(delegate { PhysicalAttack(); });

        localCanvas.interactiveObj = this;

        turnFeedbackAnim = localCanvas.turnFeedbackAnim;

        GameManager.Instance.objectList.Add(this);
        ToggleSelectedFeedback();
    }

    void Start()
    {
        localCanvas.ShowMana(inParty);
    }

    public void SetDefend(InteractiveObject npc)
    {
        defendAgainst = npc;
        localCanvas._animator.SetTrigger("Defend");
        // animation feedback shiled
    }
    public void GenerateDynamicStats() // calls at session start
    {

        List<SkillController.Type> allSkills = new List<SkillController.Type>();

        allSkills.Add(SkillController.Type.electricity);
        allSkills.Add(SkillController.Type.fire);
        allSkills.Add(SkillController.Type.gore);
        allSkills.Add(SkillController.Type.ice);
        allSkills.Add(SkillController.Type.piece);
        allSkills.Add(SkillController.Type.stone);
        allSkills.Add(SkillController.Type.water);
        allSkills.Add(SkillController.Type.wind);

        weakToDynamic.Clear();
        invToDynamic.Clear();

        foreach (SkillController.Type skill in weakToStatic)
        {
            weakToDynamic.Add(skill);
            allSkills.Remove(skill);
        }

        foreach (SkillController.Type skill in invToStatic)
        {
            invToDynamic.Add(skill);
            allSkills.Remove(skill);
        }

        foreach (SkillController.Type skill in allSkills)
        {
            float random = Random.value;

            if (random < 0.5f)
            {
                weakToDynamic.Add(skill);
            }
            else
            {
                invToDynamic.Add(skill);
            }
        }
        allSkills.Clear();

        if (!inParty)
        {
            curMaxHealth = Random.Range(minHealth, maxHealth);
            health = curMaxHealth;
            npcControl.GenerateNpcSkills();
        }
    }

    void OnMouseUpAsButton()
    {
        // click on object
        if (!GameManager.Instance.mouseOverButton && !GameManager.Instance.turnOver && !GameManager.Instance.blockSkillIcons && !GameManager.Instance.inDialog && !GameManager.Instance.choiceActive)
        {
            if (GameManager.Instance.objectsTurn == GameManager.Instance.player && GameManager.Instance.gameState == GameManager.State.Night)
            {
                GameManager.Instance.SetSelectedObject(this);
            }
            GameManager.Instance.InventoryClosed();
        }
    }

    public void ToggleTurnFeedback()
    {
        if (GameManager.Instance.objectsTurn == this && GameManager.Instance.enemyList.Count > 0)
        {
            turnFeedbackAnim.SetBool("Active", true);
            if (!inParty && npcControl != null)
                StartCoroutine("NpcSetAction");
        }
        else
            turnFeedbackAnim.SetBool("Active", false);
    }

    IEnumerator NpcSetAction()
    {
        yield return new WaitForSeconds(0.2F);
        npcControl.ChooseAction();
    }

    public void ToggleSelectedFeedback()
    {
        if (GameManager.Instance.selectedObject == this)
        {
            localCanvas.ShowIcons(this);
        }
        else
        {
            localCanvas.HideIcons();
        }
    }


    public void UseSkill(int skill)
    {
        if (GameManager.Instance.skillsCurrent[skill].GetComponent<SkillController>().manaCost <= GameManager.Instance.objectsTurn.mana)
        {
            GameManager.Instance.UseSkill(GameManager.Instance.skillsCurrent[skill], this);
            localCanvas.HideIcons();
            GameManager.Instance.objectsTurn.mana -= GameManager.Instance.skillsCurrent[skill].GetComponent<SkillController>().manaCost;
            GameManager.Instance.objectsTurn.UpdateMana();
        }
        else
        {
            GameManager.Instance.PrintActionFeedback(null, "Not enough mana!", null, false, true);
        }
    }

    public void Defend()
    {
        GameManager.Instance.Defend(this);
    }

    public void PhysicalAttack()
    {
        GameManager.Instance.UseSkill(simpleAttack, this);
        localCanvas.HideIcons();
    }

    public void UpdateMana()
    {
        manaBar.fillAmount = (mana * 1.0f) / (maxMana * 1.0f);
    }
    public void Damage(float baseDmg, InteractiveObject attacker)
    {
        float DMG = 0;
        if (baseDmg > 0)
        {
            if (!inParty && attacker != this)
            {
                if (!attacker.inParty && npcControl.agressiveTo != NpcController.Target.everyone)
                    npcControl.agressiveTo = NpcController.Target.enemies;
                else
                {
                    npcControl.agressiveTo = NpcController.Target.everyone;
                    //actionOnDialog = DialogAction.none;
                }
            }

            DMG = baseDmg;

            // CHECK IF MOB IS INV/WEAK TO SKILL
            SkillController activeSkill;
            if (GameManager.Instance.activeSkill != null)
            {
                activeSkill = GameManager.Instance.activeSkill.GetComponent<SkillController>();

                if (activeSkill.skillType == SkillController.Type.none) // simple attack
                {
                    sendWeak = false;
                    NpcDatabase.CheckSkillRelation(false, this);
                    //GameManager.Instance.UpdatePressTurns(GameManager.Instance.pressTurns - 1);
                }
                else
                {
                    foreach (SkillController.Type skill in weakToDynamic)
                    {
                        if (skill == activeSkill.skillType)
                        {
                            DMG = baseDmg * 2;
                            if (attacker == this && GameManager.Instance.attackTarget != this)
                            {
                            }
                            else
                            {
                                NpcDatabase.CheckSkillRelation(true, this);
                                if (defendAgainst != attacker)
                                    sendWeak = true;
                                //GameManager.Instance.UpdatePressTurns(GameManager.Instance.pressTurns);
                                break;
                            }
                        }
                    }
                    foreach (SkillController.Type skill in invToDynamic)
                    {
                        if (skill == activeSkill.skillType)
                        {
                            DMG = baseDmg / 2;
                            if (attacker == this && GameManager.Instance.attackTarget != this)
                            {
                            }
                            else
                            {
                                NpcDatabase.CheckSkillRelation(false, this);
                                sendWeak = false;
                                //GameManager.Instance.UpdatePressTurns(GameManager.Instance.pressTurns - 1);
                                break;
                            }
                        }
                    }
                }
            }
        }
        attacker._anim.SetTrigger("Action");

        if (this != attacker)
            StartCoroutine("ActionTriggerDelay", DMG);
        else
        {
            StartCoroutine("ActionTriggerDelaySelf", DMG);
        }

        if (defendAgainst == attacker)
            localCanvas._animator.SetTrigger("Defend");
    }

    IEnumerator ActionTriggerDelaySelf(float dmg)
    {
        yield return new WaitForSeconds(0.3F);
        health -= dmg;
        UpdateHeart();
        GameManager.Instance.CameraShake(0.5f);
        yield return new WaitForSeconds(1.2F);
        if (health <= 0)
        {
            if (GameManager.Instance.player != this)
            {
                StartCoroutine("SetAnimInactive");
                Instantiate(deathParticles, transform.position, Quaternion.identity);
            }
        }
    }
    IEnumerator ActionTriggerDelay(float dmg)
    {
        yield return new WaitForSeconds(0.3F);
        _anim.SetTrigger("Damage");
        ReduceHealth(dmg);

        yield return new WaitForSeconds(1.2F);
        if (health <= 0)
        {
            if (GameManager.Instance.player != this)
            {
                StartCoroutine("SetAnimInactive");
                Instantiate(deathParticles, transform.position, Quaternion.identity);
            }
        }
    }

    public void ReduceHealth(float dmg)
    {
        health -= dmg;
        UpdateHeart();
        GameManager.Instance.CameraShake(0.3f);
    }
    public void ReturnHealth(float dmg)
    {
        health += dmg;
        UpdateHeart();
        GameManager.Instance.CameraShake(0.3f);
    }

    public void Recover(float amount)
    {
        if (health + amount <= maxHealth)
            health += amount;
        else
            health = maxHealth;

        if (amount > 0)
            _anim.SetTrigger("Recover");

        UpdateHeart();
    }

    void UpdateHeart()
    {
        healthbar.fillAmount = health / maxHealth;
    }

    public void WalkAway()
    {
        health = 0;
        UpdateHeart();
        StartCoroutine("SetAnimInactive");
        Instantiate(deathParticles, transform.position, Quaternion.identity);
    }

    IEnumerator SetAnimInactive()
    {
        yield return new WaitForSeconds(0.3f);
        _anim.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
    public void Death()
    {
        GameManager.Instance.objectList.Remove(this);
        GameManager.Instance.allyList.Remove(this);
        GameManager.Instance.enemyList.Remove(this);
        GameManager.Instance.activeTeamList.Remove(this);

        if (GameManager.Instance.objectsTurn == this)
        {
            GameManager.Instance.objectsTurn = null;
        }

        if (inParty)
        {
            inParty = false;
        }

        Instantiate(deathParticles, transform.position, Quaternion.identity);
        _anim.SetTrigger("Damage");
        healthbar.gameObject.SetActive(false);
        turnFeedbackAnim.gameObject.SetActive(false);
        StartCoroutine("SetAnimInactive");
        //gameObject.SetActive(false);
    }

    public void StartRelationCoroutine()
    {
        StartCoroutine("SkillRelationDiscoveredFeedback");
    }

    IEnumerator SkillRelationDiscoveredFeedback()
    {
        yield return new WaitForSeconds(1.35f);
        GameManager.Instance.SkillRelationDiscoverFeedback(GameManager.Instance.activeSkill, this, sendWeak);
    }

    public void PlayerNight()
    {
        _anim.speed = 1;
        healthbar.gameObject.SetActive(true);
        turnFeedbackAnim.gameObject.SetActive(true);
    }
}