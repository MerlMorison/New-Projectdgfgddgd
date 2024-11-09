
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class FermentationClothBaguette : FermentationPlace
    {
        //バゲット生地の二次発酵布

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

            if (_pantasyManager) { return; }
            if (_pantasyManager = FindObjectOfType<PantasyManager>())
            {
                return;
            }

            Log("PantasyManagerが見つかりませんでした。");
        }
#endif

        /// <summary>
        /// 空いている場所に発酵後生地をSpawn
        /// </summary>
        public bool SpawnInPlaces()
        {
            for (int i = 0; i < _numPoints; i++)
            {
                if (!_pointBools[i])
                {
                    GameObject dough = _pantasyManager.Spawn(5, _points[i]);
                    if (dough != null)
                    {
                        _pointBools[i] = true;
                        _doughs[i] = dough.GetInstanceID();
                        Synchronize();
                        return true;
                    } else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 発酵後生地を取り出したときに該当箇所を空き状態にする
        /// </summary>
        /// <param name="other">取り出し生地のトリガー</param>
        public void OnTriggerExit(Collider other)
        {
            this.TriggerExit(other.gameObject);
        }
    }
}