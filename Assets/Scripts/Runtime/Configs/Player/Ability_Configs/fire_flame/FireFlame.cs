namespace Runtime
{
    public class FireFlame : AbilityVFX
    {
        protected override void OnPlay(AbilityContext ctx)
        {
            var ability = ctx.Ability;
            var enemy = ctx.InitialTarget;

            GameManager.Instance?.SoundManager?.PlaySound(ability.SoundPoolKey, transform.position);
            GameManager.Instance?.ParticlesManager?.PlayParticle(ability.ParticlesPoolKey, enemy);
            
            enemy.TakeDamage(ability.Damage);
            RaiseFinished(ctx.Ability);
        }
    }
}