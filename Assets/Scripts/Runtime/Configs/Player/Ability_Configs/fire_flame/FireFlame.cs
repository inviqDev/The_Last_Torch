namespace Runtime
{
    public class FireFlame : AbilityVFX
    {
        protected override void OnPlay(AbilityContext ctx)
        {
            var ability = ctx.Ability;
            var enemy = ctx.InitialTarget;

            GameManager.Instance?.SoundManager?.PlaySound(ability.AbilityHitSoundPoolKey, transform.position);
            GameManager.Instance?.ParticlesManager?.PlayParticle(ability.HitParticlePoolKey, enemy);
            
            enemy.TakeDamage(ability.Damage);
            RaiseFinished(ctx.Ability);
        }
    }
}