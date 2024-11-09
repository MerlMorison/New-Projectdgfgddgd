
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    [RequireComponent(typeof(ParentConstraint))]
    public class SetItem : UdonSharpBehaviour
    {
        /* Pickup時はコンストレイントOFF
         * 指定のトリガーに入ったらONにする */
        // 砂糖or塩壺とフタ&スプーンなどのセットアイテムに

        [SerializeField, Header("トリガー領域")]
        private Collider _trigger;

        private VRCPickup _pickup;
        private ParentConstraint _parentConstraint;

        private const float _D_TIME = 0.5f; // Update監視間隔
        private float _deltaTime = 0f;

        void Start()
        {
            _parentConstraint = this.gameObject.GetComponent<ParentConstraint>();
            _parentConstraint.constraintActive = true;

            _pickup = this.gameObject.GetComponent<VRCPickup>();
            _pickup.pickupable = true;
        }

        public void ConstraintOff()
        {
            _parentConstraint.constraintActive = false;
        }

        public void ConstraintOn()
        {
            _parentConstraint.constraintActive = true;
        }

        public override void OnPickup()
        {
            ConstraintOff();
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                nameof(ConstraintOff));
        }

        private void OnTriggerStay(Collider other)
        {
            _deltaTime += Time.deltaTime;

            if (_deltaTime > _D_TIME)
            {
                if (Networking.IsOwner(this.gameObject)
                    && !_pickup.IsHeld && other == _trigger)
                {
                    SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                        nameof(ConstraintOn));
                }

                _deltaTime = 0;
            }
        }
    }
}