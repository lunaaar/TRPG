using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Camera camera;
    Vector3 cameraPosition;

    [Header("===== HandleCameraMovement =====")]
    [SerializeField] private float edgeSize = 50f;
    [SerializeField] private float moveAmount = 100f;
    public bool useMouseInput = true;
    public bool useKeyboardInput = true;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null) instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!GuiManager.gameIsPaused)
        {
            cameraPosition = camera.transform.position;

            handleCameraMovement();
        }
    }

    //Needs to rewrite
    void handleCameraMovement()
    {
        if (useKeyboardInput)
        {
            if (Input.GetKey(KeyCode.W))
            {
                cameraPosition.y += moveAmount * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                cameraPosition.y -= moveAmount * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.D))
            {
                cameraPosition.x += moveAmount * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                cameraPosition.x -= moveAmount * Time.deltaTime;
            }
        }
        
        if (useMouseInput)
        {
            if (Input.mousePosition.x > Screen.width - edgeSize)
            {
                cameraPosition.x += moveAmount * Time.deltaTime;
            }
            if (Input.mousePosition.x < edgeSize)
            {
                cameraPosition.x -= moveAmount * Time.deltaTime;
            }

            if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                cameraPosition.y += moveAmount * Time.deltaTime;
            }
            if (Input.mousePosition.y < edgeSize)
            {
                cameraPosition.y -= moveAmount * Time.deltaTime;
            }
        }
        
        camera.transform.position = cameraPosition;
    }
}
