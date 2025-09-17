using UnityEngine;
using TMPro;

namespace Runtime
{
    public class KillsBarInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI totalKills;
        [SerializeField] private TextMeshProUGUI commonKills;
        [SerializeField] private TextMeshProUGUI uniqueKills;
        [SerializeField] private TextMeshProUGUI bossKills;

        private int totalAmount;
        private int commonAmount;
        private int uniqueAmount;
        private int bossAmount;

        private void Start()
        {
            totalAmount = 0;
            ChangeTotalKillsInfo(totalAmount);

            commonAmount = 0;
            ChangeCommonKillsInfo(commonAmount);

            uniqueAmount = 0;
            ChangeUniqueKillsInfo(uniqueAmount);

            bossAmount = 0;
            ChangeBossKillsInfo(bossAmount);
        }

        private void ChangeTotalKillsInfo(int newAmount)
        {
            totalKills.text = $" : {newAmount.ToString()}";
        }

        private void ChangeCommonKillsInfo(int newAmount)
        {
            commonKills.text = $" : {newAmount.ToString()}";
        }

        private void ChangeUniqueKillsInfo(int newAmount)
        {
            uniqueKills.text = $" : {newAmount.ToString()}";
        }

        private void ChangeBossKillsInfo(int newAmount)
        {
            bossKills.text = $" : {newAmount.ToString()}";
        }

        public void ChangeKillBarInfo(Character enemy)
        {
            totalAmount++;
            ChangeTotalKillsInfo(totalAmount);

            switch (enemy)
            {
                case BasicEnemy:
                    commonAmount++;
                    ChangeCommonKillsInfo(commonAmount);
                    break;
                case RedUniqueEnemy or BlueUniqueEnemy or YellowUniqueEnemy:
                    uniqueAmount++;
                    ChangeUniqueKillsInfo(uniqueAmount);
                    break;
                case BossEnemy or SuperBoss:
                    bossAmount++;
                    ChangeBossKillsInfo(bossAmount);
                    break;
            }
        }

        public void ResetKillBarInfo()
        {
            totalAmount = 0;
            commonAmount = 0;
            uniqueAmount = 0;
            bossAmount = 0;
            
            ChangeTotalKillsInfo(totalAmount);
            ChangeCommonKillsInfo(commonAmount);
            ChangeUniqueKillsInfo(uniqueAmount);
            ChangeBossKillsInfo(bossAmount);
        }
}
}