using UnityEngine;
using System.Collections;
using System.IO;
public class Player : MonoBehaviour
{
    float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public float RotationSpeed = 80;
    public Vector3 CurPos;
    public Quaternion CurRot;
    private Vector3 moveDirection = Vector3.zero;
    //public float speed = 10f;
    public float distanceToGround;
    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;
    public GameObject ragdoll;
    public Transform explosion;
    public float distanceToExplosion;
    private float distance = 2;
    public GameObject c4;
    public float c4Count = 1;
    public int health = 100;
    public Texture redMaterial;
    private bool flashEnabled = false;
    
    public Rect hud;
    
    Animator anim;
    int jumpCount = 3;
   public bool rolling = false;
    Camera[] cameras;
    CharacterController controller;
    public bool isGrabbing = false;
    public bool isragdoll = false;
    float canGetupTime;
    bool canGetUp;
    public bool isSliding;
    bool fp = false;
    public Vector3 PlayerLocation;
    public Quaternion PlayerRotation;
    bool gameSaved;
    string level;
    string loc;
    public GUIStyle customUI;
    public GUIStyle customUI2;
    public GUIStyle customUI3;
    public GUIStyle customUI4;
    float timer = 0;
    bool timing = true;
    GameObject [] controllerArray;
    void Start()
    {
        Cursor.visible = true;
        
       // DontDestroyOnLoad(transform.gameObject);
        anim = GetComponent<Animator>();
        cameras = gameObject.GetComponentsInChildren<Camera>();
        //cameras[1].gameObject.SetActive(true);
        cameras[0].gameObject.SetActive(false);
        //cameras[2].gameObject.SetActive(false);
        controller = GetComponent<CharacterController>();
        Collider[] col = GetComponentsInChildren<Collider>();
        foreach (Collider c in col)
        {
          c.enabled  = false;
        }
        controller.enabled = true;

        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.useGravity = false;

        }
        controllerArray = GameObject.FindGameObjectsWithTag("God");
    }
    void OnGUI()
    {
        hud = new Rect(Screen.width - 200, Screen.height - 100, 180, 90);
        GUI.Box(hud, "[" + Mathf.Round(timer)  + ":00/" +"60:00"+ "]", customUI);

        if (flashEnabled)
        {

            GUI.DrawTexture(new Rect(1, 1, Screen.width, Screen.height), redMaterial);


        }
    }
        private void OnTriggerEnter(Collider col)
    {


        if (col.transform.tag == "Vault3" && Input.GetMouseButton(0))
        {
            anim.SetBool("isVaulting", true);
            Debug.Log("Vaulting!");
        }

        if (col.transform.tag == "Finish")
        {
            timing = false;
        }
        if (col.transform.tag == "Mat" && controller.velocity.y <= -20)
        {
            GoRag(0);
        }
    }


    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Vault3")
        {
            anim.SetBool("isVaulting", false);
            Debug.Log(" Stop Vaulting!");
        }
        
    }
    void Update()
    {
        if (timing)
        {
            timer += 1 * Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            SaveGame();
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            Loadgame();
        }
        changeCamera();
       /* if (GameObject.FindGameObjectsWithTag("C4").Length != 0)
        {
            explosion = GameObject.FindGameObjectWithTag("C4").GetComponent<Transform>();
            distanceToExplosion = Vector3.Distance(transform.position, explosion.transform.position);

            if (distanceToExplosion <= 10)
            {
                TakeDamage();
            }
        }
        */
        if (Input.GetKey(KeyCode.Escape)) { Application.Quit(); }
        
        InputMovement();
        Grab();
        if (Input.GetMouseButton(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            anim.speed *= -1;
        }

        if (!(Input.GetMouseButton(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Roll")))
        {
            anim.speed = 1;
        }
        //Spawner();



        //Making the character move
        if (!isGrabbing)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);

        }
       // Debug.Log(controller.velocity.y);
        if (!isragdoll &&controller.velocity.y <= -19.1 && !isragdoll && controller.velocity.y >= -25.1 && distanceToGround <= 1 && rolling == false)
        {
            TakeFallDamage(100);
            
        }
       else if (!isragdoll && controller.velocity.y <= -30.1 && distanceToGround<=1)
        {
            TakeFallDamage(100);

        }

    }








    /*  void Spawner()
      {
          if (Input.GetMouseButtonDown(1) && c4Count == 1)
          {
              Instantiate(c4, transform.position + (transform.forward * distance), transform.rotation);
              c4Count -= 1;

          }
          if (Input.GetMouseButtonDown(1) && c4Count == 0)
          {
              // c4Count = 1;
          }

      }

      */
    
    public bool IsGrounded() 
    {
       
        RaycastHit hit;
        //Debug.DrawRay(transform.position + transform.up * 2, -transform.up *100,Color.red);
        if (Physics.Raycast(transform.position, -transform.up, out hit, 30.0F))
            distanceToGround = hit.distance;
        
        float radius = controller.radius * 0.9f;
        Vector3 pos = transform.position + transform.up * -1 * (radius * 0.9f);
        bool isGrounded = Physics.CheckSphere(pos, radius, LayerMask.GetMask("Default"));
        return isGrounded;
    }

    private void InputMovement()
    {
       
        
        if(IsGrounded()== false)
        {
            anim.SetBool("isJumping", true);
            

        }
        if (Input.GetKey(KeyCode.Space) && jumpCount > 0 && SeeWall())
        {
            anim.SetBool("isJumping", false);
            moveDirection.y = jumpSpeed;
            //speed=2;
            jumpCount--;
            if (BackToWall())
            {

                moveDirection = transform.forward * 10;
                moveDirection.y = jumpSpeed;
            }
        }
        // is the controller on the ground?
        if (controller.isGrounded && fp && transform.GetComponent<PlayerAI>().selected)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isGrounded", true);
            jumpCount = 4;
            if ((Input.GetKey(KeyCode.Q)))  //&& !((Input.GetKey(KeyCode.W))) && !((Input.GetKey(KeyCode.S))) && !((Input.GetKey(KeyCode.A))) && !((Input.GetKey(KeyCode.D))))
            {
                // GetComponent<Animation>().Stop("Idle");

            }
            //Feed moveDirection with input.
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            if (controller.isGrounded && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !rolling)
            {
                anim.SetBool("isWalking", true);
                anim.SetBool("isWalkingBack", false);
            }

            if (controller.isGrounded && (Input.GetKey(KeyCode.S)) && !rolling)
            {
                anim.SetBool("isWalkingBack", true);
            }

            if (controller.isGrounded && (!(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Q))) && !rolling)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isWalkingBack", false);

            }

            //Multiply it by speed.
            if (Input.GetKey("left shift") && (Input.GetKey(KeyCode.W))&&!isGrabbing)
            {
                if (rolling)
                {
                    speed = 5;
                }
                if (!rolling)
                {
                    anim.SetBool("isRunning", true);
                    speed = 7;
                }

            }
            else
            {
                anim.SetBool("isRunning", false);
                speed = 6;
            }
            
            moveDirection *= speed;
                
            //Jumping
            if (Input.GetKey(KeyCode.Space) && jumpCount >0)
            {

                moveDirection.y = jumpSpeed;
               
                jumpCount--;
               
               
            }

            

            else
            {
                anim.SetBool("isRolling", false);
                rolling = false;
            }
            // transform.Rotate(0, (Input.GetAxis("Mouse X") * RotationSpeed * Time.deltaTime), 0, Space.World);
        }
        //Applying gravity to the controller
        if (IsGrounded())
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                anim.SetBool("isRolling", true);
                rolling = true;

            }
        }
    }

    //TODO
    void Grab()
    {
        RaycastHit ray;
        RaycastHit ray2;
        int wallType = 0;
        //Debug.DrawRay(transform.position + transform.up * 3, transform.forward * 100, Color.red);
        //Debug.DrawRay(transform.position + transform.up * 3.2f, transform.forward * 100, Color.green);
        if (Input.GetMouseButton(1) && Physics.Raycast(transform.position + transform.up * 4.1F, transform.forward, out ray2, 2f) && !Physics.Raycast(transform.position + transform.up * 4.3f, transform.forward, out ray, 1f))
        {
            wallType = 1;
         }
        
        if (Input.GetMouseButton(1) && Physics.Raycast(transform.position + transform.up * 4.1f, transform.forward, out ray2, 2f) && Physics.Raycast(transform.position + transform.up * 4.3f, transform.forward, out ray, 2f))
        {
           
            if (ray.collider.name != ray2.collider.name)
            {
                wallType = 1;
            }

            else
            {
                wallType = 0;
            }
             Debug.Log(ray.collider.name + "    " + ray2.collider.name);
        }

        if(wallType == 1) { 
            //GetComponent<Rigidbody>().useGravity = false;
            //Debug.Log("grabiing!");
            anim.SetBool("isGrabbing", true);
            isGrabbing = true;
            jumpCount = 3;
            speed = 2;
            moveDirection = transform.forward * 2;
            //jump after grab
            jumpSpeed = 15;
            anim.SetBool("isRunning", false);
            anim.SetBool("isWalking", false);
           
        }

        else
        {
            anim.SetBool("isGrabbing", false);
            isGrabbing = false;
            jumpSpeed = 12;
           // GetComponent<Rigidbody>().useGravity = true;
        }

    }
    private void InputColorChange()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ChangeColorTo(new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)));
    }
    void ChangeColorTo(Vector3 color)
    {
        GetComponent<Renderer>().material.color = new Color(color.x, color.y, color.z, 1f);


    }

 void stopSlide()
    {
        anim.SetBool("isSliding", false);
        controller.height = 4.64f;
        isSliding = false;
    }

 void changeCamera()
    {
        RaycastHit ray;
        if (Input.GetKeyDown(KeyCode.Space) && isragdoll && Time.time >= canGetupTime + 1)
        {
            GoStiff(fp);
        }


        if (Input.GetKeyDown(KeyCode.E) && anim.GetBool("isRunning")&& !isSliding)
        {
            anim.SetBool("isSliding", true);
            isSliding = true;
            controller.height = 3;
            Invoke("stopSlide", .7f);
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(transform.position + transform.up * 1 + transform.up * 1, transform.forward, out ray, 3))
            {
                if (ray.collider.transform.root.tag == "AI")
                {
                    anim.SetBool("isPushing", true);
                   ray.collider.transform.root.GetComponent<AI>().TakeFallDamage(100);
                    Invoke("stopPush",1);
                }


            }
        }
        
      /*  if (Input.GetKeyDown(KeyCode.K) && isragdoll && Time.time >= canGetupTime + 2)
        {
            GoStiff();
        }
        */
            if (Input.GetKeyDown(KeyCode.C) &&!Input.GetKeyDown(KeyCode.W) && cameras[0].gameObject.activeSelf && transform.GetComponent<PlayerAI>().selected)
        {
            moveDirection = Vector3.zero;
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunningg", false);
            anim.SetBool("isJumping", false);
            anim.SetBool("isGrabbing", false);
            anim.SetBool("isRolling", true);
            //Debug.Log("Camera Switch1");
            controllerArray[0].SetActive(true);
            cameras[0].gameObject.SetActive(false);
            gameObject.GetComponentInChildren<MouseVerticalLook>().isActive = false;
            transform.GetComponent<PlayerAI>().isFirstPerson = false;
            //cameras[1].gameObject.SetActive(false);
            //cameras[2].gameObject.SetActive(true);
            fp = false;
        }
        else if(Input.GetKeyDown(KeyCode.C) && !Input.GetKeyDown(KeyCode.W)  && !cameras[0].gameObject.activeSelf && transform.GetComponent<PlayerAI>().selected)
        {
            moveDirection = Vector3.zero;
            anim.SetBool("isWalking", false);
            anim.SetBool("isRunningg", false);
            anim.SetBool("isJumping", false);
            anim.SetBool("isGrabbing", false);
            anim.SetBool("isRolling", true);
            //  Debug.Log("Camera Switch2");
            //cameras[2].gameObject.SetActive(false);
            //cameras[1].gameObject.SetActive(true);
            cameras[0].gameObject.SetActive(true);
            controllerArray[0].SetActive(false);
            transform.GetComponent<PlayerAI>().isFirstPerson = true;
            gameObject.GetComponentInChildren<MouseVerticalLook>().isActive = true;
            fp = true;
        }
    }

    void GoRag(int damage)
    {

        //Debug.Log(transform.GetChild(5).name);
            Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody rb in bodies)
            {
            if (rb.tag != "Player")
            {
                rb.useGravity = true;
            }

            }
        
       // cameras[1].gameObject.SetActive(false);
        cameras[0].gameObject.SetActive(false);
        //cameras[2].gameObject.SetActive(true);
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunningg", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isGrabbing", false);
        anim.SetBool("isRolling", true);
        GetComponent<Animator>().enabled = false;
        //ConfigureRagdollPosition(transform.GetChild(5), transform.GetChild(5));
        foreach (Rigidbody rb in bodies)
            {
            
            rb.isKinematic = false;
            if (damage == 1)
            {
                rb.velocity = transform.up * -10;
            }

            if (damage == 0)
            {
                rb.velocity = transform.forward * 10 + transform.up * -2;
            }
            // rb.velocity = transform.up * -20 + transform.forward * 5; 
            //rb.AddExplosionForce(20, transform.position, -200, 1.0F);
        }


       
        Collider[] col2 = GetComponentsInChildren<Collider>();
            foreach (Collider c2 in col2)
            {
                c2.enabled = true;
            }
       
        //controller.enabled = false;
            isragdoll = true;
            canGetupTime = Time.time;
        // Debug.Log("going Rag");
        //controller.GetComponent<Rigidbody>().isKinematic = true;
        controller.SimpleMove(Vector3.zero);
        controller.enabled = false;
        controller.SimpleMove(Vector3.zero);
        //controller.center = transform.GetChild(0).transform.position;


    }


       void GoStiff(bool fps)
        {
        anim.SetBool("isWalking", false);
        anim.SetBool("isRunningg", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isGrabbing", false);
        anim.SetBool("isRolling", true);
        GetComponent<Animator>().enabled = true;
        if (fps)
        {
            cameras[1].gameObject.SetActive(true);
            cameras[0].gameObject.SetActive(true);
            cameras[2].gameObject.SetActive(false);
        }
        isragdoll = false;
            Collider[] col = GetComponentsInChildren<Collider>();
            foreach (Collider c in col)
            {
                c.enabled = false;
            }
        health = 100;
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
           // controller.transform.position = rb.transform.position + rb.transform;
        }
        
        controller.SimpleMove(Vector3.zero);
        controller.enabled = true;
        controller.SimpleMove(Vector3.zero);
        


        // Debug.Log("going Stiff");

    }
    
   
    public void SaveGame()
    {
        {
            PlayerLocation = transform.position;
            PlayerRotation = transform.rotation;
            //string message = Application.loadedLevel.ToString() + "|" + PlayerLocation.ToString() + "|" + PlayerRotation.ToString();
            StreamWriter sw = new StreamWriter(Application.dataPath + "/SaveGame.txt");
            //sw.WriteLine(message);
            sw.Flush();
            sw.Close();
            sw = null;
            gameSaved = true;
        }

    }

    public void Loadgame()
    {
        {

            StreamReader sr = new StreamReader(Application.dataPath + "/SaveGame.txt");
            while (sr.Peek() >= 0)
            {

                loc = sr.ReadLine().ToString();

            }

            // sr.Flush();
            sr.Close();
            sr = null;
            string[] DEETS = loc.Split('|');
            string location = DEETS[1];
            string rotation = DEETS[2];
            string level = DEETS[0];
            location = location.Replace("(", "");
            location = location.Replace(")", "");
            rotation = rotation.Replace("(", "");
            rotation = rotation.Replace(")", "");

            string[] pos = location.Split(',');
            string[] rot = rotation.Split(',');
            float rotx = float.Parse(rot[0]);
            float roty = float.Parse(rot[1]);
            float rotz = float.Parse(rot[2]);
            float rotw = float.Parse(rot[3]);
            float posx = float.Parse(pos[0]);
            float posy = float.Parse(pos[1]);
            float posz = float.Parse(pos[2]);

           

            //Application.LoadLevel(int.Parse(level));
            transform.position = new Vector3(posx, posy, posz);
            transform.rotation = new Quaternion(rotx, roty, rotz, rotw);

            //Debug.Log(location + "<=>" + level);
        }
    }

    void stopPush()
    {
        anim.SetBool("isPushing", false);
    }
    public void TakeDamage()
    {

        health -= 25;
        flashEnabled = true;
        Invoke("DisableFlash", .1f);
        
        if (health <= 0)
        {
            Instantiate(ragdoll, transform.position, transform.rotation);

            Destroy(this.gameObject);
        }

    }
    bool SeeWall()
    {
        bool nearWall;


        RaycastHit ray;
        if (Physics.Raycast(transform.position + transform.up * 4, transform.forward, out ray, 1f) || Physics.Raycast(transform.position + transform.up * 4, transform.right *-1, out ray, 1f)
            || Physics.Raycast(transform.position + transform.up * 4, transform.right * 1, out ray, 1f)|| Physics.Raycast(transform.position + transform.up * 4, transform.forward *-1, out ray, 1f))
        {
           float distanceToHit = Vector3.Distance(this.transform.position, ray.transform.position);
            // Debug.Log("WallDistance: " + distanceToHit);
            if (distanceToHit >= 2 && ray.collider.name != "Door")
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

    bool BackToWall()
    {
        bool nearWall;


        RaycastHit ray;
        if (Physics.Raycast(transform.position + transform.up * 4, transform.forward * -1, out ray, 1f))
        {
            float distanceToHit = Vector3.Distance(this.transform.position, ray.transform.position);
            // Debug.Log("WallDistance: " + distanceToHit);
            if (distanceToHit >= 2 && ray.collider.name != "Door")
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
    public void TakeFallDamage(int damage)
    {

        health -= damage;

        // flashEnabled = true;
        // Invoke("DisableFlash", .1f);

        if (health <= 0)
        {
            GoRag(1);
           // Debug.Log("Happening");
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
}

