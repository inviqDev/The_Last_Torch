using TMPro;
using UnityEngine;

namespace Runtime
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timer;
        [SerializeField] private KillsBarInfo killsBarInfo;
        
        [SerializeField] private AbilitySlotsController abilitySlotsController;
        [SerializeField] private PlayerLevelProgressBar playerLevelSlider;
        [SerializeField] private PlayerStatsInfo playerStatsInfo;
        
        [SerializeField] private GameObject winGamePanel;
        [SerializeField] private GameObject gameOverPanel;

        private Timer _timer;
        
        public void InitLevelUI(PlayerModel player)
        {
            abilitySlotsController.ResetAbilitySlots();
            playerStatsInfo.Init(player);
            playerLevelSlider.Init(player);
            StartGameTimer();
        }

        public AbilitySlot GetAvailableAbilitySlot() => abilitySlotsController.GetAvailableSlot();

        private void StartGameTimer()
        {
            _timer = new Timer(this);
            
            _timer.OnAnyValueChanged += ChangeMainTimerValue;
            _timer.StartFromToTimer(0f, float.MaxValue, TimerType.Increasing);
        }

        private void ChangeMainTimerValue(float timerValue)
        {
            var totalSeconds = Mathf.FloorToInt(timerValue);
            
            var minutes = totalSeconds / 60;
            var seconds = totalSeconds % 60;
            var hundredths = Mathf.FloorToInt((timerValue * 100f) % 100f);

            timer.text = $"{minutes:0}:{seconds:00}:{hundredths:00}";
        }
        
        public void ChangeKillsBarInfo(Character enemy)
        {
            killsBarInfo.ChangeKillBarInfo(enemy);
        }

        public void TurnOffGameOverPanel()
        {
            gameOverPanel.SetActive(false);
            Time.timeScale = 0f;
        }

        public void TurnOnGameOverPanel()
        {
            Time.timeScale = 0f;
            gameOverPanel.SetActive(true);
        }

        public void ShowWinGamePanel()
        {
            winGamePanel.SetActive(true);
        }

        
    }
}