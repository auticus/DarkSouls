using UnityEngine;

namespace DarkSouls.Inventory
{
    public class Item : ScriptableObject
    {
        [Header("Item Information")] 
        public Sprite Icon;
        public string Name;
    }
}
