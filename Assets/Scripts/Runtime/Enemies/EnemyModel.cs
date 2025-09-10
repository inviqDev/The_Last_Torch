using System;
using UnityEngine;

namespace Runtime
{
    public class EnemyModel : CharacterBase
    {
        public event Action<EnemyModel> OnEnemyDeath;

        [SerializeField] private EnemyMovement mover;

        private float _angularSpeed;
        private float _acceleration;
        private float _stoppingDistance;
        protected float damage;
        
        public EnemyMovement Mover => mover;

        private void Start()
        {
            Debug.Assert(GameManager.Instance, "GameManager has not been found");
        }
        public void SetConfig(EnemyConfig config)
        {
            var player = GameManager.Instance.Player;
            
            health = config.maxHealth;
            currentHealth = health;
            moveSpeed = config.moveSpeed;
            
            healthBar.Init(this);

            _angularSpeed = config.angularSpeed;
            _acceleration = config.acceleration;
            _stoppingDistance = config.stoppingDistance;

            damage = config.damage;

            mover.ApplyMovementConfig(player,
                moveSpeed, _angularSpeed, _acceleration, _stoppingDistance);
        }

        public override void TakeDamage(float incomingDamage)
        {
            base.TakeDamage(incomingDamage);
            if (!(currentHealth <= 0)) return;
            
            LaunchOnEnemyDeathLogic();
        }

        private void LaunchOnEnemyDeathLogic()
        {
            var currentPos = transform.position;
            mover.StopAndReset();
            OnEnemyDeath?.Invoke(this);
                
            // Add DROP item logic here
            var drop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            drop.transform.position = currentPos;
            drop.GetComponent<BoxCollider>().isTrigger = true;
            drop.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        protected virtual void PerformAttack(CharacterBase target)
        {
        }
    }
}