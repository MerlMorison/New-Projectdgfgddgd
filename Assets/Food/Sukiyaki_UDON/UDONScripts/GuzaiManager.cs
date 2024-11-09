
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class GuzaiManager : UdonSharpBehaviour
{
	public Guzai[] guzai;

	[UdonSynced(UdonSyncMode.None)] private int index = -1;

	public void ResetGuzai()
	{
		index = -1;
		Init();
	}

	public void Init()
	{
		foreach (Guzai gu in guzai)
		{
			gu.Init();
		}
	}

	public void Tamagotoji()
	{
		foreach (Guzai gu in guzai)
		{
			gu.Tamagotoji();
		}

		ShowGuzai(false);
	}

	// オーナー用の処理
	public void PickGuzai()
	{
		index = (int)Random.Range(0, guzai.Length);
		RequestSerialization();

		ShowGuzai(true);
	}

	// 同期処理
	public override void OnDeserialization()
	{
		ShowGuzai(true);
	}

	public void ShowGuzai(bool isInit)
	{
		if (index < 0 || index >= guzai.Length)
		{
			return;
		}

		if (isInit)
		{
			foreach (Guzai gu in guzai)
			{
				gu.main.SetActive(false);
			}
		}
		guzai[index].main.SetActive(true);
	}
}
