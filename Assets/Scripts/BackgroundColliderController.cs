using UnityEngine;
using System.Collections;

public class BackgroundColliderController : MonoBehaviour {

    void OnMouseDown()
    {
        if (!GameManager.Instance.mouseOverButton && !GameManager.Instance.inDialog && !GameManager.Instance.choiceActive && !GameManager.Instance.tradeActive)
        {
            GameManager.Instance.ClearSelectedObject();
        }
    }
}