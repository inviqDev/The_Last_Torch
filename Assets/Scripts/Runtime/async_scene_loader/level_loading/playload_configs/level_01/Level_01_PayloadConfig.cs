using UnityEngine;
using UnityEngine.SceneManagement;
using MyAsserts = UnityEngine.Assertions.Assert;

namespace Runtime
{
    public class Level_01_PayloadConfig : TransitionPayload
    {
        public override void OnWillLoad()
        {
            var gameManager = GameManager.Instance;
            MyAsserts.IsNotNull(gameManager, "game manager is not found");

            if (gameManager.Player)
            {
                Destroy(gameManager.Player.gameObject);
            }

            if (gameManager.LevelEnv)
            {
                Destroy(gameManager.LevelEnv.gameObject);
            }
            
            Pool.Instance?.RestartPool();
            
            gameManager.Spawner?.StopSpawningEnemies();
            gameManager.UIManager?.LaunchStopGameLogic();
        }
        
        public override void OnDidLoad(Scene scene)
        {
            var gameManager = GameManager.Instance;
            MyAsserts.IsNotNull(gameManager, "game manager is not found");
            
            gameManager.UIManager?.gameObject.SetActive(true);
            GameManager.Instance?.Initialize();
        }
    }
}