using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public GameObject LeftBorder;
    public GameObject RightBorder;
    public Rigidbody2D rigidbody;
    public GroundDetection groundDetection;
    public bool isRightDirections;
    public float speed;
  
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private CollisionDamage collisionDamage;

    private void Update()
    {
        if (groundDetection.isGrounded)
        {
            if (transform.position.x > RightBorder.transform.position.x
                || collisionDamage.Direction < 0)
                isRightDirections = false;
            else if (transform.position.x < LeftBorder.transform.position.x
                || collisionDamage.Direction > 0)
                isRightDirections = true;
            rigidbody.velocity = isRightDirections ? Vector2.right : Vector2.left;
            rigidbody.velocity *= speed;
        }
        if (rigidbody.velocity.x > 0)
            spriteRenderer.flipX = true;
        if (rigidbody.velocity.x < 0)
            spriteRenderer.flipX = false;
    }
}
