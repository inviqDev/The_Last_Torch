using UnityEngine;

namespace Runtime
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private AbilitySlotsController abilitySlotsController;
        [SerializeField] private ExpProgressBar _expSlider;

        [SerializeField] private GameObject gameOverPanel;

        public void InitLevelUI(PlayerModel player)
        {
            abilitySlotsController.ResetAbilitySlots();
            _expSlider.Init(player);
        }

        public AbilitySlot GetAvailableAbilitySlot()
        {
            return abilitySlotsController.GetAvailableSlot();
        }

        public void TurnOffGameplayUI()
        {
            gameOverPanel.SetActive(false);
        }

        public void TurnOnGameplayUI()
        {
            gameOverPanel.SetActive(true);
        }
    }
}