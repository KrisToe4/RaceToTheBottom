using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSheet : MonoBehaviour {

    public int health = 50;

    public void takeDamage(Weapon weapon)
    {
        health -= weapon.damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Dead!");
        Destroy(gameObject);
    }
}
