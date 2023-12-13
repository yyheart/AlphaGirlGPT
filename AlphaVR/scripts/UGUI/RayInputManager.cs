using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WorldCanvas
{
    private Plane plane;

    public WorldCanvas(Vector3 center, Vector3 normal)
    {
        plane = new Plane(normal, center);
    }

    public bool GetScreenPosition(Camera cam, Ray ray, out float distance, out Vector2 screenPosition)
    {
        screenPosition = Vector2.zero;
        plane.Raycast(ray, out distance);

        if (distance < 0) return false;
        Vector3 worldPosition = ray.GetPoint(distance);
        screenPosition = cam.WorldToScreenPoint(worldPosition);
        return true;

    }
    public float GetDistance( Ray ray)
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

public class ScreenRayPosition
{
    public Vector2 position;
    public int index;
    public string name;
    
    public ScreenRayPosition(Vector2 position, int index, string name)
    {
        this.position = position;
        this.index = index;
        this.name = name;
    }
}

public class NamedRay
{
    public Ray ray;
    public string name;
    public NamedRay(Ray ray, string name)
    {
        this.ray = ray;
        this.name = name;
    }
}


public class RayInputManager
{
    public static bool GetButtonDown(string button)
    {
        return (AlphaMotion.instance &&
            ((button == "Submit" && AlphaMotion.instance.GetButtonDown(0)) ||
            (button == "Cancel" && AlphaMotion.instance.GetButtonDown(1)))) ||
            Input.GetButtonDown(button);
    }

    public static float GetAxisRaw(string axis)
    {
        float value = 0f;
        if (AlphaMotion.instance)
        {
            if (axis == "Horizontal")
            {
                if (Mathf.Abs(AlphaMotion.instance.GetAnalog(0)) > Mathf.Abs(value))
                    value = AlphaMotion.instance.GetAnalog(0);
            }
            else if (axis == "Vertical")
            {
                if (Mathf.Abs(AlphaMotion.instance.GetAnalog(1)) > Mathf.Abs(value))
                    value = AlphaMotion.instance.GetAnalog(1);
            }
        }
        //  if (Mathf.Abs(Input.GetAxisRaw(axis)) > Mathf.Abs(value))
        //     value = Input.GetAxisRaw(axis);
        return value;
       // return 0;
    }

    private static List<NamedRay> rays = new List<NamedRay>();

    public static List<WorldCanvas> canvases = new List<WorldCanvas>();

    private static Camera cam;

    public delegate void RayDelegate(ref List<NamedRay> rays);
    public static event RayDelegate onFindRay;

    public delegate void ButtonDelegate(ref bool status, string name);
    public static event ButtonDelegate onFindPressed;
    public static event ButtonDelegate onFindReleased;

    public static List<ScreenRayPosition> GetPosition()
    {
        List<ScreenRayPosition> list = new List<ScreenRayPosition>();
        if (EventCam == null)
        {
            return list;
        }
        rays.Clear();
        distances.Clear();
        if (onFindRay != null) onFindRay(ref rays);
       
        for (int i = 0; i < rays.Count; i++)
        {
           
            for (int j = 0; j < canvases.Count; j++)
            {
                float distance = float.MaxValue;
                Vector2 position;
                if (canvases[j].GetScreenPosition(EventCam, rays[i].ray, out distance, out position))
                {
                    list.Add(new ScreenRayPosition(position, i, rays[i].name));
                    distances.Add(distance);
                  
                }
            }
        }
        return list;
    }

    private static List<float> distances = new List<float>();

    public static int rayIndex = -1;
    public static string rayName;

    public static float distance;
    //{
    //    get
    //    {
    //        if (rayIndex == -1 || distances.Count <= rayIndex)
    //            return 0f;
    //        else
    //        {
    //            return Mathf.Abs(distances[rayIndex]);
    //        }
    //    }
    //}

    public static Camera EventCam {
        get { return cam; }
        set {
            cam = value;
            Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
            for (int i = 0; i < canvases.Length; i++)
            {

                if (canvases[i].renderMode == RenderMode.WorldSpace)
                {
                    canvases[i].worldCamera = cam;
                }  
            }
            
        } }

    private static bool Pressed;
    private static bool Released;

    public static bool GetMouseButtonDown(int index)
    {
        if (onFindPressed != null) onFindPressed(ref Pressed, rayName);
        return Pressed;
    }

    public static bool GetMouseButtonUp(int index)
    {
        if (onFindReleased != null) onFindReleased(ref Released, rayName);
        return Released;
    }
}
