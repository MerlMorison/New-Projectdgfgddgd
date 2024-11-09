
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
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class DeleteArea : UdonSharpBehaviour
    {
        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")]
        private int _layer = 13;

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

        public void OnTriggerEnter(Collider other)
        {
            GameObject go = other.gameObject;
            if (go.layer == _layer)
            {
                DoughPickup doughPickup = go.GetComponent<DoughPickup>();
                PizzaPickup pizzaPickup = go.GetComponent<PizzaPickup>();
                CutPizzaPickup cutPizzaPickup = go.GetComponent<CutPizzaPickup>();

                //対象パン生地類ならReturn実行
                if (doughPickup || pizzaPickup || cutPizzaPickup)
                {
                    _pantasyManager.ReturnBread(go);
                }
            }
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]DeleteArea : " + msg);
        }
    }
}