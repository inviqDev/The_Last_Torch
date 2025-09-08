using UnityEngine;

namespace Runtime
{
    public class Impale : MonoBehaviour
    {
        [SerializeField] private AbilityConfig config;
        [SerializeField] private float spawnYOffset = 0f;
        
        private float _cooldown;
        private Timer timer;
        
        public void UseAbility(EnemyModel enemy)
        {
            if (!config || !config.prefab) return;
            
            if (_cooldown > 0f) return;

            var pos = enemy.transform.position; 
            pos.y += spawnYOffset;
            var impale = Instantiate(config.prefab, pos, Quaternion.identity);

            enemy.TakeDamage(config.damage);
            _cooldown = config.cooldown;
        }
    }
}

// private AbilityHolder holder;
// private Timer timer;
//         
// public override void Init(AbilityHolder abilityHolder)
// {
//     UnityEngine.Assertions.Assert.IsNotNull(abilityHolder, "abilityHolder is null");
//     holder = abilityHolder;
//             
//     timer ??= new Timer(this);
//     timer.TimerIsOver += OnTimerIsOver;
//     timer.OnAnyValueChanged += OnAbilityCooldown;
//             
//     timer.StartFromToTimer(0, cooldown, TimerType.Increasing);
// }
//         
// public override void Activate(EnemyModel enemy)
// {
//     base.Activate(enemy);
//     print($"{nameof(Impale)} is activated");
// }
//         
// private void OnAbilityCooldown(float cooldownValue)
// {
//     holder.MoveCooldownSlider(cooldownValue);
// }
