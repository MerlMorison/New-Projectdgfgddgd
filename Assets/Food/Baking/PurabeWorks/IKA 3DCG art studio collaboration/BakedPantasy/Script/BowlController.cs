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
    public abstract class BowlController : ManualSyncUB
    {
        //ボウルの親クラス用
        [SerializeField, Header("ボウル種別"), Tooltip("1:パン生地用 2:クッキー生地用")]
        private int _type = 0;
        [SerializeField, Header("Mesh Animator")] protected Animator _animator;
        [SerializeField] protected AudioSource _audioSource;
        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")]
        protected int _layer = 13;
        [SerializeField, Header("その他の追加音")] private AudioClip _addSE;
        [SerializeField, Header("こねる音")] private AudioClip _makeSE;

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

        /// <summary>
        /// ボウル種別
        /// </summary>
        public int Type { get { return _type; } }

        /// <summary>
        /// 対象レイヤー
        /// </summary>
        public int Layer { get { return _layer; } }

        private void Start()
        {
            UpdateState();
        }
        protected abstract void UpdateState();

        protected override void AfterSynchronize()
        {
            UpdateState();
        }

        public abstract bool AddIngredient(int itemType);
        public abstract void Make();
        public abstract void ResetAll();

        protected void Log(string msg)
        {
            Debug.Log("[pura]BowlController : " + msg);
        }

        protected void LogError(string msg)
        {
            Debug.LogError("[pura]BowlController : " + msg);
        }

        /* SE */
        public void PlayOtherSE()
        {
            _audioSource.PlayOneShot(_addSE);
        }

        public void PlayMakeSE()
        {
            _audioSource.PlayOneShot(_makeSE);
        }
    }
}