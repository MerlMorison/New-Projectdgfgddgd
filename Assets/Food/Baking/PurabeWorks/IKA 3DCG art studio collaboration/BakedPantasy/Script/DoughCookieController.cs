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
    public class DoughCookieController : ManualSyncUB
    {
        //クッキー生地（切る前）
        [SerializeField] protected Animator _animator;
        [SerializeField, Header("音源")] protected AudioSource _audioSource;
        [SerializeField, Header("効果音")] protected AudioClip _se;
        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")] protected int _layer = 13;

        [SerializeField, Header("カット生地の位置")]
        private Transform[] _miniDoughPositions;

        [SerializeField, Header("判定トリガー")]
        private Collider _trigger;

        [UdonSynced(UdonSyncMode.None)]
        private bool _isCut = false;

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

        private void Start()
        {
            if (_miniDoughPositions.Length != 6)
            {
                LogError("_miniDoughPositionsの数が不正です。");
            }
        }

        public void TriggerEnter(GameObject target)
        {
            if (target.layer != _layer || _isCut)
            {
                return;
            }

            Kitchenware kitchenware = target.GetComponent<Kitchenware>();
            if (kitchenware && kitchenware.ItemType == 2)
            {
                Log("スケッパーで切り分けられました。");
                _isCut = true;
                PlaySE();
                Synchronize();

                // こね工程生地をリターン
                ReturnThis();
                // カット後の生地をSpawn
                SpawnDividedDough();
            }
        }

        private void SpawnDividedDough()
        {
            for (int i = 0; i < 6; i++)
            {
                GameObject spawned = _pantasyManager.Spawn(10, i, _miniDoughPositions[i]);
                if (spawned == null)
                {
                    return;
                }
            }
        }

        public void ResetAll()
        {
            _isCut = false;
            Synchronize();
        }

        private void ReturnThis()
        {
            SendCustomEventDelayedSeconds(nameof(ReturnThisSub), 0.5f);
        }

        public void ReturnThisSub()
        {
            GameObject parent = this.transform.parent.gameObject;
            GetOwner(parent);
            _pantasyManager.ReturnBread(parent);
        }

        protected override void AfterSynchronize()
        {
            _animator.SetBool("IsCut", _isCut);
            _trigger.enabled = !_isCut;
        }

        public void PlaySESub()
        {
            _audioSource.PlayOneShot(_se);
        }
        protected void PlaySE()
        {
            ExecuteOnAll(nameof(PlaySESub));
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]DoughCookieController : " + msg);
        }

        private void LogError(string msg)
        {
            Debug.LogError("[pura]DoughCookieController : " + msg);
        }
    }
}