using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PAUI_Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    #region Classes
    [System.Serializable]
    public class PA_ValueEvent : UnityEvent<float> { };
    [System.Serializable]
    public class PA_Vector2Event : UnityEvent<Vector2> { };
    [System.Serializable]
    public class PA_StandardEvent : UnityEvent { };
    #endregion

    public PA_ValueEvent joystickTouchX;
    public PA_ValueEvent joystickTouchY;

    [HideInInspector]
    public Image joystick;
    [HideInInspector]
    public Image joystickHandle;
    [HideInInspector]
    public Vector2 outputVector;

    private void Start()
    {
        joystick = GetComponent<Image>();
        joystickHandle = transform.GetChild(0).GetComponent<Image>();
    }

    public virtual void OnPointerDown(PointerEventData pointerED)
    {
        OnDrag(pointerED);
    }

    public virtual void OnDrag(PointerEventData pointerED)
    {
        Vector2 touchPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(joystick.rectTransform, pointerED.position, pointerED.pressEventCamera, out touchPos))
        {
            touchPos.x = (touchPos.x / joystick.rectTransform.sizeDelta.x);
            touchPos.y = (touchPos.y / joystick.rectTransform.sizeDelta.y);

            outputVector = new Vector2(touchPos.x * 2f - 1, touchPos.y * 2f - 1);
            outputVector = (outputVector.magnitude > 1.0f) ? outputVector.normalized : outputVector;
            MoveJoystickHandle(outputVector);
            joystickTouchX.Invoke(outputVector.x);
            joystickTouchY.Invoke(outputVector.y); 
        }
    }

    public virtual void OnPointerUp(PointerEventData pointerED)
    {
        outputVector = Vector2.zero;
        MoveJoystickHandle(outputVector);
        joystickTouchX.Invoke(outputVector.x);
        joystickTouchY.Invoke(outputVector.y);
    }

    public void MoveJoystickHandle(Vector2 inputVector)
    {
        joystickHandle.rectTransform.anchoredPosition = new Vector3(inputVector.x * (joystick.rectTransform.sizeDelta.x / 4.5f), inputVector.y * (joystick.rectTransform.sizeDelta.y / 4.5f));
    }
}

