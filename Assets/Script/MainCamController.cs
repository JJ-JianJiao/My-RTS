using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MainCamController : MonoBehaviour
{

    [Header("Move base on edge")]
    public bool isUseMoveOnScreenEdge = true;
    public bool isDebugScreenEdge = true;
    public int ScreenEdgeSize = 20;

    private Rect RightRect;
    private Rect UpRect;
    private Rect LeftRect;
    private Rect DownRect;

    private bool MoveUp;
    private bool MoveDown;
    private bool MoveRight;
    private bool MoveLeft;


    private Material mat;
    private Vector3 dir = Vector3.zero;

    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private float zoomSpeed;
    [SerializeField]
    private Transform firstCam;

    [SerializeField]
    private float rInput;
    [SerializeField]
    private float zoomInput;
    [SerializeField]
    private float hInput;
    [SerializeField]
    private float vInput;

    private RaycastHit mainCamZoomHit;
    public float distanceMainCamToGround;
    public float preDistanceMainCamToGround;
    public float yDeviation;

    // Start is called before the first frame update
    void Start()
    {
        CreateLineMaterial();
    }



    // Update is called once per frame
    void Update()
    {


        if (Physics.Raycast(firstCam.position, Vector3.down, out mainCamZoomHit)) {
            if (mainCamZoomHit.collider.CompareTag("Ground")) {
                //distanceMainCamToGround = Vector3.Distance(firstCam.position, mainCamZoomHit.collider.transform.position);
                distanceMainCamToGround = Vector3.Distance(firstCam.position, mainCamZoomHit.point);
                //Debug.Log("every frame the distance: " + distanceMainCamToGround);
            }
        }



        rInput = Input.GetAxis("RotateHorizontal");
        zoomInput = Input.GetAxis("Mouse ScrollWheel");
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        if (hInput > 0)
        {
            dir.z = 1;
        }
        else if (hInput < 0)
        {
            dir.z = -1;
        }
        else {
            dir.z = 0;
        }

        if (vInput   > 0)
        {
            dir.x = -1;
        }
        else if (vInput < 0)
        {
            dir.x = 1;
        }
        else
        {
            dir.x = 0;
        }

        if (TutorialManager.instance !=null && TutorialManager.instance.index == 0 && !TutorialManager.instance.isDoneMoveTutorial) {
            if (vInput > 0)
            {
                TutorialManager.instance.MoveF = true;
            }
            else if (vInput < 0) {
                TutorialManager.instance.MoveB = true;
            }
            if (hInput > 0)
            {
                TutorialManager.instance.MoveL = true;
            }
            else if (hInput < 0) {
                TutorialManager.instance.MoveR = true;
            }
        }

        if (TutorialManager.instance != null &&TutorialManager.instance.index == 1 &&!TutorialManager.instance.isDoneRotateTutorial) {
            if (rInput > 0)
            {
                TutorialManager.instance.RotateL = true;
            }
            else if (rInput < 0) {
                TutorialManager.instance.RotateR = true;
            }
        }

        if (TutorialManager.instance != null && TutorialManager.instance.index == 2 && !TutorialManager.instance.isDoneZoomTutorial)
        {
            if (zoomInput > 0)
            {
                TutorialManager.instance.ZoomIn = true;
            }
            else if (zoomInput < 0)
            {
                TutorialManager.instance.ZoomOut = true;
            }
        }

        //yDeviation = preDistanceMainCamToGround - distanceMainCamToGround;
        //if (yDeviation <= 0.001)
        //{
        //    yDeviation = 0;
        //}
        //else
        //{
        //    Debug.Log(yDeviation);
        //}


        //if (hInput != 0 || vInput != 0)
        //{
        //    preDistanceMainCamToGround = distanceMainCamToGround;
        //}



        firstCam.Rotate(Vector3.up * Time.deltaTime * rInput * rotationSpeed);

        if ((distanceMainCamToGround < 10 && zoomInput > 0) || (distanceMainCamToGround > 20 && zoomInput < 0))
        {

            zoomInput = 0;

        }

        //if ((firstCam.position.z > 1000 && hInput > 0) || (firstCam.position.z < 0 && hInput < 0)) {
        //    dir.z = 0;
        //}
        //if ((firstCam.position.x> 990 && vInput > 0) || (firstCam.position.x < 10 && vInput > 0))
        //{
        //    dir.x = 0;
        //}

        firstCam.Translate(Vector3.up * Time.deltaTime * -zoomInput * zoomSpeed);
        //if (zoomInput != 0)
        //{
        //    Debug.Log("ZooMINput: " + zoomInput);
        //    Debug.Log("ZoomInput Time : " + Time.time);
        //    Debug.Log("preDistanceMainCamToGround Time : " + preDistanceMainCamToGround);

        //    preDistanceMainCamToGround = Vector3.Distance(firstCam.position, mainCamZoomHit.point);
        //    Debug.Log("preDistanceMainCamToGround Time : " + preDistanceMainCamToGround);
        //    yDeviation = 0;
        //}


        firstCam.Translate(dir * Time.deltaTime  * movementSpeed);
        //if (yDeviation != 0)
        //{
        //    Debug.Log("ChangeY Time + " + Time.time);
        //    Debug.Log(" Zoom Input ++++ " + zoomInput);
        //    Debug.Log("call increase Once+++");
        //    Debug.Log("ORiginal Y++++:" + firstCam.position.y);
        //    firstCam.position += new Vector3(0, yDeviation, 0);
        //    Debug.Log("Changedni Y+++:" + firstCam.position.y);
        //}

        if (Vector3.Distance(firstCam.position, mainCamZoomHit.point) < 9.99999) {
            firstCam.position += new Vector3(0, 10- Vector3.Distance(firstCam.position, mainCamZoomHit.point), 0);
        }


        if (isUseMoveOnScreenEdge && hInput == 0 && vInput == 0)
        {
            UpRect = new Rect(1f, Screen.height - ScreenEdgeSize, Screen.width, ScreenEdgeSize);
            DownRect = new Rect(1f, 1f, Screen.width, ScreenEdgeSize);

            LeftRect = new Rect(1f, 1f, ScreenEdgeSize, Screen.height);
            RightRect = new Rect(Screen.width - ScreenEdgeSize, 1f, ScreenEdgeSize, Screen.height);


            MoveUp = (UpRect.Contains(Input.mousePosition));
            MoveDown = (DownRect.Contains(Input.mousePosition));

            MoveLeft = (LeftRect.Contains(Input.mousePosition));
            MoveRight = (RightRect.Contains(Input.mousePosition));

            dir.x = MoveUp ? -1 : MoveDown ? 1 : 0;
            dir.z = MoveLeft ? -1 : MoveRight ? 1 : 0;

            //if ((firstCam.position.z > 1000 && MoveRight) || (firstCam.position.z < 0 && MoveLeft))
            //{
            //    dir.z = 0;
            //}

            //if ((firstCam.position.x > 1000 && MoveUp) || (firstCam.position.z < 0 && MoveDown))
            //{
            //    dir.x = 0;
            //}
            //transform.position = Vector3.Lerp(transform.position, transform.position + dir * movementSpeed, Time.deltaTime);
            //transform.parent.position = Vector3.Lerp(transform.position, transform.position + dir * movementSpeed, Time.deltaTime);
            //firstCam.position = Vector3.Lerp(firstCam.position, firstCam.position + dir * movementSpeed, Time.deltaTime);
            //firstCam.position = Vector3.Lerp(firstCam.position, firstCam.position + dir * movementSpeed, Time.deltaTime);

            firstCam.Translate(dir * Time.deltaTime * movementSpeed);

        }

        if (firstCam.position.z > 1000)
        {
            firstCam.position = new Vector3(firstCam.position.x, firstCam.position.y, 1000);
        }
        else if (firstCam.position.z < 0)
        {
            firstCam.position = new Vector3(firstCam.position.x, firstCam.position.y, 0);
        }
        if (firstCam.position.x > 1000)
        {
            firstCam.position = new Vector3(1000, firstCam.position.y, firstCam.position.z);
        }
        else if (firstCam.position.x < 0)
        {
            firstCam.position = new Vector3(0, firstCam.position.y, firstCam.position.z);
        }

    }

    private void OnPostRender()
    {
        if (isUseMoveOnScreenEdge && isDebugScreenEdge) {
            DrawRect(UpRect, MoveUp, Color.cyan, Color.red);
            DrawRect(DownRect, MoveDown, Color.green, Color.red);
            DrawRect(LeftRect, MoveLeft, Color.yellow, Color.red);
            DrawRect(RightRect, MoveRight, Color.blue, Color.red);
        }
    }

    private void DrawRect(Rect rect, bool isMouseEnter, Color normalColor, Color HeighLightColor)
    {
        if (isMouseEnter)
        {
            DrawScreenRect(rect, HeighLightColor);
        }
        else {
            DrawScreenRect(rect, normalColor);
        }
    }

    private void DrawScreenRect(Rect rect, Color color)
    {
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        {
            mat.SetPass(0);
            GL.Color(color);
            GL.Vertex3(rect.xMin / Screen.width, rect.yMin / Screen.height, 0);
            GL.Vertex3(rect.xMin / Screen.width, rect.yMax / Screen.height, 0);

            GL.Vertex3(rect.xMin / Screen.width, rect.yMax / Screen.height, 0);
            GL.Vertex3(rect.xMax / Screen.width, rect.yMax / Screen.height, 0);

            GL.Vertex3(rect.xMax / Screen.width, rect.yMax / Screen.height, 0);
            GL.Vertex3(rect.xMax / Screen.width, rect.yMin / Screen.height, 0);

            GL.Vertex3(rect.xMax / Screen.width, rect.yMin / Screen.height, 0);
            GL.Vertex3(rect.xMin / Screen.width, rect.yMin / Screen.height, 0);
        }
        GL.End();
    }

    private void CreateLineMaterial()
    {
        if (!mat) {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            mat = new Material(shader);
            mat.hideFlags = HideFlags.HideAndDontSave;
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            mat.SetInt("_ZWrite", 0);
        }
    }

}
