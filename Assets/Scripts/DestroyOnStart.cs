using UnityEngine;
using System.Collections;

public class DestroyOnStart : MonoBehaviour {

    public float deathTime = 1;

	void Start () {
        Destroy(gameObject, deathTime);
	}
}
