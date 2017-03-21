using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySight : MonoBehaviour {

    public float chaseThreshold = 5;

    private EnemyController m_controller;
    private CircleCollider2D m_chaseTrigger;

    // Use this for initialization
    void Start ()
    {
        m_chaseTrigger = GetComponent<CircleCollider2D>();
        m_chaseTrigger.radius = chaseThreshold;

        m_controller = gameObject.transform.parent.gameObject.GetComponent<EnemyController>();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "Player")
        {
            Debug.Log("Enemy detected Player");

            if (m_controller != null)
            {
                m_controller.SetTarget(collider.gameObject);
            }
            else
            {
                Debug.Log("Couldn't find enemy script!");
            }
        }
    }
}
