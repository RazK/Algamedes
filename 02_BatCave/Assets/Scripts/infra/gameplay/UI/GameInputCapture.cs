using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace Infra.Gameplay.UI {
/// <summary>
/// Usage: Attach this to a panel at the top of the main Canvas so that UI
/// events will be handled before they are passed to the game.
/// Add the symbol CAPTURE_DRAG in player settings to capture drag input events.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class GameInputCapture : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
#if CAPTURE_DRAG
        , IDragHandler
#endif
{
    // To get touch events implement the delegate and register the listener to
    // the appropriate event.
    // e.g: GameInputCapture.OnTouchDown += this.OnTouchDown;

    public static event Action<PointerEventData> OnTouchDown;
    public static event Action<PointerEventData> OnTouchUp;
#if CAPTURE_DRAG
    public static event Action<PointerEventData> OnTouchDrag;
#endif

    public void OnPointerDown(PointerEventData e) {
        if (OnTouchDown != null) {
            OnTouchDown(e);
        }
    }

    public void OnPointerUp(PointerEventData e) {
        if (OnTouchUp != null) {
            OnTouchUp(e);
        }
    }

#if CAPTURE_DRAG
    public void OnDrag(PointerEventData e) {
        if (OnTouchDrag != null) {
            OnTouchDrag(e);
        }
    }
#endif

    protected void Reset() {
        name = GetType().ToString();
    }
}
}
