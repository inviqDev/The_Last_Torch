using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-799)]
    public class Level1_ScriptsLoader : MonoBehaviour
    {
        private void Awake()
        {
            var player = PlayerManager.Instance?.CurrentPlayer;
            UnityEngine.Assertions.Assert.IsNotNull(player, "Player is null !");
            
            player.GetComponent<CameraMover>().Init(Camera.main, player.transform);
        }
    }
}