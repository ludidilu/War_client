using UnityEngine;
using System.Collections;
using System.IO;
using System;
using superRaycast;

public class BattleControlTest : MonoBehaviour {

	[SerializeField]
	private BattleManager battleManager1;

	[SerializeField]
	private BattleManager battleManager2;

	[SerializeField]
	private GameObject battleMain1;

	[SerializeField]
	private GameObject battleMain2;

	private Battle battleServer;

	// Use this for initialization
	void Start () {

		battleServer = new Battle ();

		battleServer.ServerInit (ServerSendData, BattleOver);

		Action<MemoryStream> dele = delegate(MemoryStream obj) {
		
			ClientSendData(true, obj);
		};

		battleManager1.Init (dele, BattleOver);

		battleManager1.gameObject.SetActive (true);

		dele = delegate(MemoryStream obj) {

			ClientSendData(false, obj);
		};

		SuperRaycast.SetIsOpen (false, "");

		battleManager2.Init (dele, BattleOver);

		battleManager2.gameObject.SetActive (true);

		battleMain2.SetActive (false);

		battleServer.ServerRefresh (true);

		battleServer.ServerRefresh (false);
	}

	void FixedUpdate(){

		battleServer.Update ();
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

	private void BattleOver(){


	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyUp (KeyCode.A)) {

			battleMain1.SetActive (!battleMain1.activeSelf);

			battleMain2.SetActive (!battleMain2.activeSelf);
		}
	}
}
