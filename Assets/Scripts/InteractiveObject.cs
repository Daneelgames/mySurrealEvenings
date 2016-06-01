using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveObject : MonoBehaviour {

    public string _name = "Npc";

    public float speed = 1;

    public float health = 1;
    public float maxHealth = 1;
    public List<Effect> unitEffect = new List<Effect> {Effect.none};

    public bool inParty = false;

    [SerializeField]
    ActiveObjectCanvasController localCanvas;

    [SerializeField]
    private MeshRenderer turnFeedback;

    public NpcController npcControl;

    [HideInInspector]
    public Animator _anim;
    
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
        if (!GameManager.Instance.mouseOverButton && !GameManager.Instance.turnOver && !GameManager.Instance.blockSkillIcons)
        {
            foreach(InteractiveObject obj in GameManager.Instance.party)
            {
                if (GameManager.Instance.objectsTurn == obj)
                    GameManager.Instance.SetSelectedObject(this);
            }

        }
    }

    public void ToggleTurnFeedback()
    {
        if (GameManager.Instance.objectsTurn == this)
        {
            turnFeedback.enabled = true;
            if (npcControl != null)
                npcControl.ChooseAction();
        }
        else
            turnFeedback.enabled = false;
    }

    public void ToggleSelectedFeedback()
    {
        if (GameManager.Instance.SelectedObject == this)
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

    }

    public void Damage(float dmg, InteractiveObject attacker)
    {
        health -= dmg;
        if (dmg > 0)
            _anim.SetTrigger("Damage");

        if (npcControl != null)
        {
            if (!attacker.inParty && npcControl.agressiveTo != NpcController.Target.everyone)
                npcControl.agressiveTo = NpcController.Target.enemies;
            else
                npcControl.agressiveTo = NpcController.Target.everyone;
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
        Destroy(gameObject, 1f);
    }

    void OnDestroy()
    {
        GameManager.Instance.objectList.Remove(this);
    }
}