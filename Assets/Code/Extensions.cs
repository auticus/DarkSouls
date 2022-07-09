using UnityEngine;

namespace DarkSouls
{
    public static class Extensions
    {
        public static void SetLayer(this GameObject parent, int layer, bool includeChildren = false, bool includeInactive = false)
        {
            parent.layer = layer;
            if (!includeChildren) return;
            
            foreach (var trans in parent.transform.GetComponentsInChildren<Transform>(includeInactive))
            {
                trans.gameObject.layer = layer;
            }
        }
    }
}
