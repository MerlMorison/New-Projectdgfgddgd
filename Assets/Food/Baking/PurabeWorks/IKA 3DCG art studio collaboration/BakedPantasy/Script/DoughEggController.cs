
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DoughEggController : DoughController
    {
        [SerializeField, Header("カット生地の位置")]
        private Transform[] _miniDoughPositions;

        protected override void Start()
        {
            if (_miniDoughPositions.Length != 4)
            {
                LogError("_miniDoughPositionsの数が不正です。");
            }
        }

        public override void TriggerEnter(GameObject target)
        {
            if (target.layer != _layer)
            {
                return;
            }

            DoughFixOnBoard(target);

            Kitchenware kitchenware = target.GetComponent<Kitchenware>();
            if (kitchenware != null && kitchenware.ItemType == 2
                && _state == 3)
            {
                // スケッパーで切る
                Log("スケッパーです。");
                Process("スケッパーで切り分けられた。");
                // こね工程生地をリターン
                ReturnThis();
                // カット後の生地をSpawn
                SpawnDividedDough();
            }
        }

        public override void Make()
        {
            DoughMake();
        }

        private void SpawnDividedDough()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject spawned = _pantasyManager.Spawn(9, i, _miniDoughPositions[i]);
                if (spawned == null)
                {
                    return;
                }
            }
        }
    }
}