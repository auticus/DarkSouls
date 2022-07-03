using UnityEngine;
using UnityEngine.UI;

namespace DarkSouls.UI
{
    public class StatusBar : MonoBehaviour
    {
        private Slider _slider;
        private int _max;
        private int _current;

        /// <summary>
        /// Gets or sets the type of status bar this is.
        /// </summary>
        [field: SerializeField]
        public StatusBarTypes StatusBarType { get; set; }

        /// <summary>
        /// Gets or sets the maximum value of the Health Bar.
        /// </summary>
        public int Max
        {
            get => _max;
            set
            {
                _max = value;
                _slider.maxValue = _max;
            }
        }

        /// <summary>
        /// Gets or sets the current value of the Health Bar.
        /// </summary>
        public int Current
        {
            get => _current;
            set
            {
                _current = value;
                _slider.value = _current;
            }
        }

        private void Awake()
        {
            _slider = GetComponent<Slider>();
        }
    }
}
