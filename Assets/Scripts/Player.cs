using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class Player : MonoBehaviour
{

	public int maxHealth;
	public int currentHealth;
    public int maxSkillPt;
    public int currentSkillPt;

    public Inventory inventory;
    public LineRenderer lineRenderer;
    Animator animator;

    public int coreItems;
    public static bool isDead;

	public HealthBar healthBar;
    public HealthBar skillBar;
    public Text coreItemUI;
    public GameObject shield;
    public RaycastWeapon weapon;
    public Transform aimLookAt;

    public CinemachineVirtualCamera vcam;
    private bool side = false;

    private bool firstSpecial = false;

    // Start is called before the first frame update
    void Start()
    {
        coreItems = 9;
		currentHealth = maxHealth;
        //inventory = GetComponent<Inventory>();
		healthBar.SetMaxHealth(maxHealth);
        animator = GetComponent<Animator>();

        isDead = false;
        shield.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        weapon.transform.LookAt(aimLookAt);

		if(Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.UseItem(1);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.UseItem(2);
        }
        else if(Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.UseItem(3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.UseItem(4);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            inventory.UseItem(5);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            inventory.UseItem(6);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if(firstSpecial && !performingSpecialSkill)
            {
                Debug.Log("Perform Special Skill");
                PerformSpecialSkill();
            }
            else if(firstSpecial && performingSpecialSkill)
            {
                //kasih text --> cannot perform special effect 
            }
            else
            {
                StartCoroutine(SetBombUIEnemyInRadius(transform.position));
                StartCoroutine(turnOnRadiusLine());
            }
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            var transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

            side = !side;

            if (side)
            {
                transposer.m_ScreenX += 0.5f;
            }
            else
            {
                transposer.m_ScreenX -= 0.5f;
            }
        }
    }

    IEnumerator turnOnRadiusLine()
    {
        firstSpecial = true;
        lineRenderer.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);

        lineRenderer.gameObject.SetActive(false);
        firstSpecial = false;
    }

    //PRIM
    List<GameObject> vertex = new List<GameObject>();
    Collider[] hitColliders, prevHitColliders;
    private static int V = 5;
    static int[] parent;

    //Effect
    public GameObject lightningEffect;

    //Radius Visualization
    public LineRenderer radiusLine;
    public int segments = 40;
    public float xradius = 10f;
    public float yradius = 10f;
    public float radius = 10f;
    private bool performingSpecialSkill = false;

    //Damage
    public int electricDamage = 125;


    public LayerMask whatIsEnemy;
    IEnumerator SpecialSkillDelay()
    {
        performingSpecialSkill = true;
        yield return new WaitForSeconds(7);
        performingSpecialSkill = false;
    }
    IEnumerator SetBombUIEnemyInRadius(Vector3 center)
    {
        float startTime = Time.time;
        float timeElapsed = 0;
        while (timeElapsed <= 5)
        {
            timeElapsed = Time.time - startTime;
            hitColliders = null;
            hitColliders = Physics.OverlapSphere(transform.position, radius, whatIsEnemy);
            V = hitColliders.Length;
            if (prevHitColliders != null)
            {
                foreach (var phc in prevHitColliders)
                {
                    bool flagExist = false;
                    foreach (var hc in hitColliders)
                    {
                        if (phc.Equals(hc))
                        {
                            flagExist = true;
                            break;
                        }
                    }
                    if (!flagExist)
                    {
                        phc.GetComponent<Enemy>().setInRange(false);
                    }
                }
            }
            foreach (var h in hitColliders)
            {
                h.GetComponent<Enemy>().setInRange(true);
            }

            prevHitColliders = hitColliders;
            yield return new WaitForSeconds(0);
        }
        foreach (var phc in prevHitColliders)
        {
            phc.GetComponent<Enemy>().setInRange(false);
        }
    }

    static int minKey(float[] key, bool[] mstSet)
    {
        // Initialize min value
        float min = float.MaxValue;
        int min_index = -1;

        for (int v = 0; v < V; v++)
        {
            if (mstSet[v] == false && key[v] < min)
            {
                min = key[v];
                min_index = v;
            }
        }
        return min_index;
    }

    static void primMST(List<List<float>> graph)
    {

        parent = new int[V];

        float[] key = new float[V];

        bool[] mstSet = new bool[V];

        for (int i = 0; i < V; i++)
        {
            key[i] = float.MaxValue;
            mstSet[i] = false;
        }

        key[0] = 0;
        parent[0] = -1;

        if (V < 2)
        {
            parent[0] = 0;
        }

        for (int count = 0; count < V - 1; count++)
        {
            int u = minKey(key, mstSet);
            //Debug.Log(u);
            //Debug.Log(mstSet[u]);
            mstSet[u] = true;

            for (int v = 0; v < V; v++)
            {
                if (((List<float>)graph[u])[v] != 0 && mstSet[v] == false
                    && ((List<float>)graph[u])[v] < key[v])
                {
                    parent[v] = u;
                    key[v] = ((List<float>)graph[u])[v];
                }
            }
        }
    }

    void GiveElectricDamageAndLightningStrike()
    {
        List<List<string>> vertexConnections = new List<List<string>>(V);
        for (int i = 0; i < V; i++)
        {
            vertexConnections.Add(new List<string>());
        }

        for (int i = V > 1 ? 1 : 0; i < V; i++)
        {
            string vertex1 = vertex[i].name;
            string vertex2 = vertex[parent[i]].name;
            vertexConnections[i].Add(vertex2);


            GameObject lightning = Instantiate(lightningEffect, vertex[parent[i]].transform.position, Quaternion.identity);
            DigitalRuby.LightningBolt.LightningBoltScript lightningScript = lightning.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
            lightningScript.StartObject = vertex[parent[i]].gameObject;
            lightningScript.EndObject = vertex[i].gameObject;
            //lightningScript.Duration = 5f;
            Destroy(lightning, 6f);

            if (V > 1) //Kalau Single Vertex gaperlu add parent karena diri dia sendiri
            {
                vertexConnections[parent[i]].Add(vertex1);
            }
        }
        for (int i = 0; i < V; i++)
        {
            if (vertex[i].tag != "Player")
            {
                Enemy t = vertex[i].GetComponent<Enemy>();
                t.TakeDamage(electricDamage * vertexConnections[i].Count);
            }
        }
        currentSkillPt -= 75;
    }
    void PerformSpecialSkill()
    {
        if (vertex.Count > 0)
            vertex.Clear();

        var adjacentList = new List<List<float>>();
        makeAdjacencyList(adjacentList);
        primMST(adjacentList);
        GiveElectricDamageAndLightningStrike();
    }

    void makeAdjacencyList(List<List<float>> adjacentList)
    {
        hitColliders = Physics.OverlapSphere(transform.position, radius, whatIsEnemy);

        foreach (Collider c in hitColliders)
        {
            vertex.Add(c.gameObject);
        }
        vertex.Add(gameObject);

        V = vertex.Count;

        foreach (var i in vertex)
        {
            var adjacentListRow = new List<float>();
            foreach (var j in vertex)
            {
                float edgeWeight;
                if (i.Equals(j))
                    edgeWeight = 0;
                else
                    edgeWeight = Vector3.Distance(j.transform.position, i.transform.position);
                adjacentListRow.Add(edgeWeight);
            }
            adjacentList.Add(adjacentListRow);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        GameObject g = hit.gameObject;
        if (g.tag.Equals("Item"))
        {
            Debug.Log(g.name);
            if (g.name.Contains("Core Item"))
            {
                coreItems++;
                coreItemUI.text = "CORE ITEM : 0" + coreItems + "/09";
            }
            //Inventory
            else
            {
                inventory.AddItem(g.name);
            }
            Destroy(g);
        }
    }

    public Image attackedEffect;

    public void TakeDamage(int damage)
	{
		currentHealth -= damage;

        if(currentHealth <= 0)
        {
            animator.SetBool("isDead", true);

            StartCoroutine(Wait());

            isDead = true;
        }
        else
        {
		    healthBar.SetHealth(currentHealth);
            Color spriteColor = attackedEffect.color;
            attackedEffect.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, 100);
            StartCoroutine(fadeOut(attackedEffect, 5f));
            Debug.Log("Player >> TakeDamage()");
        }
	}

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(2);
    }

    IEnumerator fadeOut(Image image, float duration)
    {
        float counter = 0;
        Color spriteColor = image.color;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, counter / duration);

            image.color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            yield return null;
        }
    }

    internal void useDamageMultiplier()
    {
        Debug.Log("Use damage multiplier");
        StartCoroutine(applyDamageMultiplier());
    }

    IEnumerator applyDamageMultiplier()
    {
        weapon.damage *= 2;

        yield return new WaitForSeconds(5);

        weapon.damage /= 2;
    }

    internal void usePainKiller()
    {
        Debug.Log("Use pain killer");
        StartCoroutine(applyPainKiller());
    }

    IEnumerator applyPainKiller()
    {
        int prevHealth = currentHealth;
        currentHealth += 450;

        for(int i=0; i<5; i++)
        {
            yield return new WaitForSeconds(1);
            currentHealth -= 90;
        }

        if (currentHealth >= prevHealth)
        {
            currentHealth = prevHealth;
        }
    }

    internal void useAmmo()
    {
        Debug.Log("Use ammo");
        weapon.maxAmmo += 30;
    }

    internal void useHealthPotion()
    {
        currentHealth += 200;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthBar.SetHealth(currentHealth);
        Debug.Log("Health potion used!");
    }

    internal void useSkillPotion(int skill)
    {
        Debug.Log("Use skill potion");

        currentSkillPt += skill;

        if (currentSkillPt > maxSkillPt)
        {
            currentSkillPt = maxSkillPt;
        }
    }

    internal void useShield()
    {
        Debug.Log("Use shield");
        StartCoroutine(activateShield());
    }

    IEnumerator activateShield()
    {
        shield.SetActive(true);

        yield return new WaitForSeconds(7);

        shield.SetActive(false);
    }
}
