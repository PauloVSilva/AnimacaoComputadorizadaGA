using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    public List<Transform> bodyParts = new List<Transform>();

    public Transform head;
    public GameObject Eyes;
    public AIBehaviour behave;

    public bool selected;
    public bool isDead;
  
    private Vector3 direction;

    //speeds
    public float speed;
    public float speedWalking = 3.5f;
    public float speedRunning = 10.0f;
    public float speedStill = 0.0f;
    public float bonusSpeed = 0.0f;

    
    public float speedMultiplier = 4.0f;
    public float speedPower = 3.0f;

    public float FinalSpeed => speed + bonusSpeed;


    // Start is called before the first frame update
    void Start()
    {
        MusicPulsation.OnGetSpectrum += ScaleWithMusic;

        head = transform;
        isDead = false;

        StartCoroutine(UpdateStatusEveryXSeconds(0.5f));
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            //Debug.Log("Speed is " + speed.ToString());
            if (Input.GetMouseButtonDown(0))
            {
                Dash();
            }
        }
    }

    private void OnDestroy()
    {
        MusicPulsation.OnGetSpectrum -= ScaleWithMusic;
    }

    private void ScaleWithMusic(float scale)
    {
        if (isStopped)
        {
            bonusSpeed = 0.0f;
        }
        else
        {
            bonusSpeed = Mathf.Pow((scale - 1) * speedMultiplier, speedPower);
        }
    }

    public void SetBehaviour(AIBehaviour behaviour)
    {
        behave = Instantiate(behaviour);
        behave.Init(this.gameObject, this);
    }

    
    void FixedUpdate()
    {       
        behave.Execute();

        if (selected)
        {
            CameraFollow();
            
        }
    }

    public bool isRunning = false;
    public bool isStopped = false;
   
    //[Range(0.0f, 1.0f)]
    public float smoothTime = 0.05f;

    void CameraFollow()
    {
        Transform camera = GameObject.FindGameObjectWithTag("MainCamera").gameObject.transform;
        Vector3 cameraVelocity = Vector3.zero;
        camera.position = Vector3.SmoothDamp(camera.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -10), ref cameraVelocity, smoothTime);
    }

    public Transform bodyObject;

    void OnTriggerEnter2D(Collider2D other)
    {  
        if (other.gameObject.transform.tag == "Body")
        {          
            if (transform.parent.name != other.gameObject.transform.parent.name)
            {
                isDead = true;

                for (int i = 0; i < bodyParts.Count; i++)
                {
                    Destroy(bodyParts[i].gameObject);
                    Destroy(bodyParts[i]);
                }
            }           
        }

        if (other.transform.tag == "Orb")
        {
            Destroy(other.gameObject);

            //Adiciona uma parte do corpo no final
            Vector3 currentPos;
            if (bodyParts.Count == 0)
            {
                currentPos = transform.position;               
            }
            else
            {
                currentPos = bodyParts[bodyParts.Count - 1].position;
            }
            CreateNewPart(currentPos);
        }
    }

    void CreateNewPart(Vector3 currentPos)
    {
        Transform newBodyPart = Instantiate(bodyObject, currentPos, Quaternion.identity) as Transform;
        newBodyPart.parent = transform.parent;
        bodyParts.Add(newBodyPart.transform);

        int nParts = head.GetComponent<SnakeMovement>().bodyParts.Count;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = nParts;
        Eyes.GetComponent<SpriteRenderer>().sortingOrder = nParts + 1;
    }

    void DeleteLastPart()
    {
        Destroy(bodyParts[bodyParts.Count - 1].gameObject);
        Destroy(bodyParts[bodyParts.Count - 1]);
        bodyParts.RemoveAt(bodyParts.Count - 1);
        int nParts = head.GetComponent<SnakeMovement>().bodyParts.Count;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = nParts;
        Eyes.GetComponent<SpriteRenderer>().sortingOrder = nParts+1;

    }

    private void Dash()
    {        
        if (bodyParts.Count > 2)
        {
            //Debug.Log("Entrou!");
            DeleteLastPart();
            isRunning = true;
            speed = speedRunning;
        }
    }

    public void Stop()
    {
        speed = speedStill;
        isStopped = true;
    }

    IEnumerator UpdateStatusEveryXSeconds(float x)
    {
        yield return new WaitForSeconds(x);
        StopCoroutine(UpdateStatusEveryXSeconds(x));
        if (isRunning || isStopped)
        {
            isRunning = true;
            speed = speedWalking;

            isStopped = false;
        }

        StartCoroutine(UpdateStatusEveryXSeconds(x));
    }

    private void OnDrawGizmos()
    {
        Vector3 raycastDirection = transform.up; // Use 'up' or 'forward' depending on your object's orientation.
        float raycastDistance = 2f;

        Gizmos.color = Color.red; // Set the color for the raycast line (you can choose any color you like).

        // Draw the raycast as a line in the Scene view.
        Gizmos.DrawRay(transform.position, raycastDirection * raycastDistance);
    }
}
