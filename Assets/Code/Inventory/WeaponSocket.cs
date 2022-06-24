using UnityEngine;

namespace DarkSouls.Inventory
{
    public class WeaponSocket : MonoBehaviour
    {
        public Transform ParentOverride;
        public bool LeftHandSocket;
        public bool RightHandSocket;

        public GameObject WeaponModel;

        public void LoadWeaponModel(Weapon weapon)
        {
            UnloadWeaponAndDestroy();

            if (weapon == null)
            {
                UnloadWeapon(); 
                return;
            }

            var model = Instantiate(weapon.ModelPrefab);
            if (model != null)
            {
                if (ParentOverride != null)
                {
                    model.transform.parent = ParentOverride;
                }
                else
                {
                    model.transform.parent = transform;
                }

                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            WeaponModel = model;
        }

        private void UnloadWeapon()
        {
            if (WeaponModel != null)
            {
                WeaponModel.SetActive(false);
            }
        }

        private void UnloadWeaponAndDestroy()
        {
            if (WeaponModel != null)
            {
                Destroy(WeaponModel);
            }
        }
    }
}
