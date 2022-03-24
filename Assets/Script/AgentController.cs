using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    //store the mouse position of start and end
    public Vector3 startMousePos;
    public Vector3 endMousePos;
    //the corners of our 2d selection box
    private Vector2[] corners;
    //the vertices of our meshcollider
    private Vector3[] verts;


    //Collider variables
    //================================================//
    private Mesh selectionMesh;
    MeshCollider selectionBox;


    private bool dragSelect;


    public static AgentController instance;

    [SerializeField]
    private List<NavMeshAgent> agents = new List<NavMeshAgent>();

    [SerializeField]
    private Camera firstCamera;

    private RaycastHit screenGroundHit;

    private void Update()
    {
        if (PauseMenuManager.instance != null && !PauseMenuManager.instance.isGamePause)
        {
            //Hot key to stop all agents' movement
            if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.S))
            {
                StopAllAgents();
            }

            if (Input.GetKey(KeyCode.Space)) {
                if (agents.Count > 0) {
                    firstCamera.transform.root.position = agents[0].GetComponent<SoilderComtroller>().GetHotKeyCamPositon();
                    firstCamera.transform.root.rotation = agents[0].GetComponent<SoilderComtroller>().GetHotKeyCamRoatation();
                }
            }

            //select the agents
            if (Input.GetMouseButtonDown(0))
            {
                startMousePos = Input.mousePosition;
            }
            //move the agents
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.Raycast(firstCamera.ScreenPointToRay(Input.mousePosition), out screenGroundHit))
                {
                    if (screenGroundHit.collider.CompareTag("Ground"))
                    {
                        if (agents.Count > 0)
                        {
                            //TODO: display Cursor Animation
                            CursorManager.instance.DisplayMoveToCursor(screenGroundHit.point);

                            //play soldier move(yes) AS
                            SoundManager.instance.SoldierYes();

                            if (TutorialManager.instance.index == 4 && !TutorialManager.instance.moveUnitTutorial)
                            {
                                TutorialManager.instance.moveUnit = true;
                            }
                            //foreach (NavMeshAgent agent in agents)
                            //{
                            //    agent.destination = screenGroundHit.point;
                            //}

                            AssignDestiation(screenGroundHit.point);
                        }
                    }
                    else if (screenGroundHit.collider.CompareTag("Enemy")) {
                        Debug.Log("we have a attack target!");
                        //play soldier attack AS
                        if (agents.Count > 0) {
                            SoundManager.instance.SoldierAttack();
                            foreach (var agent in agents)
                            {
                                //agent.destination = screenGroundHit.collider.transform.position;
                                agent.transform.GetComponent<SoilderComtroller>().SetAttackTarget(screenGroundHit.collider.gameObject);
                            }
                        }
                    }
                }
            }

            //mouse up
            if (Input.GetMouseButtonUp(0))
            {
                if (!dragSelect)
                {
                    if (Physics.Raycast(firstCamera.ScreenPointToRay(startMousePos), out screenGroundHit))
                    {
                        Transform currentObj = screenGroundHit.collider.transform.root;
                        if (currentObj.CompareTag("Selectable"))
                        {

                            if (Input.GetKey(KeyCode.LeftShift))
                            {

                                if (!currentObj.GetComponent<Health>().isDead && currentObj.GetComponent<NavMeshAgent>())
                                {
                                    if (agents.Contains(currentObj.GetComponent<NavMeshAgent>()))
                                    {
                                        currentObj.GetComponent<SelectedCircleController>().DisableSelectedCircle();
                                        agents.Remove(currentObj.GetComponent<NavMeshAgent>());
                                    }
                                    else
                                    {
                                        //play soldier what AS
                                        SoundManager.instance.SoldierWhat();

                                        currentObj.GetComponent<SelectedCircleController>().EnableSelectedCircle();
                                        agents.Add(currentObj.GetComponent<NavMeshAgent>());
                                    }
                                }

                            }
                            else
                            {
                                ClearAgents();
                                if (!currentObj.GetComponent<Health>().isDead && currentObj.GetComponent<NavMeshAgent>())
                                {
                                    if (TutorialManager.instance.index == 3 && !TutorialManager.instance.leftClickTutorial) {
                                        TutorialManager.instance.leftClick = true;
                                    }

                                    //play soldier what AS
                                    SoundManager.instance.SoldierWhat();

                                    currentObj.GetComponent<SelectedCircleController>().EnableSelectedCircle();
                                    agents.Add(currentObj.GetComponent<NavMeshAgent>());
                                }
                            }
                        }
                        else
                        {
                            ClearAgents();
                        }
                    }
                }
                else
                { //marquee select
                    verts = new Vector3[4];
                    int i = 0;
                    endMousePos = Input.mousePosition;
                    corners = GetBoundingBox(startMousePos, endMousePos);

                    foreach (Vector2 corner in corners)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(corner);
                        if (Physics.Raycast(ray, out screenGroundHit, 50000.0f, (1 << 8)))
                        {
                            verts[i] = new Vector3(screenGroundHit.point.x, 0, screenGroundHit.point.z);
                            Debug.DrawLine(firstCamera.ScreenToWorldPoint(corner), screenGroundHit.point, Color.red, 1.0f);
                        }
                        i++;
                    }

                    //generate the mesh
                    selectionMesh = GenerateSelectionMesh(verts);
                    selectionBox = gameObject.AddComponent<MeshCollider>();

                    selectionBox.sharedMesh = selectionMesh;
                    selectionBox.convex = true;
                    selectionBox.isTrigger = true;

                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        //TODO deSelectAll
                        ClearAgents();
                    }

                    Destroy(selectionBox, 0.02f);
                }//end marquee select

                dragSelect = false;
            }

            //mouse held down
            if (Input.GetMouseButton(0))
            {
                if ((startMousePos - Input.mousePosition).magnitude > 40)
                {
                    dragSelect = true;
                }
            }
        }
    }

    private void AssignDestiation(Vector3 destination)
    {
        if (agents.Count > 0) {
            List<Vector3> positions = new List<Vector3>();
            positions = GetPostions(destination, agents.Count);
            for (int i = 0; i < agents.Count; i++)
            {
                agents[i].transform.GetComponent<SoilderComtroller>().ClearAttackTarget();
                agents[i].destination = positions[i];
                agents[i].gameObject.GetComponent<SoilderComtroller>().destionation = positions[i];
            }
        }
    }

    private List<Vector3> GetPostions(Vector3 destination, int numbers)
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < numbers; i++)
        {
            if (i == 0)
            {
                positions.Add(destination);
            }
            else {
                if (i % 11 == 1) {
                    positions.Add(new Vector3(destination.x - 4, destination.y, destination.z -4));
                }
                else if(i % 11 == 2) {
                    positions.Add(new Vector3(destination.x, destination.y, destination.z - 4));
                }
                else if (i % 11 == 3)
                {
                    positions.Add(new Vector3(destination.x + 4, destination.y, destination.z - 4));
                }
                else if (i % 11 == 4)
                {
                    positions.Add(new Vector3(destination.x - 4, destination.y, destination.z));
                }
                else if (i % 11 == 5)
                {
                    positions.Add(new Vector3(destination.x + 4, destination.y, destination.z));
                }
                else if (i % 11 == 6)
                {
                    positions.Add(new Vector3(destination.x -4, destination.y, destination.z+4));
                }
                else if (i % 11 == 7)
                {
                    positions.Add(new Vector3(destination.x, destination.y, destination.z + 4));
                }
                else if (i % 11 == 8)
                {
                    positions.Add(new Vector3(destination.x + 4, destination.y, destination.z + 4));
                }
                else if (i % 11 == 9)
                {
                    positions.Add(new Vector3(destination.x - 4, destination.y, destination.z - 8));
                }
                else if (i % 11 == 10)
                {
                    positions.Add(new Vector3(destination.x, destination.y, destination.z - 8));
                }
                else if (i % 11 == 0)
                {
                    positions.Add(new Vector3(destination.x + 4, destination.y, destination.z - 8));
                }
            }
        }
        return positions;
    }

    private void OnGUI()
    {
        if (dragSelect) {
            var rect = Utils.GetScreenRect(startMousePos, Input.mousePosition);
            //Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            //Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));

            Utils.DrawScreenRect(rect, new Color(0f, 0.6277f, 0.9339f, 0.5960f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0f, 0.6277f, 0.9339f));
        }
    }

    public void StopAllAgents()
    {
        if (agents.Count > 0) {
            foreach (NavMeshAgent agent in agents)
            {
                agent.destination = agent.transform.position;
            }
        }
    }

    public void ClearAgents() {
        if (agents.Count > 0)
        {
            foreach (NavMeshAgent agent in agents)
            {
                agent.GetComponent<SelectedCircleController>().DisableSelectedCircle();
            }
        }
        agents.Clear();
    }

    //create a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] GetBoundingBox(Vector2 p1, Vector2 p2) {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (p1.x > p2.x)
        {// if p1 is to the left of p2
            if (p1.y > p2.y)//if p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else // if p1 is below p2 
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else { //if p1 is to the right of p2
            if (p1.y > p2.y)//if p1 is above p2
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else // if p1 is below p2 
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }
        }
        Vector2[] corners = { newP1, newP2, newP3, newP4 };
        return corners;

    }

    //Generate a mesh from the 4 botom points
    Mesh GenerateSelectionMesh(Vector3[] corners) {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 };//map the tris of our cube

        //bottom rectangle
        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        //top rect
        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + Vector3.up * 100.0f;
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        //agents.Add(other.gameObject);
        if (!other.GetComponent<Health>().isDead && other.CompareTag("Selectable")) {
            if (TutorialManager.instance.index == 6 && !TutorialManager.instance.selectAllTutorial)
            {
                TutorialManager.instance.selectAll = true;
            }
            NavMeshAgent agent = other.GetComponent<NavMeshAgent>();
            if (agent && !agents.Contains(agent)){
                //play soldier what AS
                SoundManager.instance.SoldierWhat();

                agent.GetComponent<SelectedCircleController>().EnableSelectedCircle();
                agents.Add(agent);
            }
        }
    }

    public void RecieveDieMessage(NavMeshAgent unit) {
        if (agents.Contains(unit)) {
            agents.Remove(unit);
            unit.GetComponent<SelectedCircleController>().DisableSelectedCircle();
        }
    }
}
