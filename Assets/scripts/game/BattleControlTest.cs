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
	private GameObject[] cameras1;

	[SerializeField]
	private GameObject[] cameras2;

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

		for (int i = 0; i < cameras2.Length; i++) {

			GameObject go = cameras2[i];

			go.SetActive(false);
		}

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

			for (int i = 0; i < cameras1.Length; i++) {

				GameObject go = cameras1[i];

				go.SetActive(!go.activeSelf);
			}

			for (int i = 0; i < cameras2.Length; i++) {

				GameObject go = cameras2[i];

				go.SetActive(!go.activeSelf);
			}
		}
	}
}
