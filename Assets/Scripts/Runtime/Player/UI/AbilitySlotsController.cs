using System.Linq;
using UnityEngine;

namespace Runtime
{
    public class AbilitySlotsController : MonoBehaviour
    {
        [SerializeField] private AbilityConfig defaultAbilityConfig;
        [SerializeField] private AbilitySlot[] abilitySlots;
        
        public int AbilitySlotsCount => abilitySlots.Length;
        
        public void ResetAbilitySlots()
        {
            UnityEngine.Assertions.Assert.IsNotNull(abilitySlots, "abilitySlots is not set in the inspector");
            
            var defaultAbility = new Ability(defaultAbilityConfig);
            foreach (var s in abilitySlots)
            {
                s.SetUpAbilityUI(defaultAbility);
            }
        }

        public AbilitySlot GetAvailableSlot()
        {
            var availableAbilityUI = abilitySlots.First(_ => _.IsActive == false);
            if (availableAbilityUI) return availableAbilityUI;
            
            print("There is no slot for this ability");
            return null;
        }
        
        
    }
}