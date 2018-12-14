using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class casingproject1 : MonoBehaviour {
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * Random.Range(15, 30));
        rb.AddForce(transform.right * Random.Range(-30,-15));
        rb.AddForce(transform.forward * Random.Range(-20, 20));
        //transform.localEulerAngles = new Vector3(Random.Range(0, 90), Random.Range(0, 90), Random.Range(0, 90));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
