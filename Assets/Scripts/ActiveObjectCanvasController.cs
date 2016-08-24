using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ActiveObjectCanvasController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] skillIcons;

    [SerializeField]
    Canvas _canvas;

    public Animator _animator;
    public Animator turnFeedbackAnim;

    public GameObject manaGO;
    public Image healthbar;
    public Image manaBar;
    public Button skillButton_0;
    public Button skillButton_1;
    public Button skillButton_2;
    public Button skillButton_3;
    public Button defendButton;
    public Button attackButton;

    public SkillRelationController skillRelat;

    public bool iconsVisible = false;
    public InteractiveObject interactiveObj;

    void Awake()
    {
        _canvas.worldCamera = Camera.main;
    }

    void Start()
    {
        HideIcons();
    }

    public void WeakFeedback()
    {
        skillRelat.SetFeedback();
    }

    public void ShowMana(bool active)
    {
        manaGO.SetActive(active);
    }
    public void ShowIcons(InteractiveObject selected)
    {
        if (!iconsVisible)
        {

            ShowSkills();

            _animator.SetTrigger("Show");
            iconsVisible = true;

            GetFoundedSkills(selected);
        }
    }

    void GetFoundedSkills(InteractiveObject selected)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < GameManager.Instance.skillsCurrent.Count)
            {
                bool found = false;
                foreach (GameObject skill in NpcDatabase.GetSkillRelationsImmune(selected))
                {
                    //    print("foreach immune");
                    if (GameManager.Instance.skillsCurrent[i] == skill)
                    {
                        skillIcons[i].GetComponent<SkillLocalUiController>().SetImmune();
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    foreach (GameObject skill in NpcDatabase.GetSkillRelationsWeak(selected))
                    {
                        //      print("foreach weak");
                        if (GameManager.Instance.skillsCurrent[i] == skill)
                        {
                            skillIcons[i].GetComponent<SkillLocalUiController>().SetWeak();
                            found = true;
                            break;
                        }
                    }
                }
                if (!found)
                {
                    skillIcons[i].GetComponent<SkillLocalUiController>().SetClear();
                }
            }
            else
            {
                skillIcons[i].GetComponent<SkillLocalUiController>().SetClear();
            }
        }
    }

    public void ShowSkills()
    {
        for (int i = 0; i < 4; i++)
        {
            skillIcons[i].SetActive(true);
            Image img = skillIcons[i].GetComponent<Image>();
            SkillLocalUiController localSkill = skillIcons[i].GetComponent<SkillLocalUiController>();

            //if (GameManager.Instance.skillsCurrent[i] != null)
            if (GameManager.Instance.skillsCurrent.Count > i)
            {
                img.enabled = true;
                img.sprite = GameManager.Instance.skillsCurrent[i].GetComponent<SkillController>().skillSprite;
            }
            else
            {
                img.enabled = false;
            }

            //set color mana enough feedback
            if (GameManager.Instance.skillsCurrent[i].GetComponent<SkillController>().manaCost <= GameManager.Instance.objectsTurn.mana)
                img.color = Color.white;
            else
                img.color = Color.gray;
        }
    }

    public void HideIcons()
    {
        if (iconsVisible)
        {
            _animator.SetTrigger("Hide");
            iconsVisible = false;
        }
    }

    public void PointerEnterButton(int skill)
    {
        if (skill >= 0 && skill < 4)
        {
            string sendDescription = GameManager.Instance.skillsCurrent[skill].GetComponent<SkillController>().description;
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, true);
        }
        else if (skill == 4) // standart attack
        {
            string sendDescription = "Physical attack.";
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, true);
        }
        else if (skill == 5) // info
        {
            string sendDescription = "";
            if (interactiveObj.npcControl != null && !interactiveObj.inParty)
            {
                switch (GameManager.Instance.selectedObject.npcControl.agressiveTo)
                {
                    case NpcController.Target.enemies:
                        sendDescription = GameManager.Instance.selectedObject._name + " is aggressive against other monsters.";
                        break;
                    case NpcController.Target.everyone:
                        sendDescription = GameManager.Instance.selectedObject._name + " is aggressive against everyone.";
                        break;
                    case NpcController.Target.none:
                        sendDescription = GameManager.Instance.selectedObject._name + " is not aggressive.";
                        break;
                    case NpcController.Target.self:
                        sendDescription = GameManager.Instance.selectedObject._name + " want to kill himself.";
                        break;
                }
            }
            else
            {
                sendDescription = GameManager.Instance.selectedObject._name + " is in party.";
            }
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, true);
        }
        else if (skill == 6) // block
        {
            string sendDescription = "Defend against monster's attacks.";
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, true);
        }
        GameManager.Instance.mouseOverButton = true;
    }

    public void PointerExitButton()
    {
        GameManager.Instance.mouseOverButton = false;

        if (!GameManager.Instance.blockSkillIcons)
            GameManager.Instance.HideTextManually();
    }
}