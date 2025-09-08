// using UnityEngine;
//
// namespace Runtime
// {
//     public class SkillsBar : MonoBehaviour
//     {
//         [SerializeField] private AbilityHolder[] skillSlots;
//
//         private int _nextSlotIndex;
//         
//         public int skillSlotsCount => skillSlots.Length;
//
//         private void Awake()
//         {
//             _nextSlotIndex = 0;
//         }
//
//         public void ResetToDefault()
//         {
//             foreach (var s in skillSlots)
//             {
//                 s.SetDefaultSkillBarView();
//             }
//         }
//
//         public void SetNewSkill(Ability ability)
//         {
//             skillSlots[_nextSlotIndex].SetUpSkill(ability);
//             _nextSlotIndex++;
//         }
//     }
// }
