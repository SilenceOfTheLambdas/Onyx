using System;
using Inventory_System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEquipmentManager : MonoBehaviour {
    
    [Header("User Interface")]
    [SerializeField] private Image weaponSlotImage;
    private                  Inventory            _playerInventory;
    private                  EquipmentInventory   _equipmentInventory;
    [SerializeField] private UIEquipmentInventory equipmentInventoryUI;
    public                   bool                 hasWeaponEquipped;
    public                   WeaponItem           weaponItem; // Store the item in-case we need it back
    public                   HelmetItem           head;
    public                   GameObject           chest;
    public                   GameObject           boots;

    private void Awake()
    {
        _equipmentInventory = new EquipmentInventory();
        _playerInventory = GetComponent<Player>().Inventory;
        equipmentInventoryUI.SetPlayer(GetComponent<Player>());
        equipmentInventoryUI.SetInventory(_equipmentInventory);
    }

    public void EquipWeapon(WeaponItem item)
    {
        weaponItem = item;
        hasWeaponEquipped = true;
        _equipmentInventory.AddItem(item);
    }

    public void EquipHelmet(HelmetItem helmetItem)
    {
        head = helmetItem;
        _equipmentInventory.AddItem(helmetItem);
    }

    public void UnEquip(Item item)
    {
        equipmentInventoryUI.hoverInterface.SetActive(false);
        _equipmentInventory.RemoveItem(item);
        if (item is WeaponItem)
        {
            hasWeaponEquipped = false;
            weaponItem = null;
            _playerInventory.AddItem(item);
        }

        if (item is HelmetItem)
        {
            head = null;
            _playerInventory.AddItem(item);
        }
    }
}