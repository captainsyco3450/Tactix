using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyes : MonoBehaviour
{
    Transform target;
    GameObject[] array;

    RaycastHit hit2;
    // Use this for initialization
    void Start()
    {
        if (transform.tag == "Swat")
        {
            array = GameObject.FindGameObjectsWithTag("Terrorist");
        }

        if (transform.tag == "Terrorist")
        {
            array = GameObject.FindGameObjectsWithTag("Swat");
        }

    }

    // Update is called once per frame
    void Update()
    {
        SightSee();
    }
    
    void SightSee()
    {
       
        foreach (GameObject obj in array)
        { 
            // Debug.Log(hit2.collider.name);
            //target = transform.parent.GetComponent<AnimationsAI>().getCurrentNode();
            //Debug.DrawRay(transform.position, obj.transform.position - transform.position * 50, Color.red);
            //transform.LookAt(targetNode);
            Vector3 pos = (obj.transform.position - transform.position).normalized;
            if (Vector3.Dot(pos, transform.forward) > 0)
            {
                if (Physics.Raycast(transform.position + transform.up, obj.transform.position - transform.position, out hit2, 5.0F))
            {
                
                    if (transform.tag == "Swat" && hit2.collider.tag == "Terrorist")
                    {
                        target = hit2.collider.transform;
                        Debug.Log("I have eyes on tango");
                        transform.GetComponent<PlayerAI>().target = target;


                    }
                    if (transform.tag == "Terrorist" && hit2.collider.tag == "Swat")
                    {
                        target = hit2.collider.transform;
                        Debug.Log("We've Got Company");
                        transform.GetComponent<AI>().target = target;
                    }
                }
            }
        }
    }
}

