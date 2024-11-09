
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class EnterTrigger_Sukiyaki : UdonSharpBehaviour
{
	public GuzaiManager guzaiManager;
	public GameObject triggerSukiyaki;
	public GameObject triggerTokitamago;

	private void OnTriggerEnter(Collider other)
	{
		if (!triggerTokitamago.activeSelf)
		{
			// 数字は該当レイヤー番号に変える
			if (other.gameObject.layer == 30)
			{
				// ピックアップオブジェクトのオーナーにのみ処理を与える
				var player = Networking.LocalPlayer;
				if (player.IsOwner(gameObject))
				{
					PickGuzai();
				}
			}
		}
	}

	public void PickGuzai()
	{
		guzaiManager.PickGuzai();
		triggerTokitamago.SetActive(true);
	}
}