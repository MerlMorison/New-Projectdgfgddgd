
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Guzai : UdonSharpBehaviour
{
	public GameObject main;
	public GameObject tamago;

	public void Init()
	{
		main.SetActive(false);
		main.GetComponent<MeshRenderer>().enabled = true;
		tamago.GetComponent<MeshRenderer>().enabled = false;
	}

	public void Tamagotoji() {
		main.GetComponent<MeshRenderer>().enabled = false;
		tamago.GetComponent<MeshRenderer>().enabled = true;
	}

	public void Pick() {
		main.SetActive(true);
	}
}
