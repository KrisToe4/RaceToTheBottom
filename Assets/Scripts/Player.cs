using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public float speed = 10;
    public float rotationSpeed = 360;
    public float dashMultiplier = 2;

    public Map currentMap;
    public GameObject equippedWeapon;

    private Animator m_animator;
    private CharacterSheet m_characterSheet;

    private Weapon m_primaryWeapon;

    private Vector2 m_inputVector = Vector2.zero;
    private bool m_attacking = false;

    private Vector2 m_moveVector = Vector2.zero;
    private Vector2 m_dashVector = Vector2.zero;
    private bool m_dashing = false;

    private bool m_holdPosition = false;

    private bool m_enableBackup = true;

    void Start()
    {
        m_characterSheet = GetComponent<CharacterSheet>();
        if (m_characterSheet == null)
        {
            m_characterSheet = gameObject.AddComponent<CharacterSheet>();
        }

        m_animator = GetComponent<Animator>();

        m_primaryWeapon = equippedWeapon.GetComponent<Weapon>();
    }

    // Update is called once per frame
    void Update()
    {
        m_enableBackup = (Input.GetAxis("Fire2") > 0);
        m_holdPosition = (Input.GetAxis("Jump") > 0);


        // Check if we should initiate an attack
        if (Input.GetAxis("Fire1") > 0)
        {
            if (!m_attacking)
            {
                startAttack();
                m_attacking = true;
            }
        }
        else
        {
            m_attacking = false;
        }

        // Rotation Calculation
        m_inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (m_inputVector != Vector2.zero)
        {
            float startAngle = transform.eulerAngles.z;

            // *Note: We're multiplying by -1 for X to get the rotation happening in the right direction
            float inputAngle = (Mathf.Atan2(m_inputVector.x * -1, m_inputVector.y) * Mathf.Rad2Deg);           
            if (inputAngle < 0)
            {
                inputAngle = 360 + inputAngle;
            }

            // Backup behaviour
            if (m_enableBackup)
            {
                inputAngle += 180; 
            }

            float rotationAngle = Mathf.MoveTowardsAngle(startAngle, inputAngle, rotationSpeed);
            transform.eulerAngles = new Vector3(0, 0, rotationAngle);
        }

        if (m_holdPosition)
        {
            m_animator.SetBool("walking", false);
        }
        else
        { 
            if (m_dashing)
            {
                transform.position += (Vector3)(m_dashVector * speed * dashMultiplier * Time.deltaTime);
            }
            else
            {
                m_moveVector = m_inputVector;

                // Dash
                if (Input.GetAxis("Fire3") > 0)
                {
                    m_animator.SetTrigger("dash");
                }
                else
                {
                    if (m_moveVector != Vector2.zero)
                    {
                        m_animator.SetBool("walking", true);

                        transform.position += (Vector3)(m_moveVector * speed * Time.deltaTime);
                    }
                    else
                    {
                        m_animator.SetBool("walking", false);
                    }
                }
            }
        }
    }

    public void startAttack()
    {
        int attackCount = m_animator.GetInteger("attackCount") + 1;
        if (attackCount <= m_primaryWeapon.maxAttacks)
        {
            m_animator.SetInteger("attackCount", attackCount);
            m_animator.SetTrigger("attack");  
        }
    }

    public void enableWeapon()
    {
        m_primaryWeapon.setState(true);
    }

    public void disableWeapon()
    {
        m_primaryWeapon.setState(false);
    }

    public void endAttack()
    {
        m_animator.SetInteger("attackCount", 0);
    }

    public void startDash()
    {
        m_dashVector = m_moveVector;
        m_animator.SetBool("walking", false);

        m_dashing = true;
    }

    public void endDash()
    {
        m_dashing = false;
    }

    public void die()
    {

    }

}
