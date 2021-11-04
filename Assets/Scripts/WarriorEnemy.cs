using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WarriorEnemy : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    public List<Transform> patrolPoints = new List<Transform>();
    private int currPatrolPoint;

    public int maxHealth;
    public int currentHealth;
    Animator animator;
    public RaycastWeapon weapon;
    public bool chaseAbility;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public bool inPosition;
    public float walkPointRange;
    public int patrolIdx;
    public string enemyType;
    public int respawnDelay;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;
    public EnemySpawner spawner;

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
        healthBar.SetMaxHealth(maxHealth);
        currPatrolPoint = 0;
        inPosition = false;
    }

    void Update()
    {
        //Debug.Log("test");
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (inPosition && !playerInSightRange && !playerInAttackRange) Patroling();
        if (chaseAbility && playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (inPosition && playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void Patroling()
    {
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
        currPatrolPoint = (currPatrolPoint + 1) % patrolPoints.Count;
    }

    private void AttackPlayer()
    {
        //Make sure enemy doesn't move
        animator.SetBool("isShooting", true);
        agent.SetDestination(transform.position);

        transform.LookAt(player);
        Debug.Log(name);
        weapon.raycastDest = player;

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
        spawner.cleanPatroliExist(enemyType, patrolIdx, respawnDelay);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}