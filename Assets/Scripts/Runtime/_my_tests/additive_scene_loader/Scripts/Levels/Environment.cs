using UnityEngine;

namespace Runtime._my_tests
{
    public class Environment : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        public Transform SpawnPoint => _playerSpawnPoint;
    }
}
