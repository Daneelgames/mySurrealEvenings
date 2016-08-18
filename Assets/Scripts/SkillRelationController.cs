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
            //Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);
            //screenPos = new Vector3(screenPos.x, screenPos.y, transform.position.z);
            //transform.position = Vector3.Lerp(transform.position, screenPos, 0.1f);
            //transform.position = screenPos;

            Vector3 pos;
            float width = canvas.GetComponent<RectTransform>().sizeDelta.x;
            float height = canvas.GetComponent<RectTransform>().sizeDelta.y;
            float x = Camera.main.WorldToScreenPoint(targetPos).x / Screen.width;
            float y = Camera.main.WorldToScreenPoint(targetPos).y / Screen.height;
            pos = new Vector3(width * x - width / 2, y * height - height / 2);
			transform.position = pos;
        }
    }
    public void SetFeedback(GameObject trgt)
    {
        targetPos = trgt.transform.position;
        anim.SetTrigger("Update");
        active = true;
        //StartCoroutine("SetInactive");
    }
}