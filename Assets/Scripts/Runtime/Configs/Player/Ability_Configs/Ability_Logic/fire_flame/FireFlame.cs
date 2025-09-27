using UnityEngine;

namespace Runtime
{
    public class FireFlame : AbilityVFX
    {
        [SerializeField] private ParticleSystem fireFlameVFX;

        private EnemyModel _enemy;
        private Ability _ability;
        
        protected override void OnPlay(AbilityContext ctx)
        {
            _ability = ctx.Ability;
            
            var main = fireFlameVFX.main;
            main.stopAction = ParticleSystemStopAction.Callback;
            
            _enemy = ctx.InitialTarget;
            fireFlameVFX.transform.position = _enemy.transform.position;
            fireFlameVFX.transform.localScale = _enemy.transform.localScale * 1.5f;
            fireFlameVFX.transform.SetParent(_enemy.transform);
            
            _enemy.OnCharacterDeath += c => fireFlameVFX.transform.parent = null;
            _enemy.TakeDamage(_ability.Damage);
            
            fireFlameVFX.Play();
        }

        private void OnParticleSystemStopped()
        {
            _enemy.OnCharacterDeath -= c => fireFlameVFX.transform.parent = null;
            RaiseFinished(_ability);
        }
    }
}