using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDropHandler : MonoBehaviour, IDropHandler
{
    public Board game;
    public CardDragHandler drag;
    public UIScript ui;

    public void OnDrop(PointerEventData eventData)
    {
        /*
        RectTransform activeSlot = transform as RectTransform;

        if (RectTransformUtility.RectangleContainsScreenPoint(activeSlot, Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane))))
        {
            // Change 0 later
            if (game.canAfford(game.players[(int)Constants.player.magician1].getHand()[int.Parse(eventData.selectedObject.transform.parent.name)], 0))
            {
                // go ahead and add to active cards
                game.addToActiveCard(game.players[(int)Constants.player.magician1].getHand()[int.Parse(eventData.selectedObject.transform.parent.name)], (int)Constants.player.magician1, int.Parse(eventData.selectedObject.transform.parent.name));
                eventData.selectedObject.transform.SetParent(transform);
                
            }
            ui.displayHand(game.players[(int)Constants.player.magician1].getHand());


        }*/

    }

    // Start is called before the first frame update
    void Start()
    {
        game = FindObjectOfType<Board>();
        drag = FindObjectOfType<CardDragHandler>();
        ui = FindObjectOfType<UIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}