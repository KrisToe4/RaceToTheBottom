using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {

	// Use this for initialization
	void Awake ()
    {
		Physics2D.gravity = new Vector3(0f, 0f, -9.81f);

        GameObject.Destroy(GameObject.Find("Map"));
    }
}
