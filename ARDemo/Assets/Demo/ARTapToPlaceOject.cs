using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.XR;
using System;
using System.IO;

public class ARTapToPlaceOject : MonoBehaviour
{
    public GameObject placementIndicator;
    public GameObject prefabToPlace;
    
    public Camera arCamera;
    public ARRaycastManager aRRaycastManager;

    private GameObject spawnedObject;
    private Pose placementPose;
    private Vector2 touchPosition;
    private bool placementPoseIsValid = false;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public bool positionLocked = false;
    public bool rotationLocked = false;

    private float minScale = 0.3f;
    private float maxScale = 3f;

    private Touch oldTouch1;  //上次觸控點1(手指1)  
    private Touch oldTouch2;  //上次觸控點2(手指2)

    


    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    
    private void Update()
    {
        UpdatePlacementPose();

        TouchDetect();
        ObjectScaler();
        //UpdatePlacementIndicator();
        
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;
        //Debug.Log("touch");
        
        if (aRRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            SpawnObject();
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.All);

        placementPoseIsValid = hits.Count > 0;

        if (placementPoseIsValid && !positionLocked)
        {
            placementPose = hits[0].pose;
            var cameraForward = arCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;

            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private void SpawnObject()
    {
        var hitPose = hits[0].pose;
        if (spawnedObject == null)
        {
            spawnedObject = Instantiate(prefabToPlace, placementPose.position, placementPose.rotation);
        }
        else
        {
            if(!positionLocked) spawnedObject.transform.position = hitPose.position;
            if(!rotationLocked) spawnedObject.transform.rotation = placementPose.rotation;
        }
    }


    public static float quickDoubleTabInterval = 0.15f;
    private static float lastTouchTime;//上一次點擊放開的時間
    private static float begainTime = 0f;//最初點擊時間
    private static float intervals;//間隔時間
    public static float holdingTime = 3;//按住多久才會達到滿的狀態

    private static Vector2 startPos = Vector2.zero;//觸碰起始點
    private static Vector2 endPos = Vector2.zero;//觸碰結束點
    private static Vector2 direction = Vector2.zero;//移動方向

    private static Touch lastTouch;//目前沒用到，不果主要是記錄上一次的觸碰

    public static string debugInfo = "Nothing";

    private void TouchDetect()
    {
        if (Input.touchCount <= 0)
        {
            return;
        }
        if(Input.touchCount == 1)
        { 
            Touch touch = Input.GetTouch(0);
            bool isTouchUIElement = EventSystem.current.IsPointerOverGameObject(touch.fingerId);

            if (!isTouchUIElement)
            {
                switch (touch.phase)
                {
                    case TouchPhase.Began://點下去的狀態
                        startPos = touch.position;
                        begainTime = Time.realtimeSinceStartup;
                        QuickDoubleTab();
                        break;


                    case TouchPhase.Moved://手按住滑動的狀態
                        direction = touch.position - startPos;
                        intervals = Time.realtimeSinceStartup - begainTime;
                        Hold();
                        break;


                    case TouchPhase.Ended://手離開螢幕時的狀態
                        intervals = Time.realtimeSinceStartup - begainTime;
                        lastTouchTime = Time.realtimeSinceStartup;
                        endPos = startPos + direction;
                        lastTouch = touch;
                        Swipe(intervals, direction);
                        break;

                    case TouchPhase.Stationary://手按住不動的狀態
                        intervals = Time.realtimeSinceStartup - begainTime;
                        Hold();
                        break;

                }
            }
        }
    }
    //判斷雙擊事件是否成立，成立了以後要做什麼
    private void QuickDoubleTab()
    {
        if (Time.realtimeSinceStartup - lastTouchTime < quickDoubleTabInterval)
        {
            debugInfo = "touchCount";
            positionLocked = !positionLocked;
            Debug.Log("positionLocked trigger");
        }
    }
    //判斷快速滑動是否成立，成立了以後要做什麼
    private void Swipe(float intervalTime, Vector2 _direction)
    {
        if (intervalTime < 0.2f & _direction.magnitude > 120f)
        {
            debugInfo = "Swipe interval time : " + intervalTime + "Swipe direction : " + _direction;
        }
    }
    //判斷按住事件是否成立，成立了以後要做什麼
    private void Hold()
    {
        if (intervals > holdingTime)
        {
            debugInfo = "Hold MAX";
            rotationLocked = !rotationLocked;
            Debug.Log("rotationLocked trigger");
        }
        else if (intervals > 0.3f)
        {
            debugInfo = "Holding" + (intervals / holdingTime) * 100 + "%";
        }


    }
    
    
    private void ObjectScaler()
    {
        if (Input.touchCount <= 0)
        {
            return;
        }
        Touch newTouch1 = Input.GetTouch(0);
        Touch newTouch2 = newTouch1;

        if (Input.touchCount == 2)
        {
            newTouch2 = Input.GetTouch(1);
            //第2點剛開始接觸螢幕, 只記錄，不做處理  
            if (newTouch2.phase == TouchPhase.Began)
            {
                oldTouch2 = newTouch2;
                oldTouch1 = newTouch1;
                return;
            }

            //計算老的兩點距離和新的兩點間距離，變大要放大模型，變小要縮放模型  
            float oldDistance = Vector2.Distance(oldTouch1.position, oldTouch2.position);
            float newDistance = Vector2.Distance(newTouch1.position, newTouch2.position);

            //兩個距離之差，為正表示放大手勢， 為負表示縮小手勢  
            float offset = newDistance - oldDistance;

            //放大因子， 一個畫素按 0.01倍來算(100可調整)  
            float scaleFactor = offset / 100f;
            Vector3 localScale = spawnedObject.transform.localScale;
            Vector3 scale = new Vector3(localScale.x + scaleFactor,
                                        localScale.y + scaleFactor,
                                        localScale.z + scaleFactor);
            if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
                && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(1).fingerId))
            {
                //最小縮放到 0.3 倍 ，最大放大到 3 倍
                if (scale.x > minScale && scale.y > minScale && scale.z > minScale && scale.x < maxScale && scale.y < maxScale && scale.z < maxScale)
                {
                    spawnedObject.transform.localScale = scale;
                }
                //記住最新的觸控點，下次使用  
                oldTouch1 = newTouch1;
                oldTouch2 = newTouch2;
            }
        }
    }
}
