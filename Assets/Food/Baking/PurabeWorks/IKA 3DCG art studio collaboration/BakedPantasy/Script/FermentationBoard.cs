
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class FermentationBoard : FermentationPlace
    {
        //二次発酵台の制御

        [SerializeField, Header("発酵板アニメーション")]
        private Animator _animator;

        [SerializeField, Header("DoughPickup管理リスト")]
        private DoughPickupListManager _pickupListManager;

        [UdonSynced(UdonSyncMode.None)]
        private bool _fermOn = false; //布かけ発酵中かどうか

        //Update管理
        private const float _D_TIME = 0.2f; // 監視間隔
        private float _deltaTime = 0f;

        protected override void Start()
        {
            _deltaTime = 0;
            base.Start();
        }

        /// <summary>
        /// 生地の二次発酵を開始させる
        /// </summary>
        private void Ferment()
        {
            _pickupListManager.ExecuteOnAllPickups(nameof(DoughPickup.Ferment));
        }

        /// <summary>
        /// 実行で発酵ON/OFFを切り替える
        /// </summary>
        public override void Interact()
        {
            if (!_fermOn && !IsEmpty())
            {
                //発酵を開始する
                Ferment();
                _fermOn = true;
                Log("二次発酵を開始します。");
                Synchronize();
            }
            else if (_fermOn)
            {
                _fermOn = false;
                Log("布を外します。");
                Synchronize();
            }
        }

        /// <summary>
        /// 生地を置く
        /// </summary>
        /// <param name="objectId">DoughPickupのオブジェクトID</param>
        /// <returns>置き場所 置けなければnull</returns>
        public Transform SetDough(int instanceId)
        {
            //使用中or占有済みなら置けない
            if (_fermOn || IsOccupied())
            {
                return null;
            }

            //空いている箇所に生地を登録
            for (int i = 0; i < _numPoints; i++)
            {
                if (!_pointBools[i])
                {
                    _pointBools[i] = true;
                    _doughs[i] = instanceId;
                    Log("Index[" + i + "]にパン生地がセットされます。");
                    Synchronize();
                    return _points[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 二次発酵台に存在する発酵対象生地をPickupリストに追加する
        /// </summary>
        /// <param name="target">パン生地オブジェクト</param>
        public void TriggerStay(GameObject target)
        {
            if (target.layer != _layer)
            {
                return;
            }

            _deltaTime += Time.deltaTime;
            if (_deltaTime > _D_TIME)
            {
                DoughPickup doughPickup = target.GetComponent<DoughPickup>();
                if (doughPickup == null)
                {
                    return;
                }
                _pickupListManager.AddPickup(doughPickup);
                _deltaTime = 0;
            }
        }

        /// <summary>
        /// 占有状態解除に加え、Pickupリストからも除外する
        /// </summary>
        /// <param name="target">パン生地オブジェクト</param>
        public override void TriggerExit(GameObject target)
        {
            base.TriggerExit(target);

            //オーナ以外でも実施
            if (target.layer == _layer)
            {
                DoughPickup doughPickup = target.GetComponent<DoughPickup>();
                if (doughPickup != null)
                {
                    _pickupListManager.RemovePickup(doughPickup);
                }
            }
        }

        /// <summary>
        /// 発酵台が空かどうか
        /// </summary>
        /// <returns>true:空　false:1個でも載ってる</returns>
        private bool IsEmpty()
        {
            Log("空き状態を確認します。");
            for (int i = 0; i < _numPoints; i++)
            {
                if (_pointBools[i])
                {
                    Log("二次発酵台は空ではありません。");
                    return false;
                }
            }
            Log("二次発酵台は空です。");
            return true;
        }

        /// <summary>
        /// 同期後にアニメーション操作
        /// </summary>
        protected override void AfterSynchronize()
        {
            _animator.SetBool("FermOn", _fermOn);
        }

        /// <summary>
        /// 全体リセット
        /// </summary>
        public override void ResetAll()
        {
            _animator.SetBool("FermOn", false);
            _fermOn = false;
            _pickupListManager.ResetAll();
            _deltaTime = 0;
            base.ResetAll();
        }
    }
}