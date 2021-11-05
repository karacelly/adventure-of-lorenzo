using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class Item
    {
        public string name;
        public Sprite sprite;
    }

    public class InventorySlot
    {
        public Item item;
        public int amount;
    }

    [System.Serializable]
    public class InventorySlotUI
    {
        public Image img;
        public Text amount;
    }

    public List<Item> itemsAvailable; //ada item apa aja
    public List<InventorySlotUI> inventorySlotsUI;
    public Player player;

    private List<InventorySlot> slots; //isi slot
    private int maxSlot = 6;

    public void AddItem(string itemName)
    {
        //cari dulu
        Item item = new Item();
        foreach (Item i in itemsAvailable)
        {
            if (itemName.ToLower().Contains(i.name.ToLower()))
            {
                item = i;
                Debug.Log("Ini ketemu " + itemName + " di itemsAvailable yaitu " + i.name);
                break;
            }
        }

        //masukin ke slot
        if (item != null)
        {
            for (int i = 0; i < maxSlot; i++)
            {
                if (slots[i].item == null)
                {
                    slots[i].item = item;
                    Debug.Log("ADDED");
                }

                if (slots[i].item == item)
                {
                    slots[i].amount++;
                    Debug.Log("increment");
                    break;
                }
                
            }
        }
        
        
    }

    public void UseItem(int itemSlot)
    {
        itemSlot--;
        InventorySlot use = slots[itemSlot];
        if (use.item != null)
        {
            if (use.item.name.ToLower().Equals("ammo"))
            {
                player.useAmmo();
            }
            else if (use.item.name.ToLower().Equals("healthpotion"))
            {
                player.useHealthPotion();
            }
            else if (use.item.name.ToLower().Equals("skillpotion"))
            {
                player.useSkillPotion(75);
            }
            else if (use.item.name.ToLower().Equals("shield"))
            {
                player.useShield();
            }
            else if (use.item.name.ToLower().Equals("painkiller"))
            {
                player.usePainKiller();
            }
            else if (use.item.name.ToLower().Equals("damagemultiplier"))
            {
                player.useDamageMultiplier();
            }
            use.amount--;
            inventorySlotsUI[itemSlot].amount.text = use.amount.ToString();
            if (use.amount <= 0)
                removeItem(itemSlot);
        }
    }

    private void removeItem(int itemSlot)
    {
        slots.RemoveAt(itemSlot);

        InventorySlot s = new InventorySlot();
        s.item = null;
        s.amount = 0;

        slots.Add(s);

        foreach(InventorySlot inv in slots)
        {
            if(inv.item != null)
            {
                Debug.Log(inv.item.name);
            }
        }

        updateInventoryUI();
    }

    void updateInventoryUI()
    {
        for (int i = 0; i < 6; i++)
        {
            inventorySlotsUI[i].img.sprite = null;
            inventorySlotsUI[i].amount.enabled = false;

            var color = inventorySlotsUI[i].img.color;
            color.a = 0f;
            inventorySlotsUI[i].img.color = color;
        }
        
        for (int i=0; i<6; i++)
        {
            if(slots[i].item != null)
            {
                inventorySlotsUI[i].img.sprite = slots[i].item.sprite;

                var color = inventorySlotsUI[i].img.color;
                color.a = 1f;
                inventorySlotsUI[i].img.color = color;
                inventorySlotsUI[i].amount.enabled = true;
                inventorySlotsUI[i].amount.text = slots[i].amount.ToString();
                //Debug.Log("item no " + i);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        slots = new List<InventorySlot>(maxSlot+1);
        initSlots();
    }

    private void initSlots()
    {
        for (int i = 0; i < maxSlot; i++)
        {
            InventorySlot s = new InventorySlot();
            s.item = null;
            s.amount = 0;

            slots.Add(s);
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateInventoryUI();
    }
}
