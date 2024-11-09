
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class BowlBreadController : BowlController
    {
        [SerializeField, Header("Make Trigger")] private Collider _makeTrigger;
        [SerializeField, Header("Dough Position")] private Transform _doughPosition;

        [Space]

        [SerializeField, Header("卵を割る音")] private AudioClip _eggSE;
        [SerializeField, Header("液体を注ぐ音")] private AudioClip _liquidSE;

        [UdonSynced(UdonSyncMode.None)]
        private bool _hasFlour = false, _hasButter = false, _hasDryYeast = false, _hasEgg = false,
            _hasSalt = false, _hasSugar = false, _hasWater = false, _hasMilk = false;
        [UdonSynced(UdonSyncMode.None)]
        private int _state = 0; // たね生地の発酵すすみ具合
        [UdonSynced(UdonSyncMode.None)]
        private int _doughType = 0; // 生地モード
        // 1:バゲッド&フランスパン 2:食パン&クロワッサン 3:メロンパン
        [UdonSynced(UdonSyncMode.None)]
        private bool _makeable = false; // たねこねトリガー表示

        protected override void UpdateState()
        {
            _animator.SetBool("hasFlour", _hasFlour);
            _animator.SetBool("hasButter", _hasButter);
            _animator.SetBool("hasDryYeast", _hasDryYeast);
            _animator.SetBool("hasEgg", _hasEgg);
            _animator.SetBool("hasSalt", _hasSalt);
            _animator.SetBool("hasSugar", _hasSugar);
            _animator.SetBool("hasWater", _hasWater | _hasMilk);
            _animator.SetInteger("State", _state);
            _makeTrigger.gameObject.SetActive(_makeable);
        }

        public override bool AddIngredient(int itemType)
        {
            /* 材料の追加 true:追加成功 false:失敗 */

            if (_state > 0)
            {
                //材料を全て追加済みなら何もしない
                return false;
            }
            else if (!_hasFlour)
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
                case 3: //塩
                    if (!_hasSalt)
                    {
                        _hasSalt = true;
                        syncFlag = true;
                        ExecuteOnAll(nameof(PlayOtherSE));
                        Log("塩が入りました。");
                    }
                    break;
                case 4: //牛乳
                    if (!_hasMilk)
                    {
                        _hasMilk = true;
                        syncFlag = true;
                        ExecuteOnAll(nameof(PlayLiquidSE));
                        Log("牛乳が入りました。");
                    }
                    break;
                case 5: //水
                    if (!_hasWater)
                    {
                        _hasWater = true;
                        syncFlag = true;
                        ExecuteOnAll(nameof(PlayLiquidSE));
                        Log("水が入りました。");
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
                case 7: //ドライイースト
                    if (!_hasDryYeast)
                    {
                        _hasDryYeast = true;
                        syncFlag = true;
                        ExecuteOnAll(nameof(PlayOtherSE));
                        Log("ドライイーストが入りました。");
                    }
                    break;
                case 8: //卵
                    if (!_hasEgg)
                    {
                        _hasEgg = true;
                        syncFlag = true;
                        ExecuteOnAll(nameof(PlayEggSE));
                        Log("卵が入りました。");
                    }
                    break;
                default: //対象外
                    return false;
            }

            //最低必要材料が入ったらこねトリガー表示
            if (!_makeable && _hasFlour && _hasDryYeast && _hasWater && _hasSalt)
            {
                _makeable = true;
                syncFlag = true;
                Log("こねトリガーをONにします。");
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
            if (_state == 0 && _hasFlour && _hasDryYeast)
            {
                /* 材料追加中 */
                bool nextFlag = false;

                if (_hasEgg)
                {
                    /* メロンパンモード */
                    if (_hasSugar && _hasMilk && _hasButter)
                    {
                        _doughType = 3;
                        nextFlag = true;
                        Log("メロンパン生地を作ります。");
                    }
                }
                else
                {
                    /* その他のパン */
                    if (_hasSugar && _hasMilk && _hasButter)
                    {
                        //食パンまたはクロワッサンまたはピザ
                        _doughType = 1;
                        nextFlag = true;
                        Log("食パンかクロワッサンかピザの生地を作ります。");
                    }
                    else if(!_hasSugar && !_hasMilk && !_hasButter)
                    {
                        //バゲットまたはフランスパン
                        _doughType = 2;
                        nextFlag = true;
                        Log("バゲット生地を作ります。");
                    } else
                    {
                        Log("足りない材料があります。");
                    }
                }

                if (nextFlag)
                {
                    //次の工程へ進める
                    _state = 1;
                    _hasFlour = false;
                    Synchronize();
                    ExecuteOnAll(nameof(PlayMakeSE));
                }
            }
            else if (_state == 1)
            {
                /* 一次発酵 */
                _state++;
                _makeable = false;
                Synchronize();
                ExecuteOnAll(nameof(PlayMakeSE));
                Log("一次発酵(こねトリガーを5秒間非表示)");

                // 5秒後に再操作可能とする
                SendCustomEventDelayedSeconds(nameof(EnableMakeTrigger), 5.0f);
            }
            else if (_state == 2)
            {
                /* 発酵済みの生地取り出し */
                if (!_pantasyManager)
                {
                    LogError("PantasyManagerがセットされていません。");
                    return;
                }

                GameObject spawnedObject = _pantasyManager.Spawn(_doughType, _doughPosition);

                if (spawnedObject != null)
                {
                    ExecuteOnAll(nameof(PlayMakeSE));
                    ResetAll();
                    Log("生地が完成しました。");
                }
            }
        }

        /* SendCustomEvent...用メソッド */

        public void EnableMakeTrigger()
        {
            _makeable = true;
            Synchronize();
            Log("こねトリガーをONにします。");
        }

        public override void ResetAll()
        {
            ResetBools();
            _state = 0;
            _doughType = 0;
            _makeable = false;
            Synchronize();
            Log("リセットされました。");
        }

        private void ResetBools()
        {
            _hasFlour = _hasButter = _hasDryYeast = _hasEgg = _hasSalt = _hasSugar = _hasMilk = _hasWater = false;
        }

        /* サウンド再生 */

        public void PlayEggSE()
        {
            _audioSource.PlayOneShot(_eggSE);
        }

        public void PlayLiquidSE()
        {
            _audioSource.PlayOneShot(_liquidSE);
        }
    }
}