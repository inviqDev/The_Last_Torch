using System;

namespace Runtime
{
    public class SuperBoss : BossEnemy
    {
        // public Action OnSuperBossDeath;

        protected override void LaunchOnEnemyDeathLogic()
        {
            GameManager.Instance?.UIManager.ShowWinGamePanel();
            // OnSuperBossDeath?.Invoke();
            // base.LaunchOnEnemyDeathLogic();
        }
    }
}