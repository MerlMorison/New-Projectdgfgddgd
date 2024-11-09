using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    public class FermentationPlace : ManualSyncUB
    {
        //二次発酵台の共通親

        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")]
        protected int _layer = 13;
        [SerializeField, Header("固定点")]
        protected Transform[] _points;

        [UdonSynced(UdonSyncMode.None)]
        protected bool[] _pointBools; //固定点の空き状態
        [UdonSynced(UdonSyncMode.None)]
        protected int[] _doughs; //占有生地オブジェクトID列

        protected int _numPoints; //固定点の数

        public int NumPoints
        {
            get { return _numPoints; }
        }

        /// <summary>
        /// 固定点の空き状態を初期化
        /// </summary>
        protected virtual void Start()
        {
            _numPoints = _points.Length;
            _pointBools = new bool[_numPoints];
            _doughs = new int[_numPoints];
            ResetAll();
        }

        /// <summary>
        /// 二次発酵台の空き状態を返す
        /// </summary>
        /// <returns>true:空いていない false:空いている</returns>
        public bool IsOccupied()
        {
            Log("空き状態を確認します。");
            foreach (bool pointBool in _pointBools)
            {
                if (!pointBool)
                {
                    Log("二次発酵台に空きがあります。");
                    return false;
                }
            }
            Log("二次発酵台に空きはありません。");
            return true;
        }

        /// <summary>
        /// オブジェクトIDがどの位置を占有しているか返す
        /// </summary>
        /// <param name="objectId">オブジェクトID</param>
        /// <returns>占有箇所index</returns>
        protected int DoughIndex(int objectId)
        {
            for (int i = 0; i < _numPoints; i++)
            {
                if (_pointBools[i] && _doughs[i] == objectId)
                {
                    Log("DoughIndex : " + i);
                    return i;
                }
            }
            Log("DoughIndex : 該当なし");
            return -1;
        }

        /// <summary>
        /// 発酵後生地を取り出したときに該当箇所を空き状態にする
        /// </summary>
        /// <param name="other">取り出し生地のトリガー</param>
        public virtual void TriggerExit(GameObject target)
        {
            if (target.layer != _layer)
            {
                return;
            }
            if(!Networking.IsOwner(this.gameObject))
            {
                return;
            }

            DoughPickup doughPickup = target.GetComponent<DoughPickup>();
            if (doughPickup != null)
            {
                //対象の生地オブジェクトなら取り出し処理実行
                int index = DoughIndex(doughPickup.gameObject.GetInstanceID());
                if (index >= 0)
                {
                    _pointBools[index] = false;
                    Log("Index[" + index + "]の生地が取り出されました。");
                    Synchronize();
                }
            }
        }

        /// <summary>
        /// 全体リセット
        /// </summary>
        public virtual void ResetAll()
        {
            for (int i = 0; i < _numPoints; i++)
            {
                _pointBools[i] = false;
                _doughs[i] = 0;
            }
            Synchronize();
        }

        /// <summary>
        /// 情報ログ
        /// </summary>
        /// <param name="msg">内容</param>
        protected void Log(string msg)
        {
            Debug.Log("[pura]FermentationPlace : " + msg);
        }

        /// <summary>
        /// エラーログ
        /// </summary>
        /// <param name="msg">内容</param>
        protected void LogError(string msg)
        {
            Debug.LogError("[pura]FermentationPlace : " + msg);
        }
    }
}