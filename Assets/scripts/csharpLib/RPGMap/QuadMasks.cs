using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuadMasks : MonoBehaviour {

	private const string HOLE_NAME = "hole";

	private const int TARGET_NUM = 30;

	[SerializeField]
	private MeshFilter mf;
	
	[SerializeField]
	private Camera renderCamera;
	
	[SerializeField]
	private Material mat;

	private float radios = 2;
	
	private List<Transform> targetList = new List<Transform>();
	
	// Use this for initialization
	void Start () {
		
		float yScale = 10 * renderCamera.orthographicSize / 5;
		
		float xScale = 10 * renderCamera.pixelRect.width / renderCamera.pixelRect.height * renderCamera.orthographicSize / 5;
		
		Vector3[] vs = mf.mesh.vertices;
		
		for(int i = 0 ; i < vs.Length ; i++){
			
			Vector3 v = vs[i];
			
			vs[i] = new Vector3(v.x * xScale,v.y * yScale,0);
		}
		
		mf.mesh.vertices = vs;

		transform.localScale = new Vector3(1 / transform.lossyScale.x,1 / transform.lossyScale.y,0);
	}

	public void SetLightRadios(float _radios){

		radios = _radios;
	}

	public void AddTarget(Transform _target){

		targetList.Add(_target);
	}

	public void RemoveTarget(Transform _target){

		targetList.Remove(_target);
	}
	
	// Update is called once per frame
	void Update () {

		Vector4[] vs = new Vector4[TARGET_NUM];
		
		for(int i = 0 ; i < targetList.Count && i < TARGET_NUM ; i++){
			
			Transform t = targetList[i];
			
			vs[i] = new Vector4(t.position.x - renderCamera.transform.position.x,t.position.y - renderCamera.transform.position.y,radios * renderCamera.orthographicSize / 5,0);
		}

#if UNITY_5_4 || UNITY_5_5

		mat.SetVectorArray(HOLE_NAME,vs);
#else
			
		for(int i = 0 ; i < TARGET_NUM ; i++){

			mat.SetVector(HOLE_NAME + i,vs[i]);
		}
#endif
	}
}
