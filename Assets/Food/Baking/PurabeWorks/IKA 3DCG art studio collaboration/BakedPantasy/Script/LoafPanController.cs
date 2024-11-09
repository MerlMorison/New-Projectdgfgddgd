
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
    public class LoafPanController : ManualSyncUB
    {
        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")]
        private int _layer = 13;
        [SerializeField] private Animator _animator;
        [SerializeField, Header("焼成時間[s]")]
        private float _bakeTime = 20f;
        [SerializeField] private LoafPanPickup _panPickup;

        [UdonSynced(UdonSyncMode.None)]
        private int _state = 0;
        [UdonSynced(UdonSyncMode.None)]
        private bool _initFlag = false;

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
        /// パン生地を追加する
        /// </summary>
        /// <returns>true:追加できた false:できない</returns>
        public bool AddDough()
        {
            if (_state > 0 && _state < 4)
            {
                _state++;
                Synchronize();
                return true;
            }
            return false;
        }

        /// <summary>
        /// UseDown or Interact時の操作
        /// </summary>
        public void Use()
        {
            switch (_state)
            {
                case 0:
                case 4:
                case 5:
                case 8:
                    _state++;
                    _initFlag = false;
                    Synchronize();
                    break;
                case 9:
                    //食パン塊をSpawn
                    GameObject spawned = _pantasyManager.Spawn(6, this.transform.parent);
                    if (spawned != null)
                    {
                        _panPickup.Drop();
                        _state++;
                        Synchronize();
                        //食パン型をリセット
                        ResetAll();
                    }
                    break;
                default:
                    break;
            }
        }

        public void TriggerEnter(GameObject target)
        {
            if (target.layer != _layer || _state != 6)
            {
                return;
            }

            Oven oven = target.GetComponent<Oven>();
            if (oven)
            {
                _panPickup.Drop();
                Transform fixPoint = oven.LoafPanFixPoint;
                if (fixPoint)
                {
                    _panPickup.SetTransform(fixPoint);
                    Log("オーブン内に固定します : pos" + fixPoint.position + " rot " + fixPoint.rotation);
                    _state++;
                    Synchronize();
                }
            }
        }

        /* Stay監視用 */
        private const float _D_TIME = 0.2f; // Update監視間隔
        private float _deltaTime = 0f;
        private float _bakedSec = 0f;

        private Oven _oven; // 効果音再生用

        public void TriggerStay(GameObject target)
        {
            if (target.layer != _layer)
            {
                return;
            }

            Oven oven = target.GetComponent<Oven>();
            _oven = oven;

            if (!Networking.IsOwner(this.gameObject))
            {
                return;
            }

            _deltaTime += Time.deltaTime;

            if (_deltaTime > _D_TIME)
            {
                //オーブン内部にある場合

                if (oven)
                {
                    _bakedSec += _D_TIME;
                    if (_bakedSec > _bakeTime)
                    {
                        //焼成完了
                        Bake();
                        _bakeTime = 0;
                    }
                }

                _deltaTime = 0;
            }
        }

        private void Bake()
        {
            if (_state == 7)
            {
                _state++;
                Synchronize();
                _oven.PlaySEBake();
            }
        }

        protected override void AfterSynchronize()
        {
            _animator.SetInteger("State", _state);
            if (_initFlag)
            {
                _animator.SetTrigger("Init");
            }
        }

        public void ResetAll()
        {
            _panPickup.ResetTransform();
            SendCustomEventDelayedSeconds(nameof(Initialize), 1f);
        }

        public void Initialize()
        {
            _state = 0;
            _initFlag = true;
            Synchronize();
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]LoafPanControler : " + msg);
        }
    }
}