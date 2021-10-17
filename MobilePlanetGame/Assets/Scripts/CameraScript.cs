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
    protected Plane plane;

    private bool canMove = true;

    private int unlockedPlanet = 0;

    private float[,] boundary = { { 4, 2 }, { 4, 2 }, { 4, 2 }, { 4, 2 }};

    private void Start()
    {
        cam.orthographicSize = camStartingSize;
    }

    private void Update()
    {
        if (canMove)
        {
            //Update Plane
            if (Input.touchCount >= 1)
            {
                plane.SetNormalAndPosition(transform.up, transform.position);
            }

            Vector3 Delta1 = Vector3.zero;
            Vector3 Delta2 = Vector3.zero;

            //mouse controls

            if (Input.GetMouseButton(0))
            {
                Delta1 = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);
                Delta1 /= scrollSpeed;
                //MoveCamera(Delta1);
                transform.Translate(Delta1, Space.World);
            }

            if (Input.mouseScrollDelta.y != 0.0f)
            {
                SetCamZoom(cam.orthographicSize - Input.mouseScrollDelta.y);
            }


            //android controls

            //Scroll
            if (Input.touchCount >= 1)
            {
                Delta1 = PlanePositionDelta(Input.GetTouch(0));

                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    cam.transform.Translate(Delta1, Space.World);
                }
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

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        //not moved
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;

        //delta
        var rayBefore = cam.ScreenPointToRay(touch.position - touch.deltaPosition);
        var rayNow = cam.ScreenPointToRay(touch.position);
        if (plane.Raycast(rayBefore, out var enterBefore) && plane.Raycast(rayNow, out var enterNow))
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

        //not on plane
        return Vector3.zero;
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
    }
}
