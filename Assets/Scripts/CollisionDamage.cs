using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    public int damage = 10;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Health health;
    private float direction;
    public float Direction
    {
        get { return direction;}
    }
    private void OnCollisionStay2D(Collision2D col)
    {
        if (GameManager.Instance.healthContainer.ContainsKey(col.gameObject))
        {
            health = GameManager.Instance.healthContainer[col.gameObject]; 
            if (health != null)
            {
                direction = (col.transform.position - transform.position).x;
                animator.SetFloat("Direction", Mathf.Abs(direction));
            }
        }
    }
    public void SetDamage()
    {
        if (health!=null)
            health.TakeHit(damage, gameObject);
        health = null;
        direction = 0;
        animator.SetFloat("Direction",0f);
    }
}
