using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour {

    public string _name = "Npc";

    public Sprite facepic;

    public float speed = 1;

    public float health = 1;
    public float maxHealth = 1;
    public List<Effect> unitEffect = new List<Effect> {Effect.none};

    public bool inParty = false;

    //[SerializeField]
    public ActiveObjectCanvasController localCanvas;

    [SerializeField]
    private MeshRenderer turnFeedback;

    public NpcController npcControl;

    [HideInInspector]
    public Animator _anim;

    public GameObject teamUpItem;
    public int teamUpMoney = 5;

    public GameObject calmItem;
    public int calmMoney = 5;

    public enum DialogAction {none, setAgressive, trade }
    public DialogAction actionOnDialog = DialogAction.none;

    public List<ListWrapper> dialogues = new List<ListWrapper>();

    public int activeDialog = 0;
    public int activePhrase = 0;

    [SerializeField]
    private GameObject teleportParticles;

    [System.Serializable]
    public class ListWrapper
    {
        public List<string> stringList;
    }

    void Awake()
    {
        maxHealth = health;
        GameManager.Instance.objectList.Add(this);
        ToggleSelectedFeedback();
        _anim = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        // click on object
        if (!GameManager.Instance.mouseOverButton && !GameManager.Instance.turnOver && !GameManager.Instance.blockSkillIcons && !GameManager.Instance.tradeActive && !GameManager.Instance.inDialog)
        {

            foreach (InteractiveObject obj in GameManager.Instance.party)
            {
                if (GameManager.Instance.objectsTurn == obj)
                {
                    //_spriteRenderer.color = Color.gray;
                }
            }
        }
    }

    void OnMouseUp()
    {
        //_spriteRenderer.color = Color.white;
    }

    void OnMouseUpAsButton()
    {
        // click on object
        if (!GameManager.Instance.mouseOverButton && !GameManager.Instance.turnOver && !GameManager.Instance.blockSkillIcons && !GameManager.Instance.tradeActive && !GameManager.Instance.inDialog && !GameManager.Instance.choiceActive)
        {
            foreach (InteractiveObject obj in GameManager.Instance.party)
            {
                if (GameManager.Instance.objectsTurn == obj)
                {
                    GameManager.Instance.SetSelectedObject(this);
                }
            }
        }
    }

    public void ToggleTurnFeedback()
    {
        if (GameManager.Instance.objectsTurn == this)
        {
            turnFeedback.enabled = true;
            if (!inParty && npcControl != null)
                StartCoroutine("NpcSetAction");
        }
        else
            turnFeedback.enabled = false;
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

    public void ToggleSkills()
    {
        if (!localCanvas.skillsVisible)
        {
            localCanvas.ShowSkills();
        }
        else
        {
            localCanvas.HideSkills();
        }
    }

    public void UseSkill(int skill)
    {
        GameManager.Instance.UseSkill(GameManager.Instance.skillsCurrent[skill], this);
        localCanvas.HideSkills();
        localCanvas.HideIcons();
        _anim.SetTrigger("Action");
    }
    

    public void StartDialog()
    {
        if (npcControl != null && !inParty)
        {
            if (npcControl.agressiveTo != NpcController.Target.everyone)
            {
                activeDialog = 0; //default dialog

                // CHECK TEAMUP ITEM
                if (GameManager.Instance.inventoryController.money >= teamUpMoney && teamUpItem != null && GameManager.Instance.party.Count < 3)
                {
                    foreach (GameObject item in GameManager.Instance.skillsCurrent)
                    {
                        if (item == teamUpItem)
                        {
                            activeDialog = 1; // teamUp
                            break;
                        }
                    }
                }

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
                if (GameManager.Instance.inventoryController.money >= calmMoney && calmItem != null)
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

    public void TeamUp()
    {
        Transform newPlace = transform;

        if (GameManager.Instance.party.Count == 2)
            newPlace = GameManager.Instance.partyCells[1];
        if (GameManager.Instance.party.Count == 3)
            newPlace = GameManager.Instance.partyCells[2];

        Instantiate(teleportParticles, transform.position, teleportParticles.transform.rotation);
        Instantiate(teleportParticles, newPlace.position, teleportParticles.transform.rotation);

        StartCoroutine("TakeNewPlace", newPlace);
    }

    IEnumerator TakeNewPlace (Transform newPlace)
    {
        yield return new WaitForSeconds(0.5f);
        transform.position = newPlace.position;
        transform.Find("sprite").transform.Rotate(0, 180, 0);
    }

    public void Damage(float dmg, InteractiveObject attacker)
    {
        if (dmg > 0)
        {
            health -= dmg;

            _anim.SetTrigger("Damage");

            if (!inParty)
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
        if (health <= 0)
            Death();
    }

    public void Recover(float amount)
    {
        if (health + amount <= maxHealth)
            health += amount;
        else
            health = maxHealth;

        if (amount > 0)
            _anim.SetTrigger("Recover");
    }

    void Death()
    {
        GameManager.Instance.objectList.Remove(this);

        StartCoroutine("DeathCoroutine");
    }

    IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(1f);

        if (npcControl != null)
            npcControl.DropOnDead();

        Destroy(gameObject);
    }
}