using UnityEngine;

namespace Runtime
{
    public class LevelEnvironment : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        public Vector3 PlayerSpawnPoint => _playerSpawnPoint.position;
    }
}
