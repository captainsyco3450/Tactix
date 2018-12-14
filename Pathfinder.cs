using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{

    Animator anim;
    CharacterController controller;
    Transform player;
    public int nodeNumber = 1;
    float distanceToNode;
    float distanceToPlayer;
    float distanceToGround;
    Vector3 movDir = Vector3.zero;
    int oldNode = 0;
    bool nearPlayer = false;
    bool waitingforPlayer = false;
    public float speed;
    float gravity = -1;
    bool isGrabbing = false;
    bool isJumping = false;
    bool isWalking = true;
    int health = 100;
    Transform currentNode;
    bool needGrab = false;
    bool isClimbing;
    public bool isRagdoll;
    bool grounded;
    bool isStaticRag;
    bool ignoreWalls;
    bool wallRight;
    bool wallLeft;
    public bool isStopped = false;
    bool needPath = true;
   
    public Transform target;
    GameObject[] nodeArray;
    public HashSet<Transform> usedNodeArray;
    public List<Transform> path;
    int count = 0;
    bool graphBuilt = false;
    int targetNode = 0;
    int randWait = 0;
    // Use this for initialization
    void Start()
    {




        //Invoke("walkPath", 4);
        nodeArray = GameObject.FindGameObjectsWithTag("NavNode");

        anim = GetComponent<Animator>();
        controller = transform.GetComponent<CharacterController>();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isJumping", false);
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        Collider[] col = GetComponentsInChildren<Collider>();
        foreach (Collider c in col)
        {
            c.enabled = false;
        }
        controller.enabled = true;

        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.useGravity = false;

        }


    }


    private void Update()
    {
        //Check if nodeGraph was built
        if (GameObject.FindGameObjectWithTag("God"))
        {
            graphBuilt = GameObject.FindGameObjectWithTag("God").GetComponent<God>().graphBuilt;
        }
        Debug.Log("TargetNode is: " + targetNode);
        if (graphBuilt)
        {
            navigate();
            //if (needPath)
            //{
            //    needPath = false;
            //}
            //if (!needPath)
            //{
            //    navigate();
            //}
        }
        if (IsGrounded())
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }

    }

    void buildGraph()
    {
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
                //Debug.DrawRay(node.transform.position, otherNode.transform.position - node.transform.position * 100, Color.red);
                if (Physics.Linecast(node.transform.position, otherNode.transform.position, out hit))
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

        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (!isRagdoll)
        {

            RunAI();
            Grab();
            if (target && isStopped)
            {
                Vector3 targetDir = target.position - transform.position;
                Vector3 dir = Vector3.RotateTowards(transform.forward, targetDir, 5 * Time.deltaTime, 0);
                transform.rotation = Quaternion.LookRotation(dir);
            }
            if (GameObject.Find("NavNode (" + targetNode + ")"))
            {
                if (currentNode == GameObject.Find("NavNode (" + targetNode + ")").transform && GameObject.Find("NavNode (" + targetNode + ")").GetComponent<NavNode>().wasTriggered)
                {
                    Stop();
                    //shouldWalk = false;
                    needPath = true;
                    count = 0;
                    usedNodeArray = new HashSet<Transform>();
                }
            }

        }

    }

    void navigate()
    {
       // Debug.Log("count is : " + count);
        if (nodeArray.Length == 0)
        {
            nodeArray = GameObject.FindGameObjectsWithTag("NavNode");
        }
        Debug.Log("needPath: " + needPath);
        float dist;
        List<float> distList = new List<float>();
        if (needPath)
        {
            Random rnd = new Random();
            randWait = Random.Range(1, 8);
            int randNode = Random.Range(1, nodeArray.Length - 1);
            targetNode = randNode;
            foreach (GameObject node in nodeArray)
            {
                dist = Vector3.Distance(node.transform.position, transform.position);
                distList.Add(dist);

            }
            foreach (GameObject node in nodeArray)
            {

                if (Vector3.Distance(node.transform.position, transform.position) == distList.Min())
                {
                    //TODO Need to make next path happen;
                    nodeNumber = node.GetComponent<NavNode>().id;
                }

            }

        }
       
        walkPath(nodeNumber, targetNode, randWait);

    }
    // Navigate
    void findPath(int startNumber, int targetNumber)
    {
        Debug.Log("Start Node is :" + startNumber);
        usedNodeArray = new HashSet<Transform>();
        path = new List<Transform>();
        Transform targetNode = GameObject.Find("NavNode (" + targetNumber + ")").transform;
        Transform startNode = GameObject.Find("NavNode (" + startNumber + ")").transform;
        NavNode nav;
        float dist2;
        if (!path.Contains(startNode))
        {
            path.Add(startNode);
        }
        count =0;
        nav = startNode.GetComponent<NavNode>();
        List<float> distList = new List<float>();
        traverse(nav, targetNode);
    }

    void walkPath(int startNum, int targetNum, float waitTime)
    {

        if (graphBuilt)
        {
            if (needPath)
            {

                needPath = false;
                findPath(startNum, targetNum);
            }
            if (!needPath)
            {

                if (path.Contains(GameObject.Find("NavNode (" + targetNum + ")").transform))
                {
                    Debug.Log("Acquired Path");
                    
                    if (count < path.Count)
                    {
                        nodeNumber = path[count].GetComponent<NavNode>().id;
                    }
                    if (path[count].GetComponent<NavNode>().id == targetNum)
                    {
                        // shouldWalk = false;
                        //GameObject.Find("NavNode (" + targetNum + ")").GetComponent<NavNode>().wasTriggered = true;
                        GameObject.Find("NavNode (" + targetNum + ")").GetComponent<NavNode>().waitTime = waitTime;
                        
                    }

                }
                
            }
            else
            {
                findPath(startNum, targetNum);

            }

        }

    }


    void traverse(NavNode nav, Transform targetNode)
    {
        List<float> distList = new List<float>();
        float dist2;
        if (nav.id == targetNode.GetComponent<NavNode>().id)
        {
            path.Add(nav.transform);
        }
        foreach (GameObject neighbor2 in nav.neighbors)
        {
            dist2 = Vector3.Distance(targetNode.transform.position, neighbor2.transform.position);
            distList.Add(dist2);
            usedNodeArray.Add(nav.transform);

        }
        foreach (GameObject neighbor in nav.neighbors)
        {

            if (Vector3.Distance(targetNode.transform.position, neighbor.transform.position) == distList.Min() && neighbor.GetComponent<NavNode>().neighbors.Count > 0)
            {
                if (!path.Contains(neighbor.transform))
                {

                    path.Add(neighbor.transform);
                    neighbor.GetComponent<Renderer>().material.color = new Color(255, 26, 0, 1f);
                }
                if (!usedNodeArray.Contains(neighbor.transform))
                {
                    if (!path.Contains(targetNode))
                    {
                        traverse(neighbor.GetComponent<NavNode>(), targetNode);
                    }
                }

            }
        }
    }
    void LookAtNode(Transform node)
    {
        RaycastHit ray;
        float distance = .5f;
        Vector3 nodepos;
        Vector3 aipos;
        nodepos = new Vector3(node.position.x, 0, node.position.z);
        aipos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = nodepos - aipos;
        Vector3 left = transform.position;
        Vector3 right = transform.position;
        left.x += 2;
        right.x -= 2;

        if (controller.isGrounded && !ignoreWalls)
        {
            if (Physics.Raycast(transform.position + transform.right * 2 + transform.up * 1, transform.forward, out ray, distance))
            {
                direction += ray.normal * 75;
            }

            if (Physics.Raycast(transform.position + transform.right * -2 + transform.up * 1, transform.forward, out ray, distance))
            {
                direction += ray.normal * 75;
            }

            if (Physics.Raycast(transform.position + transform.up * 1, transform.right * -1.3f, out ray, distance))
            {
                Debug.Log("Wall Left");
                wallLeft = true;
                direction += ray.normal * 75;
            }
            if (!Physics.Raycast(transform.position + transform.up * 1, transform.right * -1.3f, out ray, distance))
            {

                wallLeft = false;
            }
            if (Physics.Raycast(transform.position + transform.up * 1, transform.right * 1.3f, out ray, distance))
            {
                Debug.Log("Wall Right");
                wallRight = true;
                direction += ray.normal * 75;
            }
            if (!Physics.Raycast(transform.position + transform.up * 1, transform.right * -1.3f, out ray, distance))
            {

                wallRight = false;
            }

            if (Physics.Raycast(transform.position + transform.up * 1 + transform.up * 1, transform.forward, out ray, distance))
            {
                if (ray.collider.transform.root.tag == "Terrorist")
                {
                    Stop();
                    Invoke("StartNexWaypoint", node.GetComponent<NavNode>().waitTime);
                }


            }

        }

        this.controller.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 3 * Time.deltaTime);


    }

    bool WalkToWaypoint(Transform node, bool walk)
    {


        if (waitingforPlayer = true && nearPlayer)
        {

            Invoke("StartNexWaypoint", node.GetComponent<NavNode>().waitTime);


        }
        distanceToNode = Vector3.Distance(transform.position, node.transform.position);
        // distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
        // Debug.Log("Node Type : " + node.GetComponent<NavNode>().nodeType);
        //Debug.Log(distanceToPlayer);
        bool completed = false;

        
        Vector3 direction = (node.position - transform.position);
        if (walk)
        {
            speed = 3;
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }
        if (!walk)
        {
            //if (!isSpeeding)
            //{
            //    speed = 13;
            //}


            speed = .03f;

            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }
        movDir = transform.forward;




        if (distanceToNode >= 3)
        {
            isStopped = false;
            if (walk)
            {
                Walk();
            }
            else Run();
        }



        else
        {
            node.GetComponent<NavNode>().wasTriggered = true;
            completed = true;
            // Debug.Log("Completed Task");
            if (node.GetComponent<NavNode>().nodeType != "Ping")
            {
                count++;
            }
            if (node.GetComponent<NavNode>().nodeType == "Ping")
            {
                if (node.GetComponent<NavNode>().pingCount == 0)
                {
                    count++;
                }

                else
                {
                    count--;
                    node.GetComponent<NavNode>().pingCount--;
                }
            }
            if (node.GetComponent<NavNode>().nodeType == "Loop")
            {

                //Debug.Log("Switching Node");

                count = 100;
            }

            if (node.GetComponent<NavNode>().nodeType == "GoToStart")
            {
                oldNode = nodeNumber;
                nodeNumber = node.GetComponent<NavNode>().goToNum;


            }

            if (node.GetComponent<NavNode>().nodeType == "GoToEnd")
            {

                nodeNumber = oldNode + 1;


            }

            if (node.GetComponent<NavNode>().nodeType == "WaitForPlayer")
            {
                if (nearPlayer)
                {
                    // Debug.Log("Not Waiting!");
                    count++;
                    Invoke("StartNexWaypoint", node.GetComponent<NavNode>().waitTime);
                }

                else
                {
                    waitingforPlayer = true;
                    Stop();
                }

                if (node.GetComponent<NavNode>().nodeType == "Grab")
                {
                    int prevNum = nodeNumber - 1;
                    GameObject prevNode = GameObject.Find("NavNode (" + prevNum + ")");
                    node.GetComponent<NavNode>().jumpStrength = prevNode.GetComponent<NavNode>().jumpStrength;


                }




            }
            if (node.GetComponent<NavNode>().nodeType == "Straight")
            {

                ignoreWalls = true;

            }

            if (node.GetComponent<NavNode>().nodeType != "Straight")
            {

                ignoreWalls = false;

            }



            if (node.GetComponent<NavNode>().wasTriggered == true)
            {

                if (node.GetComponent<NavNode>().waitTime > 0)
                {
                    Stop();
                    isStopped = true;
                }

                if (node.GetComponent<NavNode>().nodeType != "WaitForPlayer")
                {

                    Invoke("StartNexWaypoint", node.GetComponent<NavNode>().waitTime);

                }
                if (node.GetComponent<NavNode>().nodeType == "WaitForPlayer" && nearPlayer)
                {
                    Invoke("StartNexWaypoint", node.GetComponent<NavNode>().waitTime);
                }


            }



        }



        movDir *= speed;

        if (controller.isGrounded == false)
        {
            //Debug.Log("gravity is.." + gravity);
            gravity -= 20 * Time.deltaTime;
            movDir.y = gravity;
            controller.Move(transform.forward * speed * Time.deltaTime);
        }

        else if (controller.isGrounded)
        { gravity = -5 * Time.deltaTime; }
        if (!isGrabbing)
        {
            controller.Move(movDir * Time.deltaTime);
        }
        // Debug.Log("Jumping! : " + isJumping);

        if (controller.isGrounded && node.transform.position.y > transform.position.y + 10.5)
        {
            Debug.Log("Too high");
            nodeNumber = node.GetComponent<NavNode>().goToNum;
        }
        //Debug.Log("node: (" + nodeNumber + ")" + node.transform.position.y + "AI: " + transform.position.y);
        return completed;

    }

    public void Jump()
    {
        if (IsGrounded() || isGrabbing)
        {


            int prevNum = nodeNumber - 1;
            GameObject prevNode = GameObject.Find("NavNode (" + prevNum + ")");
            // node.GetComponent<NavNode>().jumpStrength = prevNode.GetComponent<NavNode>().jumpStrength;
            gravity = prevNode.GetComponent<NavNode>().jumpStrength;
            needGrab = false;

        }



    }

    public void Jump(float Strength)
    {
        if (IsGrounded() || isGrabbing)
        {


            int prevNum = nodeNumber - 1;
            GameObject prevNode = GameObject.Find("NavNode (" + prevNum + ")");
            // node.GetComponent<NavNode>().jumpStrength = prevNode.GetComponent<NavNode>().jumpStrength;
            gravity = Strength;
            needGrab = false;

        }
    }
    public void Grab()
    {
        RaycastHit ray;
        RaycastHit ray2;
        int wallType = 0;
        //Debug.DrawRay(transform.position + transform.up * 4, transform.forward * 100, Color.red);
        //Debug.DrawRay(transform.position + transform.up * 3.8f, transform.forward * 100, Color.green);


        if (needGrab && Physics.Raycast(transform.position + transform.up * 3.7f, transform.forward, out ray2, 2f) && Physics.Raycast(transform.position + transform.up * 4.0f, transform.forward, out ray, 2f))
        {

            if (ray.collider.name != ray2.collider.name)
            {
                wallType = 1;
                Debug.Log(ray.collider.name + "  Grabbed  " + ray2.collider.name);
            }

            else
            {
                wallType = 0;
                Debug.Log(ray.collider.name + "    " + ray2.collider.name);
            }
        }

        if (wallType == 1)
        {
            //GetComponent<Rigidbody>().useGravity = false;

            // anim.SetBool("isGrabbing", true);
            anim.Play("Grab");
            //anim.SetBool("isJumping", false);
            //anim.SetBool("isGrounded", false);


            //anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
            isGrabbing = true;
            //jumpCount = 3;
            speed = 2;
            movDir = transform.forward * 2;
            //didGrab = true;
            //jumpSpeed = 13;
            Debug.Log("Grabbing!");
            isClimbing = true;
            Invoke("ClimbUp", 2f);

        }

        else
        {
            //anim.SetBool("isGrabbing", false);
            isGrabbing = false;

            anim.SetBool("isGrabbing", false);
            //jumpSpeed = 12;
            // GetComponent<Rigidbody>().useGravity = true;
        }


        //nodeNumber++;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "grabblock")
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isGrounded", false);
            //anim.SetBool("isGrabbing", true);

            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
            needGrab = true;
            Invoke("StopGrab", 2.6F);
        }
    }
    void ClimbUp()
    {
        if (isClimbing && transform.position.y <= currentNode.transform.position.y)
        {
            gravity = 10;
            controller.slopeLimit = 90;
            controller.stepOffset = .5f;


        }
        else
        {

            gravity = -5;
            controller.slopeLimit = 45f;
            controller.stepOffset = .3f;
            isClimbing = false;


        }
        needGrab = false;
        Debug.Log("Climbing!");
    }
    void StartNexWaypoint()
    {

        isWalking = true;
    }
    void Walk()
    {
        //Vector3 moveDirection = new Vector3(1, 0, 0);
        anim.SetBool("isWalking", true);
        isWalking = true;

        //transform.Translate( transform.forward * -.04f);
    }
    public bool IsGrounded()
    {
        /*
            RaycastHit hit;
            // Debug.DrawRay(transform.position + transform.up * 2, -transform.up * 100, Color.red);
            if (Physics.Raycast(transform.position, -transform.up, out hit, 100.0F))
                distanceToGround = hit.distance;
            if (distanceToGround < 1.2f)
                return true;
            else
            {
                return false;
            }
            */

        float radius = controller.radius * 0.9f;
        Vector3 pos = transform.position + transform.up * -1 * (radius * 0.9f);
        bool isGrounded = Physics.CheckSphere(pos, radius, LayerMask.GetMask("Default"));
        return isGrounded;

    }
    void Run()
    {
        //Vector3 moveDirection = new Vector3(1, 0, 0);
        anim.SetBool("isRunning", true);
        anim.SetBool("isWalking", false);
        //Debug.Log("Running!!");
        isWalking = true;

        //transform.Translate( transform.forward * -.04f);
    }
    void Stop()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isGrounded", true);
        isWalking = false;
        gravity = -20;


    }
    void stopRoll()
    {
        anim.SetBool("isRolling", false);
    }
    void StopGrab()
    {
        needGrab = false;
    }
    void needRoll()
    {
        float stopRollTime = Time.deltaTime;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F))
            distanceToGround = hit.distance;
        if (distanceToGround < 2)
        {
            if (controller.velocity.y <= -6.1)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isRunning", false);
                anim.SetBool("isRolling", true);

                Debug.Log("Rolling!");
                Invoke("stopRoll", 1);
            }


        }
        // Debug.Log("Ground is....:  " + distanceToGround);
        // Debug.Log("Velocity is....:  " + controller.velocity.y);
    }
    public void TakeFallDamage(int damage)
    {

        health -= damage;

        // flashEnabled = true;
        // Invoke("DisableFlash", .1f);

        if (health <= 0)
        {
            if (!isRagdoll)
            {
                GoRag();
            }
            /* Instantiate(ragdoll, transform.position, transform.rotation);

            ConfigureRagdollPosition(transform.GetChild(0), ragdoll.transform.GetChild(0));
            Destroy(this.gameObject);
            */

        }
    }
    public void ConfigureRagdollPosition(Transform reference, Transform ragdollPart)
    {
        // Debug.Log("configuring");
        ragdollPart.localPosition = reference.localPosition;
        ragdollPart.localRotation = reference.localRotation;

        for (int d = 0; d < (ragdollPart.childCount); d++)
        {
            Transform ref_t = reference.GetChild(d);
            Transform rag_t = ragdollPart.GetChild(d);

            ConfigureRagdollPosition(ref_t, rag_t);
            //  Debug.Log(ref_t+"<=>"+rag_t);
            //  Debug.Log(reference.GetChild(d));
        }

    }
    void GoRag()
    {
        isRagdoll = true;
        //Debug.Log(transform.GetChild(5).name);
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        /*  foreach (Rigidbody rb in bodies)
          {
              rb.useGravity = true;

          }
         */
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunningg", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isGrabbing", false);
        anim.SetBool("isRolling", false);
        GetComponent<Animator>().enabled = false;
        ConfigureRagdollPosition(transform.GetChild(1), transform.GetChild(1));
        foreach (Rigidbody rb in bodies)
        {
            rb.useGravity = false;
            rb.useGravity = true;
            rb.isKinematic = false;
            //rb.velocity = player.transform.forward * 10 + transform.up * -10;

            //rb.AddExplosionForce(20, transform.position, -200, 1.0F);
        }



        Collider[] col2 = GetComponentsInChildren<Collider>();
        foreach (Collider c2 in col2)
        {
            c2.enabled = true;
        }
        controller.SimpleMove(Vector3.zero);
        controller.enabled = false;
        Stop();
        controller.SimpleMove(Vector3.zero);
        Invoke("GoStiff", 10);

        //controller.enabled = false;
        //isragdoll = true;
        //canGetupTime = Time.time;
        // Debug.Log("going Rag");


    }
    void GravityFix()
    {
        //isStaticRag = true;

    }

    //void GravityFix2()
    //{
    //    Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
    //    foreach (Rigidbody rb in bodies)
    //    {
    //        rb.useGravity = true;
    //    }
    //}
    void GoStiff()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunningg", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isGrabbing", false);
        anim.SetBool("isRolling", false);
        GetComponent<Animator>().enabled = true;
        isStaticRag = false;
        isRagdoll = false;
        Collider[] col = GetComponentsInChildren<Collider>();
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;

        }
        foreach (Collider c in col)
        {
            c.enabled = false;
        }
        health = 100;


        controller.transform.position = transform.GetChild(5).GetChild(0).transform.position + transform.up * 2f;
        controller.SimpleMove(Vector3.zero);
        controller.enabled = true;
        controller.SimpleMove(Vector3.zero);

        Debug.Log("Velocity is.." + controller.velocity);

        nodeNumber = currentNode.GetComponent<NavNode>().goToNum;
        Invoke("StartNexWaypoint", .01f);
    }

    void checkPlayerDistance()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            distanceToPlayer = Vector3.Distance(transform.position, GameObject.FindGameObjectWithTag("Player").transform.position);
            if (distanceToPlayer <= 3 && waitingforPlayer)
            {
                nearPlayer = true;
                isWalking = true;
            }
            if (distanceToPlayer >= 3)
            {
                nearPlayer = false;
            }
        }
    }

    public void RunAI()
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();

        if (isStaticRag)
        {
            foreach (Rigidbody rb in bodies)
            {
                rb.velocity = Vector3.zero;


            }
        }
        if (isWalking)
        {

            if (GameObject.Find("NavNode (" + nodeNumber + ")")) ;//&& !GameObject.Find("NavNode (" + nodeNumber + ")").GetComponent<NavNode>().wasTriggered)
            {
                currentNode = GameObject.Find("NavNode (" + nodeNumber + ")").GetComponent<Transform>();
                bool isSlow = GameObject.Find("NavNode (" + nodeNumber + ")").GetComponent<NavNode>().slow;
                if (isWalking && !isGrabbing)
                {

                    if (needPath == false)
                    {
                        isSlow = false;
                        WalkToWaypoint(currentNode, isSlow);



                        LookAtNode(currentNode);

                        //Debug.Log("Node is :" + targetNode);
                    }


                }
            }
        }
        needRoll();
        //Grab();
        checkPlayerDistance();

        if (controller.velocity.y <= -30.1 && distanceToGround <= 1f)
        {
            TakeFallDamage(100);

        }
        needRoll();

        if (!IsGrounded() && !isGrabbing)
        {
            anim.SetBool("isJumping", true);
        }
        else
        {
            anim.SetBool("isJumping", false);
        }
    }

}
