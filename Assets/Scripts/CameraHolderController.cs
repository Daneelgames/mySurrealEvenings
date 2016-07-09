using UnityEngine;
using System.Collections;

public class CameraHolderController : MonoBehaviour
{
	public Vector3 defaultPosition;
    public Vector3 targetPosition;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    public Camera mainCam;
    public bool focus = false;
	public float camZoomSize = 6;
	public float camDefaultSize = 6;
    void LateUpdate()
    {
        if (focus)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, camZoomSize, 3 *  Time.deltaTime);
        }
		else
		{
            transform.position = Vector3.SmoothDamp(transform.position, defaultPosition, ref velocity, smoothTime);
            mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, camDefaultSize, 3 * Time.deltaTime);
		}
    }
    public void TargetFocus(Vector3 pos)
    {
        targetPosition = new Vector3(pos.x, pos.y + 2, transform.position.z);
		focus = true;
		StartCoroutine("DisableFocus");
    }

	IEnumerator DisableFocus()
	{
		yield return new WaitForSeconds(1.25f);
		focus = false;
	}
}