using Player;
using UnityEditor;
using UnityEngine;

namespace Inventory_System
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Items/Create Weapon", order = 100)]
    public class WeaponItem : Item
    {
        [Header("Weapon Statistics")]
        
        [Tooltip("The amount of damage this weapon deals to an enemy when hit")]
        public int damage;
        
        [Tooltip("The maximum range this weapon can be used")] [Range(2f, 10f)]
        public float weaponRange;

        [Tooltip("Attack speed of the weapon (Attack every n seconds)")][Range(0.1f, 2f)]
        public float attackRate = 1f;

        [Space]
        [Header("Weapon Prefab")] [Tooltip("The prefab of the weapon that spawns when this weapon is equipped")]
        public GameObject equippedWeaponPrefab;

        public bool randomGeneration = false;

        public override void RandomlyGenerateItem()
        {
            randomGeneration = true;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(WeaponItem))]
    internal class WeaponItemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var weaponItem = (WeaponItem)target;
            if (weaponItem == null) return;


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Weapon Item Details");
            
            base.DrawDefaultInspector();
            
            if (weaponItem.randomGeneration)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Weapon Item Random Generation");
            }
        }
    }
#endif
}