
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ToastController : CutDoughController
    {
        [SerializeField, Header("ざりっ…")]
        private AudioClip _seEat2;

        // 二次発酵なし
        protected override int FermentState => -1;
        // 焼成タイミング
        protected override int BakeState => 1;
        // 終了状態
        protected override int EndState => 5;

        [UdonSynced(UdonSyncMode.None)]
        private bool _burned = false;

        /* 操作 */
        public override void UseDown()
        {
            if (_state > 1 && _state < EndState)
            {
                if (_burned)
                {
                    //焦げたパン
                    _state++;
                    Synchronize();
                    PlaySEEat2(); //ざりっ…
                    Log("焦げたトーストを食べました。");
                }
                else
                {
                    //正常なパン
                    Eat("トースト");
                }
            }
        }

        public override void TriggerEnter(GameObject go)
        {
            if (go.layer != _layer)
            {
                return;
            }

            Kitchenware kitchenware = go.GetComponent<Kitchenware>();
            if (kitchenware != null && kitchenware.ItemType == 9
                && _state == 0)
            {
                // オーブン皿に固定する
                SetOnOvenTray(go, "食パン");
            }
        }

        private void PlaySEOops()
        {
            if (_oven)
            {
                _oven.PlaySEOops();
            }
        }

        protected override void Bake()
        {
            //焼成
            if (!_baked)
            {
                _state++;
                _baked = true;
                Synchronize();
                PlaySEBake();
                Log("焼成されました。");
            }
            else
            {
                _burned = true;
                Synchronize();
                PlaySEOops();
                Log("トーストが焦げました。");
            }
        }

        //普通の焼きと焦げとで2回
        public override bool IsReadyToBake()
        {
            return _baked ? (_state == BakeState + 1) && !_burned : _state == BakeState;
        }

        /* 初期設定やリセット */
        protected override void Start()
        {
            _burned = false;
            _animator.SetBool("Burned", false);
            base.Start();
        }

        protected override void AfterSynchronize()
        {
            _animator.SetBool("Burned", _burned);
            base.AfterSynchronize();
        }

        public override void ResetAll()
        {
            _burned = false;
            base.ResetAll();
        }

        /* 効果音再生 */
        public void PlaySEEat2Sub()
        {
            _audioSource.PlayOneShot(_seEat2);
        }
        private void PlaySEEat2()
        {
            ExecuteOnAll(nameof(PlaySEEat2Sub));
        }
    }
}