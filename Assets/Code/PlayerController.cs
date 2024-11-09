using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public KeyCode castKeybind, directionKeybind;
    public float range;
    public GameObject iceWallPreview, iceWallOBJ;
    public LayerMask layermask;
    private bool direction, casting;

    // Movement
    public float speed = 10.0f;
    private float translation;
    private float strafe;

    // Camera
    public float lookSpeed = 3f;
    public GameObject bullet;
    private Vector2 rotation = Vector2.zero;
    private Transform cam;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        rb.WakeUp();

    } 

    void Update()
    {

        //Movement
        translation = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        strafe = Input.GetAxis("Horizontal") * speed * Time.deltaTime; 
        transform.Translate(strafe, 0, translation);

        //Camera
        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        rotation.x = Mathf.Clamp(rotation.x, -40f, 40f);
        transform.eulerAngles = new Vector2(0, rotation.y) * lookSpeed; 
        cam.localRotation = Quaternion.Euler(rotation.x * lookSpeed, 0, 0);

        if(Input.GetMouseButtonDown(1))
        {
            Instantiate(bullet, cam.position + cam.forward, cam.rotation);
        }

        if (Input.GetKeyDown(castKeybind))
        {
            casting = !casting;
            if (!casting)
            {
                iceWallPreview.SetActive(false);
            }

            if(casting)
            {
                CastingIceWall();
            }
        }

        if(Input.GetKeyUp(directionKeybind))
        {
            direction = !direction;
        }

        if (casting) 
        {
            CastingIceWall();
        }
    }

    void CastingIceWall()
    {
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.forward, out hit, range, layermask))
        {
            if (iceWallPreview.activeSelf)
            {
                iceWallPreview.SetActive(true);
            }

            Quaternion rotation = Quaternion.Euler(0,0,0);
            if (direction)
            {
                rotation.y = 1;
            }
            else
            {
                rotation.y = 0;
            }

            iceWallPreview.transform.localRotation = rotation;
            iceWallPreview.transform.position = hit.point;

            if(Input.GetMouseButtonDown(0))
            {
                Instantiate(iceWallOBJ, hit.point, iceWallPreview.transform.rotation);
                casting = false;
                iceWallPreview.SetActive(false); 
            }
        }
        else
        {
            iceWallPreview.SetActive(false);
        }

        if (Input.GetKeyUp(directionKeybind))
        {
            direction = !direction;
        }
    }

}
