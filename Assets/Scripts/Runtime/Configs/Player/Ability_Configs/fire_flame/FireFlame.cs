namespace Runtime
{
    public class FireFlame : AbilityVfx
    {
        protected override void OnPlay(AbilityContext ctx)
        {
            var ability = ctx.Ability;
            var enemy = ctx.InitialTarget;

            GameManager.Instance?.SoundManager?.PlaySound(ability.HitSfxPoolKey, transform.position);
            GameManager.Instance?.ParticlesManager?.PlayParticle(ability.HitVfxPoolKey, enemy);
            
            enemy.TakeDamage(ability.Damage);
            RaiseFinished(ctx.Ability);
        }
    }
}