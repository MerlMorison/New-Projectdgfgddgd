
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRC_Pickup))]
    public class LoafPanPickup : ManualSyncUB
    {
        [SerializeField] private LoafPanController _main;

        private VRCPickup _pickup;
        private VRCObjectSync _objSync;

        //初期位置
        private Vector3 _initPosition;
        private Quaternion _initRotation;

        private void Start()
        {
            _initPosition = transform.position;
            _initRotation = transform.rotation;
            _pickup = this.gameObject.GetComponent<VRCPickup>();
            _objSync = this.gameObject.GetComponent<VRCObjectSync>();
        }

        public override void OnPickup()
        {
            GetOwner(_main.gameObject);
        }
        public override void OnPickupUseDown()
        {
            _main.Use();
        }

        /// <summary>
        /// PickupをDropさせる
        /// </summary>
        public void Drop()
        {
            _pickup.Drop();
        }

        /// <summary>
        /// 位置を移動
        /// </summary>
        /// <param name="point"></param>
        public void SetTransform(Transform point)
        {
            if (_objSync)
            {
                _objSync.FlagDiscontinuity();
            }
            this.transform.SetPositionAndRotation(point.position, point.rotation);
        }

        private void OnDisable()
        {
            ResetTransform();
            _main.ResetAll();
        }

        public void ResetTransform()
        {
            if (_objSync)
            {
                _objSync.FlagDiscontinuity();
            }
            this.transform.SetPositionAndRotation(_initPosition, _initRotation);
        }
    }
}