using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D point, doorway, attack, target, arrow;
    public Camera firstCam;
    Ray ray;
    RaycastHit hitInfo;

    public static CursorManager instance;

    [SerializeField]
    private GameObject _moveToPosCursor;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetCursorTexture();
    }

    public void DisplayMoveToCursor(Vector3 generatePosition) {
        GameObject movetoCursor = Instantiate(_moveToPosCursor, generatePosition, Quaternion.identity);
    }

    void SetCursorTexture()
    {
        ray = firstCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitInfo))
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;
                case "NPC":
                    Cursor.SetCursor(point, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }
        }
    }
}
