using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillRelationController : MonoBehaviour
{
    public Animator anim;
    public Canvas canvas;
    public Text _text;
    public Vector3 targetPos;
    public bool active = false;

    void Update()
    {
        // set position to target
        if (active)
        {
        }
    }
    public void SetFeedback(GameObject trgt)
    {
        targetPos = trgt.transform.position;
        print(targetPos);
        anim.SetTrigger("Update");
        active = true;
        //StartCoroutine("SetInactive");
        Vector3 screenPos = Camera.main.WorldToScreenPoint(targetPos);
        //screenPos = new Vector3(screenPos.x, screenPos.y, transform.position.z);
        //transform.position = Vector3.Lerp(transform.position, screenPos, 0.1f);
        transform.position = screenPos;
    }
}