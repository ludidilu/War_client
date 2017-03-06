﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using assetManager;
using System;

namespace textureFactory{
	
	public class TextureFactoryUnit2<T> :ITextureFactoryUnit where T:UnityEngine.Object {
		
		private string name;
		private T[] data;
		private int type = -1;
		
		private bool isDispose = false;

		private LinkedList<KeyValuePair<Action<T,string>,int>> callBackList = new LinkedList<KeyValuePair<Action<T, string>, int>>();
		
		public TextureFactoryUnit2(string _name){
			
			name = _name;
		}
		
		public T GetTexture(int _index,Action<T,string> _callBack){
			
			if (type == -1) {
				
				type = 0;

				callBackList.AddLast (new KeyValuePair<Action<T, string>, int>(_callBack,_index));

				T[] result = AssetManager.Instance.GetAsset<T> (name,GetAsset);

				if(result == null){

					return default(T);

				}else{

					return AssetManager.Instance.GetAsset<T> (name,GetAsset)[_index];
				}
				
			} else if (type == 0) {

				callBackList.AddLast (new KeyValuePair<Action<T, string>, int>(_callBack,_index));
				
				return default(T);
				
			} else {
				
				if(_callBack != null){
					
					_callBack(data[_index],string.Empty);
				}
				
				return data[_index];
			}
		}
		
		private void GetAsset(T[] _data,string _msg){
			
			if(isDispose){

				for(int i = 0 ; i < _data.Length ; i++){
				
					Resources.UnloadAsset(_data[i]);
				}

				return;
			}
			
			data = _data;
			
			type = 1;

			LinkedList<KeyValuePair<Action<T,string>,int>>.Enumerator enumerator = callBackList.GetEnumerator();

			while(enumerator.MoveNext()){

				Action<T,string> callBack = enumerator.Current.Key;

				if(callBack != null){

					int index = enumerator.Current.Value;
					
					callBack(data[index],string.Empty);
				}
			}

			callBackList.Clear();
		}
		
		public void Dispose(){
			
			if (type == 1) {
				
				for(int i = 0 ; i < data.Length ; i++){
					
					Resources.UnloadAsset(data[i]);
				}
				
				data = null;
				
			}else{
				
				isDispose = true;
			}
		}
	}
}
