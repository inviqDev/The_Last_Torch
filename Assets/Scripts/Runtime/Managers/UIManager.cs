using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Slider[] skillsCooldowns;

        public void Init()
        {
            UnityEngine.Assertions.Assert.IsTrue(skillsCooldowns.Length > 0, "skillsCooldowns is not set up");
            if (skillsCooldowns.Length > 0)
            {
                
            }
        }
    }
}
