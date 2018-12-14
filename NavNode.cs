using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode : MonoBehaviour
{
    public bool wasTriggered = false;
    public float waitTime = 0;
    public string nodeType = "Standard";
    public bool standard = true;
    public bool jump = false;
    public bool loop = false;
    public bool waitForPlayer = false;
    public bool start = false;
    public bool end = false;
    public bool grab = false;
    public bool fast = false;
    public bool straight = false;
    float jumpTime =0;
    public int id;

    public bool slow = true;
    public int pingCount = 0;
    public float jumpStrength =.7f;
    public int goToNum =1;
    public List<GameObject> neighbors; 
    // Use this for initialization 
    void Start()
    {
       // jumpTime = Time.deltaTime;

    }

    // Update is called once per frame
    void Update()
    {
        Check();
        SetNodeType();

        if (wasTriggered)
        {
            transform.GetComponent<Renderer>().material.color = new Color(255, 0, 0, 1f);
        }
    }

    void Check()
    {
        if (jump)
        {

            float radius = 2;
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hit in colliders)
            {
                if (hit.tag == "AI")
                {
                    Debug.Log("Jumping!");
                    hit.transform.GetComponent<AI>().Jump();
                    hit.transform.GetComponent<AI>().Jump(jumpStrength);
                    jumpTime = Time.deltaTime;
                }
            }

        }

            if (fast)
            {

            float radius = 2;
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                foreach (Collider hit in colliders)
                {
                    if (hit.tag == "AI")
                    {

                    //hit.transform.GetComponent<AI>().isSpeeding = true;
                        
                    }
                }
            }
        }

      /*  if (grab)
        {
            //waitTime = 0;
            float radius = 1f;
            Collider[] colliders = Physics.OverlapSphere(transform.GetChild(0).transform.position, radius);
            foreach (Collider hit in colliders)
            {
                if (hit.tag == "AI")
                {
                    //hit.transform.GetComponent<AnimationsAI>().nodeNumber++;
                    hit.transform.GetComponent<AnimationsAI>().Grab();
                  
                }
            }
        }
*/
    

    void SetNodeType()
    {
        if (jump) { this.nodeType = "Jump"; }
        if (standard) { this.nodeType = "Standard"; }
        if (waitForPlayer) { this.nodeType = "WaitForPlayer"; }
        if (start) { this.nodeType = "GoToStart"; }
        if (end) { this.nodeType = "GoToEnd"; }
        if (loop) { this.nodeType = "Loop"; }
        if (grab) { this.nodeType = "Grab"; }
        if (straight) { this.nodeType = "Straight"; }
    }
    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}