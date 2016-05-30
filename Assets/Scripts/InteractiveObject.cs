using UnityEngine;
using System.Collections;

public class InteractiveObject : MonoBehaviour {

    public float speed = 1;

    [SerializeField]
    ActiveObjectCanvasController localCanvas;


    void Start()
    {
        GameManager.Instance.objectList.Add(this);
        ToggleSelectedFeedback();
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