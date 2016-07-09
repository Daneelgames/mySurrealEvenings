using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour
{

    public string _name = "Npc";

    public Sprite facepic;

    public float speed = 1;

    public float health = 1;
    public float maxHealth = 1;
    public List<Effect> unitEffect = new List<Effect> { Effect.none };

    public bool inParty = false;

    //[SerializeField]
    public ActiveObjectCanvasController localCanvas;

    [SerializeField]
    private Animator turnFeedbackAnim;

    public NpcController npcControl;

    public Animator _anim;

    public GameObject calmItem;
    public int calmMoney = 5;

    public enum DialogAction { none, setAgressive, trade }
    public DialogAction actionOnDialog = DialogAction.none;

    public List<ListWrapper> dialogues = new List<ListWrapper>();

    public int activeDialog = 0;
    public int activePhrase = 0;

    public Animator healthbar;

    [SerializeField]
    private GameObject teleportParticles;

    [System.Serializable]
    public class ListWrapper
    {
        public List<string> stringList;
    }

    public GameObject deathParticles;

    void Awake()
    {
        maxHealth = health;
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


    public void StartDialog()
    {
        if (npcControl != null && !inParty)
        {
            if (npcControl.agressiveTo != NpcController.Target.everyone)
            {
                activeDialog = 0; //default dialog

                if (actionOnDialog == DialogAction.trade)
                    activeDialog = 2; // TRADE DIALOG

                else if (actionOnDialog == DialogAction.setAgressive)
                {
                    activeDialog = 3; // SETAGRESSIVE
                    npcControl.agressiveTo = NpcController.Target.everyone;
                    npcControl.SetAgressiveFeedback();
                }

            }
            else if (npcControl.agressiveTo == NpcController.Target.everyone)
            {
                activeDialog = 4; // BASIC AGGRESSIVE DIALOG

                // CHECK CALM ITEM
                if (GameManager.Instance.inventoryController.candies >= calmMoney && calmItem != null)
                {
                    foreach (GameObject item in GameManager.Instance.skillsCurrent)
                    {
                        if (item == calmItem)
                        {
                            activeDialog = 5; // CALM
                            break;
                        }
                    }
                }

            }
        }
        else if (inParty)
        {
            activeDialog = 6;
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
            DMG = baseDmg * (100 / GameManager.Instance.curSanity);

            if (!inParty && attacker != this)
            {
                if (!attacker.inParty && npcControl.agressiveTo != NpcController.Target.everyone)
                    npcControl.agressiveTo = NpcController.Target.enemies;
                else
                {
                    npcControl.agressiveTo = NpcController.Target.everyone;
                    npcControl.SetAgressiveFeedback();
                    //actionOnDialog = DialogAction.none;
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
        yield return new WaitForSeconds(0.5F);
        if (health <= 0)
        {
            if (GameManager.Instance.player != this)
            {
                _anim.gameObject.SetActive(false);
                Instantiate(deathParticles, transform.position, Quaternion.identity);
            }

            if (npcControl != null)
                npcControl.DropOnDead();
        }
    }
    IEnumerator ActionTriggerDelay(float dmg)
    {
        yield return new WaitForSeconds(0.3F);
        health -= dmg;
        _anim.SetTrigger("Damage");
        UpdateHeart();
        GameManager.Instance.CameraShake(0.5f);

        yield return new WaitForSeconds(0.5F);
        if (health <= 0)
        {
            if (GameManager.Instance.player != this)
            {
                _anim.gameObject.SetActive(false);
                Instantiate(deathParticles, transform.position, Quaternion.identity);
            }

            if (npcControl != null)
                npcControl.DropOnDead();
        }
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
}