namespace Runtime
{
    public class SuperBoss : BossEnemy
    {
        // public Action OnSuperBossDeath;

        protected override void LaunchOnCharacterDeathLogic()
        {
            base.LaunchOnCharacterDeathLogic();
            GameManager.Instance?.UIManager.ShowWinGameUI();
        }
    }
}