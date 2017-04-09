using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;

public class BattleControl : MonoBehaviour {

	enum PlayerState
	{
		FREE,
		SEARCH,
		BATTLE
	}

	enum PlayerAction
	{
		SEARCH,
		CANCEL_SEARCH
	}

	[SerializeField]
	private BattleManager battleManager;

	// Use this for initialization
	void Start () {
	
		battleManager.Init (SendData, BattleOver);

		Connection.Instance.Init (ConfigDictionary.Instance.ip, ConfigDictionary.Instance.port, GetBytes, ConfigDictionary.Instance.uid);
	}

	private void BattleOver(){

		SceneManager.LoadScene ("entrance");
	}

	private void SendData(MemoryStream _ms){

		using (MemoryStream ms = new MemoryStream ()) {

			using (BinaryWriter bw = new BinaryWriter (ms)) {

				bw.Write ((short)0);

				short length = (short)_ms.Length;

				bw.Write (length);

				bw.Write (_ms.GetBuffer (), 0, length);

				Connection.Instance.Send (ms);
			}
		}
	}

	private void GetBytes(byte[] _bytes){

		using (MemoryStream ms = new MemoryStream (_bytes)) {

			using (BinaryReader br = new BinaryReader (ms)) {

				short type = br.ReadInt16 ();

				switch (type) {

				case 0:

					if (!battleManager.gameObject.activeSelf) {

						battleManager.BattleStart ();
					}

					short length = br.ReadInt16 ();

					byte[] bytes = br.ReadBytes (length);

					battleManager.GetBytes (bytes);

					break;

				case 1:

					PlayerState playerState = (PlayerState)br.ReadInt16 ();

					switch (playerState) {

					case PlayerState.BATTLE:

						if (!battleManager.gameObject.activeSelf) {

							battleManager.BattleStart ();
						}

						break;

					case PlayerState.FREE:
						
						RequestBattle ();

						break;

					case PlayerState.SEARCH:


						break;
					}

					break;
				}
			}
		}
	}

	private void RequestBattle(){

		using (MemoryStream ms = new MemoryStream ()) {

			using (BinaryWriter bw = new BinaryWriter (ms)) {

				bw.Write ((short)1);

				bw.Write ((short)PlayerAction.SEARCH);

				Connection.Instance.Send (ms);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
