using UdonSharp;

namespace PurabeWorks.BakedPantasy
{
    public class DoughPickupListManager : UdonSharpBehaviour
    {
        //DoughPickupをローカルで保存/操作する制御
        //TriggerStayで使用する想定

        private const int _NUM_DOUGH_PICKUPS = 50; //Pickupリストの最大値
        private DoughPickup[] _doughPickups; //DoughPickupのリスト
        private int _numPickups = 0;

        private void Start()
        {
            _doughPickups = new DoughPickup[_NUM_DOUGH_PICKUPS];
        }

        /// <summary>
        /// Pickupリストに存在するオブジェクトのindexを検索して返す
        /// </summary>
        /// <param name="instanceId">対象オブジェクトID</param>
        /// <returns>index 存在しなければ-1</returns>
        private int IndexInPickupList(int instanceId)
        {
            for (int i = 0; i < _numPickups; i++)
            {
                if (_doughPickups[i] && _doughPickups[i].gameObject.GetInstanceID() == instanceId)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// PickupリストにDoughPickupオブジェクトを挿入する
        /// </summary>
        /// <param name="doughPickup">対象DoughPickupオブジェクト</param>
        /// <returns>途中の空きに入ればtrue 隙間なく詰まっていたらfalse</returns>
        private bool InsertPickup(DoughPickup doughPickup)
        {
            for (int i = 0; i < _numPickups; i++)
            {
                if (_doughPickups[i] == null)
                {
                    _doughPickups[i] = doughPickup;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// PickupリストにDoughPickupオブジェクトを追加する
        /// </summary>
        /// <param name="doughPickup">対象DoughPickupオブジェクト</param>
        public void AddPickup(DoughPickup doughPickup)
        {
            if (IndexInPickupList(doughPickup.gameObject.GetInstanceID()) < 0)
            {
                if (!InsertPickup(doughPickup))
                {
                    _doughPickups[_numPickups++] = doughPickup;
                }
            }
        }

        /// <summary>
        /// PickupリストからDoughPickupオブジェクトを削除する
        /// </summary>
        /// <param name="doughPickup">対象DoughPickupオブジェクト</param>
        public void RemovePickup(DoughPickup doughPickup)
        {
            int index = IndexInPickupList(doughPickup.gameObject.GetInstanceID());
            if (index >= 0)
            {
                _doughPickups[index] = null;
                if (index == _numPickups - 1)
                {
                    _numPickups--;
                }
            }
        }

        /// <summary>
        /// 指定の関数をPickupリスト全体で実行する
        /// </summary>
        /// <param name="functionName">関数名文字列</param>
        public void ExecuteOnAllPickups(string functionName)
        {
            for (int i = 0; i < _numPickups; i++)
            {
                if (_doughPickups[i] != null)
                {
                    _doughPickups[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, functionName);
                }
            }
        }

        public void ResetAll()
        {
            _numPickups = 0;

            if (_doughPickups == null)
            {
                //初期化前
                return;
            }
            for (int i = 0; i < _NUM_DOUGH_PICKUPS; i++)
            {
                if (_doughPickups[i])
                {
                    _doughPickups[i] = null;
                }
            }
        }
    }
}