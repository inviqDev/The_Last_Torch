using UnityEngine;
using UnityEngine.SceneManagement;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Loading Level/Payload Config/Level 01", fileName = "level 01_payload_config")]
    public class Level_01_PayloadConfig : TransitionPayload
    {
        public override void OnDidLoad(Scene scene)
        {
            GameManager.Instance?.Init();
        }
    }
}