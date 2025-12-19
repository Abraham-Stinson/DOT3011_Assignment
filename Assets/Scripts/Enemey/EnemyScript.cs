using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class EnemyScript : MonoBehaviour, IDamageable
{


    [Header("Health")]
    public float maxHealth = 100f;
    private float enemyHealth;

    [Header("Health Bar")]
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private float reduceSpeed = 2f;

    [SerializeField] private GameObject playerGO;
    [SerializeField] private float patrolMinTime = 1f;
    [SerializeField] private float patrolMaxTime = 3f;

    private Camera cam;

    private enum State
    {
        chase,
        attack,
        idle
    }
    [Header("Enemey Movement")]
    [SerializeField] private State currentState;
    [SerializeField] private float lastAttackTime;
    [SerializeField, Range(0f, 100f)] private float enemyMovementSpeed = 5f;
    [SerializeField, Range(0f, 50f)] private float rotationSpeed = 5f;
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 1.5f;
    private bool isRotating = true;
    private Quaternion targetRotation;
    private float patrolTimer;

    [Header ("DamageUI")]
    [SerializeField] private GameObject DamageUI;
    [SerializeField] private float minDamageUIScale = 1.0f;
    [SerializeField] private float maxDamageUIScale = 2.0f;
    [SerializeField] private float maxHealthRatioForMaxScale = 0.3f;
    void Start()
    {
        cam = Camera.main;
        enemyHealth = maxHealth;
        /*healthBar = GameObject.Find("/healthBar");
        healthBarImage = GameObject.Find("/foregroundHB").GetComponent<Image>();*/
        /*if (playerGO == null)
        {
            FindPlayerGO();
        }*/
        FindPlayerGO();
        PickNewPatrolDirection();

    }

    void Update()
    {

        HealthUI();
        //ApprochingToThePlayer();
        DesicionAI();
    }
    private void FindPlayerGO()
    {
        playerGO = FindObjectOfType<ThirdPersonController>().gameObject;

    }
    private void HealthUI()
    {
        if (healthBar.activeSelf)
        {
            healthBarImage.fillAmount = Mathf.MoveTowards(healthBarImage.fillAmount, enemyHealth / maxHealth, reduceSpeed * Time.deltaTime);
            healthBar.transform.rotation = Quaternion.LookRotation(healthBar.transform.position - cam.transform.position);
        }
    }

    private void DesicionAI()
    {
        if (GameManager.instance.gameState != EGameState.INGAME) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerGO.transform.position);

        if (distanceToPlayer > chaseRange)
        {
            currentState = State.idle;
        }

        else if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
        {
            currentState = State.chase;
        }

        else if (distanceToPlayer <= attackRange)
        {
            currentState = State.attack;
        }

        switch (currentState)
        {
            case State.idle:
                Patrol();
                break;
            case State.chase:
                MoveToPlayer();
                break;
            case State.attack:
                Attack();
                break;
        }
    }

    private void Patrol()
    {
        //PATROL 
        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isRotating = false;
                patrolTimer = Random.Range(patrolMinTime, patrolMaxTime);
            }
        }
        else
        {
            transform.Translate(Vector3.forward * enemyMovementSpeed * Time.deltaTime);
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0)
            {
                PickNewPatrolDirection();
            }
        }

    }
    private void PickNewPatrolDirection()
    {
        float randomY = Random.Range(0, 360);
        targetRotation = Quaternion.Euler(0, randomY, 0);
        isRotating = true;
    }
    private void MoveToPlayer()
    {
        //approching to player
        Vector3 direction = playerGO.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        }
        transform.Translate(Vector3.forward * enemyMovementSpeed * Time.deltaTime);

    }
    private void Attack()
    {
        Debug.Log("Saldııır!");
    }

    /*private void ApprochingToThePlayer()
    {
        if (playerGO == null && GameManager.instance.gameState != EGameState.INGAME)
        {
            FindPlayerGO();
            return;
        }
        if (GameManager.instance.gameState == EGameState.INGAME)
        {
            Vector3 playerPosition = new Vector3(playerGO.transform.position.x, transform.position.y, playerGO.transform.position.z);
            Vector3 direction = (playerPosition - transform.position ).normalized;
            transform.position += direction * Time.deltaTime * enemyMovementSpeed;
            
        }


    }*/

<<<<<<< Updated upstream
    public void DealDamage(float damage)
=======
    public void TakeDamage(float damage)//TAKEN DAMAGE
>>>>>>> Stashed changes
    {
        enemyHealth -= damage;
        Debug.Log("Enemy Dealed damage" + damage);

        ShowOnUI(damage);
        if (enemyHealth <= 0)
        {
            Debug.Log("Düşman Öldü");
            Destroy(gameObject);
        }

        if (!healthBar.activeSelf)
        {
            healthBar.SetActive(true);
        }

    }

    void ShowOnUI(float damage)
    {
        float damageRatio = damage / maxHealth;
        
        float normalizedScaleRatio = Mathf.Clamp01(damageRatio / maxHealthRatioForMaxScale);
        
        float finalScale = Mathf.Lerp(minDamageUIScale, maxDamageUIScale, normalizedScaleRatio);

        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;
        
        GameObject dealedDamageUI = Instantiate(DamageUI, spawnPos, Quaternion.identity);
        
        var dealedDamageUIScript = dealedDamageUI.GetComponent<enemyDealedDamageUI>();
        
        if (dealedDamageUIScript != null)
        {
            dealedDamageUIScript.Initialize(damage, finalScale);
        }

    }
}
