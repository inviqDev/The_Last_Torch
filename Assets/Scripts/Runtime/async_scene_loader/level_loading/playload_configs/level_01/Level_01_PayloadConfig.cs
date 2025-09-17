using UnityEngine;
using UnityEngine.SceneManagement;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    [CreateAssetMenu(menuName = "Loading Level/Payload Config/Level 01", fileName = "level 01_payload_config")]
    public class Level_01_PayloadConfig : TransitionPayload
    {
        public override void OnWillLoad()
        {
            var gameManager = GameManager.Instance;
            MyAsserts.IsNotNull(gameManager, "game manager is not found");
            
            gameManager.Spawner?.StopSpawningEnemies();
            gameManager.UIManager?.LaunchStopGameLogic();
        }
        
        public override void OnDidLoad(Scene scene)
        {
            var gameManager = GameManager.Instance;
            MyAsserts.IsNotNull(gameManager, "game manager is not found");
            
            gameManager.UIManager?.gameObject.SetActive(true);
            GameManager.Instance?.Init();
        }
    }
}