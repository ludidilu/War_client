﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace gameObjectFactory{

	public class GameObjectFactory{

		public Dictionary<string,GameObjectFactoryUnit> dic = new Dictionary<string, GameObjectFactoryUnit>();

		private static GameObjectFactory _Instance;

		public static GameObjectFactory Instance {

			get {

				if(_Instance == null){

					_Instance = new GameObjectFactory();
				}

				return _Instance;
			}
		}

		public void PreloadGameObjects(string[] _paths,Action<string> _callBack){

			int loadNum = _paths.Length;

			string msg = string.Empty;

			Action<string> callBack = delegate(string _msg) {

				loadNum--;

				if(!string.IsNullOrEmpty(_msg)){

					msg += _msg;
				}

				if(loadNum == 0){

					_callBack(msg);
				}
			};

			for(int i = 0 ; i < _paths.Length ; i++){

				PreloadGameObject(_paths[i],callBack);
			}
		}

		public void PreloadGameObject(string _path,Action<string> _callBack){

			GameObjectFactoryUnit unit;
			
			if (!dic.ContainsKey (_path)) {
				
				unit = new GameObjectFactoryUnit (_path);
				
				dic.Add(_path,unit);
				
			} else {
				
				unit = dic [_path];
			}
			
			unit.PreloadGameObject (_callBack);
		}

		public GameObject GetGameObject(string _path,Action<GameObject,string> _callBack){
			
			GameObjectFactoryUnit unit;
			
			if (!dic.ContainsKey (_path)) {
				
				unit = new GameObjectFactoryUnit (_path);
				
				dic.Add(_path,unit);
				
			} else {
				
				unit = dic [_path];
			}
			
			return unit.GetGameObject (_callBack);
		}

		public bool Hold(string _path){

			if(!dic.ContainsKey(_path)){

				return false;

			}else{

				GameObjectFactoryUnit unit = dic[_path];

				unit.AddUseNum();

				return true;
			}
		}

		public bool Release(string _path){

			if(!dic.ContainsKey(_path)){
				
				return false;
				
			}else{
				
				GameObjectFactoryUnit unit = dic[_path];
				
				unit.DelUseNum();
				
				return true;
			}
		}

		public void Dispose(bool _force){

			List<string> delKeyList = new List<string> ();

			Dictionary<string,GameObjectFactoryUnit>.Enumerator enumerator = dic.GetEnumerator();

			while(enumerator.MoveNext()){

				KeyValuePair<String,GameObjectFactoryUnit> pair = enumerator.Current;

				if (_force || pair.Value.useNum == 0) {

					pair.Value.Dispose ();

					delKeyList.Add (pair.Key);
				}
			}

			for(int i = 0 ; i < delKeyList.Count ; i++){

				dic.Remove(delKeyList[i]);
			}
		}
	}
}