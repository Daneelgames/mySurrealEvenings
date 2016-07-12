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
            //DMG = baseDmg * (100 / GameManager.Instance.curSanity);

            DMG = baseDmg;

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
        _anim.SetTrigger("Damage");
        ReduceHealth(dmg);

        yield return new WaitForSeconds(1.2F);
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