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
        //�{�E���̐e�N���X�p
        [SerializeField, Header("�{�E�����"), Tooltip("1:�p�����n�p 2:�N�b�L�[���n�p")]
        private int _type = 0;
        [SerializeField, Header("Mesh Animator")] protected Animator _animator;
        [SerializeField] protected AudioSource _audioSource;
        [SerializeField, Header("�Ώۃ��C���["), Tooltip("13: Pickup")]
        protected int _layer = 13;
        [SerializeField, Header("���̑��̒ǉ���")] private AudioClip _addSE;
        [SerializeField, Header("���˂鉹")] private AudioClip _makeSE;

        [HideInInspector]
        public PantasyManager _pantasyManager = null; // �p�����n��Spawn�Ǘ�

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

            Log("PantasyManager��������܂���ł����B");
        }

        internal void RecordSelf()
        {
            UdonSharpEditorUtility.CopyProxyToUdon(this);
            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif

        /// <summary>
        /// �{�E�����
        /// </summary>
        public int Type { get { return _type; } }

        /// <summary>
        /// �Ώۃ��C���[
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