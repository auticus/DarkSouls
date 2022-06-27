using UnityEngine;

namespace DarkSouls.UI
{
    /// <summary>
    /// Controller placed on the Player that is responsible for updating the UI.
    /// </summary>
    public class UIController : MonoBehaviour
    {
        private Healthbar _healthBar;

        private void Start()
        {
            // health bar will reside on a canvas object outside of the player hierarchy.
            _healthBar = FindObjectOfType<Healthbar>(); //very inefficient method
        }

        /// <summary>
        /// Will set the health bar maximum.  Will adjust health the current health if it is higher than the max.
        /// </summary>
        /// <param name="value"></param>
        public void SetHealthBarMaximum(int value)
        {
            _healthBar.Max = value;
            if (_healthBar.Current > _healthBar.Max) _healthBar.Current = _healthBar.Max;
        }

        /// <summary>
        /// Will attempt to set the health bar value.  If it is less than zero or greater than the max, it will clamp the values.
        /// </summary>
        /// <param name="value"></param>
        public void SetHealthBarValue(int value)
        {
            if (value < 0) value = 0;
            if (value > _healthBar.Max) value = _healthBar.Max;
            _healthBar.Current = value;
        }
    }
}
