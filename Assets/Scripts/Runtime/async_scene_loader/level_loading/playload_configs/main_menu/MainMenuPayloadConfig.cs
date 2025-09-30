using UnityEngine.SceneManagement;

namespace Runtime
{
    public class MainMenuPayloadConfig : TransitionPayload
    {
        public override void OnWillLoad()
        {
            
            var gameManager = GameManager.Instance;
            UnityEngine.Assertions.Assert.IsNotNull(
                gameManager, "game manager is not found");

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
            UnityEngine.Assertions.Assert.IsNotNull(
                gameManager, "game manager is not found");
            
            gameManager.UIManager?.gameObject.SetActive(false);
        }
    }
}