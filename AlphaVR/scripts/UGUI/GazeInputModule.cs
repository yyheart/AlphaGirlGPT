//------------------------------------------------------------------------------
// Copyright 2016 Baofeng Mojing Inc. All rights reserved.
//------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class CanvasPlane
{
    private Plane plane;

    public CanvasPlane(Vector3 center, Vector3 normal)
    {
        plane = new Plane(normal, center);
    }

    public bool GetScreenPosition(Camera cam, Ray ray, out float distance, out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;
        if (plane.Raycast(ray, out distance))
        {
            if (distance < 0) return false;
            Vector3 worldPosition = ray.GetPoint(distance);
            screenPosition = cam.WorldToScreenPoint(worldPosition);
            return true;
        }
        else
        {
            return false;
        }





    }
    public float GetDistance(Ray ray)
    {
        float distance = 0;
        plane.Raycast(ray, out distance);
        return distance;
    }

    public bool GetScreenViewport(Camera cam, Ray ray, out float distance, out Vector2 screenViewport)
    {
        screenViewport = Vector2.zero;
        plane.Raycast(ray, out distance);
        if (distance < 0) return false;
        Vector3 worldPosition = ray.GetPoint(distance);
        screenViewport = cam.WorldToViewportPoint(worldPosition);
        return true;
    }

    public void SetPlane(Vector3 center, Vector3 normal)
    {
        plane.SetNormalAndPosition(normal, center);
    }
}


// An implementation of the BaseInputModule that uses the player's gaze and the magnet trigger
// as a raycast generator.  To use, attach to the scene's EventSystem object.  Set the Canvas
// object's Render Mode to World Space, and set its Event Camera to a (mono) camera that is
// controlled by a MojingVrHead.  If you'd like gaze to work with 3D scene objects, add a
// PhysicsRaycaster to the gazing camera, and add a component that implements one of the Event
// interfaces (EventTrigger will work nicely).  The objects must have colliders too.
public class GazeInputModule : BaseInputModule
{
   // [Tooltip("Whether gaze input is active in VR Mode only (true), or all the time (false).")]
     //bool vrModeOnly = false;

    //[Tooltip("Optional object to place at raycast intersections as a 3D cursor. " +"Be sure it is on a layer that raycasts will ignore.")]
    //public GameObject cursor;

    // Time in seconds between the pointer down and up events sent by a magnet click.
    // Allows time for the UI elements to make their state transitions.
    [HideInInspector]
    public float clickTime = 0.1f;  // Based on default time for a button to animate to Pressed.

    // The pixel through which to cast rays, in viewport coordinates.  Generally, the center
    // pixel is best, assuming a monoscopic camera is selected as the Canvas' event camera.
    [HideInInspector]
    public Vector2 hotspot = new Vector2(0.5f, 0.5f);

    private PointerEventData pointerData = null;

    public event Action OnTrigger = null;

    // Transform line;
    // Transform sphere;
    //Vector3 lineScale;
    GameObject lastTarget = null;

    public delegate void InteractTraget(GameObject go);
    public static event InteractTraget OnEnterTarget;
    public static event InteractTraget OnExitTarget;
    [HideInInspector]
    public Camera theEventCamera;

    public Color HoverColor = Color.green;
    
    float originalScale = 0;
   // float originalLength = 0;
    Color originalColor = Color.white;
    Renderer lineRender;

    protected override void Start()
    {
        base.Start();
      //  if (theEventCamera==null)
       // {
            theEventCamera = transform.parent.GetComponent<Camera>();
       // }
        //line = transform.parent.parent.Find("Line");
        //sphere = transform.parent.parent.Find("Sphere");
        //if (line != null)
        //{
        //    originalScale = line.localScale.x;
        //   // originalLength = line.localScale.z;
        //    lineRender = line.GetComponent<Renderer>();
        //    originalColor = lineRender.material.color;
        //}
        //NoloController.Instance.OnRightTriggerPressed += () =>
        //{
        //    touch_begin = true;
        //};
        //NoloController.Instance.OnRightTriggerReleased += () =>
        //{
        //    touch_end = true;
        //};
        Canvas[] canvases =Resources.FindObjectsOfTypeAll<Canvas>();
        for (int i = 0; i < canvases.Length; i++)
        {
            if (canvases[i].renderMode == RenderMode.WorldSpace)
            {
              //  canvases[i].worldCamera = transform.parent.GetComponent<Camera>();
                canvases[i].worldCamera = theEventCamera;

               // canvases[i].gameObject.AddComponent<World3DUGUI>();
            }
        }
        OnEnterTarget += (go) =>
        {
          //  lineScale.Set(0.01f, 0.01f, hitInfo.distance);
            //if (line != null)
            //{
            //   // line.localScale = lineScale;
            //    SetMaterialColor(lineRender, HoverColor);
            //}
            //if (sphere != null)
            //{
            //    sphere.position = hitInfo.point;
            //}

        };
        OnExitTarget += (go) =>
        {
            //lineScale.Set(0.01f, 0.01f, 5);
            //if (line != null)
            //{
            //    line.localScale = lineScale;
            //    SetMaterialColor(lineRender, originalColor);
            //    if (sphere != null)
            //    {
            //        sphere.position = line.position + line.forward * (lineScale.z+ sphere.localScale.x);
            //    }
            //}
           

        };
    }
    private void DispatchTrigger()
    {
        if (OnTrigger != null)
        {
            OnTrigger();
        }
    }

    public override bool ShouldActivateModule()
    {
        if (!base.ShouldActivateModule())
        {
            return false;
        }
        //return Mojing.SDK.VRModeEnabled || !vrModeOnly;
        return true;
    }

    //private Vector2 position;
    public override void  ActivateModule()
    {
        if (pointerData == null)
        {
            pointerData = new PointerEventData(eventSystem);
        }
        //position = new Vector2(hotspot.x * Screen.width, hotspot.y * Screen.height);
       
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();

        if (pointerData != null)
        {
            HandleClick();
            HandlePointerExitAndEnter(pointerData, null);
            pointerData = null;
        }

        eventSystem.SetSelectedGameObject(null, GetBaseEventData());
        //if (cursor != null)
        //{
        //    cursor.SetActive(false);
        // }
       
    }

    public override bool IsPointerOverGameObject(int pointerId)
    {
        return pointerData != null && pointerData.pointerEnter != null;
    }

    private bool Triggered { get; set; }

    bool touch_end = false;
    protected virtual bool IsKeyUp()
    {
		bool temp_touch_end=false;
#if UNITY_IOS && !UNITY_EDITOR
		if (Input.touchCount == 1) {
			if (Input.GetTouch (0).phase == TouchPhase.Ended) {
				touch_end = true;	
			}
		}
		return touch_end;
#else
        //if(Input.GetMouseButtonUp(0)){
        //	touch_end = true;
        //}
        //if (NoloController.Instance.GetKeyUp(NoloClientCSharp.EControlerButtonType.eTriggerBtn))
        //{
        //    touch_end = true;
        //}
        // temp_touch_end = touch_end;
        temp_touch_end = AlphaMotion.instance.GetButtonUp(0);
        return temp_touch_end;
#endif
    }
    bool touch_begin = false;
    protected virtual bool IsKeyDown()
    {
	  bool	temp_touch_begin=false;
#if UNITY_IOS && !UNITY_EDITOR
		if (Input.touchCount == 1) {
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				touch_begin = true;	
			}
		}
		return touch_begin;
#else
        //      if (Input.GetMouseButtonDown(0)){
        //	touch_begin = true;
        //}
        //if (NoloController.Instance.GetKeyDown(NoloClientCSharp.EControlerButtonType.eTriggerBtn))
        //{
        //    touch_begin = true;
        //}
        // temp_touch_begin = touch_begin;
        temp_touch_begin = AlphaMotion.instance.GetButtonDown(0);
        return temp_touch_begin;
#endif
    }

    private GameObject lastGameObject = null;
    public override void Process()
    {
        try
        {
            pointerData.Reset();

            // Find the gameObject which is in the ray of view
            pointerData.position = new Vector2(hotspot.x * Screen.width, hotspot.y * Screen.height);//position;
            eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
            pointerData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
            m_RaycastResultCache.Clear();
            GameObject go = pointerData.pointerCurrentRaycast.gameObject;

            // just do update work if the game object changed.
            if (go != lastGameObject)
            {
                // Send enter events and update the highlight.
                HandlePointerExitAndEnter(pointerData, go);
                // Update the current selection, or clear if it is no longer the current object.
                var selected = ExecuteEvents.GetEventHandler<ISelectHandler>(go);
                if (selected == eventSystem.currentSelectedGameObject)
                {
                    ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
                }
                else
                {
                    eventSystem.SetSelectedGameObject(null, pointerData);
                }
            }

            PlaceCursor();
            HandleClick();

            lastGameObject = go;
        }
        catch (Exception e)
        {
            print(e.Message);
            //MojingLog.LogError(e.ToString());
        }
    }
    
	private float distance_default=1.5f;
    private void PlaceCursor()
    {

        //if (sphere == null)
        //{
        //    return;
        //}
        var go = pointerData.pointerCurrentRaycast.gameObject;

        //cursor.SetActive(go != null);
       // cursor.SetActive(true);
       
            Camera cam = pointerData.enterEventCamera;
       
        if (cam != null)
            {   // Note: rays through screen start at near clipping plane.
            float dist = cam.nearClipPlane;

            if (go != null)
				{//collider
                    dist += pointerData.pointerCurrentRaycast.distance ;
                  //  dist = dist / Mathf.Abs(Mathf.Cos(vrHead.transform.rotation.eulerAngles.y * Mathf.PI / 180));
					distance_default = dist;
				}
                else
				{//default
                    //dist += 5;
					dist = 5;
				}
            //    sphere.position = cam.transform.position + cam.transform.forward * dist ;
            ////sphere.position = transform.position + transform.forward * dist;

            ////Debug.Log (dist.ToString ());
            //lineScale.Set(originalScale, originalScale, dist);
            //if (line != null)
            //{
            //    line.localScale = lineScale;

            //}
        }

    }
    
    private void HandleClick()
    {
        if (IsKeyDown())
        {
           // touch_begin = false;
            var go = pointerData.pointerCurrentRaycast.gameObject;

            if (go != null)
            {
                // Send pointer down event.
                pointerData.pressPosition = pointerData.position;
                pointerData.pointerPressRaycast = pointerData.pointerCurrentRaycast;
                pointerData.pointerPress = ExecuteEvents.ExecuteHierarchy(go, pointerData, ExecuteEvents.pointerDownHandler) ?? ExecuteEvents.GetEventHandler<IPointerClickHandler>(go);

                // Save the pending click state.
                pointerData.rawPointerPress = go;
                pointerData.eligibleForClick = true;
                pointerData.clickCount = 1;
                pointerData.clickTime = Time.unscaledTime;
            }
        }

        if (IsKeyUp())
        {
           // touch_end = false;
            if (!pointerData.eligibleForClick && (Time.unscaledTime - pointerData.clickTime < clickTime))
            {
                return;
            }

            // Send pointer up and click events.
            ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(pointerData.pointerPress, pointerData, ExecuteEvents.pointerClickHandler);

            DispatchTrigger();

           // Clear the click state.
            pointerData.pointerPress = null;
            pointerData.rawPointerPress = null;
            pointerData.eligibleForClick = false;
            pointerData.clickCount = 0;
        }
    }

    private void Update()
    {
        if (pointerData!=null && pointerData.pointerEnter != lastTarget)
        {
            if (lastTarget == null)
            {
                //进入UI
                //进入
                if (OnEnterTarget!=null)
                {

                    OnEnterTarget(pointerData.pointerEnter);
                }
               // OnEnterTarget?.Invoke(pointerData.pointerEnter);
            }
            else if (pointerData.pointerEnter == null)
            {
                //退出UI
                if (OnExitTarget != null)
                {

                    OnExitTarget(lastTarget);
                }
               // OnExitTarget?.Invoke(lastTarget);
            }
            else
            {
               
                //退出UI
                if (OnExitTarget != null)
                {

                    OnExitTarget(lastTarget);
                }
                if (OnEnterTarget != null)
                {

                    OnEnterTarget(pointerData.pointerEnter);
                }
                //先退出在进入
                // OnEnterTarget?.Invoke(pointerData.pointerEnter);
                //sOnExitTarget?.Invoke(lastTarget);
            }
            lastTarget = pointerData.pointerEnter;
        }
    }
    public void SetMaterialColor(Renderer go, Color c)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        go.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", c);
        // throw new NotImplementedException();
        go.SetPropertyBlock(mpb);
    }
}
