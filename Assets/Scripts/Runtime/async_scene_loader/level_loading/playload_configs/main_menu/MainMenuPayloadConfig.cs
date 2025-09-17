using UnityEngine;
using UnityEngine.SceneManagement;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Loading Scene/Payloads", fileName = "main_menu_payload")]
    public class MainMenuPayloadConfig : TransitionPayload
    {
        public override void OnWillLoad()
        {
            var gameManager = GameManager.Instance;
            MyAsserts.IsNotNull(gameManager, "game manager is not found");
            
            gameManager.Spawner?.StopSpawningEnemies();
        }

        public override void OnDidLoad(Scene scene)
        {
            var gameManager = GameManager.Instance;
            MyAsserts.IsNotNull(gameManager, "game manager is not found");
            
            gameManager.UIManager?.gameObject.SetActive(false);
        }
    }
}