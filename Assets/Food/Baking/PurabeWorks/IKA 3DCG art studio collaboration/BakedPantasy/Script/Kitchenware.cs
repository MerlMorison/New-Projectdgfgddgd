
using UdonSharp;
using UnityEngine;

namespace PurabeWorks.BakedPantasy
{
    public class Kitchenware : UdonSharpBehaviour
    {
        [SerializeField, Header("道具種別")]
        [Tooltip("1:木べら 2:スケッパー 3:めん棒 4:丸カッター 5:食パン型 6:バゲット布 7:二次発酵台 8:クーペナイフ 9:オーブン皿 10:パン包丁 11:オーブン内部")]
        private int _itemType = 0;

        public int ItemType { get { return _itemType; } }
    }
}