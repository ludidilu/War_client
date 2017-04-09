using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

	[SerializeField]
	private GameObject panel;

	[SerializeField]
	private Text btText;

	private PlayerState playerState;

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
					
					short length = br.ReadInt16 ();

					byte[] bytes = br.ReadBytes (length);

					battleManager.GetBytes (bytes);

					break;

				case 1:

					playerState = (PlayerState)br.ReadInt16 ();

					switch (playerState) {

					case PlayerState.BATTLE:

						if (panel.activeSelf) {

							panel.SetActive (false);
						}

						battleManager.BattleStart ();

						break;

					case PlayerState.FREE:
						
						panel.SetActive (true);

						btText.text = "Search";

						break;

					case PlayerState.SEARCH:

						panel.SetActive (true);

						btText.text = "Cancel";

						break;
					}

					break;
				}
			}
		}
	}

	public void Click(){

		if (playerState == PlayerState.FREE) {

			RequestBattle ();

		} else {

			CancelRequestBattle ();
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

	private void CancelRequestBattle(){

		using (MemoryStream ms = new MemoryStream ()) {

			using (BinaryWriter bw = new BinaryWriter (ms)) {

				bw.Write ((short)1);

				bw.Write ((short)PlayerAction.CANCEL_SEARCH);

				Connection.Instance.Send (ms);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
