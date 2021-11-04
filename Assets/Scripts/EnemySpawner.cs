using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public class EnemyPatrol
    {
        public GameObject enemy; //prefab
        public NavMeshAgent agent;
        public int patrolIdx;
        public Transform patrolPos;
        public bool arrived;

        public EnemyPatrol(GameObject enemy, NavMeshAgent agent, int patrolIdx, Transform patrolPos)
        {
            this.enemy = enemy;
            this.agent = agent;
            this.patrolIdx = patrolIdx;
            this.patrolPos = patrolPos;
            arrived = false;
        }
    }

    [System.Serializable]
    public class GeneratePatrol
    {
        public string enemyType;
        public GameObject enemyPrefabs;
        public Transform[] patrolPos;
        public Transform spawnPosition;
        public List<EnemyPatrol> enemyList;
        public bool[] inPos;
    }

    public List<GeneratePatrol> enemyGroup = new List<GeneratePatrol>();
    public Transform player;

    void Start()
    {
        foreach (GeneratePatrol p in enemyGroup)
        {
            p.inPos = new bool[p.patrolPos.Length];
            p.enemyList = new List<EnemyPatrol>();
        }
    }

    void Update()
    {
        foreach (GeneratePatrol p in enemyGroup)
        {
            if (p.enemyList.Count < p.patrolPos.Length) //cek smua enemy listnya udh di smua pos blm
                generateEnemy(p);
            updateEnemyArrival(p); 
        }
    }

    //mau cek udh smpe di start patrol blm
    void updateEnemyArrival(GeneratePatrol p)
    {
        foreach (EnemyPatrol e in p.enemyList)
        {
            if (e.enemy == null)
            {
                p.enemyList.Remove(e);
                break;
            }
            else
            {
                if (!e.arrived)
                {
                    if (Vector3.Distance(e.enemy.transform.position, e.patrolPos.position) < 0.5f)
                    {
                        e.arrived = true;
                        e.agent.SetDestination(e.enemy.transform.position);
                        e.enemy.GetComponent<Enemy>().inPosition = true;
                    }
                    else
                    {
                        e.agent.SetDestination(e.patrolPos.position);
                    }
                }

            }

        }
    }

    private void generateEnemy(GeneratePatrol p)
    {
        for(int i=0; i<p.inPos.Length; i++)
        {
            if(p.inPos[i] == false)
            {
                spawnEnemy(p, i);
                p.inPos[i] = true;
            }
        }
    }

    private void spawnEnemy(GeneratePatrol p, int posIdx)
    {
        Transform destination = p.patrolPos[posIdx];
        GameObject enemy = Instantiate(p.enemyPrefabs, p.spawnPosition.position, Quaternion.identity);
        
        Enemy e = enemy.GetComponent<Enemy>();
        e.patrolIdx = posIdx;

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();

        for (int i = 0; i < destination.transform.childCount; i++)
        {
            e.patrolPoints.Add(destination.transform.GetChild(i));
            e.player = player;
            e.spawner = this;
        }

        if (p.enemyType.Equals("Kyle"))
        {
            e.chaseAbility = false;
        }
        else
        {
            e.chaseAbility = true;
        }
        agent.transform.LookAt(destination);
        agent.SetDestination(destination.position);

        p.enemyList.Add(new EnemyPatrol(enemy, agent, posIdx, destination));
    }

    GeneratePatrol findPatroli(string enemyType)
    {
        foreach (GeneratePatrol p in enemyGroup)
        {
            if (enemyType.ToLower().Contains(p.enemyType.ToLower()))
                return p;
        }
        return null;
    }

    public void cleanPatroliExist(string enemyType, int patrolIndex, int delay)
    {
        StartCoroutine(makePatrolPointEmpty(enemyType, patrolIndex, delay));
    }

    public IEnumerator makePatrolPointEmpty(string enemyType, int patrolIndex, int delay)
    {
        GeneratePatrol p = findPatroli(enemyType);
        if (p != null)
        {
            //Debug.Log("Start Delay for " + delay) ;
            yield return new WaitForSeconds(delay);
            //Debug.Log("End Delay");
            p.inPos[patrolIndex] = false;
        }

        yield return null;
    }
}
