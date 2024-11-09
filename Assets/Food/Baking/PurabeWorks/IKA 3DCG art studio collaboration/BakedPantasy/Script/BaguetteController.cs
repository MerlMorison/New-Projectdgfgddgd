
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    public class BaguetteController : CutDoughController
    {
        //長いバゲットのコントローラー

        //二次発酵タイミング
        protected override int FermentState => 0;
        //焼成タイミング
        protected override int BakeState => 3;
        //終了状態
        protected override int EndState => 7;

        /// <summary>
        /// 出現と同時に発酵開始
        /// </summary>
        private void OnEnable()
        {
            SendCustomEventDelayedSeconds(nameof(FermSub), 1f);
        }

        public void FermSub()
        {
            if (_state == 0)
            {
                _state = 1;
            }
            Synchronize();
        }

        public override void UseDown()
        {
            Eat("バゲット");
        }

        public override void TriggerEnter(GameObject go)
        {
            if(go.layer != _layer)
            {
                return;
            }

            Kitchenware kitchenware = go.GetComponent<Kitchenware>();
            if (kitchenware != null)
            {
                if (kitchenware.ItemType == 8 && _state == 1)
                {
                    // クーペナイフで切れ込みを入れる
                    Process("クーペナイフでバゲット生地に切れ込みを入れました。");
                }
                else if (kitchenware.ItemType == 9 && _state == 2)
                {
                    // オーブン皿に固定する
                    SetOnOvenTray(go, "バゲット");
                }
            }
        }
    }
}