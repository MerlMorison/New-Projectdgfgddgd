
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PurabeWorks.BakedPantasy
{
    [RequireComponent(typeof(Collider))]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class FermentationBoardTrigger : UdonSharpBehaviour
    {
        //二次発酵台のトリガー制御
        [SerializeField] private FermentationBoard _fermentationBoard;

        //生地をセットする
        public Transform SetDough(int instanceId)
        {
            return _fermentationBoard.SetDough(instanceId);
        }

        private void OnTriggerExit(Collider other)
        {
            _fermentationBoard.TriggerExit(other.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            _fermentationBoard.TriggerStay(other.gameObject);
        }
    }
}