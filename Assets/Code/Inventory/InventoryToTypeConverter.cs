namespace DarkSouls.Inventory
{
    public static class InventoryToTypeConverter
    {
        /// <summary>
        /// Extension method that will return the type of item back in <see cref="InventoryType"/> format.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static InventoryType ConvertItemToType(this Item item)
        {
            if (item is Weapon) return InventoryType.Weapons;
            return InventoryType.None;
        }
    }
}
