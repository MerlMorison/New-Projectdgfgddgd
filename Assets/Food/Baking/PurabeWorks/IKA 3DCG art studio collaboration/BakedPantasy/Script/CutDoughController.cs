using System.Runtime.CompilerServices;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public abstract class CutDoughController : DoughController
    {
        //成形済み生地制御の親クラス

        [SerializeField, Header("咀嚼効果音")]
        private AudioClip _seEat;
        [SerializeField, Header("焼成時間[s]")]
        private float _bakeTime = 10f;

        [UdonSynced(UdonSyncMode.None)]
        protected bool _baked = false;

        /* 食べるときの効果音 */
        public void PlaySEEatSub()
        {
            _audioSource.PlayOneShot(_seEat);
        }

        /// <summary>
        /// 食べる効果音再生
        /// </summary>
        protected void PlaySEEat()
        {
            ExecuteOnAll(nameof(PlaySEEatSub));
        }

        /* オーブンでのSE再生 */
        protected Oven _oven;

        /// <summary>
        /// 焼成時の効果音再生
        /// </summary>
        protected void PlaySEBake()
        {
            if (_oven)
            {
                _oven.PlaySEBake();
            }
        }

        public override void ResetAll()
        {
            _baked = false;
            _deltaTime = 0;
            _bakeTime = 0;
            base.ResetAll();
        }

        //焼成タイミングのState値
        protected abstract int BakeState
        {
            get;
        }

        //焼成タイミングかどうか
        public virtual bool IsReadyToBake()
        {
            return _state == BakeState;
        }

        protected virtual void Bake()
        {
            // 焼成
            if (_state == BakeState)
            {
                _state++;
                _baked = true;
                Synchronize();
                _doughPickup.EnablePickup();
                PlaySEBake();
                Log("焼成されました。");
            }
        }

        //二次発酵タイミングのState値
        protected virtual int FermentState
        {
            get;
        }

        //二次発酵準備ができているか
        public bool IsReadyToFerment()
        {
            return _state == FermentState;
        }

        public void Ferment()
        {
            // 二次発酵
            if (_state == FermentState)
            {
                _state++;
                Synchronize();
            }
        }

        /// <summary>
        /// オーブン皿に生地を固定する
        /// </summary>
        /// <param name="go">オーブン皿オブジェクト</param>
        /// <param name="doughName">生地の名前</param>
        protected void SetOnOvenTray(GameObject go, string doughName)
        {
            Log("オーブン皿です。");
            OvenTray ovenTray = go.GetComponent<OvenTray>();
            if (ovenTray != null)
            {
                //ホールド状況チェック
                if (ovenTray.IsHeld)
                {
                    return;
                }

                //固定処理
                Transform fixPoint = ovenTray.SetDough(_doughPickup);
                if (fixPoint != null)
                {
                    _state++;
                    Synchronize();
                    _doughPickup.StickOnTrayOrDish(fixPoint, ovenTray.ConstPoint);
                    Log(doughName + "生地をオーブン皿に固定しました。");
                }
            }
        }

        /// <summary>
        /// 二次発酵台に生地を乗せて発酵させる
        /// </summary>
        /// <param name="go">二次発酵台オブジェクト</param>
        /// <param name="doughName">生地の名前</param>
        protected void SetOnFermentPlace(GameObject go, string doughName)
        {
            Log("二次発酵台です。");
            FermentationBoardTrigger fermentation = go.GetComponent<FermentationBoardTrigger>();
            if (fermentation == null)
            {
                return;
            }

            // 二次発酵台に固定する
            Transform fixPoint = fermentation.SetDough(_doughPickup.gameObject.GetInstanceID());
            if (fixPoint != null && Networking.IsOwner(this.gameObject))
            {
                _state++;
                Synchronize();
                _doughPickup.SetFixed(fixPoint);
                Log(doughName + "を二次発酵台に固定しました。");
            }
        }

        /// <summary>
        /// 終了状態のstate番号
        /// </summary>
        protected virtual int EndState
        {
            get;
        }

        /// <summary>
        /// 食べる
        /// </summary>
        /// <param name="doughName">生地の名前</param>
        protected void Eat(string doughName)
        {
            if (_baked && _state > BakeState && _state < EndState)
            {
                //焼成済みの場合
                _state++;
                Synchronize();
                PlaySEEat();
                Log(doughName + "を食べました。");

                if (_state == EndState)
                {
                    //パンをリターン
                    ReturnThis();
                    Log(doughName + "をリターンしました。");
                }
            }
        }

        /* Stay監視用 */
        private const float _D_TIME = 0.2f; // Update監視間隔
        private float _deltaTime = 0f;
        private float _bakedSec = 0f;

        public void TriggerStay(GameObject target)
        {
            if (target.layer != _layer)
            {
                return;
            }

            _deltaTime += Time.deltaTime;

            //一定時間毎に処理
            if (_deltaTime > _D_TIME)
            {
                /* オーブン音源のセット */
                Oven oven = target.GetComponent<Oven>();
                if (oven)
                {
                    _oven = oven;
                }

                /* オーブン内部での焼成 */
                if (Networking.IsOwner(this.gameObject) && IsReadyToBake())
                {
                    Kitchenware kitchenware = target.GetComponent<Kitchenware>();
                    if (kitchenware && kitchenware.ItemType == 11)
                    {
                        //焼成時間計上
                        _bakedSec += _D_TIME;
                        if (_bakedSec > _bakeTime)
                        {
                            //焼成完了
                            Bake();
                            _bakedSec = 0f;
                        }
                    }
                }

                _deltaTime = 0f;
            }
        }
    }
}