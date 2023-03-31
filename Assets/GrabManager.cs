using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GrabManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Slot currentSlot;

    public Sprite uiMask;

    private Canvas canvas;

    private GraphicRaycaster gRaycaster;

    public GameObject dragHandler;

    private Vector2 lastMousePosition;

    void Start()
    {
        uiMask = GetComponent<Image>().sprite;
    }

    void Update()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentSlot = this.transform.parent.GetComponent<Slot>();

        if (currentSlot.empty)
            return;
        lastMousePosition = transform.position;
        transform.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0) / transform.lossyScale.x;

        if (!canvas)
        {
            canvas = dragHandler.GetComponent<Canvas>();
            gRaycaster = canvas.GetComponent<GraphicRaycaster>();
        }
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (currentSlot.empty)
            return;

        Vector2 currentMousePosition = eventData.position;
        transform.position = currentMousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if (currentSlot.empty)
            return;

        var results = new List<RaycastResult>();
        gRaycaster.Raycast(eventData, results);
        foreach (var hit in results)
        {
            var slot = hit.gameObject.GetComponent<Slot>();
            if (slot)
            {
                if (slot.empty)
                {
                    var currentSlotID = currentSlot.slotId;
                    var newSlotID = slot.slotId;

                    var anotherItem = slot.transform.GetChild(0);
                    anotherItem.SetParent(currentSlot.transform);
                    anotherItem.transform.localPosition = Vector3.zero;

                    currentSlot.empty = true;

                    currentSlot = slot;
                    transform.SetParent(currentSlot.transform);
                    transform.localPosition = Vector3.zero;

                    currentSlot.empty = false;

                    break;
                }
            }
        }

        transform.SetParent(currentSlot.transform);
        transform.localPosition = Vector3.zero;
    }
}
