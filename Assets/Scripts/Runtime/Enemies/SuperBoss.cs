using System;

namespace Runtime
{
    public class SuperBoss : BossEnemy
    {
        // public Action OnSuperBossDeath;

        protected override void LaunchOnEnemyDeathLogic()
        {
            UIManager.Instance?.ShowWinGamePanel();
            // OnSuperBossDeath?.Invoke();
            // base.LaunchOnEnemyDeathLogic();
        }
    }
}