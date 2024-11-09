
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DoughPlainController : DoughController
    {
        [SerializeField, Header("クロワッサン用カット生地の位置")]
        private Transform[] _croissantDoughPositions;

        [SerializeField, Header("効果音2")]
        private AudioClip _se2;

        [UdonSynced(UdonSyncMode.None)]
        private bool _isRolled = false; //こね棒で接触されたか

        [UdonSynced(UdonSyncMode.None)]
        private bool _hasOliveOil = false, _hasBasil = false,
            _hasCheese = false, _hasTomatoSauce = false; //ピザ材料

        /* 効果音再生 */
        public void PlaySE2Sub()
        {
            _audioSource.PlayOneShot(_se2);
        }

        private void PlaySE2()
        {
            ExecuteOnAll(nameof(PlaySE2Sub));
        }

        protected override void Start()
        {
            if (_croissantDoughPositions.Length != 7)
            {
                LogError("_croissantDoughPositionsの数が不正です。");
            }
            _animator.SetBool("IsRolled", false);
            base.Start();
        }

        protected override void AfterSynchronize()
        {
            _animator.SetBool("IsRolled", _isRolled);
            _animator.SetBool("hasOliveOil", _hasOliveOil);
            _animator.SetBool("hasBasil", _hasBasil);
            _animator.SetBool("hasCheese", _hasCheese);
            _animator.SetBool("hasTomatoSauce", _hasTomatoSauce);
            base.AfterSynchronize();
        }

        public override void TriggerEnter(GameObject target)
        {
            if (target.layer != _layer)
            {
                return;
            }

            //こね板への固定
            DoughFixOnBoard(target);

            //調理ツール処理
            Kitchenware kitchenware = target.GetComponent<Kitchenware>();
            if (kitchenware != null)
            {
                if (kitchenware.ItemType == 3)
                {
                    // めん棒でのばす
                    Log("めん棒です。");
                    if (!_isRolled && (_state == 2 || _state == 3))
                    {
                        // クロワッサン,食パン工程
                        _isRolled = true;
                        Process("めん棒で延ばされました。");
                        return;
                    }
                }
                else if (kitchenware.ItemType == 4)
                {
                    // 丸カッターで切る
                    Log("丸カッターです。");
                    if (_isRolled && _state == 3)
                    {
                        // クロワッサン工程
                        Process("丸カッターでカットされました。");
                        // 生地をリターン
                        ReturnThis();
                        // カット後の生地をSpawn
                        SpawnCutDough();
                        return;
                    }
                }
                else if (kitchenware.ItemType == 5)
                {
                    // 食パン型に入れる
                    Log("食パン型です。");
                    if (_isRolled && _state == 5)
                    {
                        //型に生地を入れる
                        LoafPanTrigger trigger = target.GetComponent<LoafPanTrigger>();
                        if (trigger && trigger.AddDough())
                        {
                            //生地を追加した
                            _state++;
                            Synchronize();
                            Log("食パン生地が型に入りました。");
                            // 生地をリターン
                            ReturnThis();
                            return;
                        }
                    }
                }
            }

            //ピザ材料対応
            if (_state == 3 && !_isRolled && _hasTomatoSauce && !_hasBasil)
            {
                Ingredient ingredient = target.GetComponent<Ingredient>();
                if (ingredient && ingredient.ItemType == 10)
                {
                    Log("バジルが入りました。");
                    _hasBasil = true;
                    Synchronize();
                    PlaySE2();
                    SpawnIfPizzaCompleted();
                }
            }
        }

        /// <summary>
        /// ピザ材料パーティクル処理
        /// </summary>
        /// <param name="target"></param>
        public override void ParticleCollision(GameObject target)
        {
            Ingredient ingredient = target.GetComponent<Ingredient>();
            if (ingredient)
            {
                int itemType = ingredient.ItemType;

                switch (itemType)
                {
                    case 9: //トマトソース
                        if (!_hasTomatoSauce && _state == 2)
                        {
                            _hasTomatoSauce = true;
                            _state++;
                            Synchronize();
                            PlaySE2();
                            Log("トマトソースが入りました。");
                        }
                        break;
                    case 11: //チーズ
                        if (_hasTomatoSauce && !_hasCheese && _state == 3)
                        {
                            Log("チーズが入りました。");
                            _hasCheese = true;
                            Synchronize();
                            PlaySE2();
                            SpawnIfPizzaCompleted();
                        }
                        break;
                    case 12: //オリーブオイル
                        if (_hasTomatoSauce && !_hasOliveOil && _state == 3)
                        {
                            Log("オリーブオイルが入りました。");
                            _hasOliveOil = true;
                            Synchronize();
                            PlaySE2();
                            SpawnIfPizzaCompleted();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// ピザ生地が完成したら次の生地をSpawnして終了
        /// </summary>
        private void SpawnIfPizzaCompleted()
        {
            if (_state == 3 && _hasTomatoSauce && _hasBasil && _hasCheese && _hasOliveOil)
            {
                //ピザ生地をSpawn
                GameObject spawned = _pantasyManager.Spawn(12, this.transform);

                if(spawned != null)
                {
                    _state++;
                    Synchronize();
                    Log("ピザ生地が完成しました。");

                    //リターン
                    ReturnThis();
                }
            }
        }

        public override void Make()
        {
            DoughMake();

            if (_state == 5 && _isRolled)
            {
                // 食パン1/3完成
                Log("食パン用の生地ができました。");
                // こね板から外す
                Log("こね板から外します。");
                GetOwner(_doughPickup.gameObject);
                _doughPickup.Release();
            }
        }

        public override void ResetAll()
        {
            _isRolled = false;
            _hasOliveOil = _hasBasil = _hasCheese = _hasTomatoSauce = false;
            base.ResetAll();
        }

        private void SpawnCutDough()
        {
            for (int i = 0; i < 7; i++)
            {
                GameObject spawned = _pantasyManager.Spawn(8, i, _croissantDoughPositions[i]);
                if (spawned == null)
                {
                    return;
                }
            }
        }
    }
}