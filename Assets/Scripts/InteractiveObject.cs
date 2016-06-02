﻿using UnityEngine;
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

    public GameObject teamUpItem;
    public int teamUpItemValue = 1;

    public GameObject calmItem;
    public int calmItemValue = 1;

    public enum DialogAction {none, setAgressive, trade }
    public DialogAction actionOnDialog = DialogAction.none;

    public List<ListWrapper> dialogues = new List<ListWrapper>();

    public int activeDialog = 0;
    public int activePhrase = 0;

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
        if (npcControl != null)
        {
            if (npcControl.agressiveTo != NpcController.Target.everyone)
            {
                if (actionOnDialog == DialogAction.setAgressive)
                {
                    activeDialog = 3; // SETAGRESSIVE
                    npcControl.agressiveTo = NpcController.Target.everyone;
                }
                else if (actionOnDialog == DialogAction.trade)
                    activeDialog = 2; // SET TRADE DIALOG
                // NEED TO SET ITEMS CHECK
            }
            else if (npcControl.agressiveTo == NpcController.Target.everyone)
            {
                activeDialog = 4; // SET BASIC AGGRESSIVE DIALOG
                // NEED TO SET ITEMS CHECK
            }
        }
        activePhrase = 0;
        GameManager.Instance.DialogStart(this);
        localCanvas.HideIcons();
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