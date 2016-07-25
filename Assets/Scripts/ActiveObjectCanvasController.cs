using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ActiveObjectCanvasController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] skillIcons;

    [SerializeField]
    Canvas _canvas;

    [SerializeField]
    Sprite emptySkill;

    [SerializeField]
    Animator _animator;


    public bool iconsVisible = false;

    void Awake()
    {
        _canvas.worldCamera = Camera.main;
    }

    void Start()
    {
        HideIcons();
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
        for (int i = 0; i < 8; i++)
        {
            if (i < GameManager.Instance.skillsCurrent.Count)
            {
                bool found = false;
                foreach (GameObject skill in NpcDatabase.GetSkillRelationsImmune(selected))
                {
                    print("foreach immune");
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
                        print("foreach weak");
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
        for (int i = 0; i < 8; i++)
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
        if (skill >= 0)
        {
            string sendDescription = GameManager.Instance.skillsCurrent[skill].GetComponent<SkillController>().description;
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);
        }
        else if (skill == -1) // Trade
        {
            string sendDescription = "Trade with the monster.";
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);
        }
        else if (skill == -2) // talk
        {
            string sendDescription = "Talk to the monster.";
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);
        }
        else if (skill == -3) // repel
        {
            string sendDescription = "Repel the monster.";
            GameManager.Instance.PrintActionFeedback(null, sendDescription, null, false, false, true);
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