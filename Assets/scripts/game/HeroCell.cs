using UnityEngine;
using System.Collections;
using superList;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeroCellData{

	public int id;

	public bool added;

	public int cd;

	public bool selectable;

	public HeroCellData(int _id, bool _added, int _cd, bool _selectable){

		id = _id;

		added = _added;

		cd = _cd;

		selectable = _selectable;
	}
}

public class HeroCell : SuperListCell {

	[SerializeField]
	private Image img;

	[SerializeField]
	private Text idTf;

	[SerializeField]
	private Text prizeTf;

	[SerializeField]
	private Image cdImg;

	public override void OnPointerClick (PointerEventData eventData)
	{
		if (selected) {

			superList.SetSelectedIndex (-1);

		} else {

			HeroCellData cellData = data as HeroCellData;

			if(cellData.selectable){

				base.OnPointerClick (eventData);
			}
		}
	}  

	public override bool SetData(object _data){

		HeroCellData cellData = _data as HeroCellData;

		UnitSDS sds = StaticData.GetData<UnitSDS> (cellData.id);

		idTf.text = sds.ID.ToString ();

		prizeTf.text = sds.prize.ToString ();

		if (cellData.added) {

			idTf.color = Color.red;

		} else {
			
			idTf.color = Color.black;
		}

		if (cellData.cd == 0){

			cdImg.gameObject.SetActive(false);

		} else {

			cdImg.gameObject.SetActive(true);

			UnitSDS unitSDS = StaticData.GetData<UnitSDS>(cellData.id);

			SkillSDS skillSDS = StaticData.GetData<SkillSDS>(unitSDS.skill);

			cdImg.fillAmount = (float)cellData.cd / skillSDS.cd;
		}

		if (selected && !cellData.selectable) {

			superList.SetSelectedIndex(-1);
		}

		return base.SetData (_data);
	}

	public override void SetSelected(bool _value){

		img.color = _value ? Color.green : Color.white;

		base.SetSelected (_value);
	}
}
