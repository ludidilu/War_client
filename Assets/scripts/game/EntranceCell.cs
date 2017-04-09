using UnityEngine;
using System.Collections;
using superList;
using UnityEngine.UI;

public class EntranceCell : SuperListCell {

	[SerializeField]
	private Text text;

	public override bool SetData (object _data)
	{
		text.text = (string)_data;

		return base.SetData (_data);
	}
}
