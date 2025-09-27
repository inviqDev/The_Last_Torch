namespace Runtime
{
    public class SuperBoss : BossEnemy
    {
        // public Action OnSuperBossDeath;

        public override void LaunchOnCharacterDeathLogic()
        {
            base.LaunchOnCharacterDeathLogic();
            GameManager.Instance?.UIManager.ShowWinGamePanel();
        }
    }
}