using UdonSharp;


namespace PurabeWorks.BakedPantasy
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SetPlace : ManualSyncUB
    {
        [UdonSynced(UdonSyncMode.None)]
        private bool _isOccupied = false;
        [UdonSynced(UdonSyncMode.None)]
        private int _instanceID = -1;

        //占有状態
        public bool IsOccupied()
        {
            return _isOccupied;
        }

        //指定のオブジェクトIDで占有されているか
        public bool IsOccupiedWith(int targetID)
        {
            if (_isOccupied && targetID == _instanceID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //指定のオブジェクトIDで占有する
        public void OccupyWith(int targetID)
        {
            _isOccupied = true;
            _instanceID = targetID;
            Synchronize();
        }

        //占有解除
        public void Unoccupy()
        {
            _isOccupied = false;
            Synchronize();
        }
    }
}