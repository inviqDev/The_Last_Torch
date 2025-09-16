using UnityEngine;
using UnityEngine.SceneManagement;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Loading Scene/Payloads", fileName = "main_menu_payload")]
    public class MainMenuPayloadConfig : TransitionPayload
    {
        
        public override void OnDidLoad(Scene scene)
        {
        }
    }
}