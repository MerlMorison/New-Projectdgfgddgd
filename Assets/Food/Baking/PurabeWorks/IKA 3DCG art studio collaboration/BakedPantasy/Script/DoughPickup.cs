
using UdonSharp;
using UnityEngine;
using UnityEngine.Animations;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(VRCPickup))]
    public class DoughPickup : ManualSyncUB
    {
        //パン生地Pickup
        //制限: オーブン皿固定中にオーナが退出した場合、皿から生地が外れる

        [SerializeField] private DoughController _mainController;
        [SerializeField, Header("生地タイプ"), Tooltip("1:プレーンたね 2:バゲットたね 3:たまごたね 4:クッキーたね 5:クロワッサン 6:クッキー 7:メロンパン本体 8:ミニバゲット 9:バゲット 10:食パン1枚 11:食パン塊")]
        private int _doughType = 0;

        private VRCPickup _pickup;
        private VRCObjectSync _objectSync;

        private CutDoughController _cutDoughController = null;

        [UdonSynced(UdonSyncMode.None)]
        private bool _pickupable = true;
        [UdonSynced(UdonSyncMode.None)]
        private bool _isFixed = false;

        /* Parent Constraint */
        [UdonSynced(UdonSyncMode.None)]
        private Vector3 _offsetPos = Vector3.zero;
        [UdonSynced(UdonSyncMode.None)]
        private Quaternion _offsetRot = Quaternion.identity;

        private ParentConstraint _const = null;
        private ConstraintSource _source;
        private Transform _constPoint;

        public int DoughType { get { return _doughType; } }

        void Start()
        {
            _pickup = this.gameObject.GetComponent<VRCPickup>();
            //コンストレイント
            _const = this.gameObject.GetComponent<ParentConstraint>();
            if (_const)
            {
                _source = new ConstraintSource();
            }
            //成形済み生地制御
            _cutDoughController = _mainController.gameObject.GetComponent<CutDoughController>();
            //同期ズレ防止
            _objectSync = this.gameObject.GetComponent<VRCObjectSync>();
        }

        private void OnDisable()
        {
            ResetAll();
        }

        public void OnTriggerEnter(Collider other)
        {
            GameObject go = other.gameObject;
            if (Networking.IsOwner(this.gameObject))
            {
                _mainController.TriggerEnter(go);
            }
        }

        public override void OnPickupUseDown()
        {
            _mainController.UseDown();
        }

        /// <summary>
        /// 指定の位置に移動し、Pickup無効化
        /// </summary>
        /// <param name="fixPoint">指定位置</param>
        public void SetFixed(Transform fixPoint)
        {
            GetOwner(_pickup.gameObject);
            _pickup.Drop();
            _pickupable = false;
            Synchronize();
            _objectSync.FlagDiscontinuity();
            _objectSync.TeleportTo(fixPoint);
            Log("位置を固定されました。 " + fixPoint.position.ToString());
        }

        /// <summary>
        /// トレイ/皿上の指定Transformに貼り付け
        /// </summary>
        /// <param name="fixPoint">指定Transform</param>
        public void StickOnTrayOrDish(Transform fixPoint, Transform constPoint)
        {
            Log("トレイ/皿上に貼り付けます。");
            Log("fixPoint pos: " + fixPoint.position.ToString() + " rot:" + fixPoint.rotation.ToString());
            Log("constPoint pos: " + constPoint.position.ToString() + " rot:" + constPoint.rotation.ToString());
            SetFixed(fixPoint);
            //相対座標を計算して保存
            _offsetPos = constPoint.InverseTransformPoint(fixPoint.position);
            _offsetRot = fixPoint.rotation * Quaternion.Inverse(constPoint.rotation);
            _isFixed = true;
            Synchronize();
        }

        public void Release()
        {
            _pickupable = true;
            Synchronize();
            Log("固定を解除されました。");
        }

        public void DisablePickup()
        {
            GetOwner(_pickup.gameObject);
            _pickup.Drop();
            _pickupable = false;
            _pickup.pickupable = false;
            Log("Pickupが無効化されました。");
            Synchronize();
        }

        public void EnablePickup()
        {
            _pickupable = true;
            Log("Pickupが有効化されました。");
            Synchronize();
        }

        /* Stay監視用 */
        private const float _D_TIME = 0.2f; // Update監視間隔
        private float _deltaTime = 0f;

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.layer != _mainController.Layer)
            {
                return;
            }

            if (_cutDoughController)
            {
                _cutDoughController.TriggerStay(other.gameObject);
            }

            _deltaTime += Time.deltaTime;

            //一定時間毎に処理
            if (_deltaTime > _D_TIME)
            {
                /* 固定点の取得 */
                if (_const && !_const.constraintActive)
                {
                    OvenTray ovenTray = other.gameObject.GetComponent<OvenTray>();
                    if (ovenTray)
                    {
                        _constPoint = ovenTray.ConstPoint;
                        SetConstraint();
                    }
                }
                _deltaTime = 0f;
            }
        }

        //二次発酵
        public void Ferment()
        {
            if (_cutDoughController)
            {
                _cutDoughController.Ferment();
            }
        }

        public override void OnPickup()
        {
            GetOwner(_mainController.gameObject);
            if (_const && _isFixed)
            {
                _isFixed = false;
                Synchronize();
            }
        }

        /// <summary>
        /// メイン部分のオーナ権限を委譲
        /// </summary>
        /// <param name="player"></param>
        public void OwnershipChange(VRCPlayerApi player)
        {
            if (!Networking.IsOwner(player, _mainController.gameObject))
            {
                Log("メイン部分のオーナ権限を委譲します。To player: " + player.displayName);
                Networking.SetOwner(player, _mainController.gameObject);
            }
        }

        /// <summary>
        /// オーブン皿or食器皿への固定処理
        /// </summary>
        /// <param name="target">対象オブジェクト</param>
        private void SetConstraint()
        {
            if (!_isFixed)
            {
                return;
            }

            //コンストレイント設定
            Log("コンストレイント設定");
            Log("constPoint : " + _constPoint.ToString());
            Log("offsetPos : " + _offsetPos.ToString() + " offsetRot : " + _offsetRot.ToString());
            _source.sourceTransform = _constPoint;
            _source.weight = 1.0f;
            _const.SetSource(0, _source);
            _const.SetTranslationOffset(0, _offsetPos);
            _const.SetRotationOffset(0, _offsetRot.eulerAngles);

            _const.constraintActive = true;
            Log("トレイ/皿に固定されました。");
        }

        /// <summary>
        /// クッキー生地の消費
        /// </summary>
        public void UseCookie()
        {
            _mainController.UseCookie();
        }

        protected override void AfterSynchronize()
        {
            _pickup.pickupable = _pickupable;
            if (!_isFixed && _const)
            {
                _const.constraintActive = false;
            }
        }

        private void Log(string msg)
        {
            Debug.Log("[pura]DoughPickup : " + msg);
        }

        public void ResetAll()
        {
            _pickupable = true;
            _isFixed = false;
            _source = new ConstraintSource();
            if (_const)
            {
                _const.constraintActive = false;
            }
            _deltaTime = 0f;
            _offsetPos = Vector3.zero;
            _offsetRot = Quaternion.identity;
            Synchronize();
            _mainController.ResetAll();
        }
    }
}