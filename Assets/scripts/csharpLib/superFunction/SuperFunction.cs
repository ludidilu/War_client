﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using superFunction;

namespace superFunction{

	public class SuperFunction{

		private static SuperFunction _Instance;

		public static SuperFunction Instance{

			get{

				if(_Instance == null){

					_Instance = new SuperFunction();
				}

				return _Instance;
			}
		}

		public delegate void SuperFunctionCallBack0(int _index);
		public delegate void SuperFunctionCallBack1<T1>(int _index, T1 t1);
		public delegate void SuperFunctionCallBack2<T1, T2>(int _index, T1 t1, T2 t2);
		public delegate void SuperFunctionCallBack3<T1, T2, T3>(int _index, T1 t1, T2 t2, T3 t3);
		public delegate void SuperFunctionCallBack4<T1, T2, T3, T4>(int _index, T1 t1, T2 t2, T3 t3, T4 t4);

		private Dictionary<int,SuperFunctionUnit> dic;
		private Dictionary<GameObject,Dictionary<string,List<SuperFunctionUnit>>> dic2;

		private int index = 0;

		private Action<int> removeDelegate;

		public SuperFunction(){

			dic = new Dictionary<int, SuperFunctionUnit>();
			dic2 = new Dictionary<GameObject,Dictionary<string,List<SuperFunctionUnit>>>();
		}

		public void AddRemoveDelegate(Action<int> _dele){

			removeDelegate += _dele;
		}

		public void RemoveRemoveDelegate(Action<int> _dele){

			removeDelegate -= _dele;
		}

		public int AddOnceEventListener(GameObject _target,string _eventName,SuperFunctionCallBack0 _callBack){

			return AddEventListener(_target,_eventName,_callBack,true);
		}

		public int AddEventListener(GameObject _target,string _eventName,SuperFunctionCallBack0 _callBack){

			return AddEventListener(_target,_eventName,_callBack,false);
		}

		public int AddOnceEventListener<T1>(GameObject _target,string _eventName,SuperFunctionCallBack1<T1> _callBack){

			return AddEventListener(_target,_eventName,_callBack,true);
		}

		public int AddEventListener<T1>(GameObject _target,string _eventName,SuperFunctionCallBack1<T1> _callBack){

			return AddEventListener(_target,_eventName,_callBack,false);
		}

		public int AddOnceEventListener<T1,T2>(GameObject _target,string _eventName,SuperFunctionCallBack2<T1,T2> _callBack){

			return AddEventListener(_target,_eventName,_callBack,true);
		}

		public int AddEventListener<T1,T2>(GameObject _target,string _eventName,SuperFunctionCallBack2<T1,T2> _callBack){

			return AddEventListener(_target,_eventName,_callBack,false);
		}

		public int AddOnceEventListener<T1,T2,T3>(GameObject _target,string _eventName,SuperFunctionCallBack3<T1,T2,T3> _callBack){

			return AddEventListener(_target,_eventName,_callBack,true);
		}

		public int AddEventListener<T1,T2,T3>(GameObject _target,string _eventName,SuperFunctionCallBack3<T1,T2,T3> _callBack){

			return AddEventListener(_target,_eventName,_callBack,false);
		}

		public int AddOnceEventListener<T1,T2,T3,T4>(GameObject _target,string _eventName,SuperFunctionCallBack4<T1,T2,T3,T4> _callBack){

			return AddEventListener(_target,_eventName,_callBack,true);
		}

		public int AddEventListener<T1,T2,T3,T4>(GameObject _target,string _eventName,SuperFunctionCallBack4<T1,T2,T3,T4> _callBack){

			return AddEventListener(_target,_eventName,_callBack,false);
		}

		private int AddEventListener(GameObject _target,string _eventName,Delegate _callBack,bool _isOnce){

			int result = GetIndex();

			SuperFunctionUnit unit = new SuperFunctionUnit(_target,_eventName,_callBack,result,_isOnce);

			dic.Add(result,unit);

			Dictionary<string,List<SuperFunctionUnit>> tmpDic;

			if(dic2.ContainsKey(_target)){

				tmpDic = dic2[_target];

			}else{

				_target.AddComponent<SuperFunctionControl>();

				tmpDic = new Dictionary<string,List<SuperFunctionUnit>>();

				dic2.Add(_target,tmpDic);
			}

			List<SuperFunctionUnit> tmpList;

			if(tmpDic.ContainsKey(_eventName)){

				tmpList = tmpDic[_eventName];

			}else{

				tmpList = new List<SuperFunctionUnit>();

				tmpDic.Add(_eventName,tmpList);
			}

			tmpList.Add(unit);

			return result;
		}

		public void RemoveEventListener(int _index){

			if(dic.ContainsKey(_index)){

				SuperFunctionUnit unit = dic[_index];

				dic.Remove(_index);

				if(removeDelegate != null){

					removeDelegate(_index);
				}

				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[unit.target];

				List<SuperFunctionUnit> tmpList = tmpDic[unit.eventName];

				tmpList.Remove(unit);

				if(tmpList.Count == 0){

					tmpDic.Remove(unit.eventName);

					if(tmpDic.Count == 0){

						DestroyControl(unit.target);
					}
				}
			}
		}

		public void RemoveEventListener(GameObject _target){

			if(dic2.ContainsKey(_target)){

				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];
				
				DestroyControl(_target);

				Dictionary<string,List<SuperFunctionUnit>>.ValueCollection.Enumerator enumerator = tmpDic.Values.GetEnumerator();

				while(enumerator.MoveNext()){

					List<SuperFunctionUnit> tmpList = enumerator.Current;

					for(int i = 0 ; i < tmpList.Count ; i++){

						SuperFunctionUnit unit = tmpList[i];

						dic.Remove(unit.index);

						if(removeDelegate != null){

							removeDelegate(unit.index);
						}
					}
				}
			}
		}

		public void RemoveEventListener(GameObject _target,string _eventName){

			if(dic2.ContainsKey(_target)){
				
				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];

				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnit> list = tmpDic[_eventName];

					for(int i = 0 ; i < list.Count ; i++){

						SuperFunctionUnit unit = list[i];

						dic.Remove(unit.index);

						if(removeDelegate != null){

							removeDelegate(unit.index);
						}
					}

					tmpDic.Remove(_eventName);

					if(tmpDic.Count == 0){

						DestroyControl(_target);
					}
				}
			}
		}

		public void RemoveEventListener(GameObject _target,string _eventName,SuperFunctionCallBack0 _callBack){

			RemoveEventListenerReal(_target,_eventName,_callBack);
		}

		public void RemoveEventListener<T1>(GameObject _target,string _eventName,SuperFunctionCallBack1<T1> _callBack){

			RemoveEventListenerReal(_target,_eventName,_callBack);
		}

		public void RemoveEventListener<T1,T2>(GameObject _target,string _eventName,SuperFunctionCallBack2<T1,T2> _callBack){

			RemoveEventListenerReal(_target,_eventName,_callBack);
		}

		public void RemoveEventListener<T1,T2,T3>(GameObject _target,string _eventName,SuperFunctionCallBack3<T1,T2,T3> _callBack){

			RemoveEventListenerReal(_target,_eventName,_callBack);
		}

		public void RemoveEventListener<T1,T2,T3,T4>(GameObject _target,string _eventName,SuperFunctionCallBack4<T1,T2,T3,T4> _callBack){

			RemoveEventListenerReal(_target,_eventName,_callBack);
		}

		private void RemoveEventListenerReal(GameObject _target,string _eventName,Delegate _callBack){

			if(dic2.ContainsKey(_target)){
				
				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){
					
					List<SuperFunctionUnit> list = tmpDic[_eventName];

					for(int i = 0 ; i < list.Count ; i++){

						SuperFunctionUnit unit = list[i];

						if(unit.callBack == _callBack){

							dic.Remove(unit.index);

							if(removeDelegate != null){

								removeDelegate(unit.index);
							}

							list.RemoveAt(i);

							break;
						}
					}

					if(list.Count == 0){

						tmpDic.Remove(_eventName);

						if(tmpDic.Count == 0){

							DestroyControl(_target);
						}
					}
				}
			}
		}

		public void DispatchEvent(GameObject _target,string _eventName){

			if (dic2.ContainsKey (_target)) {
				
				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];
				
				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnit> tmpList = tmpDic[_eventName];

					LinkedList<SuperFunctionUnit> unitList = null;

					for(int i = 0 ; i < tmpList.Count ; i++){

						SuperFunctionUnit unit = tmpList[i];

						if(unit.callBack is SuperFunctionCallBack0){

							if(unitList == null){

								unitList = new LinkedList<SuperFunctionUnit>();
							}

							unitList.AddLast(unit);
						}
					}

					if(unitList != null){

						LinkedList<SuperFunctionUnit>.Enumerator enumerator = unitList.GetEnumerator();

						while(enumerator.MoveNext()){

							if(enumerator.Current.isOnce){

								RemoveEventListener(enumerator.Current.index);
							}

							SuperFunctionCallBack0 cb = enumerator.Current.callBack as SuperFunctionCallBack0;

							cb(enumerator.Current.index);
						}
					}
				}
			}
		}

		public void DispatchEvent<T1>(GameObject _target,string _eventName,T1 t1){

			if (dic2.ContainsKey (_target)) {

				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];

				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnit> tmpList = tmpDic[_eventName];

					LinkedList<SuperFunctionUnit> unitList = null;

					for(int i = 0 ; i < tmpList.Count ; i++){

						SuperFunctionUnit unit = tmpList[i];

						if(unit.callBack is SuperFunctionCallBack1<T1>){

							if(unitList == null){

								unitList = new LinkedList<SuperFunctionUnit>();
							}

							unitList.AddLast(unit);
						}
					}

					if(unitList != null){

						LinkedList<SuperFunctionUnit>.Enumerator enumerator = unitList.GetEnumerator();

						while(enumerator.MoveNext()){

							if(enumerator.Current.isOnce){

								RemoveEventListener(enumerator.Current.index);
							}

							SuperFunctionCallBack1<T1> cb = enumerator.Current.callBack as SuperFunctionCallBack1<T1>;

							cb(enumerator.Current.index,t1);
						}
					}
				}
			}
		}

		public void DispatchEvent<T1,T2>(GameObject _target,string _eventName,T1 t1, T2 t2){

			if (dic2.ContainsKey (_target)) {

				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];

				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnit> tmpList = tmpDic[_eventName];

					LinkedList<SuperFunctionUnit> unitList = null;

					for(int i = 0 ; i < tmpList.Count ; i++){

						SuperFunctionUnit unit = tmpList[i];

						if(unit.callBack is SuperFunctionCallBack2<T1,T2>){

							if(unitList == null){

								unitList = new LinkedList<SuperFunctionUnit>();
							}

							unitList.AddLast(unit);
						}
					}

					if(unitList != null){

						LinkedList<SuperFunctionUnit>.Enumerator enumerator = unitList.GetEnumerator();

						while(enumerator.MoveNext()){

							if(enumerator.Current.isOnce){

								RemoveEventListener(enumerator.Current.index);
							}

							SuperFunctionCallBack2<T1,T2> cb = enumerator.Current.callBack as SuperFunctionCallBack2<T1,T2>;

							cb(enumerator.Current.index,t1,t2);
						}
					}
				}
			}
		}

		public void DispatchEvent<T1,T2,T3>(GameObject _target,string _eventName,T1 t1,T2 t2,T3 t3){

			if (dic2.ContainsKey (_target)) {

				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];

				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnit> tmpList = tmpDic[_eventName];

					LinkedList<SuperFunctionUnit> unitList = null;

					for(int i = 0 ; i < tmpList.Count ; i++){

						SuperFunctionUnit unit = tmpList[i];

						if(unit.callBack is SuperFunctionCallBack3<T1,T2,T3>){

							if(unitList == null){

								unitList = new LinkedList<SuperFunctionUnit>();
							}

							unitList.AddLast(unit);
						}
					}

					if(unitList != null){

						LinkedList<SuperFunctionUnit>.Enumerator enumerator = unitList.GetEnumerator();

						while(enumerator.MoveNext()){

							if(enumerator.Current.isOnce){

								RemoveEventListener(enumerator.Current.index);
							}

							SuperFunctionCallBack3<T1,T2,T3> cb = enumerator.Current.callBack as SuperFunctionCallBack3<T1,T2,T3>;

							cb(enumerator.Current.index,t1,t2,t3);
						}
					}
				}
			}
		}

		public void DispatchEvent<T1,T2,T3,T4>(GameObject _target,string _eventName,T1 t1,T2 t2,T3 t3,T4 t4){

			if (dic2.ContainsKey (_target)) {

				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];

				if(tmpDic.ContainsKey(_eventName)){

					List<SuperFunctionUnit> tmpList = tmpDic[_eventName];

					LinkedList<SuperFunctionUnit> unitList = null;

					for(int i = 0 ; i < tmpList.Count ; i++){

						SuperFunctionUnit unit = tmpList[i];

						if(unit.callBack is SuperFunctionCallBack4<T1,T2,T3,T4>){

							if(unitList == null){

								unitList = new LinkedList<SuperFunctionUnit>();
							}

							unitList.AddLast(unit);
						}
					}

					if(unitList != null){

						LinkedList<SuperFunctionUnit>.Enumerator enumerator = unitList.GetEnumerator();

						while(enumerator.MoveNext()){

							if(enumerator.Current.isOnce){

								RemoveEventListener(enumerator.Current.index);
							}

							SuperFunctionCallBack4<T1,T2,T3,T4> cb = enumerator.Current.callBack as SuperFunctionCallBack4<T1,T2,T3,T4>;

							cb(enumerator.Current.index,t1,t2,t3,t4);
						}
					}
				}
			}
		}

		public void DestroyGameObject(GameObject _target){

			if(dic2.ContainsKey(_target)){

				Dictionary<string,List<SuperFunctionUnit>> tmpDic = dic2[_target];
				
				dic2.Remove(_target);

				Dictionary<string,List<SuperFunctionUnit>>.ValueCollection.Enumerator enumerator = tmpDic.Values.GetEnumerator();
				
				while(enumerator.MoveNext()){
					
					List<SuperFunctionUnit> tmpList = enumerator.Current;
					
					for(int i = 0 ; i < tmpList.Count ; i++){

						SuperFunctionUnit unit = tmpList[i];

						dic.Remove(unit.index);

						if(removeDelegate != null){

							removeDelegate(unit.index);
						}
					}
				}
			}
		}

		private void DestroyControl(GameObject _target){

			SuperFunctionControl[] controls = _target.GetComponents<SuperFunctionControl>();

			for(int i = 0 ; i < controls.Length ; i++){
			
				SuperFunctionControl control = controls[i];

				if(!control.isDestroy){

					control.isDestroy = true;
					
					GameObject.Destroy(control);
				}
			}

			dic2.Remove(_target);
		}

		private int GetIndex(){

			index++;

			int result = index;

			return result;
		}

		public int GetNum(){

			return dic.Count;
		}
	}
}