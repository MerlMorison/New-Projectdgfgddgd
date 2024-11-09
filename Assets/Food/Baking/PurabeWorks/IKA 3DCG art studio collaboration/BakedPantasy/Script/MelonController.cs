
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    public class MelonController : CutDoughController
    {
        //二次発酵タイミング
        protected override int FermentState => 2;
        //焼成タイミング
        protected override int BakeState => 6;
        //終了状態
        protected override int EndState => 10;

        public override void UseDown()
        {
            Eat("メロンパン");

            if (!_baked && _state == 0)
            {
                // メロンパン生地の成形
                Process("メロンパン生地を丸めました。");
            }
        }

        public override void TriggerEnter(GameObject go)
        {
            if (go.layer != _layer)
            {
                return;
            }

            //クッキー生地の判定
            DoughPickup doughPickup = go.GetComponent<DoughPickup>();
            if (doughPickup != null && doughPickup.DoughType == 6
                && _state == 3)
            {
                // クッキー生地を合わせる
                Process("クッキー生地を合わせました。");
                doughPickup.UseCookie();
                return;
            }

            //ツール判定
            Kitchenware kitchenware = go.GetComponent<Kitchenware>();
            if (kitchenware != null)
            {
                int itemType = kitchenware.ItemType;

                if (itemType == 7 && _state == 1)
                {
                    // 二次発酵
                    SetOnFermentPlace(go, "メロンパン");
                }
                else if (itemType == 8 && _state == 4)
                {
                    // クーペナイフで切れ込みを入れる
                    Process("メロンパン生地に切れ込みを入れました。");
                }
                else if (itemType == 9 && _state == 5)
                {
                    // オーブン皿に固定する
                    SetOnOvenTray(go, "メロンパン");
                }
            }
        }
    }
}