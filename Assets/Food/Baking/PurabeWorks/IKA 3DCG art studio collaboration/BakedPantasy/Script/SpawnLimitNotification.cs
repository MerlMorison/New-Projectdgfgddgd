
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    public class SpawnLimitNotification : UdonSharpBehaviour
    {
        // 生地Spawnの出現上限を知らせるNotification操作
        [SerializeField]
        private AudioClip _se;

        [SerializeField]
        private AudioSource _source;

        [SerializeField]
        private float Damping = 1.0f;

        private Animator _animator;

        private void Start()
        {
            _animator = this.gameObject.GetComponent<Animator>();
        }

        private void Update()
        {
            if (Networking.LocalPlayer == null || !Networking.LocalPlayer.IsValid())
            {
                return;
            }

            var head = Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);
            if (Networking.LocalPlayer.IsUserInVR())
            {
                this.transform.position = Vector3.Lerp(this.transform.position, head.position, this.Damping * Time.deltaTime);
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, head.rotation, this.Damping * Time.deltaTime);
            }
            else
            {
                this.transform.position = head.position;
                this.transform.rotation = head.rotation;
            }
        }

        public void ShowNotification()
        {
            Log("ShowNotification がコールされました");
            _animator.SetTrigger("Show");
            _source.PlayOneShot(_se);
        }

        private void Log(string message)
        {
            Debug.Log("[pura]SpawnLimitNotification : " + message);
        }
    }
}