using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour {

    [SerializeField]
    ActiveObjectCanvasController localCanvas;

    void Start()
    {
        ToggleSelectedFeedback();
        GameManager.Instance.objectList.Add(this);
    }

    void OnMouseDown()
    {
        GameManager.Instance.SetSelectedObject(this);
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

    void OnDestroy()
    {
        GameManager.Instance.objectList.Remove(this);
    }

    public void StartDialog()
    {

    }
}