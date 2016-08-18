using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillRelationController : MonoBehaviour
{
    public Animator anim;
    public Text _text;
    public GameObject target;
    public bool active = false;

    void Update()
    {
        // set position to target
        if (active)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
            screenPos = new Vector3(screenPos.x, screenPos.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, screenPos, 1f);
        }

    }
    public void SetFeedback(GameObject trgt)
    {
        target = trgt;
        anim.SetTrigger("Update");
        active = true;
    }
}
