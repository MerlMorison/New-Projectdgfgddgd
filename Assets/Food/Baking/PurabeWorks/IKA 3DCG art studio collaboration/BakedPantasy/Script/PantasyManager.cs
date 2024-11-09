
using System.Drawing;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PantasyManager : UdonSharpBehaviour
    {
        /* パン生地のSpawn管理 */

        [SerializeField, Header("食パンとクロワッサンの大生地")] private VRCObjectPool _doughPlain;
        [SerializeField, Header("バゲットとフランスパンの大生地")] private VRCObjectPool _doughBucket;
        [SerializeField, Header("メロンパン本体大生地")] private VRCObjectPool _doughMelonBread;
        [SerializeField, Header("メロンパンのクッキー生地")] private VRCObjectPool _cookie;
        [SerializeField, Header("バゲット用の二次発酵以降生地")] private VRCObjectPool _baguette;
        [SerializeField, Header("食パンの塊")] private VRCObjectPool _loafBread;
        [SerializeField, Header("ミニバゲット用のカット済み生地"), Tooltip("4種類")]
        private VRCObjectPool[] _miniBaguette;
        [SerializeField, Header("クロワッサン用のカット済み生地"), Tooltip("7種類")]
        private VRCObjectPool[] _croissant;
        [SerializeField, Header("メロンパン用のカット済み生地"), Tooltip("4種類")]
        private VRCObjectPool[] _melonBread;
        [SerializeField, Header("メロンパンクッキーのカット済み生地"), Tooltip("6種類")]
        private VRCObjectPool[] _cookieDivided;
        [SerializeField, Header("食パン1枚")]
        private VRCObjectPool _toast;
        [SerializeField, Header("ピザまるごと")]
        private VRCObjectPool _wholePizza;
        [SerializeField, Header("ピザ1枚")]
        private VRCObjectPool[] _cutPizza;

        [Space]

        [SerializeField, Header("注意書き表示")]
        private SpawnLimitNotification _notification;

        private VRCObjectPool[] _poolList;
        private const int NUM_POOLS = 37;

        private void Start()
        {
            _poolList = new VRCObjectPool[NUM_POOLS];
            int index = 0;
            _poolList[index++] = _doughPlain;
            _poolList[index++] = _doughBucket;
            _poolList[index++] = _doughMelonBread;
            _poolList[index++] = _cookie;
            _poolList[index++] = _baguette;
            _poolList[index++] = _loafBread;
            SetPoolArray(ref index, _miniBaguette);
            SetPoolArray(ref index, _croissant);
            SetPoolArray(ref index, _melonBread);
            SetPoolArray(ref index, _cookieDivided);
            _poolList[index++] = _toast;
            _poolList[index++] = _wholePizza;
            SetPoolArray(ref index, _cutPizza);
        }

        private void SetPoolArray(ref int index, VRCObjectPool[] pools)
        {
            foreach (VRCObjectPool p in pools)
            {
                _poolList[index++] = p;
            }
        }

        /// <summary>
        /// 指定された番号に対応するパン生地を出現させる。
        /// </summary>
        /// <param name="type">生地タイプ</param>
        /// <param name="index">切り分け生地用のインデックス</param>
        /// <param name="point">出現位置</param>
        /// <returns>Spawnされたオブジェクト</returns>
        public GameObject Spawn(int type, int index, Transform point)
        {
            GameObject spawnedItem = null;
            switch (type)
            {
                case 1: //食パンとクロワッサンの大生地
                    spawnedItem = SpawnSub(_doughPlain, point);
                    break;
                case 2: //バゲットとフランスパンの大生地
                    spawnedItem = SpawnSub(_doughBucket, point);
                    break;
                case 3: //メロンパン本体大生地
                    spawnedItem = SpawnSub(_doughMelonBread, point);
                    break;
                case 4: //メロンパンのクッキー生地
                    spawnedItem = SpawnSub(_cookie, point);
                    break;
                case 5: //バゲット生地
                    spawnedItem = SpawnSub(_baguette, point);
                    break;
                case 6: //食パン塊
                    spawnedItem = SpawnSub(_loafBread, point);
                    break;
                case 7: //ミニバゲット用のカット済み生地
                    spawnedItem = SpawnSub(_miniBaguette[index], point);
                    break;
                case 8: //クロワッサン用のカット済み生地
                    spawnedItem = SpawnSub(_croissant[index], point);
                    break;
                case 9: //メロンパン用のカット済み生地
                    spawnedItem = SpawnSub(_melonBread[index], point);
                    break;
                case 10: //クッキーのカット済み生地
                    spawnedItem = SpawnSub(_cookieDivided[index], point);
                    break;
                case 11: //食パン1枚
                    spawnedItem = SpawnSub(_toast, point);
                    break;
                case 12: //ピザまるごと
                    spawnedItem = SpawnSub(_wholePizza, point);
                    break;
                case 13: //ピザ1枚
                    spawnedItem = SpawnSub(_cutPizza[index], point);
                    break;
                default: //該当なし
                    break;
            }
            return spawnedItem;
        }

        /// <summary>
        /// 指定された番号に対応するパン生地を出現させる。
        /// </summary>
        /// <param name="type">生地タイプ</param>
        /// <param name="point">出現位置</param>
        /// <returns>Spawnされたオブジェクト</returns>
        public GameObject Spawn(int type, Transform point)
        {
            return Spawn(type, 0, point);
        }

        private GameObject SpawnSub(VRCObjectPool pool, Transform point)
        {
            // 全部Spawn済みなら何もしない
            if (AllActive(pool))
            {
                Log("Spawnできるオブジェクトがありません。");
                ShowNotification();
                return null;
            }

            // Spawn対象Poolのオーナ権限取得
            SetOwner(pool.gameObject);
            // Pool先頭のオブジェクトSpawn
            GameObject spawnedItem = pool.TryToSpawn();

            if (spawnedItem == null)
            {
                Log("Spawnできるオブジェクトがありません。");
                ShowNotification();
                return null;
            }

            Log("オブジェクトがSpawnされました。 Name= " + spawnedItem.name);
            // オーナ権限取得
            SetOwner(spawnedItem);
            DoughPickup doughPickup = spawnedItem.GetComponent<DoughPickup>();
            if (doughPickup != null)
            {
                doughPickup.OwnershipChange(Networking.LocalPlayer);
            }

            // オブジェクトを指定位置に移動
            VRCObjectSync objectSync = spawnedItem.GetComponent<VRCObjectSync>();
            if (objectSync)
            {
                objectSync.FlagDiscontinuity();
            }

            Vector3 position = point.position;
            Quaternion rotation;

            CutPizzaPickup cutPizza = spawnedItem.GetComponent<CutPizzaPickup>();
            if (cutPizza)
            {
                // ピザ1切れの場合は相対角度にSpawn
                Log("ピザ一切れのSpawn");
                rotation = point.rotation * Quaternion.Inverse(cutPizza.RelativeRotation());
            }
            else
            {
                // それ以外
                Log("その他のSpawn");
                rotation = point.rotation;
            }

            Log("オブジェクト移動 : pos " + position.ToString() + " rot " + rotation.ToString());
            spawnedItem.transform.SetPositionAndRotation(position, rotation);

            return spawnedItem;
        }

        /// <summary>
        /// パン生地オブジェクトをReturnする
        /// </summary>
        /// <param name="bread">Return対象オブジェクト</param>
        public void ReturnBread(GameObject bread)
        {
            SetOwner(bread);
            foreach (VRCObjectPool pool in _poolList)
            {
                SetOwner(pool.gameObject);
                pool.Return(bread);
            }
        }

        /// <summary>
        /// Spawnできる生地が無かったときの注意書き表示
        /// </summary>
        private void ShowNotification()
        {
            _notification.SendCustomNetworkEvent(
                VRC.Udon.Common.Interfaces.NetworkEventTarget.All,
                nameof(SpawnLimitNotification.ShowNotification));
        }

        private void SetOwner(GameObject target)
        {
            if (!Networking.IsOwner(target))
            {
                Log(target.name + " のオーナ権限を取得します。");
                Networking.SetOwner(Networking.LocalPlayer, target);
            }
        }

        private bool AllActive(VRCObjectPool pool)
        {
            foreach (GameObject item in pool.Pool)
            {
                if (!item.activeInHierarchy)
                {
                    return false;
                }
            }
            return true;
        }

        public void ResetAll()
        {
            ReturnAllArray(_poolList);
        }

        private void ReturnAll(VRCObjectPool pool)
        {
            SetOwner(pool.gameObject);
            foreach (GameObject item in pool.Pool)
            {
                if (item.activeInHierarchy)
                {
                    SetOwner(item);
                    pool.Return(item);
                }
            }
        }

        private void ReturnAllArray(VRCObjectPool[] pools)
        {
            foreach (VRCObjectPool p in pools)
            {
                ReturnAll(p);
            }
        }

        private void Log(string message)
        {
            Debug.Log("[pura]PantasyManager : " + message);
        }

        private void LogError(string message)
        {
            Debug.LogError("[pura]PantasyManager : " + message);
        }
    }
}