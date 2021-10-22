using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

	public int maxHealth;
	public int currentHealth;
    public Inventory inventory;

    private int coreItems;

	public HealthBar healthBar;
    public Text coreItemUI;

    // Start is called before the first frame update
    void Start()
    {
        coreItems = 0;
		currentHealth = maxHealth;
        //inventory = GetComponent<Inventory>();
		healthBar.SetMaxHealth(maxHealth);
        
    }

    // Update is called once per frame
    void Update()
    {
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

    void TakeDamage(int damage)
	{
		currentHealth -= damage;

		healthBar.SetHealth(currentHealth);
	}

    internal void useDamageMultiplier()
    {
        throw new NotImplementedException();
    }

    internal void usePainKiller()
    {
        throw new NotImplementedException();
    }

    internal void useAmmo()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    internal void useShield()
    {
        throw new NotImplementedException();
    }
}
