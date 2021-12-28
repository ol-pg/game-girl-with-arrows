using System.Collections;
using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] public int health;
    public Action<int, GameObject> OnTakeHit;
    public int CurrentHealth
    {
        get { return health; }
    }
    [SerializeField] private Animator animator;

    private void Start()
    {
        GameManager.Instance.healthContainer.Add(gameObject, this);
    }

    public void TakeHit(int damage, GameObject attacker)
    {
        health -= damage;

        if (OnTakeHit != null)
            OnTakeHit(damage,attacker);
        if (health <= 0)
            Destroy(gameObject);
    }

    public void SetHealth(int bonusHealth)
    {
        health += bonusHealth;
    }


}
