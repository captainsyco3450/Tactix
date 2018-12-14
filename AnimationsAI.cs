using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsAI : MonoBehaviour {

    Animator anim;
    bool isWalking = false;
    bool playStarted = false;
    public Transform navNode;
    Transform currentNode;
    float distanceToNode;
    float distanceToPlayer;
    float distanceToHit;
    public GameObject[] nodeArray;
   public int nodeNumber = 1;
    int tempNodeCount = 0;
    CharacterController controller;
    bool wallRight = false;
    bool wallLeft = false;
    public GameObject tempNode;
    int speed;
    Vector3 movDir = Vector3.zero;
    public Transform ragdoll;
    public bool canSeeWaypoint = false;
    bool isJumping = false;
    int jumpCount = 3;
    float distanceToGround;
    int oldNode=0;
    bool nearPlayer =false;
    bool waitingforPlayer=false;
    float gravity = -1;
    bool isGrabbing = false;
   public int health = 100;
    bool didGrab;
    void Start() {

        anim = GetComponent<Animator>();
        controller = transform.GetComponent<CharacterController>();
        anim.SetBool("isGrounded", true);
        anim.SetBool("isJumping", false);
        Stop();
       //Invoke("Die",3;);
        //Invoke("WalkRoute", 3);
        //navNode = GameObject.FindGameObjectWithTag("NavNode").GetComponent<Transform>();
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "grabblock")
        {
            Grab();
        }
    }
    // Update is called once per frame

    private void Update()
    {
        /*  if (controller.isGrounded == false)
          {
              movDir.y -= 10* Time.deltaTime;
          }
          */
      
    }
    void LateUpdate() {


       // Grab();
        if (controller.velocity.y <= -30.1 && distanceToGround <= 2f)
        {
            TakeFallDamage(100);

        }
        needRoll();
       // Debug.Log("Waiting forplayer ?  " + waitingforPlayer + " Near Player?: " + nearPlayer);
        checkPlayerDistance();
        //Debug.Log("Grounded: " + controller.isGrounded);
        if (controller.isGrounded && playStarted)
        {
            anim.SetBool("isGrounded", true);
            anim.SetBool("isJumping", false);
            
        }

        if (!IsGrounded() && playStarted)
        {
            anim.SetBool("isGrounded", false);
            if (!isGrabbing)
            {
                anim.SetBool("isJumping", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && !isWalking)
        {
            playStarted = true;
            isWalking = true;
             

        }

        else if (Input.GetKeyDown(KeyCode.E) && isWalking)
        {

            isWalking = false;
            Stop();
        }
        if (health < 0)
        {
            movDir = Vector3.zero;
            isWalking = false;
            playStarted = false;
        }
        if (isWalking)
        {
            if (GameObject.Find("NavNode (" + nodeNumber + ")"))
            {
                currentNode = GameObject.Find("NavNode (" + nodeNumber + ")").GetComponent<Transform>();
                bool isSlow = GameObject.Find("NavNode (" + nodeNumber + ")").GetComponent<NavNode>().slow;

                if ((!SeeDoor() || !SeeWall()) && isWalking &&!isGrabbing)
                {

                    WalkToWaypoint(currentNode, isSlow);
                  
                        LookAtNode(currentNode);
                    
            }

                //if(SeeDoor())
                //{ Stop();
                //    Debug.Log("Near Door");
                //}

               
              

            }
        }
        //Debug.Log("Velocity:  " + controller.velocity.y);
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

        if(nodeNumber ==oldNode){
            nodeNumber++;
        }
        Vector3 direction = (node.position - transform.position);
        if (walk) { speed = 6;
            anim.SetBool("isWalking", true);
            anim.SetBool("isRunning", false);
        }
        if (!walk) { speed = 13;
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunning", true);
        }
        movDir = transform.forward;
       


       
        if (distanceToNode >= 3) {
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
                nodeNumber++;
            }
            if (node.GetComponent<NavNode>().nodeType == "Ping")
            {
                if (node.GetComponent<NavNode>().pingCount == 0)
                {
                    nodeNumber++;
                }

                else
                {
                    nodeNumber--;
                    node.GetComponent<NavNode>().pingCount--;
                }
            }
            if (node.GetComponent<NavNode>().nodeType == "Loop")
            {

                //Debug.Log("Switching Node");

                nodeNumber = 1;
            }

            if (node.GetComponent<NavNode>().nodeType == "GoToStart")
            {
                 oldNode = nodeNumber;
                 nodeNumber =node.GetComponent<NavNode>().goToNum;

                
            }

            if (node.GetComponent<NavNode>().nodeType == "GoToEnd")
            {
                
                nodeNumber = oldNode + 1;


            }

           if (node.GetComponent<NavNode>().nodeType == "WaitForPlayer")
            {
                if(nearPlayer)
                {
                   // Debug.Log("Not Waiting!");
                    nodeNumber++;
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

          


            if (node.GetComponent<NavNode>().wasTriggered == true)
            {
               
                if (node.GetComponent<NavNode>().waitTime > 0)
                {
                    Stop();
                }

                if (node.GetComponent<NavNode>().nodeType != "WaitForPlayer")
                {

                    Invoke("StartNexWaypoint", node.GetComponent<NavNode>().waitTime);

                }
                if (node.GetComponent<NavNode>().nodeType == "WaitForPlayer"&& nearPlayer)
                {
                    Invoke("StartNexWaypoint", node.GetComponent<NavNode>().waitTime);
                }

                
            }

           

        }
        //if (isJumping)
        //{
            
        //    int prevNum = nodeNumber - 1;
        //    GameObject prevNode = GameObject.Find("NavNode (" + prevNum + ")");
        //    //Debug.Log(distanceToNode);
        //    if (distanceToNode > 1)
        //    {


        //        if (didGrab)
        //        {
        //            Debug.Log("Here!");
        //           gravity = 30;
        //        }

        //        else
        //        {
        //            gravity = prevNode.GetComponent<NavNode>().jumpStrength * Time.deltaTime;
        //        }
        //        speed = 13;
        //        //movDir.y -= .8f * Time.deltaTime;
        //    }
        //    else
        //    {
        //        isJumping = false;
        //        //gravity -= 25 * Time.deltaTime;
        //    }
            
        //}

       
        movDir *= speed;

        if (controller.isGrounded == false)
        {
            //Debug.Log("gravity is.." + gravity);
            gravity -= 50 * Time.deltaTime;
            movDir.y = gravity;
        }

        else if(controller.isGrounded)
           { gravity = -5; }
        if (!isGrabbing)
        {
            controller.Move(movDir * Time.deltaTime);
        }
        Debug.Log("Jumping! : " +isJumping);

        if (controller.isGrounded && node.transform.position.y > transform.position.y + 7.5)
        {
            Debug.Log("Too high");
           nodeNumber = node.GetComponent<NavNode>().goToNum;
        }
       //Debug.Log("node: (" + nodeNumber + ")" + node.transform.position.y + "AI: " + transform.position.y);
        return completed;

    }

   public bool Jump()
    {
       // if (controller.isGrounded)
        //{
            isJumping = true;
        isGrabbing = false;
        anim.SetBool("isGrabbing", false);
            Invoke("stopJump", .1f);
       // Debug.Log("Still Jumping!");
        int prevNum = nodeNumber - 1;
          GameObject prevNode = GameObject.Find("NavNode (" + prevNum + ")");
        gravity = prevNode.GetComponent<NavNode>().jumpStrength * Time.deltaTime;
        //}
        return isJumping;
            
    }

   public bool stopJump()
    {
        didGrab = false;
        isJumping = false;
        
        return isJumping;
    }

    void Walk()
    {
        //Vector3 moveDirection = new Vector3(1, 0, 0);
        anim.SetBool("isWalking", true);
        isWalking = true;

        //transform.Translate( transform.forward * -.04f);
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
        
        
    }


   void StartNexWaypoint()
    {
        
        isWalking = true;
    }

    void LookAtNode(Transform node)
    {
        RaycastHit ray;
        RaycastHit ray2;
        RaycastHit ray3;
        RaycastHit ray4;
        RaycastHit ray5;
        int distance = 3;
        bool isRight =false;
        bool isLeft = false;
        bool isAhead = false;
        bool rightSide = false;
        bool leftSide = false;
        Vector3 nodepos;
        Vector3 aipos;
        nodepos = new Vector3(node.position.x, 0, node.position.z);
        aipos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 direction = nodepos - aipos;
        Vector3 left = transform.position;
        Vector3 right = transform.position;
        left.x += 2;
        right.x -= 2;
        

       // Debug.DrawRay(transform.position + transform.up * 2, transform.forward * 8, Color.green);
       // Debug.DrawRay(transform.position + transform.up * 2, transform.right * 4, Color.green);
       // Debug.DrawRay(transform.position + transform.up * 2, transform.right * -4, Color.green);
       // Debug.DrawRay(transform.position + transform.right * 2 + transform.up * 1 + transform.forward * -2, transform.forward * 9, Color.red);
       // Debug.DrawRay(transform.position + transform.right * -2 + transform.up * 1 +transform.forward * -2, transform.forward * 9, Color.blue);
        /*
                if (Physics.Raycast(transform.position +  transform.up * 2, transform.forward, out ray, 9f)) {
                    //direction += ray.normal * 25;
                    isAhead = true;
                }

                    if (Physics.Raycast(transform.position + transform.up * 1, transform.right * 1.3f, out ray4, 4f))
                {
                    rightSide = true;
                }

                else
                {
                    rightSide = false;
                }

                if (Physics.Raycast(transform.position + transform.up * 1, transform.right * -1.3f, out ray5, 4f))
                {
                    leftSide = true;
                }
                else
                {
                    leftSide = false;
                }
                    if (Physics.Raycast(transform.position + transform.right * -2 + transform.up * 1 + transform.forward * -2, transform.forward, out ray2, 9f))
                {
                    isLeft = true;
                    //direction += ray.normal * 25;
                }
                else
                {
                    isLeft = false;
                }
                if (Physics.Raycast(transform.position + transform.right * 2 + transform.up * 1 + transform.forward * -2, transform.forward, out ray3, 9f))
                {
                    isRight = true;
                    //direction += ray.normal * 25;
                }
                else
                {
                    isRight = false;
                }

                if(isRight && !isLeft)
                {
                    //direction += ray3.normal * 25;
                    direction.x += 2000;

                }

               else if (!isRight && isLeft)
                {
                    direction.x -= 2000;
                }


               else if (isRight && isLeft)
                 {
                     if (rightSide && !leftSide)
                     {
                         direction += ray5.normal * 250;
                         //Debug.Log("Going Left");
                     }

                     if (rightSide && leftSide)
                     {
                         direction.x += 2000;
                         //Debug.Log("Going Round");
                     }
                     if (!rightSide && leftSide)
                     {
                         direction += ray4.normal * 25;
                         Debug.Log("Going Right");
                     }

                     if(!(rightSide || leftSide))
                     {
                         direction.x += 2000; 
                     }

                 }
                else
                {
                    direction = (node.position - transform.position);
                }

                */
        // Debug.Log("isRight: " + isRight + "  isLeft: " + isLeft + "  rightSide: " + rightSide + "  lefttSide: " + leftSide);
        /*
                 Debug.Log("isRight: " + isRight + "isLeft: " + isLeft + "rightSide: " + rightSide + "lefttSide: " + leftSide);
*/

        //if (Physics.Raycast(transform.position + transform.up * 1, transform.forward, out ray, distance) || Physics.Raycast(transform.position + transform.up * 1, transform.right, out ray, distance)
        //    || Physics.Raycast(transform.position + transform.right * 1.3f + transform.up * 2, transform.forward, out ray, distance) || Physics.Raycast(transform.position + transform.right * -1.3f + transform.up * 2, transform.forward, out ray, distance))
        //{
        //    //Debug.Log("Turning: ");
        //    direction += ray.normal * 75;
        //}

        //if (Physics.Raycast(transform.position + transform.up * 1, transform.forward, out ray, distance) || Physics.Raycast(transform.position + transform.up * 1, transform.right * -1.3f, out ray, distance))
        //{
        //   // Debug.Log("Turning: ");
        //    direction += ray.normal * 75;
        //}
        if (controller.isGrounded)
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
                direction += ray.normal * 75;
            }
            if (Physics.Raycast(transform.position + transform.up * 1, transform.right * 1.3f, out ray, distance))
            {
                direction += ray.normal * 75;
            }

        }

        this.controller.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 7 * Time.deltaTime);
       
        
    }
    bool SeeWall()
    {
        bool nearWall;
       

            RaycastHit ray;
            if (Physics.Raycast(transform.position + transform.up * 1, transform.forward, out ray, 2f))
            {
                distanceToHit = Vector3.Distance(this.transform.position, ray.transform.position);
              // Debug.Log("WallDistance: " + distanceToHit);
            if(distanceToHit >= 2 && ray.collider.name !="Door")
            {
                nearWall = true;
               
            }

            else
            {
                nearWall = false;
            }
            }

           
        
        else
        {
            nearWall = false;
        }
        return nearWall;
    }

    bool SeeDoor()
    {
        bool nearDoor;
        RaycastHit ray;
        //Debug.DrawRay(transform.position + transform.right * 2 + transform.up*1, transform.forward*4, Color.red);
        //Debug.DrawRay(transform.position + transform.right * -2 + transform.up * 1, transform.forward * 4, Color.green);
        //Debug.DrawRay(transform.position + transform.up * 1, transform.right * -1.3f, Color.blue);
        //Debug.DrawRay(transform.position + transform.up * 1, transform.forward * 9, Color.red);
        if (Physics.Raycast(transform.position + transform.up * 3, transform.forward, out ray, 2f) || Physics.Raycast(transform.position + transform.up * .3f, transform.forward, out ray, 2f))
        {
            Vector3 rot = transform.rotation.eulerAngles;
            Vector3 rot2;
            //rot2 = new Vector3(rot.x, rot.y + 90, rot.z);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(rot2), RotationSpeed2);
            //Debug.Log("I hit a door!");
            if (ray.collider.name == "Door") { 
            nearDoor = true;
            // moveDirection = transform.forward * speed;
        }
            else { nearDoor = false; }
    }
        else
        {
            nearDoor = false;
        }
        return nearDoor;
    }

    void pathFind()
    {
        
    }
    public Transform getCurrentNode()
    {
        return currentNode;
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
    void needRoll()
    {
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
    public void Die()
    {
        
        Vector3 rot = transform.rotation.eulerAngles;
        Vector3 rot2 = new Vector3(rot.x - 90, rot.y + 30, rot.z + 10);
       
        Transform ob = (Transform)Instantiate(ragdoll, transform.position, transform.rotation);
        //Debug.Log(transform.GetChild(0));
        Destroy(gameObject);
        ConfigureRagdollPosition(transform.GetChild(0), ob.GetChild(0));

        Destroy(this.gameObject);
        

    }

    public bool IsGrounded()
    {

        RaycastHit hit;
       // Debug.DrawRay(transform.position + transform.up * 2, -transform.up * 100, Color.red);
        if (Physics.Raycast(transform.position, -transform.up, out hit, 10.0F))
            distanceToGround = hit.distance;
        if (distanceToGround < 1f)
            return true;
        else
        {
            return false;
        }

    }
    void stopRoll()
    {
        anim.SetBool("isRolling", false);
    }
    void checkPlayerDistance()
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

    public void TakeFallDamage(int damage)
    {

        health -= damage;

        // flashEnabled = true;
        // Invoke("DisableFlash", .1f);

        if (health <= 0)
        {
            GoRag();
            /* Instantiate(ragdoll, transform.position, transform.rotation);

            ConfigureRagdollPosition(transform.GetChild(0), ragdoll.transform.GetChild(0));
            Destroy(this.gameObject);
            */

        }
    }
    void GoRag()
    {

        //Debug.Log(transform.GetChild(5).name);
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.useGravity = true;

        }
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunningg", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isGrabbing", false);
        anim.SetBool("isRolling", true);
        GetComponent<Animator>().enabled = false;
        ConfigureRagdollPosition(transform.GetChild(5), transform.GetChild(5));
        foreach (Rigidbody rb in bodies)
        {

            rb.isKinematic = false;
           // rb.velocity = transform.forward * 5;
            //rb.AddExplosionForce(20, transform.position, -200, 1.0F);
        }



        Collider[] col2 = GetComponentsInChildren<Collider>();
        foreach (Collider c2 in col2)
        {
            c2.enabled = true;
        }

        //controller.enabled = false;
        //isragdoll = true;
        //canGetupTime = Time.time;
        // Debug.Log("going Rag");
        controller.enabled = false;

    }
    public void Grab()
    {
        RaycastHit ray;
        RaycastHit ray2;
        int wallType = 0;
        Debug.DrawRay(transform.position + transform.up * 4, transform.forward * 100, Color.red);
        Debug.DrawRay(transform.position + transform.up * 4.2f, transform.forward * 100, Color.green);
        if (Input.GetMouseButton(1) && Physics.Raycast(transform.position + transform.up * 4, transform.forward, out ray2, 2f) && !Physics.Raycast(transform.position + transform.up * 4.2f, transform.forward, out ray, 2f))
        {
            wallType = 1;
        }

        if (Input.GetMouseButton(1) && Physics.Raycast(transform.position + transform.up * 4, transform.forward, out ray2, 2f) && Physics.Raycast(transform.position + transform.up * 4.2f, transform.forward, out ray, 2f))
        {
            if (ray.collider.name != ray2.collider.name)
            {
                wallType = 1;
            }

            else
            {
                wallType = 0;
            }
        }

        if (SeeWall())
        {
            //GetComponent<Rigidbody>().useGravity = false;
            
            anim.SetBool("isJumping", true);
            anim.SetBool("isGrabbing", true);
            
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
            isGrabbing = true;
            //jumpCount = 3;
            speed = 2;
            movDir = transform.forward * 2;
            didGrab = true;
            //jumpSpeed = 13;
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);

        }

        else
        {
            //anim.SetBool("isGrabbing", false);
            isGrabbing = false;
            //jumpSpeed = 12;
            // GetComponent<Rigidbody>().useGravity = true;
        }
        Debug.Log(" Still grabiing!");
        Invoke("Jump", 3);
        //nodeNumber++;
    }

}

