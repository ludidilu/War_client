using UnityEngine;
using System.Collections;
using System.IO;
using System;
using superRaycast;
using UnityEngine.SceneManagement;

public class BattleControlTest : MonoBehaviour {

	[SerializeField]
	private BattleManager battleManager1;

	[SerializeField]
	private BattleManager battleManager2;

	[SerializeField]
	private Camera battleCamera1;

	[SerializeField]
	private Camera battleCamera2;

	[SerializeField]
	private GameObject battleUI1;

	[SerializeField]
	private GameObject battleUI2;

	private Battle battleServer;

	private bool isStart = false;

	// Use this for initialization
	void Start () {

		battleServer = new Battle ();

		battleServer.ServerInit (ServerSendData);

		battleServer.ServerStart ();

		Action<MemoryStream> dele = delegate(MemoryStream obj) {
		
			ClientSendData(true, obj);
		};

		battleManager1.Init (dele, BattleOver);

		battleManager1.BattleStart ();

		dele = delegate(MemoryStream obj) {

			ClientSendData(false, obj);
		};

		SuperRaycast.SetIsOpen (false, "");

		battleManager2.Init (dele, BattleOver);

		battleManager2.BattleStart ();

		SuperRaycast.SetCamera (battleCamera1);

		battleCamera2.gameObject.SetActive (false);

		battleUI2.SetActive (false);

		battleServer.ServerRefresh (true);

		battleServer.ServerRefresh (false);

		isStart = true;
	}

	private void BattleOver(){

		SceneManager.LoadScene ("entrance");
	}

	void FixedUpdate(){

		if (isStart) {

			bool mWin;
			bool oWin;

			battleServer.Update (out mWin, out oWin);

			if (mWin || oWin) {

				isStart = false;
			}
		}
	}

	private void ClientSendData(bool _isMine, MemoryStream _ms){

		battleServer.ServerGetBytes (_isMine, _ms.GetBuffer ());
	}

	private void ServerSendData(bool _isMine, MemoryStream _ms){

		if (_isMine) {

			battleManager1.GetBytes (_ms.GetBuffer ());

		} else {

			battleManager2.GetBytes (_ms.GetBuffer ());
		}
	}

	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyUp (KeyCode.A)) {

			battleCamera1.gameObject.SetActive (!battleCamera1.gameObject.activeSelf);

			battleUI1.SetActive (!battleUI1.activeSelf);

			battleCamera2.gameObject.SetActive (!battleCamera2.gameObject.activeSelf);

			battleUI2.SetActive (!battleUI2.activeSelf);

			SuperRaycast.SetCamera (battleCamera1.gameObject.activeSelf ? battleCamera1 : battleCamera2);
		}
	}
}
