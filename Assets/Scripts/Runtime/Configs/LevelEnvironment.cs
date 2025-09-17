using Unity.AI.Navigation;
using UnityEngine;

namespace Runtime
{
    public class LevelEnvironment : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnPoint;
        [SerializeField] private NavMeshSurface navMeshSurface;

        public Vector3 PlayerSpawnPoint => _playerSpawnPoint.position;

        public void RebakeNavMeshSurface()
        {
            if (!navMeshSurface)
            {
                UnityEngine.Assertions.Assert.IsNotNull(navMeshSurface,
                    "nav mesh surface component is missing");
            }

            navMeshSurface.RemoveData();
            navMeshSurface.BuildNavMesh();
        }

        private void OnDisable()
        {
            navMeshSurface?.RemoveData();
        }
    }
}