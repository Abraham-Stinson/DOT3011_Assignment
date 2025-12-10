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

public class EnemyScript : MonoBehaviour
{
    [SerializeField] private bool isEnemyCanDesicion;
    [Header("Animation")]
    private Animator animator;

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
    [SerializeField, Range(0f, 100f)] private float enemyChasingSpeed = 7f;
    [SerializeField, Range(0f, 50f)] private float rotationSpeed = 5f;
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCooldown = 1.5f;
    private bool isRotating = true;
    private Quaternion targetRotation;
    private float patrolTimer;

    [Header("DamageUI")]
    [SerializeField] private GameObject DamageUI;
    [SerializeField] private float minDamageUIScale = 1.0f;
    [SerializeField] private float maxDamageUIScale = 2.0f;
    [SerializeField] private float maxHealthRatioForMaxScale = 0.3f;

    [Header("Attack")]
    [SerializeField] private bool isPatternRandom;
    [SerializeField] private List<EnemyAttackSO> enemyAttackSOs;
    [SerializeField] private bool isEnemyCanDealDamage;
    private List<GameObject> dealtDamage;
    [SerializeField, Range(0f, 10f)] private float enemyWeaponRange;
    [SerializeField] private Transform[] enemyWeapon;
    [SerializeField] private bool afterTakeDamageIsEnemyGetStun;
    [SerializeField, Range(0, 5f)] private float stunTime = 1f;
    private float currentActiveDamage;
    private float stunClipDuration;
    [Header("Attack Cooldown")]
    float lastClickedTime;
    float lastComboTime;
    int comboCounter;
    float maxCombatTime = 0.5f;
    float maxClickTime = 0.2f;
    void Awake()
    {
        animator = GetComponent<Animator>();
        isEnemyCanDesicion = true;
        dealtDamage = new List<GameObject>();
    }
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
        UpdateStunClipDuration();

    }

    void Update()
    {

        HealthUI();
        //ApprochingToThePlayer();
        DesicionAI();

        CheckDealtDamage();
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
        if (GameManager.instance.gameState != EGameState.INGAME || !isEnemyCanDesicion) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerGO.transform.position);
        //Debug.Log("Mesafe: " + distanceToPlayer);

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
                if (distanceToPlayer <= chaseRange)
                    currentState = State.chase;
                break;

            case State.chase:
                MoveToPlayer();
                // BURASI ÖNEMLİ: Saldırıya geçiş
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.attack;
                    // Saldırıya geçer geçmez hareket animasyonlarını sustur
                    ChangeMovementAnimatorParameters(false, false, false);
                }
                break;

            case State.attack:
                Attack();
                // BURASI ÖNEMLİ: Saldırıdan çıkış (Hysteresis)
                // Oyuncu 3.0 birimden 3.1 birime çıktığı an koşmaya başlama.
                // Biraz pay bırak (attackRange + 1f). Yani 4.0 birim uzaklaşana kadar saldırmaya devam etsin.
                if (distanceToPlayer > attackRange + 1f)
                {
                    currentState = State.chase;
                }
                break;
        }
    }

    private void Patrol()
    {
        //PATROL 
        if (isRotating)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            ChangeMovementAnimatorParameters(true, false, false);//IDLE,WALK,CHASE
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isRotating = false;
                patrolTimer = Random.Range(patrolMinTime, patrolMaxTime);
            }
        }
        else
        {
            ChangeMovementAnimatorParameters(false, true, false);//IDLE,WALK,CHASE
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
            ChangeMovementAnimatorParameters(false, false, true);//IDLE,WALK,CHASE
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        }
        transform.Translate(Vector3.forward * enemyChasingSpeed * Time.deltaTime);

    }
    private void Attack()
    {
        ChangeMovementAnimatorParameters(false, false, false);
        Vector3 direction = playerGO.transform.position - transform.position;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        if (Time.time - lastClickedTime > attackCooldown)
        {
            // Saldırı Animasyonunu Başlat
            // Listenin dışına taşmayı önle
            if (comboCounter >= enemyAttackSOs.Count) comboCounter = 0;

            var currentAttackData = enemyAttackSOs[comboCounter];

            // Animator'a saldırı klibini yükle
            animator.runtimeAnimatorController = currentAttackData.animatorOV;

            // Animasyonu ZORLA oynat
            animator.Play("Attack_1", 0, 0f);

            // Hasarı ayarla
            currentActiveDamage = currentAttackData.damage;

            Debug.Log("Saldırı Yapıldı! Combo: " + comboCounter);

            // Bir sonraki vuruş için sayacı ve zamanı güncelle
            lastClickedTime = Time.time;

            // Combo sayacını artır (veya rastgele seç)
            if (isPatternRandom)
                comboCounter = Random.Range(0, enemyAttackSOs.Count);
            else
                comboCounter++;
        }
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

    public void DealDamage(float damage)//TAKEN DAMAGE
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
        if (afterTakeDamageIsEnemyGetStun)
        {
            StartCoroutine(Stun());
        }

    }
    IEnumerator Stun()
    {
        ChangeMovementAnimatorParameters(false, false, false);
        isEnemyCanDesicion = false;

        float multiplier = stunClipDuration / stunTime;

        if (stunTime <= 0) multiplier = 1;

        animator.SetFloat("stunSpeed", multiplier);
        animator.SetTrigger("stun");

        yield return new WaitForSeconds(stunTime);

        isEnemyCanDesicion = true;
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

    public void StartEnemyDealingDamage()
    {
        isEnemyCanDealDamage = true;
        //Debug.Log($"{gameObject.name}: StartEnemyDealingDamage() Can deal damage: {isEnemyCanDealDamage}");
    }
    public void FinishEnemyDealingDamage()
    {
        isEnemyCanDealDamage = false;
        dealtDamage.Clear();
        //Debug.Log($"{gameObject.name}: FinishEnemyDealingDamage() Can deal damage: {isEnemyCanDealDamage}");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (Transform weapon in enemyWeapon)
        {
            Gizmos.DrawLine(weapon.position, weapon.position - (-weapon.up) * enemyWeaponRange);
        }

    }

    #region Animation Method
    private void ChangeMovementAnimatorParameters(bool isIdle, bool isWalking, bool isChasing)
    {
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isMoving", isWalking);
        animator.SetBool("isChasing", isChasing);
    }
    #endregion
    private void CheckDealtDamage()
    {
        if (!isEnemyCanDealDamage) return;

        foreach (Transform weapon in enemyWeapon)
        {
            Collider[] hits = Physics.OverlapSphere(weapon.position, enemyWeaponRange);

            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                if (!dealtDamage.Contains(hit.gameObject))
                {
                    var playerHealth = hit.GetComponent<PlayerHealthManager>();

                    if (playerHealth != null)
                    {
                        dealtDamage.Add(hit.gameObject);

                        playerHealth.ModfiyHealth(-currentActiveDamage);

                        Debug.Log($"Player vuruldu! Hasar: {currentActiveDamage}");
                    }
                }
            }
        }
    }
    private void UpdateStunClipDuration()
    {
        if (animator == null || animator.runtimeAnimatorController == null) return;

        // Animator'daki tüm klipleri bir diziye al
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach (AnimationClip clip in clips)
        {
            // Clip isminde "Stun" geçiyorsa veya adı tam olarak "Stun" ise
            // (Küçük/büyük harf duyarlılığını kaldırmak için ToLower yapıyoruz)
            if (clip.name.ToLower().Contains("stun"))
            {
                stunClipDuration = clip.length;
                // Debug.Log($"Stun animasyonu bulundu: {clip.name}, Süresi: {stunClipDuration}");
                return; // Bulduk, döngüden çık
            }
        }

        // Eğer bulunamazsa hata vermemesi için varsayılan bir değer ata
        Debug.LogWarning(gameObject.name + ": 'Stun' isminde bir animasyon klibi bulunamadı!");
        stunClipDuration = 1f;
    }
}
