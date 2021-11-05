using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    //private void Awake()
    //{
        
        
    //}

    private void Start()
    {
        initEnemy(); 
    }

    void Update()
    {
        EnemyRoutinesCheck();
    }

    #region Enemy Profile

    Animator animator;
    public HealthBar healthBar;
    public int currentHealth, maxHealth;

    private bool isDead = false;
    private void initEnemy()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>().transform;
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        currPatrolPoint = 0;
        inPosition = false;
        isDead = true;

        rifle_walk.SetActive(true);
        rifle_shoot.SetActive(false);
        bomb.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        this.currentHealth -= damage;
        healthBar.SetHealth(this.currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    #endregion

    #region Enemy Attack, Chase, Patrol

    public RaycastWeapon weapon;
    public GameObject rifle_shoot, rifle_walk;
    public GameObject bomb;
    public EnemySpawner spawner;
    public Transform player;
    public NavMeshAgent agent;
    public LayerMask whatIsGround, whatIsPlayer;
    public List<Transform> patrolPoints = new List<Transform>();

    public bool chaseAbility, inPosition;
    public int patrolIdx, respawnDelay, chanceOfDrop;
    private int currPatrolPoint;

    public float timeBetweenAttacks;
    private bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private bool inRange, changeDirection = false;

    private void EnemyRoutinesCheck()
    {
        updateInRangeState();
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
        rifle_walk.SetActive(true);
        rifle_shoot.SetActive(false);

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
        rifle_shoot.SetActive(true);
        rifle_walk.SetActive(false);

        animator.SetBool("isShooting", true);
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            Debug.Log(name + " want to shoot");
            weapon.Shoot();
            //weapon.StartFiring();
            //if (weapon.isFiring)
            //{
            //    weapon.UpdateFiring(Time.deltaTime);
            //}

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }
    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    #endregion

    #region Enemy Special Effect Impact
    private void updateInRangeState()
    {
        bomb.SetActive(inRange);
    }

    public void setInRange(bool value)
    {
        inRange = value;
    }

    #endregion

    #region Enemy Die Logic

    public GameObject coreItem;

    public bool willDropItem()
    {
        if (Random.Range(1, 100) <= chanceOfDrop)
            return true;

        return false;
    }

    public void Die()
    {
        if (name.ToLower().Contains("mech"))
        {

        }
        else
        {
            isDead = true;
            Instantiate(coreItem, transform.position, Quaternion.identity);
            if (willDropItem())
            {
                DropItem dropItem = FindObjectOfType<DropItem>();
                Instantiate(dropItem.randomItemDrop(), transform.position, Quaternion.identity);
            }
            DestroyEnemy();
        }
    }

    private void DestroyEnemy()
    {
        spawner.cleanPatroliExist(name, patrolIdx, respawnDelay);
        Destroy(gameObject, 4f);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
