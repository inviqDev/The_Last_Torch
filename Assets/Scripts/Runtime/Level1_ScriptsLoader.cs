using System.Threading;
using UnityEngine;

namespace Runtime
{
    [DefaultExecutionOrder(-799)]
    public class Level1_ScriptsLoader : MonoBehaviour
    {

        private CancellationTokenSource _cts;
        private void Awake()
        {
            // spawn level
            // spawn player
            // spawn spawner
            // spawn enemies
            
            var player = PlayerManager.Instance?.CurrentPlayer;
            UnityEngine.Assertions.Assert.IsNotNull(player, "Player is null !");
            
            player.GetComponent<CameraMover>().Init(Camera.main, player.transform);
        }
    }
}