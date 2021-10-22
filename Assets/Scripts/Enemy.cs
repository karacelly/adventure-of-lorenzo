using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    public Transform[] patrolPoints;
    private int currPatrolPoint;

    public int maxHealth;
    public int currentHealth;
    Animator animator;
    public RaycastWeapon weapon;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public GameObject rifle_shoot, rifle_walk;

    public HealthBar healthBar;

    bool changeDirection = false;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        //inventory = GetComponent<Inventory>();
        healthBar.SetMaxHealth(maxHealth);
        currPatrolPoint = 0;
        //agent.SetDestination(startPatrol.transform.position);
    }

    void Update()
    {
        //Debug.Log("test");
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void Patroling()
    {
        rifle_walk.SetActive(true);
        animator.SetBool("isShooting", false);
        if (agent.remainingDistance <= 0.1f)
        {
            changePatrolPoint();
        }
        
        transform.LookAt(patrolPoints[currPatrolPoint].position);
        agent.SetDestination(patrolPoints[currPatrolPoint].position);        
    }

    private void changePatrolPoint()
    {
        currPatrolPoint = (currPatrolPoint + 1) % patrolPoints.Length;
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        rifle_shoot.SetActive(true);
        rifle_walk.SetActive(false);

        animator.SetBool("isShooting", true);
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            weapon.StartFiring();
            if (weapon.isFiring)
            {
                weapon.UpdateFiring(Time.deltaTime);
            }
            weapon.UpdateBullets(Time.deltaTime);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        this.currentHealth -= damage;
        healthBar.SetHealth(this.currentHealth);
        Debug.Log("health rn " + currentHealth + " dmg" + damage);

        if (currentHealth <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
