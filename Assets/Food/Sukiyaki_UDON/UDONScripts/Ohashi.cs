
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class Ohashi : UdonSharpBehaviour
{
	public GuzaiManager guzaiManager;
	public GameObject triggerSukiyaki;
	public GameObject triggerTokitamago;

	public override void OnPickupUseDown()
	{
		SendCustomNetworkEvent(NetworkEventTarget.All, nameof(EatGuzai));
	}

	public void EatGuzai()
	{
		guzaiManager.ResetGuzai();
		triggerTokitamago.SetActive(false);
	}
}