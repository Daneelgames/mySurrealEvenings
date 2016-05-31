using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour {

    public float speed = 1;

    public float health = 1;

    [SerializeField]
    ActiveObjectCanvasController localCanvas;

    [SerializeField]
    private MeshRenderer turnFeedback;

    void Start()
    {
        GameManager.Instance.objectList.Add(this);
        ToggleSelectedFeedback();
    }

    void OnMouseDown()
    {
        if (!GameManager.Instance.mouseOverButton && !GameManager.Instance.turnOver)
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
            turnFeedback.enabled = true;
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
        print("skill used:" + skill);

        GameManager.Instance.SetTurn();
        localCanvas.HideSkills();
        localCanvas.HideIcons();
    }
    

    public void StartDialog()
    {

    }

    void Damage(float dmg)
    {
        health -= dmg;
    }

    void OnDestroy()
    {
        GameManager.Instance.objectList.Remove(this);
    }
}