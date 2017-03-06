using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Follow2D{

	private int num;

	private List<KeyValuePair<Vector2, int>> path = new List<KeyValuePair<Vector2, int>>();

	private Action<int,Vector2,int> changeCallBack;

	private Vector2 headPos;

	private float gap;

	public void Init(Vector2 _pos,int _num,float _gap,Action<int,Vector2,int> _changeCallBack){

		headPos = _pos;

		num = _num;

		gap = _gap;

		changeCallBack = _changeCallBack;
	}

	public void ResetPos(){

		path.Clear();
	}
	
	public void AddPathPoint(int _data){
		
		path.Add(new KeyValuePair<Vector2, int>(headPos,_data));
	}
	
	public void SetPos(Vector2 _pos){

		headPos = _pos;

		Vector2 next = _pos;
		
		float tmpDis = 0;

		int startIndex = path.Count - 1;
		
		for(int i = 0 ; i < num; i++){
			
			float dis = (i + 1) * gap;
			
			bool posChange = false;
			
			for(int m = startIndex ; m > -1 ; m--){

				KeyValuePair<Vector2,int> pair = path[m];

				Vector2 p = pair.Key;
				
				float dis2 = Vector2.Distance(next,p);
				
				float tmpDis2 = tmpDis + dis2;
				
				if(tmpDis2 > dis){
					
					startIndex = m;
					
					Vector2 result = Vector2.Lerp(p,next,(tmpDis2 - dis) / dis2);
					
					changeCallBack(i,result,pair.Value);
					
					posChange = true;
					
					break;
				}
				
				tmpDis = tmpDis2;
				
				next = p;
			}
			
			if(!posChange){
				
				return;
				
			}else{
				
				if(i == num - 1){
					
					if(startIndex > 0){
						
						path.RemoveRange(0,startIndex);
					}
				}
			}
		}
	}
}
