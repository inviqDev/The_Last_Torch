using System.Linq;
using UnityEngine;

namespace Runtime
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private GameObject gameplayUI;
        
        [SerializeField] private AbilityConfig defaultAbilityConfig;
        [SerializeField] private AbilityUI[] abilityUIs;
        
        public void ResetAllAbilitiesUI()
        {
            UnityEngine.Assertions.Assert.IsNotNull(abilityUIs, "abilityUIs is not set in the inspector");
            for (var i = 0; i < abilityUIs.Length; i++)
            {
                abilityUIs[i].SetUpAbilityUI(defaultAbilityConfig, false);
            }
        }

        public AbilityUI GetAbilityUI()
        {
            var availableAbilityUI = abilityUIs.First(_ => _.IsActive == false);
            if (availableAbilityUI) return availableAbilityUI;
            
            print("There is no slot for this ability");
            return null;
        }

        // // // // // // // // //
        // Create "ON START GAME LOGIC" in a single method => call this method in GameManager logic on start game
        
        
        public void TurnOffGameplayUI()
        {
            gameplayUI.SetActive(false);
        }

        public void TurnOnGameplayUI()
        {
            gameplayUI.SetActive(true);
        }
    }
}