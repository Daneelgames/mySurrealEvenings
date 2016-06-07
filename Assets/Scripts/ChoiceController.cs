using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChoiceController : MonoBehaviour {

    private Animator _anim;

    public Text title;
    public Text description;

    private InteractiveObject npc;

    void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void ShowWindow(InteractiveObject _npc)
    {
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
        _anim.SetTrigger("Active");
    }

    public void Yes()
    {
        if (npc.activeDialog == 1) // team up
        {
            npc.inParty = true;
            GameManager.Instance.party.Add(npc);
            GameManager.Instance.inventoryController.MoneyLose(npc.teamUpMoney);
            GameManager.Instance.inventoryController.ItemLost(npc.teamUpItem.GetComponent<SkillController>());

            npc.TeamUp();
            npc.npcControl.agressiveTo = NpcController.Target.none;
        }
        else if (npc.activeDialog == 5) // calm down
        {
            GameManager.Instance.inventoryController.MoneyLose(npc.calmMoney);
            GameManager.Instance.inventoryController.ItemLost(npc.calmItem.GetComponent<SkillController>());
            npc.npcControl.agressiveTo = NpcController.Target.none;
        }

        GameManager.Instance.ChoiceInactive();
        _anim.SetTrigger("Yes");
    }

    public void No()
    {
        GameManager.Instance.ChoiceInactive();
        _anim.SetTrigger("No");
    }
}
