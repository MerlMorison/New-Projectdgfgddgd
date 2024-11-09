
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    public class ConsumableIngredient : Ingredient
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private SetItem _setItem;

        [UdonSynced(UdonSyncMode.None)] private bool _isUsed = false;

        private VRCPickup _pickup;

        //使用後に位置リセットするタイミング
        private const float _resetTime = 3.0f;
        //時間カウント
        private float _countTime = 0.0f;
        //初期位置
        private Vector3 _initPosition;
        private Quaternion _initRotation;

        void Start()
        {
            _isUsed = false;
            _pickup = this.gameObject.GetComponent<VRCPickup>();
            //初期位置の保存
            _initPosition = transform.position;
            _initRotation = transform.rotation;
        }

        public void Consume()
        {
            _isUsed = true;
            _pickup.Drop();
            Synchronize();
        }

        protected override void AfterSynchronize()
        {
            _animator.SetBool("Used", _isUsed);
        }

        private void Update()
        {
            /* 使用から一定時間後に位置リセット */
            if (_isUsed)
            {
                // 時間カウント
                _countTime += Time.deltaTime;
                if (_resetTime < _countTime)
                {
                    // 位置リセット
                    ResetPosition();
                    // 状態リセット
                    _isUsed = false;
                    AfterSynchronize();

                    _countTime = 0.0f;
                }
            }
            else
            {
                // 使用済みではない
                _countTime = 0.0f;
            }
        }

        private void ResetPosition()
        {
            transform.SetPositionAndRotation(_initPosition, _initRotation);
            _setItem.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                nameof(_setItem.ConstraintOn));
        }

        public void ResetAll()
        {
            ResetPosition();
            _isUsed = false;
            AfterSynchronize();
        }
    }
}