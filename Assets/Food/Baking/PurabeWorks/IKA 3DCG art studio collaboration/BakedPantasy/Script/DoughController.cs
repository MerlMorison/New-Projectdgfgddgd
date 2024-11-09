using UdonSharp;
using UnityEngine;

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
    public abstract class DoughController : ManualSyncUB
    {
        //生地コントローラー
        [SerializeField, Header("Pickupハンドラ")] protected DoughPickup _doughPickup;
        [SerializeField] protected Animator _animator;
        [SerializeField, Header("音源")] protected AudioSource _audioSource;
        [SerializeField, Header("効果音")] protected AudioClip _se;
        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")] protected int _layer = 13;

        [UdonSynced(UdonSyncMode.None)]
        protected int _state = 0; //状態進行度

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

        protected virtual void Start()
        {
            _state = 0;
            _animator.SetInteger("State", 0);
        }

        protected override void AfterSynchronize()
        {
            _animator.SetInteger("State", _state);
        }

        public void PlaySESub()
        {
            _audioSource.PlayOneShot(_se);
        }
        protected void PlaySE()
        {
            ExecuteOnAll(nameof(PlaySESub));
        }

        public virtual void TriggerEnter(GameObject go) { }

        protected void Log(string msg)
        {
            Debug.Log("[pura]DoughController : " + msg);
        }

        protected void LogError(string msg)
        {
            Debug.LogError("[pura]DoughController : " + msg);
        }

        /// <summary>
        /// 3秒遅延させてからReturn処理実行
        /// </summary>
        protected void ReturnThis()
        {
            Log("オブジェクトをReturnします。ID= " + this.gameObject.GetInstanceID());
            _doughPickup.DisablePickup();
            SendCustomEventDelayedSeconds(nameof(ReturnThisSub), 3f);
        }

        /// <summary>
        /// 指定秒数遅延させてからReturn処理実行
        /// </summary>
        /// <param name="seconds">遅延時間[s]</param>
        protected void ReturnThis(float seconds)
        {
            SendCustomEventDelayedSeconds(nameof(ReturnThisSub), seconds);
        }

        public void ReturnThisSub()
        {
            GetOwner(_doughPickup.gameObject);
            _pantasyManager.ReturnBread(_doughPickup.gameObject);
        }

        public virtual void ResetAll()
        {
            _state = 0;
            Synchronize();
        }

        public virtual void Make() { }

        public virtual void UseDown() { }

        public virtual void ParticleCollision(GameObject target) { }

        protected void DoughMake()
        {
            // 手で広げるor折りたたむ
            Process("手で広げるor折りたたみました。");
        }

        protected void DoughFixOnBoard(GameObject target)
        {
            // こね板に固定する
            KneadBoard kb = target.GetComponent<KneadBoard>();
            if (kb != null && _state == 0)
            {
                Transform fixPoint;
                if(DoughType == 11)
                {
                    //食パン塊
                    fixPoint = kb.LoafFixPoint;
                } else
                {
                    //そのほか
                    fixPoint = kb.FixPoint;
                }
                if (fixPoint != null)
                {
                    GetOwner(_doughPickup.gameObject);
                    _doughPickup.SetFixed(fixPoint);
                    _state++;
                    Synchronize();
                    Log("こね板に固定されました。");
                }
            }
        }

        /// <summary>
        /// 工程を進める
        /// </summary>
        /// <param name="message">ログ表示内容</param>
        protected void Process(string message)
        {
            _state++;
            Synchronize();
            PlaySE();
            Log(message);
        }

        /// <summary>
        /// 生地タイプ
        /// </summary>
        public int DoughType
        {
            get { return _doughPickup.DoughType; }
        }

        /// <summary>
        /// 対象レイヤー
        /// </summary>
        public int Layer
        {
            get { return _layer; }
        }

        /// <summary>
        /// メロンパンのクッキー装着
        /// </summary>
        public virtual void UseCookie() { }
    }
}