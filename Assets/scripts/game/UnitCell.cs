using UnityEngine;
using System.Collections;
using superList;
using UnityEngine.UI;

public class UnitCellData{

	public int id;
	public int num;

	public UnitCellData(int _id, int _num){

		id = _id;
		num = _num;
	}
}

public class UnitCell : SuperListCell {

	[SerializeField]
	private Text idTf;

	[SerializeField]
	private Text prizeTf;

	[SerializeField]
	private Text numTf;

	public override bool SetData(object _data){

		UnitCellData cellData = _data as UnitCellData;

		UnitSDS sds = StaticData.GetData<UnitSDS> (cellData.id);

		idTf.text = sds.ID.ToString ();

		prizeTf.text = sds.prize.ToString ();

		numTf.text = cellData.num.ToString ();

		return base.SetData (_data);
	}
}
