using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Freecell_CardClickManager : MonoBehaviour
{
    private FreeCell_Deck_Data.CardData _activeCardData;
    private GameObject _activeCardObj;
    private GameObject _dropCardLocation;
    private GameObject _lastClickedCard;

    private Transform _originalPos;
    private Transform _lastPlayPos;
    private float _doubleClickTimer;
    private bool _dragging;
    private int _activeCardIndex;
    private int _draggedCardAmount;

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

    public void CardDrag(GameObject _card)
    {
        SetActiveCardData(_card);

        Vector3 _mousePos = Input.mousePosition;

        if (_dragging == false) //sets data first frame object is dragged and checks if column can be moved
        {
            GetDraggedCardAmount();
            _originalPos = _card.transform.parent.transform;
            _card.GetComponent<Image>().raycastTarget = false; //make it so the card doesn't block pointer data for drop location
            _card.transform.SetParent(_draggedCardHolder.transform);
            ToggleCardRaycastsOnTopCells(false);
            

            if (_draggedCardAmount == 1) //check for how many cards are being moved at once, check to see if there's enough free slots on board to accomodate a move of a stack of cards
            {
                _dragging = true;
            }
            else
            {
                int _freeSlotsForMoving = 0;
                for (int a = 0; a < 8; a ++)
                {
                    if (_deckDataController._cardColumns[a].transform.childCount == 0)
                    {
                        _freeSlotsForMoving += 1;
                    }
                }
                for (int b = 0; b < 4; b++)
                {
                    if (_deckDataController._freeCells[b]._value == 0)
                    {
                        _freeSlotsForMoving += 1;
                    }
                }
                if (_freeSlotsForMoving >= _draggedCardAmount)
                {
                    _dragging = true;
                }
            }
        }
        if (_dragging == true)
        {
            _activeCardObj.transform.position = new Vector3(_mousePos.x, _mousePos.y - 125, _mousePos.z); //moves the card with the mouse

            //checks for objects below while dragging to search for a spot to drop
            PointerEventData _pointData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            List<RaycastResult> _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_pointData, _results);

            if (_results.Count > 0)
            {
                _dropCardLocation = _results[0].gameObject;
            }
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
            _dragging = false;

            //check if its a valid spot to drop, only check for yes situations to turn _canDrop to true
            switch (_dropCardLocation.transform.tag)
            {
                case "PlayingCard":
                    FreeCell_Deck_Data.CardData _dropData = _dropCardLocation.GetComponent<FreeCell_SingleCardData>()._data;
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
                    if (_draggedCardAmount == 1) //only works on solo cards
                    {
                        _index = int.Parse(_dropCardLocation.name.Substring(_dropCardLocation.name.Length - 1, 1));
                        if (_deckDataController._freeCells[_index]._value == 0)
                        {
                            _canDrop = true;
                            _deckDataController._freeCells[_index] = _activeCardData;
                        }
                    }
                    break;
                case "CompleteCell":
                    if (_draggedCardAmount == 1)
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
                _activeCardObj.transform.SetParent(_dropCardLocation.transform);
                _activeCardObj.transform.localPosition = new Vector3(0, -40, 0);
                _lastPlayPos = _dropCardLocation.transform;

                switch (_originalPos.tag) //handling for data if card is moved from top slots
                {
                    case "FreeCell":
                        _activeCardIndex = int.Parse(_originalPos.name.Substring(_originalPos.name.Length - 1, 1));
                        _deckDataController._freeCells[_activeCardIndex]._value = 0;
                        break;
                    case "CompleteCell":
                        _activeCardIndex = int.Parse(_originalPos.name.Substring(_originalPos.name.Length - 1, 1));
                        _deckDataController._completeCells[_activeCardIndex]._value = _activeCardData._value - 1;
                        break;
                }

                switch (_dropCardLocation.tag)
                {
                    case "FreeCell":
                        _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case "CompleteCell":
                        int _stack = _activeCardObj.transform.parent.transform.childCount;
                        _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                        if(_stack > 1)
                        {
                            //disable raycasts of card below in same pile to make less work for the toggle raycast function
                            _activeCardObj.transform.parent.GetChild(_stack - 2).GetComponent<Image>().raycastTarget = false; 
                        }
                        break;
                    case "Column":
                        _activeCardObj.transform.localPosition = new Vector3(0, 175, 0);
                        break;
                }
                
            }
            else
            {
                Debug.Log(_originalPos.transform + "????");
                _card.transform.SetParent(_originalPos.transform);
                _card.transform.localPosition = new Vector3(0, -40, 0);
                
                switch (_originalPos.tag) //if drop fails, return to proper position
                {
                    case "FreeCell":
                    case "CompleteCell":
                        _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                        break;
                    case "Column":
                        _activeCardObj.transform.localPosition = new Vector3(0, 175, 0);
                        break;
                }
            }

            _card.GetComponent<Image>().raycastTarget = true; //re-enable clicking on the card
            ToggleCardRaycastsOnTopCells(true);
        }
    }

    public void CardClick(GameObject _card)
    {
        SetActiveCardData(_card);
        if (_doubleClickTimer > 0 && _lastClickedCard == _card) //double click trigger
        {
            CardDoubleClick();
        }
        else //set timer for double click window
        {
            _doubleClickTimer = 1;
            _lastClickedCard = _card;
        }
    }

    //on click function that takes double click to move cards to top Completed Cell if possible, if not then top bottom empty column, if not then empty freecell 
    private void CardDoubleClick() 
    {
        
        if (_activeCardObj.transform.childCount == 0) //check if no children, must be empty free card
        {
            if (_activeCardData._value == 1) //if ace move to empty spot
            {
                Debug.Log("its an ace");
                CheckDoubleClickForEmptyCompleteCell();
                _activeCardObj.transform.SetParent(_deckDataController._completeCellHolder.transform.GetChild(_activeCardIndex).transform);
                _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                //data handling
            }
            else if (CheckDoubleClickForCompleteCellMatch() != 9) //check for completed Cell matches, return index if true
            {
                Debug.Log("its a complete pile match");
                _activeCardObj.transform.SetParent(_deckDataController._completeCellHolder.transform.GetChild(_activeCardIndex).transform);
                _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                //data handling
            }
            else if (CheckDoubleClickForEmptyBoardMatch() != 9) //check for empty board columns, return index if true
            {
                Debug.Log("its an empty board match");
                _activeCardObj.transform.SetParent(_deckDataController._cardColumns[_activeCardIndex].transform);
                _activeCardObj.transform.localPosition = new Vector3(0, 175, 0);
                //data handling
            }
            else if (CheckDoubleClickForEmptyFreeCellMatch() != 9) //check for empty free cells, return index if true
            {
                Debug.Log("its a free cell match");
                _activeCardObj.transform.SetParent(_deckDataController._freeCellHolder.transform.GetChild(_activeCardIndex).transform);
                _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                //data handling
            }
        }
        else
        {
            Debug.Log("Nowhere for double click to go, sad face :(");
        }
    }

    private int CheckDoubleClickForEmptyCompleteCell()
    {
        _activeCardIndex = 9;  //a number out of bounds to check against for fails

        for (int a = 0; a < 4; a++)
        {
            if (_deckDataController._completeCells[a]._value == 0)
            {
                _activeCardIndex = a;
                break;
            }
        }
        return _activeCardIndex;
    }

    private int CheckDoubleClickForCompleteCellMatch()
    {
        _activeCardIndex = 9;

        if (_activeCardData._suit == _deckDataController._completeCells[0]._suit ||
                _activeCardData._suit == _deckDataController._completeCells[1]._suit ||
                _activeCardData._suit == _deckDataController._completeCells[2]._suit ||
                _activeCardData._suit == _deckDataController._completeCells[3]._suit)
        {
            for (int a = 0; a < 4; a++)
            {
                if (_activeCardData._value == _deckDataController._completeCells[a]._value + 1)
                {
                    _activeCardIndex = a;
                }
            }
        }
        return _activeCardIndex;
    }

    private int CheckDoubleClickForEmptyBoardMatch()
    {
        _activeCardIndex = 9;

        for(int a = 0; a < 8; a++)
        {
            if (_deckDataController._cardColumns[a].transform.childCount == 0)
            {
                _activeCardIndex = a;
                break;
            }
        }
        return _activeCardIndex;
    }

    private int CheckDoubleClickForEmptyFreeCellMatch()
    {
        _activeCardIndex = 9;

        for(int a = 0; a < 4; a++)
        {
            if (_deckDataController._freeCells[a]._value == 0)
            {
                _activeCardIndex = a;
                break;
            }
        }
        return _activeCardIndex;
    }

    private void SetActiveCardData(GameObject _card)
    {
        _activeCardData = _card.GetComponent<FreeCell_SingleCardData>()._data;
        _activeCardObj = _card;
    }

    public void UndoLastMove()
    {
        int _lastPlayIndex = int.Parse(_lastPlayPos.name.Substring(_lastPlayPos.name.Length - 1, 1));
        _activeCardObj.transform.SetParent(_originalPos.transform);
        _activeCardObj.transform.localPosition = new Vector3(0, -40, 0);

        switch (_originalPos.tag) //handles changes from original spot
        {
            case "FreeCell":
                _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                _deckDataController._freeCells[_activeCardIndex] = _activeCardData;
                break;
            case "CompleteCell":
                _activeCardObj.transform.localPosition = new Vector3(0, 0, 0);
                _deckDataController._completeCells[_activeCardIndex] = _activeCardData;
                break;
            case "Column":
                _activeCardObj.transform.localPosition = new Vector3(0, 175, 0);
                break;
        }
        Debug.Log("I came from " + _lastPlayPos.name);
        switch (_lastPlayPos.tag) //handles any data changes from destination spot
        {
            case "FreeCell":
                _deckDataController._freeCells[_lastPlayIndex]._value = 0;
                break;
            case "CompleteCell":
                _deckDataController._completeCells[_lastPlayIndex]._value -= 1;
                break;
        }

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

    //function that iterates through the held card and its heirarchy to determine how many cards are being dragged
    private void GetDraggedCardAmount()
    {
        Transform _childCounter = _activeCardObj.transform;

        _draggedCardAmount = 1;

        for (int a = 0; a < 13; a++) //13 loops since 12 is the max amount of cards you can move at once. so one extra check to make sure there's a fail for too many (4 cells, 7 columns, one in hand)
        {
            if (_childCounter.childCount > 0)
            {
                _draggedCardAmount += 1;
                _childCounter = _childCounter.transform.GetChild(0).transform;
            }
            else
            {
                break;
            }
            if (a == 12)
            {
                _draggedCardAmount = 0;
            }
        }
    }
}
