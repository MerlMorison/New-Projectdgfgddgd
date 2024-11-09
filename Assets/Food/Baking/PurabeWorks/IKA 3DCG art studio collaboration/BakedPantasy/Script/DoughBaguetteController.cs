
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DoughBaguetteController : DoughController
    {
        [SerializeField, Header("ミニバゲット用カット生地の位置")]
        private Transform[] _miniBaguetteDoughPositions;
        protected override void Start()
        {
            if (_miniBaguetteDoughPositions.Length != 4)
            {
                LogError("_miniBaguetteDoughPositionsの数が不正です。");
            }
            base.Start();
        }

        public override void TriggerEnter(GameObject target)
        {
            if (target.layer != _layer)
            {
                return;
            }

            DoughFixOnBoard(target);

            Kitchenware kitchenware = target.GetComponent<Kitchenware>();
            if (kitchenware != null)
            {
                if (kitchenware.ItemType == 2)
                {
                    // スケッパーで切る
                    Log("スケッパーです。");
                    if (_state == 3)
                    {
                        // カット後の生地をSpawn
                        if (SpawnDividedDough())
                        {
                            // ミニバゲット工程
                            Process("スケッパーで切り分けられた。");
                            // こね工程生地をリターン
                            ReturnThis();
                        }
                    }
                }
                else if (kitchenware.ItemType == 6)
                {
                    // バゲット二次発酵布
                    Log("バゲット二次発酵布です。");
                    if (_state == 3)
                    {
                        // バゲット工程
                        // バゲット生地を布に出現
                        FermentationClothBaguette fermentation = target.GetComponent<FermentationClothBaguette>();
                        if (fermentation != null && !fermentation.IsOccupied())
                        {
                            // 二次発酵布で生地をSpawn
                            if (fermentation.SpawnInPlaces())
                            {
                                // 元生地の処理
                                Process("バゲットの二次発酵が開始されます。");
                                // こね工程生地をリターン
                                ReturnThis();
                            }
                        }
                    }
                }
            }
        }

        public override void Make()
        {
            DoughMake();

            if (_state == 3)
            {
                // 長バゲット工程
                Log("こね板から外します。");
                GetOwner(_doughPickup.gameObject);
                _doughPickup.Release();
            }
        }

        private bool SpawnDividedDough()
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject spawned = _pantasyManager.Spawn(7, i, _miniBaguetteDoughPositions[i]);
                if(spawned == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}