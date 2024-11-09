
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BowlCookieController : BowlController
    {
        [UdonSynced(UdonSyncMode.None)]
        private bool _hasFlour = false, _hasButter = false,
            _hasSugar = false, _completed = false;

        protected override void UpdateState()
        {
            _animator.SetBool("hasFlour", _hasFlour);
            _animator.SetBool("hasSugar", _hasSugar);
            _animator.SetBool("hasButter", _hasButter);
            _animator.SetBool("completed", _completed);
        }

        public override bool AddIngredient(int itemType)
        {
            /* 材料の追加 true:追加成功 false:失敗 */

            if (!_hasFlour)
            {
                if (itemType == 1)
                {
                    //小麦粉を追加
                    _hasFlour = true;
                    Synchronize();
                    ExecuteOnAll(nameof(PlayOtherSE));
                    Log("小麦粉が入りました。");
                    return true;
                }
                //他の材料は小麦粉より後
                return false;
            }

            bool syncFlag = false;

            switch (itemType)
            {
                case 2: //砂糖
                    if (!_hasSugar)
                    {
                        _hasSugar = true;
                        syncFlag = true;
                        ExecuteOnAll(nameof(PlayOtherSE));
                        Log("砂糖が入りました。");
                    }
                    break;
                case 6: //バター
                    if (!_hasButter)
                    {
                        _hasButter = true;
                        syncFlag = true;
                        ExecuteOnAll(nameof(PlayOtherSE));
                        Log("バターが入りました。");
                    }
                    break;
                default: //対象外
                    return false;
            }

            if (syncFlag)
            {
                Synchronize();
                return true;
            }

            return false;
        }

        public override void Make()
        {
            /* 材料をすべて添加後に木べらでかき混ぜる */
            if (_hasFlour && _hasSugar && _hasButter)
            {
                //初期化
                _hasFlour = _hasSugar = _hasButter = false;
                //完成モード
                _completed = true;

                ExecuteOnAll(nameof(PlayMakeSE));
                Log("かき混ぜて完成します。");
                Synchronize();
            }
        }

        public void TriggerEnter(GameObject target)
        {
            if (target.layer != _layer || !_completed)
            {
                return;
            }

            KneadBoard kb = target.GetComponent<KneadBoard>();
            if (kb)
            {
                Log("クッキー生地をこね板にSpawnします。");
                //クッキー生地をSpawn
                GameObject spawned = _pantasyManager.Spawn(4, kb.FixPoint);
                if (spawned != null)
                {
                    //ボウルをリセット
                    ResetAll();
                }
            }
        }

        public override void ResetAll()
        {
            _hasFlour = _hasSugar = _hasButter = _completed = false;
            Synchronize();
            Log("リセットされました。");
        }
    }
}