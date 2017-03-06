﻿using UnityEngine;
using System.Collections;

namespace scene{

	public class LightmapGameObject : MonoBehaviour {

		[SerializeField] public int lightmapIndex;
		[SerializeField] public Vector4 lightmapScaleOffset;

		void Awake(){

			Renderer r = gameObject.GetComponent<Renderer>();

			r.lightmapScaleOffset = lightmapScaleOffset;

			r.lightmapIndex = lightmapIndex;
		}
	}
}