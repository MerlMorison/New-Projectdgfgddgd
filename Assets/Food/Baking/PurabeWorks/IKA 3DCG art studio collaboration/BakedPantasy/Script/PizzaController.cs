
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

#if UNITY_EDITOR
using UnityEditor;
#if UNITY_2021_2_OR_NEWER
using UnityEditor.SceneManagement;
#else
using UnityEditor.Experimental.SceneManagement;
#endif
using UdonSharpEditor;
#endif

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PizzaController : ManualSyncUB
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private PizzaPickup _pickup;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _se;
        [SerializeField, Header("焼成時間[s]")]
        private float _bakeTime = 10f;

        [UdonSynced(UdonSyncMode.None)]
        private int _state = 0;

        //焼成タイミング
        private const int _bakeState = 1;
        //ピザの切れ数
        private const int _cutPizzaNum = 8;

        [HideInInspector]
        public PantasyManager _pantasyManager = null; // パン生地のSpawn管理

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        private void OnValidate()
        {
            EditorApplication.delayCall += () => { if (this) this.FindPantasyManager(); };
        }

        private void FindPantasyManager()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) { return; }
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) { return; }
            if (PrefabUtility.IsPartOfPrefabAsset(this)) { return; }

            if (_pantasyManager) { return; }
            if (_pantasyManager = FindObjectOfType<PantasyManager>())
            {
                RecordSelf();
                return;
            }

            Log("PantasyManagerが見つかりませんでした。");
        }

        internal void RecordSelf()
        {
            UdonSharpEditorUtility.CopyProxyToUdon(this);
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif

        protected override void AfterSynchronize()
        {
            _animator.SetInteger("State", _state);
        }

        /* SE再生 */
        public void PlaySESub()
        {
            _audioSource.PlayOneShot(_se);
        }

        private void PlaySE()
        {
            ExecuteOnAll(nameof(PlaySESub));
        }

        /* オーブンでのSE再生 */
        private Oven _oven;

        /// <summary>
        /// 焼成SE再生用Ovenコンポ
        /// </summary>
        public Oven Oven
        {
            set { _oven = value; }
        }

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

        //焼成タイミングかどうか
        private bool IsReadyToBake()
        {
            return _state == _bakeState;
        }

        /// <summary>
        /// 焼成する
        /// </summary>
        private void Bake()
        {
            if (_state == _bakeState)
            {
                _state++;
                Synchronize();
                PlaySEBake();
                Log("焼成されました。");
            }
        }

        public void TriggerEnter(GameObject target)
        {
            if (_state == 0)
            {
                /* オーブン内部に固定 */
                Oven oven = target.GetComponent<Oven>();
                if (oven)
                {
                    //オーブンコンポをセット
                    _oven = oven;
                    //オーナ権限主のみ操作
                    if (Networking.IsOwner(this.gameObject))
                    {
                        SetToPoint(oven.PizzaFixpoint, "オーブン内");
                        return;
                    }
                }
            }
            else if (_state == 2 && Networking.IsOwner(this.gameObject))
            {
                /* こね板に固定 */
                KneadBoard kb = target.GetComponent<KneadBoard>();
                if (kb)
                {
                    SetToPoint(kb.FixPoint, "こね板");
                    return;
                }
            }
            else if (_state == 3 && Networking.IsOwner(this.gameObject))
            {
                /* ピザカッターで切る */
                Kitchenware kitchenware = target.GetComponent<Kitchenware>();
                if (kitchenware && kitchenware.ItemType == 4)
                {
                    _state++;
                    Synchronize();
                    PlaySE();

                    //リターン
                    ReturnThis();

                    //切れたピザをSpawn
                    SpawnCutPizzas();
                }
            }
        }

        private const float _D_TIME = 0.2f; // Update監視間隔
        private float _deltaTime = 0f;
        private float _bakedSec = 0f;

        public void TriggerStay(GameObject target)
        {
            _deltaTime += Time.deltaTime;

            if (_deltaTime > _D_TIME)
            {
                /* オーブン内部での焼成 */
                if (Networking.IsOwner(this.gameObject)
                    && IsReadyToBake())
                {
                    Oven oven = target.GetComponent<Oven>();
                    if (oven)
                    {
                        _bakedSec += _D_TIME;
                        if (_bakedSec > _bakeTime)
                        {
                            //時間になったら焼成
                            Bake();
                            _bakedSec = 0;
                        }
                    }
                }

                _deltaTime = 0;
            }
        }

        /// <summary>
        /// 指定された場所に固定する
        /// </summary>
        /// <param name="point">固定点</param>
        /// <param name="where">どこか</param>
        private void SetToPoint(Transform point, string where)
        {
            _state++;
            Synchronize();
            _pickup.MoveTo(point);
            Log(where + "に固定されました。");
        }

        /// <summary>
        /// 切れたピザをSpawn
        /// </summary>
        private void SpawnCutPizzas()
        {
            for (int i = 0; i < _cutPizzaNum; i++)
            {
                GameObject spawned = _pantasyManager.Spawn(13, i, this.transform);
                if (spawned == null)
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 3秒遅延させてからReturn処理実行
        /// </summary>
        protected void ReturnThis()
        {
            Log("オブジェクトをReturnします。ID= " + this.gameObject.GetInstanceID());
            SendCustomEventDelayedSeconds(nameof(ReturnThisSub), 3f);
        }

        public void ReturnThisSub()
        {
            GetOwner(_pickup.gameObject);
            _pantasyManager.ReturnBread(_pickup.gameObject);
        }

        public void ResetAll()
        {
            _state = 0;
            Synchronize();
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]PizzaController : " + msg);
        }
    }
}