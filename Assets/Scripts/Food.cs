using UnityEngine;
using System.Collections;

public class Food : MonoBehaviour {

	public float speed = 1f;
	public int type;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards (transform.position, new Vector3 (0, 4, 0), step);
	}

	public int GetFoodType() {
		return type;
	}
}
