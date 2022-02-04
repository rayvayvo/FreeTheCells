using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Freecell_CardClickManager : MonoBehaviour
{
    private FreeCell_Deck_Data.CardData _activeCardData;
    private GameObject _dragCard;
    private GameObject _dropCardLocation;
    private GameObject _lastClickedCard;

    private Transform _originalPos;
    private float _doubleClickTimer;
    private bool _dragging;

    public FreeCell_Deck_Data _deckDataController;
    public GameObject _draggedCardHolder; //this is so the UI element shows up on top of the others when dragging

    private void Start()
    {
        _doubleClickTimer = 0;
        _dragging = false;
    }

    private void Update()
    {
        if (_doubleClickTimer > 0)
        {
            _doubleClickTimer -= Time.deltaTime;
        }
    }

    private void SetActiveCardData(GameObject _card)
    {
        _activeCardData = _card.GetComponent<FreeCell_SingleCardData>()._data;
        _dragCard = _card;
    }

    public void UndoLastMove()
    {
        _dragCard.transform.SetParent(_originalPos.transform);
        _dragCard.transform.localPosition = new Vector3(0, -40, 0);

        switch (_originalPos.tag) //different positions for different spots on board
        {
            case "FreeCell":
            case "CompleteCell":
                _dragCard.transform.localPosition = new Vector3(0, 0, 0);
                break;
            case "Column":
                _dragCard.transform.localPosition = new Vector3(0, 175, 0);
                break;
        }
        //this needs data clearing to in the case of undo for a free cell or a Complete cell
    }

    private void ToggleCardRaycastsOnTopCells(bool _activeStatus)
    {
        //makes top cards not raycastable when dragging and dropping so drag and drop detects the cell slots below
        for (int a = 0; a < 4; a++)
        {
            GameObject _cellHolder = _deckDataController._completeCellHolder.transform.GetChild(a).gameObject;
            int _count = _cellHolder.transform.childCount;

            if (_deckDataController._freeCellHolder.transform.GetChild(a).transform.childCount > 0)
            {
                _deckDataController._freeCellHolder.transform.GetChild(a).transform.GetChild(0).gameObject.GetComponent<Image>().raycastTarget = _activeStatus;
            }
            if (_cellHolder.transform.childCount > 0)
            {
                _cellHolder.transform.GetChild(_count - 1).gameObject.GetComponent<Image>().raycastTarget = _activeStatus;
            }
        }
    }
    public void CardDrag(GameObject _card)
    {
        SetActiveCardData(_card);

        Vector3 _mousePos = Input.mousePosition;

        if (_dragging == false) //sets data first frame object is dragged
        {
            _originalPos = _card.transform.parent.transform;
            _card.GetComponent<Image>().raycastTarget = false; //make it so the card doesn't block pointer data for drop location
            _card.transform.SetParent(_draggedCardHolder.transform);
            ToggleCardRaycastsOnTopCells(false);
        }

        _dragging = true;
        _dragCard.transform.position = new Vector3(_mousePos.x, _mousePos.y - 125, _mousePos.z); //moves the card with the mouse

        //checks for objects below while dragging to search for a spot to drop
        PointerEventData _pointData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> _results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(_pointData, _results);
        
        if (_results.Count > 0)
        {
            _dropCardLocation = _results[0].gameObject;
            Debug.Log(_dropCardLocation.name + " ?? " + _results[0].gameObject.tag);
        }
    }

    public void CardDrop(GameObject _card)
    {
        //on drop function that lets you move cards from stack to stack
        if (_dragging == true)
        {
            bool _suitMatch = false;
            bool _canDrop = false;
            int _index;
            int _originIndex;
            Debug.Log("card drop");
            _card.GetComponent<Image>().raycastTarget = true; //re-enable clicking on the card
            _dragging = false;


            //check if its a valid spot to drop, only check for yes situations to turn _canDrop to true
            switch (_dropCardLocation.transform.tag)
            {
                case "PlayingCard":
                    FreeCell_Deck_Data.CardData _dropData = _dropCardLocation.GetComponent<FreeCell_SingleCardData>()._data;
                    Debug.Log("can I drop my " + _dragCard.name + " onto a " + _dropCardLocation.name);
                    switch (_activeCardData._suit)
                    {
                        case "Clubs":
                        case "Spades":
                            if (_dropData._suit == "Diamonds" || _dropData._suit == "Hearts")
                            {
                                _suitMatch = true;
                            }
                            break;
                        case "Diamonds":
                        case "Hearts":
                            if (_dropData._suit == "Clubs" || _dropData._suit == "Spades")
                            {
                                _suitMatch = true;
                            }
                            break;
                    }
                    if (_suitMatch == true)
                    {
                        if (_activeCardData._value == _dropData._value - 1)
                        {
                            if (_dropCardLocation.transform.childCount == 0)
                            {
                                _canDrop = true;
                            }
                        }
                    }
                    break;
                case "FreeCell":
                    _index = int.Parse(_dropCardLocation.name.Substring(_dropCardLocation.name.Length - 1, 1));
                    Debug.Log(_index);
                    Debug.Log(_deckDataController._freeCells.Length);
                    if (_deckDataController._freeCells[_index]._value == 0)
                    {
                        _canDrop = true;
                        _deckDataController._freeCells[_index] = _activeCardData;
                    }
                    break;
                case "CompleteCell":
                    if (_dragCard.transform.childCount == 0)
                    {
                        _index = int.Parse(_dropCardLocation.name.Substring(_dropCardLocation.name.Length - 1, 1));

                        if (_deckDataController._completeCells[_index]._value == 0 && _activeCardData._value == 1) //drop ace into empty column
                        {
                            _canDrop = true;
                            _deckDataController._completeCells[_index] = _activeCardData;
                        }
                        else if (_deckDataController._completeCells[_index]._suit == _activeCardData._suit && _deckDataController._completeCells[_index]._value == _activeCardData._value - 1)
                        {
                            _canDrop = true;
                            _deckDataController._completeCells[_index] = _activeCardData;
                        }
                    }
                    break;
                case "Column":
                    if (_dropCardLocation.transform.childCount == 0)
                    {
                        _canDrop = true;
                    }
                    break;
            }
            if (_canDrop == true) 
            {
                _dragCard.transform.SetParent(_dropCardLocation.transform);
                _dragCard.transform.localPosition = new Vector3(0, -40, 0);

                switch (_originalPos.tag) //handling for data if card is moved from top slots
                {
                    case "FreeCell":
                        _originIndex = int.Parse(_originalPos.name.Substring(_originalPos.name.Length - 1, 1));
                        _deckDataController._freeCells[_originIndex]._value = 0;
                        break;
                    case "CompleteCell":
                        _originIndex = int.Parse(_originalPos.name.Substring(_originalPos.name.Length - 1, 1));
                        _deckDataController._completeCells[_originIndex]._value = _activeCardData._value - 1;
                        break;
                }

                switch (_dropCardLocation.tag)
                {
                    case "FreeCell":
                        _dragCard.transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case "CompleteCell":
                        int _stack = _dragCard.transform.parent.transform.childCount;
                        _dragCard.transform.localPosition = new Vector3(0, 0, 0);
                        if(_stack > 1)
                        {
                            //disable raycasts of cards below in same pile to make less work for the toggle raycast function
                            _dragCard.transform.parent.GetChild(_stack - 2).GetComponent<Image>().raycastTarget = false; 
                        }
                        break;
                    case "Column":
                        _dragCard.transform.localPosition = new Vector3(0, 175, 0);
                        break;
                }
                ToggleCardRaycastsOnTopCells(true);
            }
            else
            {
                _card.transform.SetParent(_originalPos.transform);
                _card.transform.localPosition = new Vector3(0, -40, 0);
                
                switch (_originalPos.tag) //if drop fails, return to proper position
                {
                    case "FreeCell":
                    case "CompleteCell":
                        _dragCard.transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case "Column":
                        _dragCard.transform.localPosition = new Vector3(0, 175, 0);
                        break;
                }
            }
        }
    }

    public void CardClick(GameObject _card)
    {
        SetActiveCardData(_card);
        if (_doubleClickTimer > 0 && _lastClickedCard == _card)
        {
            CardDoubleClick(_card);
        }
        else
        {
            _doubleClickTimer = 1;
            _lastClickedCard = _card;
            Debug.Log("I clicked on a " + (_activeCardData._value).ToString() + " of " + _activeCardData._suit + "!");
        }
    }

    public void CardDoubleClick(GameObject _card)
    {
        Debug.Log("card Double click");
        //on click function that takes double click to move cards to bottom empty column, if none then top freecell or completed stack if applicable
        //this function will require data of clicked on card, completed stacks, and if empty slots on board or freecells
    }
}
