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
    [SerializeField] private string animationName = "combatAttack";
    
    private float attackAnimationTime = 0f;
    private float nextAttackTime = 0f;
    
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        canDealDamage = false;
        dealtDamage = new List<GameObject>();
        
        attackAnimationTime = GetAttackAnimtionTime(animationName);
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
                    hit.transform.gameObject.GetComponent<EnemyScript>().DealDamage(nightStickDamage);
                }
            }
        }
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
    
    public override void MainAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            ThirdPersonController.instance.isAttacking = true;
            Debug.Log("Main attack yaptım");
            animator.SetTrigger("attack");
            
            nextAttackTime = Time.time + attackAnimationTime;
            
            //Debug.Log($"Next attack time: {nextAttackTime}, Current time: {Time.time}");
        }
        else
        {
           // Debug.Log($"Cooldown! Kalan süre: {nextAttackTime - Time.time:F2} saniye");
        }
    }
    
    public override void SecondaryAttack()
    {

    }
    
    private float GetAttackAnimtionTime(string name)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == name)
            {
                return clip.length;
            }
        }
        Debug.LogWarning($"Animasyon bulunamadı: {name}");
        return 1f;
    }
}