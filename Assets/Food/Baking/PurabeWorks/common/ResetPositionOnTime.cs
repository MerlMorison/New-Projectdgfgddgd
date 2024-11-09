
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks
{
    /// <summary>
    /// 一定時間だれにもPickupされていなければ元の位置に戻る
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [RequireComponent(typeof(VRC_Pickup))]
    public class ResetPositionOnTime : UdonSharpBehaviour
    {
        [SerializeField, Header("位置リセットまでの時間[s]")]
        private float resetTime = 60.0f;

        private VRC_Pickup _pickup;
        private Rigidbody _rigidbody;
        private VRCObjectSync _objSync;
        private Vector3 _initPosition;
        private Quaternion _initRotation;
        private float countTime = 0f;
        private bool isCountStarted = false;


        private void Start()
        {
            _pickup = (VRC_Pickup)this.gameObject.GetComponent(typeof(VRC_Pickup));
            _rigidbody = this.gameObject.GetComponent<Rigidbody>();
            _objSync = this.gameObject.GetComponent<VRCObjectSync>();
            _initPosition = this.transform.position;
            _initRotation = this.transform.rotation;
        }

        private void Update()
        {
            if (transform.position != _initPosition && !_pickup.IsHeld
                && Networking.IsOwner(this.gameObject))
            {
                if (isCountStarted)
                {
                    countTime += Time.deltaTime;
                    if (resetTime < countTime)
                    {
                        isCountStarted = false;
                        ResetTransform();
                    }
                }
                else
                {
                    isCountStarted = true;
                    countTime = 0f;
                }
            }
            else
            {
                isCountStarted = false;
            }
        }

        private void ResetTransform()
        {
            if (_objSync)
            {
                _objSync.FlagDiscontinuity();
            }
            _rigidbody.Sleep();
            transform.SetPositionAndRotation(_initPosition, _initRotation);
        }
    }
}