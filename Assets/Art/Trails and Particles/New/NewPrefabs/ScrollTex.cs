using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollTex : MonoBehaviour {

	public float Scrollx = 0.5f;
	public float Scrolly = 0.5f;

	void Update () {
		float Offsetx = Time.time * Scrollx;
		float Offsety = Time.time * Scrolly;
		GetComponent<Renderer> ().material.mainTextureOffset = new Vector2 (Offsetx, Offsety);

	}
}
