using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Transform start;
    public RectTransform activeSlot;
    public Board game;
    public UIScript ui;
    public AudioSource cardSound;

    public void OnDrag(PointerEventData eventData)
    {
        transform.parent.gameObject.transform.SetAsLastSibling();
        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        transform.localScale = new Vector3(3, 3, 3);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1, 1, 1);
        //RectTransform activeSlot = transform as RectTransform;
        Debug.Log("dropped");
        Debug.Log(eventData.selectedObject);
        if (RectTransformUtility.RectangleContainsScreenPoint(activeSlot, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane))))
        {
            // Change 0 later
            if (game.canAfford(game.players[game.getActivePlayer()].getHand()[int.Parse(eventData.selectedObject.transform.name)], game.getActivePlayer()))
            {
                // go ahead and add to active cards
                game.addToActiveCard(game.players[game.getActivePlayer()].getHand()[int.Parse(eventData.selectedObject.transform.name)], game.getActivePlayer(), int.Parse(eventData.selectedObject.transform.name));
                eventData.selectedObject.transform.SetParent(activeSlot);/*
                ((RectTransform)transform).anchorMax = new Vector2(0.5f, 0.5f);
                ((RectTransform)transform).anchorMin = new Vector2(0.5f, 0.5f);
                transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));*/
                cardSound.Play();
            } else
            {

            }
            

        }

        ui.displayHand(game.players[game.getActivePlayer()].getHand());
        ui.displayActive(game.getActiveCards(game.getActivePlayer()));
    }

    public void placeStartingPos()
    {
        transform.position = start.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        start = transform;
        game = FindObjectOfType<Board>();
        ui = FindObjectOfType<UIScript>();
        //activeSlot = game.gameObject.GetComponentInChildren(typeof(RectTransform));
        Transform tempCanvas = (Transform)(game.gameObject.GetComponentInChildren(typeof(Transform)));
        activeSlot = (RectTransform)(tempCanvas.GetChild(1));
        cardSound = GameObject.Find("CardPlayedMusic").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
