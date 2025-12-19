using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NightStickScript : WeaponBase
{
    Animator animator;

    bool canDealDamage;
    List<GameObject> dealtDamage;
    [SerializeField] float stickLenght;
    [SerializeField] float nightStickDamage;


    [Header("Combo")]
    [SerializeField] private List<AttackSO> combo;
    float lastClickedTime;
    float lastComboTime;
    int comboCounter;

    float maxCombatTime = 0.5f;
    float maxClickTime = 0.2f;
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        canDealDamage = false;
        dealtDamage = new List<GameObject>();

    }

    void Update()
    {
        if (canDealDamage)
        {
            RaycastHit hit;
            int layerMask = 1 << 7;

            if (Physics.Raycast(transform.position, -(-transform.right), out hit, stickLenght, layerMask))
            {
                if (!dealtDamage.Contains(hit.transform.gameObject))
                {
                    dealtDamage.Add(hit.transform.gameObject);
                    if (hit.transform.gameObject.GetComponent<EnemyScript>() != null)
                    {
<<<<<<< Updated upstream
                        hit.transform.gameObject.GetComponent<EnemyScript>().DealDamage(nightStickDamage);
=======
                        var playerStatisticSO  = GameManager.instance.characters[GameManager.instance.currentHeroIndex].playerStatisticsSO;
                        hit.transform.gameObject.GetComponent<EnemyScript>().TakeDamage(playerStatisticSO.damage*playerStatisticSO.damageMultiplayer);
                        Debug.Log($"Player StatisticSO: {playerStatisticSO.damage*playerStatisticSO.damageMultiplayer}");
>>>>>>> Stashed changes
                    }


                }
            }

        }

        ExitAttack();

    }
    public override void MainAttack()
    {
        if (Time.time - lastComboTime > maxCombatTime && comboCounter < combo.Count)
        {
            CancelInvoke("EndCombo");
            if (Time.time - lastClickedTime >= maxClickTime)
            {
                animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                animator.Play("Attack", 0, 0);
                comboCounter++;

                lastClickedTime = Time.time;

                if (comboCounter > combo.Count)
                {
                    comboCounter = 0;
                }
            }

        }
    }
    private void ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }
    private void EndCombo()
    {
        comboCounter = 0;
        lastComboTime = Time.time;
    }
    public override void SecondaryAttack()
    {

    }

    public override void UltimateAttack()
    {
        
    }

    public void StartDealDamage()
    {
        canDealDamage = true;
        dealtDamage.Clear();
        Debug.Log("Function: StartDealDamage() Can deal damage: " + canDealDamage);
    }

    public void EndDealDamage()
    {
        canDealDamage = false;
        Debug.Log("Function: EndDealDamage() Can deal damage: " + canDealDamage);
        ThirdPersonController.instance.isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position - (-transform.right) * stickLenght);
    }


}