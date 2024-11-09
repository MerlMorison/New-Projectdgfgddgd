
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class EnterTrigger_Tokitamago : UdonSharpBehaviour
{
	public GuzaiManager guzaiManager;

	private void OnTriggerEnter(Collider other)
	{
		// 数字は該当レイヤー番号に変える
		if (other.gameObject.layer == 31)
		{
			SendCustomNetworkEvent(NetworkEventTarget.All, nameof(TamagoGuzai));
		}
	}

	public void TamagoGuzai()
	{
		guzaiManager.Tamagotoji();
	}
}
