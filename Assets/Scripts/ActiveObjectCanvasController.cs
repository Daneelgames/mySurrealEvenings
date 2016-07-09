using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ActiveObjectCanvasController : MonoBehaviour
{

    [SerializeField]
    private GameObject[] buttonIcons;
    [SerializeField]
    private GameObject[] skillIcons;

    [SerializeField]
    Canvas _canvas;

    [SerializeField]
    Sprite emptySkill;

    [SerializeField]
    Animator _animator;
    [SerializeField]
    Animator aggressiveAnimator;

    [SerializeField]
    Image aggressiveIcon;

    public bool iconsVisible = false;

    void Awake()
    {
        _canvas.worldCamera = Camera.main;
    }

    void Start()
    {
        HideIcons();
    }

    public void ShowIcons()
    {
        if (!iconsVisible)
        {
            foreach (GameObject go in buttonIcons)
                go.SetActive(true);
                
            ShowSkills();

            _animator.SetTrigger("Show");
            iconsVisible = true;
        }
    }

    public void ShowSkills()
    {
        for (int i = 0; i < GameManager.Instance.skillsCurrent.Count; i++)
        {
            skillIcons[i].SetActive(true);
            //if (GameManager.Instance.skillsCurrent[i] != null)
            if (GameManager.Instance.skillsCurrent.Count > i)
            {
                skillIcons[i].GetComponent<Image>().enabled = true;
                skillIcons[i].GetComponent<Image>().sprite = GameManager.Instance.skillsCurrent[i].GetComponent<SkillController>().skillSprite;
            }
            else
            {
                skillIcons[i].GetComponent<Image>().enabled = false;
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
        GameManager.Instance.mouseOverButton = true;
    }

    public void PointerExitButton()
    {
        GameManager.Instance.mouseOverButton = false;

        if (!GameManager.Instance.blockSkillIcons)
            GameManager.Instance.HideTextManually();
    }

    public void AggressiveStart()
    {
        aggressiveIcon.enabled = true;
        aggressiveAnimator.SetBool("Active", true);
    }

    public void AggressiveOver()
    {
        StartCoroutine("DisableAggressiveIcon");
        aggressiveAnimator.SetBool("Active", false);
    }

    IEnumerator DisableAggressiveIcon()
    {
        yield return new WaitForSeconds(0.5f);
        aggressiveIcon.enabled = false;
    }
}