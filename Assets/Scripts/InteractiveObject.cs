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

    public bool gotExtraPress = false;
    public int mana = 0;

    public List<SkillController.Type> weakToStatic;
    public List<SkillController.Type> invToStatic;
    public List<SkillController.Type> weakToDynamic;
    public List<SkillController.Type> invToDynamic;

    public bool inParty = false;
    public ActiveObjectCanvasController localCanvas;
    [SerializeField]
    private Animator turnFeedbackAnim;
    public NpcController npcControl;
    public Animator _anim;
    public List<ListWrapper> dialogues = new List<ListWrapper>();
    public int activeDialog = 0;
    public int activePhrase = 0;
    public Animator healthbar;
    [System.Serializable]
    public class ListWrapper
    {
        public List<string> stringList;
    }

    public GameObject deathParticles;

    bool sendWeak = false;

    public void SetGotExtraPress(bool got)
    {
        gotExtraPress = got;
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

    void Awake()
    {
        GameManager.Instance.objectList.Add(this);
        ToggleSelectedFeedback();
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
        if (GameManager.Instance.objectsTurn == this)
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
        }
        else
        {
            GameManager.Instance.PrintActionFeedback(null, "Not enough mana!", null, false, true);
        }
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
                            sendWeak = true;
                            GameManager.Instance.UpdatePressTurns(GameManager.Instance.pressTurns);
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
                            GameManager.Instance.UpdatePressTurns(GameManager.Instance.pressTurns - 1);
                            break;
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

            if (npcControl != null)
                npcControl.DropOnDead();
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

            if (npcControl != null)
                npcControl.DropOnDead();
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
        if (GameManager.Instance.player == this && health <= 0)
        {
            healthbar.SetFloat("HealthPercentage", 0.2f);
        }
        else
        {
            healthbar.SetFloat("HealthPercentage", health / maxHealth);
        }
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

    public void PlayerDay()
    {
        _anim.speed = 0;
        healthbar.gameObject.SetActive(false);
        turnFeedbackAnim.gameObject.SetActive(false);
    }

    public void PlayerNight()
    {
        _anim.speed = 1;
        healthbar.gameObject.SetActive(true);
        turnFeedbackAnim.gameObject.SetActive(true);
    }
}