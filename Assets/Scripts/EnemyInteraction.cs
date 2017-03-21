using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteraction : MonoBehaviour {

    public CharacterSheet characterSheet;

    public GameObject body;
    public float colorTimer = 0.5f;

    private Weapon m_weapon;

    private IEnumerator coroutine;

    // Use this for initialization
    void Start ()
    {
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        m_weapon = collider.gameObject.GetComponent<Weapon>();
        if ((m_weapon != null) && (characterSheet != null))
        {
            characterSheet.takeDamage(m_weapon);
            body.GetComponent<SpriteRenderer>().color = Color.red;

            coroutine = ResetColor(colorTimer);
            StartCoroutine(coroutine);
        }
    }

    IEnumerator ResetColor(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        body.GetComponent<SpriteRenderer>().color = Color.white;
    }

}
