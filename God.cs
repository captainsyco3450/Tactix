using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    public Transform node;

    int nodeNum = 1;
    float speed = 9;
    float timer = 0;
    float nexTime = 0;
    int unitNumber = 0;
    int nodeNum1 = 1;
    int nodeNum2 = 1;
    int nodeNum3 = 1;
    int nodeNum4 = 1;
    bool play = false;
    public bool graphBuilt = false;
    public GameObject[] array;
    GameObject[] nodeArray;
    int num = 1;
    // Use this for initialization
    void Start()
    {

        array = GameObject.FindGameObjectsWithTag("Swat");
        
       
        Invoke("buildGraph", 4);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !play)
        {
            foreach (GameObject obj in array)
            {
                obj.transform.GetComponent<PlayerAI>().setIswalking(true);
            }

        }
        if (Input.GetKey(KeyCode.Space) && play)
        {
            foreach (GameObject obj in array)
            {
                obj.transform.GetComponent<PlayerAI>().setIswalking(false);
            }
        }

        timer++;
        Move();
        if (Input.GetMouseButtonDown(0))
        {
            switch (unitNumber)
            {
                case 1:
                    nodeNum = nodeNum1;
                    break;
                case 2:
                    nodeNum = nodeNum2;
                    break;
                case 3:
                    nodeNum = nodeNum3;
                    break;
                case 4:
                    nodeNum = nodeNum4;
                    break;



            }
            RaycastHit hit;
            Ray ray = transform.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // Debug.Log(hit.transform.tag);
                if (hit.transform.tag == "walkable" && unitNumber != 0)
                {

                    Transform newNode = Instantiate(node, hit.point, transform.rotation);
                    newNode.name = "NavNode (" + nodeNum + " | " + unitNumber + ")";
                    newNode.GetComponent<NavNode>().waitTime = .000000004f;
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        newNode.GetComponent<NavNode>().slow = false;

                    }
                    switch (unitNumber)
                    {
                        case 1:
                            nodeNum1++;
                            break;
                        case 2:
                            nodeNum2++;
                            break;
                        case 3:
                            nodeNum3++;
                            break;
                        case 4:
                            nodeNum4++;
                            break;
                    }
                }
                if (hit.transform.tag == "Swat" && !hit.transform.GetComponent<PlayerAI>().selected && timer > nexTime)
                {
                    unitNumber = hit.transform.GetComponent<PlayerAI>().GetUnitNumber();
                    nexTime = timer + 6;
                    foreach (GameObject obj in array)
                    {
                        obj.transform.GetComponent<PlayerAI>().selected = false;
                    }
                    hit.transform.GetComponent<PlayerAI>().selected = true;


                    if (hit.transform.tag == "Swat" && hit.transform.GetComponent<PlayerAI>().selected && timer > nexTime)
                    {
                        nexTime = timer + 6;
                        hit.transform.GetComponent<PlayerAI>().selected = false;
                    }

                }

            }
        }
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 50;
        }
        else
        {
            speed = 9;
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += transform.right * Time.deltaTime * -speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * speed;
        }
    }

    void buildGraph()
    {
        nodeArray = GameObject.FindGameObjectsWithTag("NavNode");
        foreach (GameObject node in nodeArray)
        {
            NavNode nav = node.GetComponent<NavNode>();
            nav.name = "NavNode (" + num + ")";
            nav.id = num;
            num++;
        }
        nodeArray = GameObject.FindGameObjectsWithTag("NavNode");
        foreach (GameObject node in nodeArray)
        {
            NavNode nav = node.GetComponent<NavNode>();

            foreach (GameObject otherNode in nodeArray)
            {
                //NavNode otherNav = otherNode.GetComponent<NavNode>();


                RaycastHit hit;
                float dist = Vector3.Distance(otherNode.transform.position, node.transform.position);
                // Debug.Log("Distance is: " + dist);
                ////Debug.DrawRay(node.transform.position, otherNode.transform.position - node.transform.position * 100, Color.red);
                //if (Physics.Linecast(node.transform.position, otherNode.transform.position, out hit))
                //{
                ////Debug.DrawRay(node.transform.position, otherNode.transform.position - node.transform.position * 100, Color.red);
                if (Physics.Raycast(node.transform.position, otherNode.transform.position -node.transform.position , out hit,5))
                {
                    //Debug.DrawLine(node.transform.position, hit.point, Color.cyan);
                    //Debug.Log(hit.transform.name);
                    if (hit.collider.tag == "NavNode")
                    {
                        hit.transform.GetComponent<Renderer>().material.color = new Color(0, 234, 0, 1f);
                        if (dist < 20)
                        {
                            if (!nav.neighbors.Contains(hit.transform.gameObject))
                            {
                                nav.neighbors.Add(hit.transform.gameObject);
                            }
                        }
                    }


                }
            }

        }
        foreach (GameObject node in nodeArray)
        {
            node.GetComponent<SphereCollider>().enabled = false;
            //node.GetComponent<Renderer>().enabled = false;
        }
        graphBuilt = true;
    }
}

