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

    public List<GameObject> foundSkillsRelations;

    public List<SkillController> weakToStatic;
    public List<SkillController> invToStatic;
    public List<SkillController> weakToDynamic;
    public List<SkillController> invToDynamic;

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

    public void GenerateDynamicStats() // calls at session start
    {
        foundSkillsRelations.Clear();

        if (!inParty)
        {
            List<GameObject> allSkills = new List<GameObject>(GameManager.Instance.skillList.allSkills);

            weakToDynamic.Clear();
            invToDynamic.Clear();

            foreach (SkillController skill in weakToStatic)
            {
                weakToDynamic.Add(skill);
                allSkills.Remove(skill.gameObject);
            }

            foreach (SkillController skill in invToStatic)
            {
                invToDynamic.Add(skill);
                allSkills.Remove(skill.gameObject);
            }

            foreach (GameObject skill in allSkills)
            {
                float random = Random.value;

                if (random < 0.5f)
                {
                    weakToDynamic.Add(skill.GetComponent<SkillController>());
                }
                else
                {
                    invToDynamic.Add(skill.GetComponent<SkillController>());
                }
            }
            allSkills.Clear();

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
            if (GameManager.Instance.objectsTurn == GameManager.Instance.player)
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
            localCanvas.ShowIcons();
        }
        else
        {
            localCanvas.HideIcons();
        }
    }


    public void UseSkill(int skill)
    {
        GameManager.Instance.UseSkill(GameManager.Instance.skillsCurrent[skill], this);
        localCanvas.HideIcons();
    }


    public void StartDialog(string theme)
    {
        if (npcControl != null && !inParty)
        {
            if (theme == "Trade")
            {
                activeDialog = 0; //0 trade dialog
            }
            else if (theme == "Repel")
            {
                activeDialog = 1; //1 repel dialog
            }
            else if (theme == "Talk")
            {
                if (npcControl.agressiveTo == NpcController.Target.none)
                {
                    activeDialog = 2; //2 none dialog
                }
                else if (npcControl.agressiveTo == NpcController.Target.everyone)
                {
                    activeDialog = 3; //3 everyone dialog
                }
                else if (npcControl.agressiveTo == NpcController.Target.enemies)
                {
                    activeDialog = 4; //4 enemies dialog
                }
                else if (npcControl.agressiveTo == NpcController.Target.self)
                {
                    activeDialog = 5; //5 self dialog
                }
            }
            else if (theme == "Angry")
            {
                activeDialog = 6; //6 angry dialog  
                if (npcControl != null)
                    npcControl.agressiveTo = NpcController.Target.everyone;
            }
            else if (theme == "Happy")
            {
                activeDialog = 7; //7 happy dialog  
                npcControl.agressiveTo = NpcController.Target.none;
            }
        }
        else if (inParty)
        {
            activeDialog = 2;
        }
        activePhrase = 0;
        GameManager.Instance.DialogStart(this);
        localCanvas.HideIcons();
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

                foreach (SkillController skill in weakToDynamic)
                {
                    if (skill == activeSkill)
                    {
                        DMG = baseDmg * 2;
                        if (!inParty)
                        {
                            if (attacker == this && GameManager.Instance.attackTarget != this)
                            {
                            }
                            else
                            {
                                CheckSkillRelation(attacker);
                                sendWeak = true;
                                break;
                            }
                        }
                    }
                }

                foreach (SkillController skill in invToDynamic)
                {
                    if (skill == activeSkill)
                    {
                        DMG = baseDmg / 2;
                        if (!inParty)
                        {
                            if (attacker == this && GameManager.Instance.attackTarget != this)
                            {
                            }
                            else
                            {
                                CheckSkillRelation(attacker);
                                sendWeak = false;
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
    }
    public void Death()
    {
        GameManager.Instance.objectList.Remove(this);

        if (GameManager.Instance.objectsTurn == this)
        {
            GameManager.Instance.objectsTurn = null;
        }

        if (inParty)
        {
            inParty = false;
        }

        gameObject.SetActive(false);
    }

    void CheckSkillRelation(InteractiveObject attacker)
    {
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
                StartCoroutine(SkillRelationDiscoveredFeedback(activeSkill));
            }
        }
        else
            StartCoroutine(SkillRelationDiscoveredFeedback(activeSkill));
    }

    IEnumerator SkillRelationDiscoveredFeedback(GameObject skill)
    {
        foundSkillsRelations.Add(skill);
        yield return new WaitForSeconds(1.35f);
        GameManager.Instance.SkillRelationDiscoverFeedback(skill, this, sendWeak);
    }
}