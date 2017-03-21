using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour
{
    public float speed = 2;

    public float attackThreshold = 0.5f;
    private bool m_attacking = false;

    private Animator m_animator;
    private CharacterSheet m_characterSheet;
    private Seeker m_seeker;

    private GameObject m_target;
    private Path m_path;

    public float nextWaypointDistance = 3;
    public int m_currentWaypoint = 0;

    public float repathRate = 0.5f;
    private float lastRepath = -9999;


    // Use this for initialization
    void Start()
    {
        m_animator = this.GetComponentInChildren<Animator>();

        m_characterSheet = this.GetComponent<CharacterSheet>();
        if (m_characterSheet == null)
        {
            m_characterSheet = gameObject.AddComponent<CharacterSheet>();
        }
        gameObject.GetComponentInChildren<EnemyInteraction>().characterSheet = m_characterSheet;

        m_seeker = GetComponent<Seeker>();
        m_seeker.pathCallback += OnPathComplete;

        m_target = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Stop and return (disabling all other behaviour)
        if (m_target == null)
        {
            m_animator.SetBool("walking", false);

            return;
        }

        m_attacking = ((m_target.transform.position - transform.position).magnitude <= attackThreshold);

        if (m_attacking)
        {
            m_animator.SetBool("walking", false);
        }
        else
        {
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        if (Time.time - lastRepath > repathRate && m_seeker.IsDone())
        {
            lastRepath = Time.time + Random.value * repathRate * 0.5f;

            // Start a new path to the targetPosition, call the the OnPathComplete function
            // when the path has been calculated (which may take a few frames depending on the complexity)
            m_seeker.StartPath(transform.position, m_target.transform.position);
        }

        if (m_path == null)
        {
            // We have no path to follow yet, so don't do anything
            return;
        }

        if (m_currentWaypoint > m_path.vectorPath.Count)
        {
            return;
        }
        else if (m_currentWaypoint == m_path.vectorPath.Count)
        {
            m_currentWaypoint++;
            return;
        }

        // Check if we should be attacking or moving towards our target
        Vector2 moveVector = (m_path.vectorPath[m_currentWaypoint] - transform.position).normalized;
        if (moveVector != Vector2.zero)
        { 
            m_animator.SetBool("walking", true);

            // *Note: We're multiplying by -1 for X to get the rotation happening in the right direction
            float angle = Mathf.Atan2(moveVector.x * -1, moveVector.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            Vector3 positionChange = moveVector * speed * Time.deltaTime;
            transform.position += positionChange;
        }
        else
        {
            m_animator.SetBool("walking", false);
        }

        // The commented line is equivalent to the one below, but the one that is used
        // is slightly faster since it does not have to calculate a square root
        //if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
        if ((transform.position - m_path.vectorPath[m_currentWaypoint]).sqrMagnitude < (nextWaypointDistance * nextWaypointDistance))
        {
            m_currentWaypoint++;
        }
    }

    void AttackTarget()
    {

    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            m_path = p;
            m_currentWaypoint = 0;
        }

    }

    void OnDisable()
    {
        m_seeker.pathCallback -= OnPathComplete;
    }



    #region PUblic Methods
    public void SetTarget(GameObject target)
    {
        if (m_target == null)
        {
            m_target = target;
            m_seeker.StartPath(transform.position, m_target.transform.position);
        }
    }
    #endregion
}
