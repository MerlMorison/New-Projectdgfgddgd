
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class LoafBreadController : DoughController
    {
        [SerializeField, Header("食パンのカット後の位置")]
        private Transform[] _toastPositions;

        protected override void Start()
        {
            if (_toastPositions.Length != 9)
            {
                LogError("_toastPositionsの数が不正です。");
            }
            base.Start();
        }

        public override void TriggerEnter(GameObject go)
        {
            if (go.layer != _layer)
            {
                return;
            }

            DoughFixOnBoard(go); // こね板固定

            Kitchenware kitchenware = go.GetComponent<Kitchenware>();
            if (kitchenware != null && kitchenware.ItemType == 10
                && _state == 1)
            {
                // パン包丁で切る
                Process("パン包丁で切りました。");
                // 食パン1枚をSpawn
                SpawnToast();
                // リターン
                ReturnThis();
            }
        }

        private void SpawnToast()
        {
            for (int i = 0; i < _toastPositions.Length; i++)
            {
                GameObject spawned = _pantasyManager.Spawn(11, _toastPositions[i]);
                if (spawned == null)
                {
                    return;
                }
            }
        }
    }
}