using System;
using System.Collections;
using System.Linq;
using AOT.Utls;
using AOT.Views;
using DG.Tweening;
using log4net.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_StageClear : MonoBehaviour
{
    [SerializeField]private View _view_StageClearMgr;
    private View_StageClearMgr StageClearMgr { get; set; }
    [SerializeField]private PlayerLevel Player;
    private void Start()
    {
        StageClearMgr = new View_StageClearMgr(_view_StageClearMgr, () => XDebug.Log($"{nameof(View_StageClearMgr)} Clicked!"));
    }

    [Button]
    public void OnAddExp(int exp)
    {
        StartCoroutine(PlayUpgrade());
        IEnumerator PlayUpgrade()
        {
            var r = Player.AddExp(exp);
            yield return StageClearMgr.PlayUpgrade("test", 3, r, null);
        }
    }

    [Button]public void OnReset() => Player.Reset();


    [Serializable]private class PlayerLevel 
    {
        [SerializeField] private LevelField[] LevelFields;
        private PlayerUpgradeHandler _upgradeHandler;
        public bool IsLevelUp { get; private set; }
        private PlayerUpgradeHandler UpgradeHandler =>
            _upgradeHandler ?? (_upgradeHandler = new PlayerUpgradeHandler(LevelFields));

        public int GetPlayerLevel() => UpgradeHandler.CurrentLevel.Level;
        public int GetPlayerExp() => UpgradeHandler.Exp;
        public int GetMaxExp(int level) => Array.Find(LevelFields, l => l.Level == level)?.MaxExp ?? -1;
        public int GetMaxExpOfCurrentLevel() => UpgradeHandler.CurrentLevel.MaxExp;

        public UpgradingRecord AddExp(int exp)
        {
            if (UpgradeHandler.CurrentLevel == null) Reset();
            return UpgradeHandler.Upgrade(exp);
        }

        public void Reset() => UpgradeHandler.Reset();

        [Serializable]private class LevelField : IPlayerLevelField
        {
            public int Level;
            public int MaxExp;
            int IPlayerLevelField.Level => Level;
            int IPlayerLevelField.MaxExp => MaxExp;
        }
    }
}