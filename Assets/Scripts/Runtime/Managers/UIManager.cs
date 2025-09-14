using TMPro;
using UnityEngine;

namespace Runtime
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private AbilitySlotsController abilitySlotsController;
        [SerializeField] private PlayerLevelProgressBar playerLevelSlider;
        [SerializeField] private PlayerStatsInfo playerStatsInfo;
        [SerializeField] private GameObject gameOverPanel;

        public void InitLevelUI(PlayerModel player)
        {
            abilitySlotsController.ResetAbilitySlots();
            playerStatsInfo.Init(player);
            playerLevelSlider.Init(player);
        }

        public AbilitySlot GetAvailableAbilitySlot()
        {
            return abilitySlotsController.GetAvailableSlot();
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
    }
}