using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class PauseGame : MonoBehaviour
    {
        private Button _button;
        
        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(TurnOnPause);
        }

        private void TurnOnPause()
        {
            Time.timeScale = Time.timeScale switch
            {
                0 => 1,
                1 => 0,
                _ => Time.timeScale
            };
        }
    }
}
