using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //#if UNITY_IOS || UNITY_ANDROID
    [SerializeField] private Camera cam;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float camStartingSize;
    [SerializeField] private float camZoomMax;
    [SerializeField] private float camZoomMin;

    private Vector3 origin;
    private Vector3 difference;
    private Vector3 defaultPos;

    bool drag = false;

    protected Plane plane;

    private bool canMove = true;

    private int unlockedPlanet = 0;

    private float[,] boundary = { { 4.5f, 2.5f }, { 4, 2 }, { 4, 2 }, { 4, 2 }};

    private void Start()
    {
        cam.orthographicSize = camStartingSize;
        defaultPos = cam.transform.localPosition;
    }

    private void Update()
    {
        if (canMove)
        {
            //mouse controls

            if (Input.GetMouseButton(0))
            {
                difference = cam.ScreenToWorldPoint(Input.mousePosition) - cam.transform.position;

                if(!drag)
                {
                    drag = true;
                    origin = cam.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            else
            {
                drag = false;
            }
            if (Input.mouseScrollDelta.y != 0.0f)
            {
                SetCamZoom(cam.orthographicSize - Input.mouseScrollDelta.y);
            }


            //android controls

            //Scroll
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    difference = cam.ScreenToWorldPoint(Input.GetTouch(0).position) - cam.transform.position;

                    if (!drag)
                    {
                        drag = true;
                        origin = cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                    }
                }
                else
                {
                    drag = false;
                }

                /*Delta1 = PlanePositionDelta(Input.GetTouch(0));

                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    cam.transform.Translate(Delta1, Space.World);
                }*/
            }

            if (drag)
            {
                cam.transform.position = origin - difference;
                cam.transform.position = new Vector3(
                    Mathf.Clamp(cam.transform.position.x, -boundary[unlockedPlanet, 0], boundary[unlockedPlanet, 0]),
                    Mathf.Clamp(cam.transform.position.y, -boundary[unlockedPlanet, 1], boundary[unlockedPlanet, 1]),
                    -10);
            }


            //Pinch
            if (Input.touchCount >= 2)
            {
                var pos1 = PlanePosition(Input.GetTouch(0).position);
                var pos2 = PlanePosition(Input.GetTouch(1).position);
                var pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
                var pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

                //calc zoom
                var zoom = Vector3.Distance(pos1, pos2) /
                           Vector3.Distance(pos1b, pos2b);

                //edge case
                if (zoom == 0 || zoom > 10)
                {
                    return;
                }

                //Move cam amount the mid ray
                cam.transform.position = Vector3.LerpUnclamped(pos1, cam.transform.position, 1 / zoom);
            }
        }
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        //position
        var rayNow = cam.ScreenPointToRay(screenPos);
        if (plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }

    public void MoveCamera(Vector3 _pos)
    {
        /*transform.position = new Vector3(
            Mathf.Clamp(_pos.x, -boundary[unlockedPlanet, 0], boundary[unlockedPlanet, 0]),
            Mathf.Clamp(_pos.y, -boundary[unlockedPlanet, 1], boundary[unlockedPlanet, 1]),
            -10);*/
        transform.position = new Vector3(_pos.x, _pos.y,  -10);
    }

    public void SetUnlockedPlanet(int _i)
    {
        unlockedPlanet = _i;
    }

    public float GetStartingZoom()
    {
        return camStartingSize;
    }

    public void SetCamZoom(float _zoom)
    {
        cam.orthographicSize = Mathf.Clamp(_zoom, camZoomMin, camZoomMax);
    }

    public void SetCanMove(bool _move)
    {
        canMove = _move;

        cam.transform.localPosition = defaultPos;
    }
}
