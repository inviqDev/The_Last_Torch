using UnityEngine;

namespace Runtime
{
    public class AbilityVFX : MonoBehaviour
    {
        public virtual void UseAbility(Transform playerTransform, Transform enemyTransform)
        {
            print("ORIGINAL");
        }
    }
}