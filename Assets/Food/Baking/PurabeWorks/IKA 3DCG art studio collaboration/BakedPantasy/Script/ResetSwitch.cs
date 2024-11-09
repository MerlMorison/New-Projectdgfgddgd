
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ResetSwitch : UdonSharpBehaviour
    {
        //リセットスイッチ

        [SerializeField, Header("リセット対象")] GameObject[] targets;

        private int _num;
        private Vector3[] _initPositions;
        private Quaternion[] _initRotations;
        private VRCPlayerApi _localPlayer;

        private void Start()
        {
            _num = targets.Length;
            _initPositions = new Vector3[_num];
            _initRotations = new Quaternion[_num];
            SetInitPosRot();
        }

        public override void Interact()
        {
            for (int i = 0; i < _num; ++i)
            {
                if (targets[i])
                {
                    ResetUdonBehaviour(targets[i]);
                    ResetPosRot(i);
                }
            }
        }

        /// <summary>
        /// 初期位置を保存
        /// </summary>
        private void SetInitPosRot()
        {
            for (int i = 0; i < _num; i++)
            {
                if (targets[i])
                {
                    _initPositions[i] = targets[i].transform.position;
                    _initRotations[i] = targets[i].transform.rotation;
                }
            }
        }

        /// <summary>
        /// 位置をリセット
        /// </summary>
        /// <param name="index">配列番号</param>
        private void ResetPosRot(int index)
        {
            VRCPickup pickup = targets[index].GetComponent<VRCPickup>();
            if (!pickup)
            {
                //位置リセット不要
                return;
            }

            //オーナ権限取得
            if (!Networking.IsOwner(targets[index]))
            {
                if (_localPlayer == null)
                {
                    _localPlayer = Networking.LocalPlayer;
                }
                Networking.SetOwner(_localPlayer, targets[index]);
            }

            //移動処理
            VRCObjectSync objSync = targets[index].GetComponent<VRCObjectSync>();
            if (objSync)
            {
                objSync.FlagDiscontinuity();
            }
            Rigidbody rd = targets[index].GetComponent<Rigidbody>();
            if (rd)
            {
                rd.Sleep();
            }
            targets[index].transform.SetPositionAndRotation(_initPositions[index], _initRotations[index]);
        }

        /// <summary>
        /// UdonBevaiourすべてで"ResetAll"を実行する
        /// </summary>
        /// <param name="go">対象のオブジェクト</param>
        private void ResetUdonBehaviour(GameObject go)
        {
            UdonBehaviour[] udons = go.GetComponents<UdonBehaviour>();
            ResetUdonBahaviourSub(udons);

            udons = go.GetComponentsInChildren<UdonBehaviour>();
            ResetUdonBahaviourSub(udons);
        }

        private void ResetUdonBahaviourSub(UdonBehaviour[] udons)
        {
            foreach (UdonBehaviour udon in udons)
            {
                //オーナにResetAllを実行させる
                udon.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ResetAll");
            }
        }
    }
}