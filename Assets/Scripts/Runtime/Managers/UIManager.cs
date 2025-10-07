using System;
using TMPro;
using UnityEngine;

namespace Runtime
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject gameplayUI;

        [SerializeField] private TextMeshProUGUI timer;
        [SerializeField] private KillsBarInfo killsBarInfo;

        [SerializeField] private AbilitySlotsController abilitySlotsController;
        [SerializeField] private PlayerLevelProgressBar playerLevelSlider;
        [SerializeField] private PlayerStatsInfo playerStatsInfo;

        [SerializeField] private GameObject winGamePanel;
        [SerializeField] private GameObject gameOverPanel;

        private Timer _timer;

        public void InitializeLevelUI(Player player)
        {
            abilitySlotsController.ResetAbilitySlots();
            playerStatsInfo.Initialize(player);
            playerLevelSlider.Initialize(player);
            
            ShowGameplayUI(true);
            gameOverPanel.SetActive(false);
            winGamePanel.SetActive(false);

            StartGameTimer();
        }

        public AbilitySlot GetAvailableAbilitySlot() => abilitySlotsController.GetAvailableSlot();

        private void StartGameTimer()
        {
            _timer = new Timer(this);
            _timer.OnAnyValueChanged += ChangeMainTimerValue;
            _timer.StartFromToTimer(0f, float.MaxValue, TimerType.Increasing);
        }

        public void LaunchStopGameLogic()
        {
            _timer?.StopTimer();

            abilitySlotsController.ResetAbilitySlots();
            killsBarInfo.ResetKillBarInfo();
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

        private void ShowGameplayUI(bool show)
        {
            gameplayUI.SetActive(show);
        }

        public void ShowGameOverUI()
        {
            ShowGameplayUI(false);
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        public void ShowWinGameUI()
        {
            ShowGameplayUI(false);
            winGamePanel.SetActive(true);
            Time.timeScale = 0f;
        }

        private void OnDisable()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}