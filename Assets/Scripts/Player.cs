using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//namespace PlayerTools
//{
public class Player : MonoBehaviour
{
        [SerializeField]private float speed = 2.5f;
        public float Speed
        {
            get { return speed; }
            set
            {
                if (value > 0.5)
                    speed=value;
            }
        }
        [SerializeField] private float force;
        public float Force
        {
            get { return force; }
            set
            {
                if (value > 0.5)
                    force = value;
            }
        }
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private float minimalHeight;
        public float MinimalHeight
        {
            get { return minimalHeight; }
            set
            {
                if (value > -15)
                    minimalHeight = value;
            }
        }
        [SerializeField] private bool isCheatMode;
        [SerializeField] private GroundDetection groundDetection;
        private Vector3 direction;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;
        //[SerializeField] private bool isJumping;
        [SerializeField] private Arrow arrow;
        [SerializeField] private Transform arrowSpawnPoint;
        [SerializeField] private float shootForce=5;
        [SerializeField] private float cooldown=3;
        [SerializeField] private float damageForce;
        [SerializeField] private int arrowsCount=3;
        [SerializeField] private Health health;
        [SerializeField] private BuffReciever buffReciever;
        [SerializeField] private Camera playerCamera;
        private UICharacterController controller;
        public Health Health { get { return health; } }
        [SerializeField] private Image circleImage;
        private Arrow currentArrow;
        private float bonusForce;
        private float bonusDamage;
        private float bonusHealth;
        private bool isJumping;
        private bool isCooldown = false;
        private bool isBlockMovement;
        private List<Arrow> arrowPool;

        private void Awake()
        {
            Instance = this;
        }

        public void InitUIController(UICharacterController uiController)
        {
            controller = uiController;
            controller.Jump.onClick.AddListener(Jump);
            controller.Fire.onClick.AddListener(CheckShoot);
        }

        #region Singleton
        public static Player Instance { get; set; }
        #endregion

        private void Start()
        {
            arrowPool = new List<Arrow>();
            for (int i=0; i<arrowsCount; i++)
            {
               var arrowTemp = Instantiate(arrow, arrowSpawnPoint);
                arrowPool.Add(arrowTemp);
                arrowTemp.gameObject.SetActive(false);
            }

            health.OnTakeHit += TakeHit;
            buffReciever.OnBuffsChanged += ApplyBuffs;
        }
        
        private void ApplyBuffs()
        {
            var forceBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Force);
            var damageBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Damage);
            var armorBuff = buffReciever.Buffs.Find(t => t.type == BuffType.Armor);
            bonusForce = forceBuff == null ? 0 : forceBuff.additiveBonus;
            bonusHealth = armorBuff == null ? 0 : armorBuff.additiveBonus;
            health.SetHealth((int)bonusHealth);
            bonusDamage = damageBuff == null ? 0 : damageBuff.additiveBonus;
        }

        private void TakeHit(int damage, GameObject attacker)
        {
            animator.SetBool("Damage",true);
            animator.SetTrigger("TakeHit");
            isBlockMovement = true;
            rigidbody.AddForce(transform.position.x < attacker.transform.position.x ? 
                new Vector2(-damageForce, 0):new Vector2(damageForce,0), ForceMode2D.Impulse);
        }
        public void UnblockMovment()
        {
            isBlockMovement = false;
            animator.SetBool("Damage", false);
        }
        void FixedUpdate()
        {
            Move();
            animator.SetFloat("Speed", Mathf.Abs(direction.x));
            CheckFall();
        }

        private Arrow GetArrowFromPool()
        {
            if (arrowPool.Count>0)
            {
                var arrowTemp = arrowPool[0];
                arrowPool.Remove(arrowTemp);
                arrowTemp.gameObject.SetActive(true);
                arrowTemp.transform.parent = null;
                arrowTemp.transform.position = arrowSpawnPoint.transform.position;
                return arrowTemp;
            }
            return Instantiate(arrow, arrowSpawnPoint.position, Quaternion.identity);
        }

        public void ReturnArrowToPool(Arrow arrowTemp)
        {
            if (!arrowPool.Contains(arrowTemp))
                arrowPool.Add(arrowTemp);
            arrowTemp.transform.parent = arrowSpawnPoint;
            arrowTemp.transform.position = arrowSpawnPoint.transform.position;
            arrowTemp.gameObject.SetActive(false);
        }
        void CheckFall()
        {
            if (transform.position.y < minimalHeight && isCheatMode)
            {
                rigidbody.velocity = new Vector2(0, 0);
                transform.position = new Vector3(0, 0, 0);
            }
            else if (transform.position.y < minimalHeight && !isCheatMode)
                Destroy(gameObject);
        }
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
                GameManager.Instance.OnClickPause();
            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
#endif
        }
        private void Move()
        {
            animator.SetBool("isGrounded", groundDetection.isGrounded);
            if (!isJumping && !groundDetection.isGrounded)
                animator.SetTrigger("StartFall");
            isJumping = isJumping && !groundDetection.isGrounded;
            direction = Vector3.zero;
#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.A))
                direction = Vector3.left;
            if (Input.GetKey(KeyCode.D))
                direction = Vector3.right;
#endif
            if (controller.Left.IsPressed)
                direction = Vector3.left;
            if (controller.Right.IsPressed)
                direction = Vector3.right;
            direction *= speed;
            direction.y = rigidbody.velocity.y;
            if(!isBlockMovement)
                 rigidbody.velocity = direction;

            if (direction.x > 0)
                spriteRenderer.flipX = false;
            if (direction.x < 0)
                spriteRenderer.flipX = true;
        }
        private void Jump()
        {
            if (groundDetection.isGrounded)
            {
                rigidbody.AddForce(Vector2.up * (force + bonusForce), ForceMode2D.Impulse);
                animator.SetTrigger("StartJump");
                isJumping = true;
            }
        }
        private void CheckShoot()
        {
            if (!isCooldown)
            {
                animator.SetTrigger("StartShoot");
            }
        }

        public void InitArrow()
        {
            currentArrow = GetArrowFromPool();
            currentArrow.SetImpulse(Vector2.right,0, 0, this);
        }
        private void Shoot()
        {
            currentArrow.SetImpulse
                (Vector2.right, spriteRenderer.flipX ?
                -force * shootForce : force * shootForce, (int)bonusDamage, this);
            StartCoroutine(ReloadProcess());
        }
        //private IEnumerator Cooldown()
        //{
          //  isCooldown = true;
            //yield return new WaitForSeconds(cooldown);
            //isCooldown = false;
        //}

    
    IEnumerator ReloadProcess()
    {
        isCooldown = true;
        float timer = cooldown;
        while (timer >= 0)
        {
            yield return null;
            timer -= Time.deltaTime;
            circleImage.fillAmount = timer / cooldown;
        }
        isCooldown = false;
    }

    private void OnDestroy()
        {
            playerCamera.transform.parent = null;
            playerCamera.enabled = true;
        }
}


