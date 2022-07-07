using UnityEngine;

namespace DarkSouls.Inventory
{
    public interface IInventoryItem
    {
        Sprite GetIcon();
        string GetName();
    }

    public class Item : ScriptableObject, IInventoryItem
    {
        [Header("Item Information")]
        public Sprite Icon;

        public string Name;

        public Sprite GetIcon() => Icon;
        public string GetName() => Name;
    }
}
