using System.Linq;
using UnityEngine;

namespace Runtime
{
    public class UIManager : Singleton<UIManager>
    {
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
    }
}