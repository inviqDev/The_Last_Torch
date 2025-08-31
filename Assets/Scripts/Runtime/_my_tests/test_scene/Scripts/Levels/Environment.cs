using UnityEngine;

namespace Runtime._my_tests.test_scene.Scripts.Levels
{
    public class Environment : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        public Transform SpawnPoint => _playerSpawnPoint;
    }
}
