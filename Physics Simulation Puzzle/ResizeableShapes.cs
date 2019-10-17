using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public class ResizeableShapes : MonoBehaviour
{
    public Camera RayCastCamera; // Camera For RayCast
    public TextMeshProUGUI infoText; // Information Text
    public float RotateSpeed; // Set The Speed Of the Rotation
    [SerializeField]
    private Vector2 MousePosition1, MousePosition2, distance; // Mouse Position and distance from mouse stored here
    private Vector2 mousepos, objpos, distanceMove, objScale; // mouse position distance from object start edge used When Creating New Object
    private float Magnitudedistance; // Magnitude For Scaling Equaly
    private GameObject obj; // Newly Object stored Here When Created
    [SerializeField]
    private GameObject SelectionCube;// This Object is for drag and select objects
    [SerializeField]
    private GameObject[] Objects; // Objects That Can Be Created object scale must be 1 by1 and size should be 1 by 1 meter or 1 by 1 unit
    private int ObjectToSpawn; // Set Object To Creat By This Int
    private List<GameObject> objlist = new List<GameObject>(); // All Objects Created Stored In this List
    private List<int> selectedObjects = new List<int>(); // Selected Objects In Selection Mode Stored Here
    private int chk; // To Check If We Selecting A Selected Object Again
    private RaycastHit HitInfo; // RayCast To Select Object
    private bool IsSelection = false, IsMoving = false, MoveObjects = false, IsScaling = false,
                    ObjectCreating = false, IsRotating = false; //Check In Which State We Are
    private bool positionToMouse = false; // Set The Position Of the Object When Creating New Object

    void Start()
    {
        infoText.text = "Click And Drag To Creat Objects";
        primitiveTypeBtn(1);
        ObjectCreating = true;
    }

    void Update()
    {
        //Object Rotation
        if (IsRotating == true)
        {
            if (Input.GetMouseButton(0))
            {
                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    float rot = Input.GetAxis("Mouse X") * RotateSpeed * Mathf.Deg2Rad;
                    objlist[selectedObjects[i]].transform.RotateAround(Vector3.forward, -rot);
                }
            }
        }

        // Click And Drag To Move Object
        if (IsMoving == true)
        {
            infoText.text = "Move Objects";
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = RayCastCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out HitInfo))
                {
                    if (HitInfo.collider != null)
                    {
                        obj = HitInfo.collider.gameObject;
                        MoveObjects = true;
                        mousepos = RayCastCamera.ScreenToWorldPoint(Input.mousePosition);
                        objpos = obj.transform.position;
                        distanceMove = mousepos - objpos;
                    }
                }

            }
            if (MoveObjects == true)
            {
                if (obj.transform.GetComponent<Rigidbody>())
                {
                    mousepos = RayCastCamera.ScreenToWorldPoint(Input.mousePosition);
                    obj.transform.GetComponent<Rigidbody>().MovePosition(mousepos - distanceMove);
                    obj.transform.GetComponent<Rigidbody>().useGravity = false;
                    obj.transform.GetComponent<Rigidbody>().velocity = Vector3.zero;

                }
                else
                {
                    mousepos = RayCastCamera.ScreenToWorldPoint(Input.mousePosition);
                    obj.transform.position = mousepos - distanceMove;
                }

            }
            if (Input.GetMouseButtonUp(0))
            {
                if (obj.transform.GetComponent<Rigidbody>())
                {
                    obj.transform.GetComponent<Rigidbody>().useGravity = true;
                }
                MoveObjects = false;
            }
            ObjectCreating = false;
        }


        //=========================================================================

        // Object Selection Mode ======================

        if (IsSelection == true)
        {

            //  Miltiple Selection Mode  ======================================================

            infoText.text = "Object Selection Mode";
            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl))
            {
                Ray ray = RayCastCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out HitInfo))
                {
                    if (HitInfo.collider != null)
                    {
                        obj = HitInfo.collider.gameObject;
                        selectedObjects.Add(objlist.IndexOf(obj));
                        chk = selectedObjects[selectedObjects.Count - 1];
                        if (selectedObjects.Count != 0)
                        {
                            for (int i = 0; i < selectedObjects.Count - 1; i++)
                            {
                                if (selectedObjects[i] == chk)
                                {

                                    objlist[selectedObjects[i]].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                                    selectedObjects.RemoveAt(selectedObjects.Count - 1);
                                    selectedObjects.RemoveAt(i);
                                }

                            }
                        }

                        for (int i = 0; i < selectedObjects.Count; i++)
                        {
                            objlist[selectedObjects[i]].GetComponent<Renderer>().material.color = new Color32(11, 126, 186, 255);

                        }

                    }

                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;
                    if (Input.GetKey(KeyCode.LeftControl))
                        return;
                    for (int i = 0; i < selectedObjects.Count; i++)
                    {
                        objlist[selectedObjects[i]].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                    }
                    selectedObjects.Clear();
                }

            }

            //  Single Selection Mode  ======================================================

            else if (Input.GetMouseButtonDown(0))
            {
                Ray ray = RayCastCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out HitInfo))
                {
                    if (HitInfo.collider != null)
                    {
                        obj = HitInfo.collider.gameObject;
                        obj.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                        selectedObjects.Clear();
                        selectedObjects.Add(objlist.IndexOf(obj));
                        for (int i = 0; i < objlist.Count; i++)
                        {
                            if (objlist[i] != null)
                            {
                                objlist[i].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);

                            }
                        }
                        objlist[selectedObjects[0]].GetComponent<Renderer>().material.color = new Color32(11, 126, 186, 255);

                    }

                }
                else
                {
                    if (EventSystem.current.IsPointerOverGameObject())
                        return;
                    for (int i = 0; i < selectedObjects.Count; i++)
                    {
                        objlist[selectedObjects[i]].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
                    }
                    selectedObjects.Clear();
                }

            }


        }
        else
        {
            CreatingObjects();

        }

    }

    // Change The Objects Color To Default If DeSelected Of Any Other Operation Activated
    void ChangeColorToDefault()
    {
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            objlist[selectedObjects[i]].GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255);
        }
        selectedObjects.Clear(); ;
    }

    // Creating Objects 
    void CreatingObjects()
    {
        if (ObjectCreating == true)
        {

            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                MousePosition1 = RayCastCamera.ScreenToWorldPoint(Input.mousePosition);
                obj = Instantiate(Objects[ObjectToSpawn]);
                objlist.Add(obj);
                obj.transform.position = new Vector2(MousePosition1.x + 0.5f, MousePosition1.y - 0.5f);
                positionToMouse = true;
            }

            if (positionToMouse)
            {
                infoText.text = "Hold Shift to Scale Equaly";
                MousePosition2 = RayCastCamera.ScreenToWorldPoint(Input.mousePosition);
                if (MousePosition1.x > MousePosition2.x || MousePosition1.y > MousePosition2.y ||
                    MousePosition1.x < MousePosition2.x || MousePosition1.y < MousePosition2.y)
                {
                    distance = MousePosition1 - MousePosition2;

                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        Magnitudedistance = (MousePosition1 - MousePosition2).magnitude;
                        obj.transform.localScale = new Vector3(Magnitudedistance, Magnitudedistance, 1);
                    }
                    else
                    {
                        obj.transform.localScale = new Vector3(distance.x, distance.y, 1);
                    }
                    obj.transform.position = new Vector2(MousePosition1.x - distance.x / 2, MousePosition1.y - distance.y / 2);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                MousePosition1 = Vector2.zero;
                positionToMouse = false;
            }
        }
    }

    public void Rotation()
    {
        IsRotating = true;
        IsSelection = false;
        IsScaling = false;
        IsMoving = false;
        ObjectCreating = false;
    }
    public void Scale()
    {
        ChangeColorToDefault();
        ObjectCreating = false;
        IsMoving = false;
        IsRotating = false;
        IsSelection = false;
    }

    public void Move()
    {
        ChangeColorToDefault();
        IsMoving = true;
        ObjectCreating = false;
        IsScaling = false;
        IsRotating = false;
        IsSelection = false;
    }
    public void selection()
    {
        IsSelection = true;
        IsScaling = false;
        IsMoving = false;
        IsRotating = false;
        ObjectCreating = false;
    }
    public void AddRigidBody()
    {
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            objlist[selectedObjects[i]].AddComponent(typeof(Rigidbody));
            objlist[selectedObjects[i]].GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX |
                                                                                RigidbodyConstraints.FreezeRotationY |
                                                                                RigidbodyConstraints.FreezePositionZ;
        }
        ChangeColorToDefault();
    }
    public void DeleteObject()
    {
        for (int i = 0; i < selectedObjects.Count; i++)
        {
            Destroy(objlist[selectedObjects[i]]);
        }

        objlist.RemoveAll(objlist => objlist == null);
        selectedObjects.Clear();

    }
    public void primitiveTypeBtn(int primitiveSwitch)
    {
        ChangeColorToDefault();
        IsScaling = true;
        IsMoving = false;
        IsSelection = false;
        IsRotating = false;
        ObjectCreating = true;
        switch (primitiveSwitch)
        {
            case 1:
                ObjectToSpawn = 0;
                infoText.text = "Click or Drag To Creat Cube";
                break;
            case 2:
                ObjectToSpawn = 1;
                infoText.text = "Click or Drag To Creat Circle";
                break;
            case 3:
                ObjectToSpawn = 2;
                infoText.text = "Click or Drag To Creat Torus";
                break;
            case 4:
                ObjectToSpawn = 3;
                infoText.text = "Click or Drag To Creat Sphear";
                break;
        }
    }
}
