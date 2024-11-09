
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class OvenTray : ManualSyncUB
    {
        // オーブントレイの制御
        [SerializeField, Header("固定点（ミニ）")]
        private Transform[] _miniFixPoints;
        [SerializeField, Header("固定点（長バゲット）")]
        private Transform[] _bagFixPoints;
        [SerializeField, Header("対象レイヤー"), Tooltip("13: Pickup")]
        private int layer = 13;
        [SerializeField, Header("Pickup")]
        private VRCPickup _pickup;
        [SerializeField, Header("焼成時間[s]")]
        private float _bakeTime = 10f;
        [SerializeField, Header("Constraint source")]
        private Transform _constPoint;

        private const int _NUM = 6; // 固定点数（ミニ換算）
        private const int _NUM_BAGUETTE = 2; // 固定点数（長バゲット）

        [UdonSynced(UdonSyncMode.None)]
        private bool[] _occupiedPoints; // 占有状態
        [UdonSynced(UdonSyncMode.None)]
        private int[] _miniIds; // ミニパンのオブジェクトID
        [UdonSynced(UdonSyncMode.None)]
        private int[] _bagIds; // 長バゲットのオブジェクトID
        [UdonSynced(UdonSyncMode.None)]
        private bool _isPickupable = true;

        /* 初期位置 */
        private Vector3 _initPosition;
        private Quaternion _initRotation;

        public Transform ConstPoint
        {
            get { return _constPoint; }
        }

        public bool IsHeld
        {
            get { return _pickup.IsHeld; }
        }

        protected override void AfterSynchronize()
        {
            _pickup.pickupable = _isPickupable;
        }

        void Start()
        {
            if (_miniFixPoints.Length != _NUM)
            {
                LogError(nameof(_miniFixPoints) + "の長さが不正です。");
            }
            if (_bagFixPoints.Length != _NUM_BAGUETTE)
            {
                LogError(nameof(_bagFixPoints) + "の長さが不正です。");
            }
            if (_pickup == null)
            {
                LogError(nameof(_pickup) + "がnullです。");
            }
            _occupiedPoints = new bool[_NUM];
            _miniIds = new int[_NUM];
            _bagIds = new int[_NUM_BAGUETTE];
            _initPosition = _pickup.gameObject.transform.position;
            _initRotation = _pickup.gameObject.transform.rotation;

            ResetAll();
        }

        /// <summary>
        /// オーブン皿にパン生地を占有させ、固定位置Transformを返す
        /// </summary>
        /// <param name="doughPickup">対象パン生地のpickup</param>
        /// <returns>固定位置Transform 固定不可の場合はnull</returns>
        public Transform SetDough(DoughPickup doughPickup)
        {
            if (doughPickup == null || !_isPickupable)
            {
                //オーブン内部にあるときはset不可
                return null;
            }

            if (doughPickup.DoughType == 9)
            {
                //長いバゲット
                int index = GetLongBaguetteIndex();
                if (index >= 0)
                {
                    for (int i = index; i < index * 3 + 3; i++)
                    {
                        _occupiedPoints[i] = true;
                    }
                    _bagIds[index] = doughPickup.gameObject.GetInstanceID();
                    Synchronize();
                    return _bagFixPoints[index];
                }
            }
            else
            {
                //それ以外
                int index = GetMiniIndex();
                if (index >= 0)
                {
                    _occupiedPoints[index] = true;
                    _miniIds[index] = doughPickup.gameObject.GetInstanceID();
                    Synchronize();
                    return _miniFixPoints[index];
                }
            }

            return null;
        }

        /// <summary>
        /// 長いバゲットの固定index値を返す
        /// </summary>
        /// <returns>固定index 占有済みなら-1</returns>
        private int GetLongBaguetteIndex()
        {
            for (int i = 0; i < _NUM_BAGUETTE; i++)
            {
                if (!_occupiedPoints[i * 3] && !_occupiedPoints[i * 3 + 1] && !_occupiedPoints[i * 3 + 2])
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// ミニパンの固定点indexを返す
        /// </summary>
        /// <returns>固定index 占有済みなら-1</returns>
        private int GetMiniIndex()
        {
            for (int i = 0; i < _NUM; i++)
            {
                if (!_occupiedPoints[i])
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 取り出されて離れた生地を除外する
        /// </summary>
        /// <param name="other">トリガーから離れたコライダー</param>
        private void OnTriggerExit(Collider other)
        {
            GameObject target = other.gameObject;
            if (target.layer == layer)
            {
                DoughPickup doughPickup = target.GetComponent<DoughPickup>();
                if (doughPickup)
                {
                    int index = IndexOnTray(target);
                    if (index >= 0)
                    {
                        if (doughPickup.DoughType == 9)
                        {
                            // 長いバゲットの場合
                            for (int i = index; i < index * 3 + 3; i++)
                            {
                                _occupiedPoints[i] = false;
                            }
                        }
                        else
                        {
                            // それ以外（ミニパン）
                            _occupiedPoints[index] = false;
                        }
                        Synchronize();
                    }
                    Log("OnTriggerExit Returned Index : " + index);
                }
            }
        }

        /// <summary>
        /// オーブン内部に入れた際に内部に固定する
        /// </summary>
        /// <param name="other">オーブン内部コライダー</param>
        private void OnTriggerEnter(Collider other)
        {
            if (!Networking.IsOwner(_pickup.gameObject))
            {
                return;
            }
            GameObject target = other.gameObject;
            if (target.layer == layer)
            {
                Oven oven = target.GetComponent<Oven>();
                if (oven)
                {
                    // オーブン内部
                    _pickup.Drop();
                    _isPickupable = false;
                    Synchronize();

                    // トレーをオーブン内位置にセット
                    Transform fixPoint = oven.TrayFixPoint;
                    _pickup.transform.SetPositionAndRotation(fixPoint.position, fixPoint.rotation);

                    // 焼成時間経過後にトレーを再度pickup可能にする
                    SendCustomEventDelayedSeconds(nameof(ReleaseTray), _bakeTime);
                }
            }
        }

        /// <summary>
        /// トレーに載っているパン生地にtargetがあるか調べてindexを返す
        /// </summary>
        /// <param name="target">パン生地オブジェクト</param>
        /// <returns>対応index 無い場合は-1</returns>
        private int IndexOnTray(GameObject target)
        {
            int id = target.GetInstanceID();
            for (int i = 0; i < _NUM; i++)
            {
                if (_occupiedPoints[i] && _miniIds[i] == id)
                {
                    return i;
                }
            }
            for (int i = 0; i < _NUM_BAGUETTE; i++)
            {
                if (_occupiedPoints[i * 3] && _occupiedPoints[i * 3 + 1] && _occupiedPoints[i * 3 + 2]
                    && _bagIds[i] == id)
                {
                    return i;
                }
            }
            return -1;
        }

        public void ReleaseTray()
        {
            _isPickupable = true;
            Synchronize();
        }

        public void ResetAll()
        {
            for (int i = 0; i < _NUM; i++)
            {
                _occupiedPoints[i] = false;
                _miniIds[i] = 0;
            }
            for (int i = 0; i < _NUM_BAGUETTE; i++)
            {
                _bagIds[i] = 0;
            }
            _pickup.Drop();
            _isPickupable = true;
            Rigidbody rd = _pickup.gameObject.GetComponent<Rigidbody>();
            if (rd != null)
            {
                rd.Sleep();
            }
            _pickup.gameObject.transform.SetPositionAndRotation(_initPosition, _initRotation);

            Synchronize();
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]OvenTray : " + msg);
        }

        private void LogError(string msg)
        {
            Debug.LogError("[pura]OvenTray : " + msg);
        }
    }
}