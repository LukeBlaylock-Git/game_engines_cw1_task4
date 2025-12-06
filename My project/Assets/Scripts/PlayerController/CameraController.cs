using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public float Distance = 10f;
    public float RotationSpeed = 2;
    public float MinVertical = 0;
    public float MaxVerticalAngle = 85;
    public float ZoomSpeed = 5f;
    public float MinZoom = 2f;
    public float MaxZoom = 15f;



    private Vector2 Rotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel"); //Gets the Input_Manager... input, uses it for our scroll variable.
        Distance -= scroll * ZoomSpeed; //Distance subtract scroll * ZoomSpeed result
        Distance = Mathf.Clamp(Distance, MinZoom, MaxZoom); //The clamp is to keep our camera between a Min and Max, hence why Min zoom and Max zoom are here, distance updates, min and max stay static.

        Rotation += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X") * RotationSpeed); //simply put Rotation moves the camera up and down depending on the mouse input, which is multplied for the rotation speed.
        Rotation.x = Mathf.Clamp(Rotation.x, MinVertical, MaxVerticalAngle);

        var TargetRotation = Quaternion.Euler(Rotation); //Bit confusing and this probably isnt accurate, but this takes Vector2 angles, converts them into a Quaternion, which is what unity uses to represent movement in a 3D space (kinda)

        transform.position = Target.position - TargetRotation * new Vector3(0f, 0f, Distance);
        transform.rotation = TargetRotation;


    }
}
