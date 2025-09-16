// using UnityEngine;
//
// namespace Runtime._my_tests.async_scene_loader
// {
//     public class GameSceneHook : MonoBehaviour, ISceneLoadHook
//     {
//         private GameManager _gm;
//         public void OnSceneLoaded(TransitionPayload payload)
//         {
//             if (payload is LevelPayload p)
//             {
//                 if (!GameManager.Instance)
//                 {
//                     UnityEngine.Assertions.Assert.IsNotNull(GameManager.Instance, "GameManager is null");
//                     Debug.Break();
//                 } 
//                 
//                 _gm = GameManager.Instance;
//                 _gm.ApplyConfig(p.gameConfig);
//             }
//         }
//     }
// }