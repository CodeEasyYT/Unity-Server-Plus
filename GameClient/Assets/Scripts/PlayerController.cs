using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //assignables
    public Transform camTransform;
    private Rigidbody rb;

    //colliders to disable on lerp
    [Tooltip("Character Controller Also Works")]
    public Collider[] colliders;

    //movement ID
    private int movementID;

    //x and y movemets
    private float x, y;
    private readonly float jumpForce = 4;

    //S P E E D
    private readonly float speed = 5f;

    //health of player
    public static int health = 100;

    //movement booleans
    private bool crouching;
    private bool jumping;

    //distance for ray to contact ground
    private float raycastDistance = 1.1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ClientSend.PlayerShoot(camTransform.forward);
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            ClientSend.PlayerThrowItem(camTransform.forward);
        }
    }

    private void FixedUpdate()
    {
        CheckPos();
        SendInputToServer();
    }

    /// <summary>Sends player input to the server.</summary>
    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.LeftControl),
            Input.GetKey(KeyCode.Space)
        };

        //moves player client side for client prediction
        Move(_inputs);

        ClientSend.PlayerMovement(_inputs, movementID);
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        //set max fps to 30, this is changeable
        Application.targetFrameRate = 30;
    }

    private void Move(bool[] inputs)
    {
        //attempting move
        TranslateInputs(inputs);

        Jump();

        Vector3 movement = new Vector3(x, 0, y) * speed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + rb.transform.TransformDirection(movement);

        movementID++;

        SharedVariables.movementStore.Add(movementID, transform.position);

        rb.MovePosition(newPosition);
    }

    private void TranslateInputs(bool[] inputs)
    {
        if (health <= 0f)
        {
            return;
        }

        y = 0;
        x = 0;

        if (inputs[0])
        {
            y += 1;
        }

        if (inputs[1])
        {
            y -= 1;
        }

        if (inputs[2])
        {
            x -= 1;
        }

        if (inputs[3])
        {
            x += 1;
        }

        if (inputs[4])
        {
            crouching = true;
        }
        else
        {
            crouching = false;
        }

        if (inputs[5])
        {
            jumping = true;
        }
        else
        {
            jumping = false;
        }
    }

    private void Jump()
    {
        if (jumping && IsGrounded())
        {
            rb.AddForce(0, jumpForce, 0, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, raycastDistance);
    }


    //position checking for client prediction
    public void CheckPos()
    {
        if (!SharedVariables.doCheck)
            return;
        else
        {
            //check if the distance between the client and server client is greater than one
            if (Vector3.Distance(SharedVariables.serverPos, SharedVariables.clientPos) > 0.5f)
            {
                Debug.Log("resetting position");

                StartCoroutine(InsterpolateToPosition(SharedVariables.serverPos, 0.2f));

                SharedVariables.doCheck = false;
            }
            else
            {
                return;
            }
        }
    }
    
    public IEnumerator InsterpolateToPosition(Vector3 _targetPosition, float _time)
    {
        foreach (Collider _collider in colliders)
        {
            _collider.enabled = false;
        }

        rb.isKinematic = true;

        float _timePercentage = 0;
        while(_timePercentage < 1)
        {
            _timePercentage += Time.deltaTime / _time;
            transform.position = Vector3.Lerp(transform.position, _targetPosition, _timePercentage);
            yield return null;
        }

        foreach (Collider _collider in colliders)
        {
            _collider.enabled = true;
        }

        rb.isKinematic = false;
    }
}
