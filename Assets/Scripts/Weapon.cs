using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public int maxAttacks = 2;
    public int damage = 10;

    private Collider2D m_collider;

	// Use this for initialization
	void Start () {
        m_collider = GetComponent<Collider2D>();
        m_collider.enabled = false;
	}

    public void setState(bool active)
    {
        m_collider.enabled = active;
    }
}
