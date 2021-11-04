using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

	public int maxHealth;
	public int currentHealth;
    public int maxSkillPt;
    public int currentSkillPt;

    public Inventory inventory;
    public LineRenderer lineRenderer;
    Animator animator;

    private int coreItems;
    public static bool isDead;

	public HealthBar healthBar;
    public HealthBar skillBar;
    public Text coreItemUI;
    public GameObject shield;
    public RaycastWeapon weapon;
    public Transform aimLookAt;

    private bool firstSpecial = false;

    // Start is called before the first frame update
    void Start()
    {
        coreItems = 0;
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
            StartCoroutine(turnOnRadiusLine());
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

    public void TakeDamage(int damage)
	{
		currentHealth -= damage;

        if(currentHealth <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
        }
        else
        {
		    healthBar.SetHealth(currentHealth);
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

    internal void useSkillPotion()
    {
        Debug.Log("Use skill potion");

        currentSkillPt += 75;

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
