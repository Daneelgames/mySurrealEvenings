using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChoiceController : MonoBehaviour
{

    private Animator _anim;

    public Text title;
    public Text description;

    private InteractiveObject npc;
    private bool sleepChoice = false;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void ShowWindow(InteractiveObject _npc, bool sleepNearEnemy)
    {
        if (!sleepNearEnemy)
        {
            sleepChoice = false;
            npc = _npc;

            if (npc.activeDialog == 1) //TEAM Up
            {
                title.text = npc._name + " may join";
                description.text = "If you give " + npc.teamUpMoney + " moneye and " + npc.teamUpItem.GetComponent<SkillController>().skillName;
            }
            else if (npc.activeDialog == 5) //CALM
            {
                title.text = npc._name + " may calm down";
                description.text = "If you give " + npc.calmMoney + " moneye and " + npc.calmItem.GetComponent<SkillController>().skillName;
            }
        }
        else
        {
            sleepChoice = true;
            npc = null;

            title.text = "Go to sleep?";
            description.text = "Aggressive monsters will scare you and damage your toy while you're sleeping.";
        }

        _anim.SetTrigger("Active");
    }

    public void Yes()
    {
        if (!sleepChoice)
        {
            if (npc.activeDialog == 1) // team up
            {
                npc.inParty = true;
                GameManager.Instance.party.Add(npc);
                GameManager.Instance.inventoryController.CandyLose(npc.teamUpMoney);
                GameManager.Instance.inventoryController.ItemLost(npc.teamUpItem.GetComponent<SkillController>());

                npc.TeamUp();
                npc.npcControl.agressiveTo = NpcController.Target.none;
                npc.npcControl.RemoveAggressiveFeedback();
            }
            else if (npc.activeDialog == 5) // calm down
            {
                GameManager.Instance.inventoryController.CandyLose(npc.calmMoney);
                GameManager.Instance.inventoryController.ItemLost(npc.calmItem.GetComponent<SkillController>());
                npc.npcControl.agressiveTo = NpcController.Target.none;
                npc.actionOnDialog = InteractiveObject.DialogAction.none;
                npc.npcControl.RemoveAggressiveFeedback();
            }
        }


        if (sleepChoice)
        {
            GameManager.Instance.FrenzyDamage(25f);
            GameManager.Instance.party[0].Damage(0.25f * GameManager.Instance.party[0].maxHealth, GameManager.Instance.party[0]);
        }
        
        GameManager.Instance.ChoiceInactive(sleepChoice);
        _anim.SetTrigger("Yes");
    }

    public void No()
    {
        GameManager.Instance.ChoiceInactive(false);
        _anim.SetTrigger("No");
    }
}
