using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class RayInputModule : PointerInputModule
{
    private float m_PrevActionTime;
    private int m_ConsecutiveMoveCount = 0;
    private Vector2 m_LastMoveVector;
    public static GameObject hoveredObj;

    [SerializeField]
    private string m_HorizontalAxis = "Horizontal";

    /// <summary>
    /// Name of the vertical axis for movement (if axis events are used).
    /// </summary>
    [SerializeField]
    private string m_VerticalAxis = "Vertical";

    /// <summary>
    /// Name of the submit button.
    /// </summary>
    [SerializeField]
    private string m_SubmitButton = "Submit";

    /// <summary>
    /// Name of the submit button.
    /// </summary>
    [SerializeField]
    private string m_CancelButton = "Cancel";
    [SerializeField]
    private float m_InputActionsPerSecond = 10;
    [SerializeField]
    private float m_RepeatDelay = 0.5f;

    public override void Process()
    {
        bool usedEvent = SendUpdateEventToSelectedObject();
        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            //if (!usedEvent)
            //    SendSubmitEventToSelectedObject();
        }
        if (!ProcessTouchEvents() && input.mousePresent)
            ProcessMouseEvent();
    }

    public override void ActivateModule()
    {
        base.ActivateModule();

        var toSelect = eventSystem.currentSelectedGameObject;
        if (toSelect == null)
            toSelect = eventSystem.firstSelectedGameObject;

        eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();
        ClearSelection();
    }

    private bool ProcessTouchEvents()
    {
        for (int i = 0; i < input.touchCount; ++i)
        {
            Touch touch = input.GetTouch(i);

            if (touch.type == TouchType.Indirect)
                continue;

            bool released;
            bool pressed;
            var pointer = GetTouchPointerEventData(touch, out pressed, out released);
            if (i == 0 && pointer.pointerCurrentRaycast.gameObject)
                hoveredObj = pointer.pointerCurrentRaycast.gameObject;
            ProcessTouchPress(pointer, pressed, released);

            if (!released)
            {
                ProcessMove(pointer);
                ProcessDrag(pointer);
            }
            else
                RemovePointerData(pointer);
        }
        return input.touchCount > 0;
    }

    protected void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
    {
        var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

        // PointerDown notification
        if (pressed)
        {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, pointerEvent);

            if (pointerEvent.pointerEnter != currentOverGo)
            {
                // send a pointer enter to the touched element if it isn't the one to select...
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
                pointerEvent.pointerEnter = currentOverGo;
            }

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // Debug.Log("Pressed: " + newPressed);

            float time = Time.unscaledTime;

            if (newPressed == pointerEvent.lastPress)
            {
                var diffTime = time - pointerEvent.clickTime;
                if (diffTime < 0.3f)
                    ++pointerEvent.clickCount;
                else
                    pointerEvent.clickCount = 1;

                pointerEvent.clickTime = time;
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            pointerEvent.pointerPress = newPressed;
            pointerEvent.rawPointerPress = currentOverGo;

            pointerEvent.clickTime = time;

            // Save the drag handler as well
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
        }

        // PointerUp notification
        if (released)
        {
            // Debug.Log("Executing pressup on: " + pointer.pointerPress);
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.pointerDrag = null;

            // send exit events as we need to simulate this on touch up on touch device
            ExecuteEvents.ExecuteHierarchy(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
            pointerEvent.pointerEnter = null;
        }
    }

    protected bool SendUpdateEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.updateSelectedHandler);
        return data.used;
    }

    public override bool IsModuleSupported()
    {
        return input.mousePresent || input.touchSupported;
    }

    public override bool ShouldActivateModule()
    {
        bool shouldActivate = base.ShouldActivateModule();
        if (input.touchCount > 0)
            shouldActivate = true;
        return shouldActivate;
    }

    protected void ProcessMouseEvent()
    {
        ProcessMouseEvent(0);
    }

    /// <summary>
    /// Process all mouse events.
    /// </summary>
    protected void ProcessMouseEvent(int id)
    {
        var mouseData = GetMousePointerEventData(id);
        var leftButtonData = mouseData.GetButtonState(PointerEventData.InputButton.Left).eventData;

        // Process the first mouse button fully
        ProcessMousePress(leftButtonData);
        ProcessMove(leftButtonData.buttonData);
        ProcessDrag(leftButtonData.buttonData);

        if (!Mathf.Approximately(leftButtonData.buttonData.scrollDelta.sqrMagnitude, 0.0f))
        {
            var scrollHandler = ExecuteEvents.GetEventHandler<IScrollHandler>(leftButtonData.buttonData.pointerCurrentRaycast.gameObject);
            ExecuteEvents.ExecuteHierarchy(scrollHandler, leftButtonData.buttonData, ExecuteEvents.scrollHandler);
        }
    }

    protected override MouseState GetMousePointerEventData(int id)
    {
        MouseState state = new MouseState();
        PointerEventData data;
        GetPointerData(kMouseLeftId, out data, true);
        data.Reset();
        List<ScreenRayPosition> positions = RayInputManager.GetPosition();
        if (positions == null) return state;
        RaycastResult[] results = new RaycastResult[positions.Count];
        data.button = PointerEventData.InputButton.Left;
        Vector2 lastPosition = data.position;
        for (int i = 0; i < positions.Count; i++)
        {
            m_RaycastResultCache.Clear();
            data.position = positions[i].position;
            data.delta = positions[i].position - lastPosition;
            eventSystem.RaycastAll(data, m_RaycastResultCache);
            results[i] = FindFirstRaycast(m_RaycastResultCache);
           
        }
        var raycast = FindFirstRaycast(new List<RaycastResult>(results));
        if (raycast.gameObject == null) RayInputManager.rayIndex = -1;
        else
        {
            for (int i = 0; i < results.Length; i++)
            {
                if (raycast.gameObject == results[i].gameObject)
                {
                  
                    RayInputManager.rayIndex = positions[i].index;
                    RayInputManager.rayName = positions[i].name;
                  
                    break;
                }
            }
        }
        data.pointerCurrentRaycast = raycast;
        hoveredObj = raycast.gameObject;
       // RayInputManager.distance = raycast.distance;


        m_RaycastResultCache.Clear();
        state.SetButtonState(PointerEventData.InputButton.Left, GetState(0), data);
        return state;
    }
    
    protected PointerEventData.FramePressState GetState(int buttonId)
    {
        var pressed = RayInputManager.GetMouseButtonDown(buttonId);
        var released = RayInputManager.GetMouseButtonUp(buttonId);
        if (pressed && released)
            return PointerEventData.FramePressState.PressedAndReleased;
        if (pressed)
            return PointerEventData.FramePressState.Pressed;
        if (released)
            return PointerEventData.FramePressState.Released;
        return PointerEventData.FramePressState.NotChanged;
    }

    public bool TouchOverUI()
    {
        if (input.touchCount > 0)
        {
            PointerEventData data = GetMousePointerEventData(0).GetButtonState(PointerEventData.InputButton.Left).eventData.buttonData;
            List<RaycastResult> results = new List<RaycastResult>();
            eventSystem.RaycastAll(data, results);
            return results.Count > 0;
        }
        return false;
    }

    /// <summary>
    /// keyboard Event
    /// </summary>
    /// <returns></returns>
    protected bool SendMoveEventToSelectedObject()
    {
        float time = Time.unscaledTime;

        Vector2 movement = GetRawMoveVector();
        if (Mathf.Approximately(movement.x, 0f) && Mathf.Approximately(movement.y, 0f))
        {
            m_ConsecutiveMoveCount = 0;
            return false;
        }

        // If user pressed key again, always allow event
        bool allow = input.GetButtonDown(m_HorizontalAxis) || input.GetButtonDown(m_VerticalAxis);
        bool similarDir = (Vector2.Dot(movement, m_LastMoveVector) > 0);
        if (!allow)
        {
            // Otherwise, user held down key or axis.
            // If direction didn't change at least 90 degrees, wait for delay before allowing consequtive event.
            if (similarDir && m_ConsecutiveMoveCount == 1)
                allow = (time > m_PrevActionTime + m_RepeatDelay);
            // If direction changed at least 90 degree, or we already had the delay, repeat at repeat rate.
            else
                allow = (time > m_PrevActionTime + 1f / m_InputActionsPerSecond);
        }
        if (!allow)
            return false;

        // Debug.Log(m_ProcessingEvent.rawType + " axis:" + m_AllowAxisEvents + " value:" + "(" + x + "," + y + ")");
        var axisEventData = GetAxisEventData(movement.x, movement.y, 0.6f);

        if (axisEventData.moveDir != MoveDirection.None)
        {
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
            if (!similarDir)
                m_ConsecutiveMoveCount = 0;
            m_ConsecutiveMoveCount++;
            m_PrevActionTime = time;
            m_LastMoveVector = movement;
        }
        else
        {
            m_ConsecutiveMoveCount = 0;
        }

        return axisEventData.used;
    }

    /// <summary>
    /// Process the current mouse press.
    /// </summary>
    protected void ProcessMousePress(MouseButtonEventData data)
    {
        var pointerEvent = data.buttonData;
        var currentOverGo = pointerEvent.pointerCurrentRaycast.gameObject;

        // PointerDown notification
        if (data.PressedThisFrame())
        {
            pointerEvent.eligibleForClick = true;
            pointerEvent.delta = Vector2.zero;
            pointerEvent.dragging = false;
            pointerEvent.useDragThreshold = true;
            pointerEvent.pressPosition = pointerEvent.position;
            pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;

            DeselectIfSelectionChanged(currentOverGo, pointerEvent);

            // search for the control that will receive the press
            // if we can't find a press handler set the press
            // handler to be what would receive a click.
            var newPressed = ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.pointerDownHandler);

            // didnt find a press handler... search for a click handler
            if (newPressed == null)
                newPressed = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // Debug.Log("Pressed: " + newPressed);

            float time = Time.unscaledTime;

            if (newPressed == pointerEvent.lastPress)
            {
                var diffTime = time - pointerEvent.clickTime;
                if (diffTime < 0.3f)
                    ++pointerEvent.clickCount;
                else
                    pointerEvent.clickCount = 1;

                pointerEvent.clickTime = time;
            }
            else
            {
                pointerEvent.clickCount = 1;
            }

            pointerEvent.pointerPress = newPressed;
            pointerEvent.rawPointerPress = currentOverGo;

            pointerEvent.clickTime = time;

            // Save the drag handler as well
            pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(currentOverGo);

            if (pointerEvent.pointerDrag != null)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
        }

        // PointerUp notification
        if (data.ReleasedThisFrame())
        {
            // Debug.Log("Executing pressup on: " + pointer.pointerPress);
            ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);

            // Debug.Log("KeyCode: " + pointer.eventData.keyCode);

            // see if we mouse up on the same element that we clicked on...
            var pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentOverGo);

            // PointerClick and Drop events
            if (pointerEvent.pointerPress == pointerUpHandler && pointerEvent.eligibleForClick)
            {
                ExecuteEvents.Execute(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
            }
            else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
            {
                ExecuteEvents.ExecuteHierarchy(currentOverGo, pointerEvent, ExecuteEvents.dropHandler);
            }

            pointerEvent.eligibleForClick = false;
            pointerEvent.pointerPress = null;
            pointerEvent.rawPointerPress = null;

            if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
                ExecuteEvents.Execute(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);

            pointerEvent.dragging = false;
            pointerEvent.pointerDrag = null;

            // redo pointer enter / exit to refresh state
            // so that if we moused over somethign that ignored it before
            // due to having pressed on something else
            // it now gets it.
            if (currentOverGo != pointerEvent.pointerEnter)
            {
                HandlePointerExitAndEnter(pointerEvent, null);
                HandlePointerExitAndEnter(pointerEvent, currentOverGo);
            }
        }
    }

  
    void OnMyClick()
    {

    }

    private Vector2 GetRawMoveVector()
    {
        Vector2 move = Vector2.zero;
        move.x = RayInputManager.GetAxisRaw(m_HorizontalAxis);
        move.y = RayInputManager.GetAxisRaw(m_VerticalAxis);

        if (input.GetButtonDown(m_HorizontalAxis))
        {
            if (move.x < 0)
                move.x = -1f;
            if (move.x > 0)
                move.x = 1f;
        }
        if (input.GetButtonDown(m_VerticalAxis))
        {
            if (move.y < 0)
                move.y = -1f;
            if (move.y > 0)
                move.y = 1f;
        }
        return move;
    }

    /// <summary>
    /// Process submit keys.
    /// </summary>
    protected bool SendSubmitEventToSelectedObject()
    {
        if (eventSystem.currentSelectedGameObject == null)
            return false;

        var data = GetBaseEventData();
        if (RayInputManager.GetButtonDown(m_SubmitButton))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);

        if (RayInputManager.GetButtonDown(m_CancelButton))
            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
        return data.used;
    }
}
