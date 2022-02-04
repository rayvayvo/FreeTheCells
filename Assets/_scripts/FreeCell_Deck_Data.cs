using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeCell_Deck_Data : MonoBehaviour
{
    [System.Serializable]
    public struct CardData
    {
        public string _suit;
        public int _value;
    }

    [System.Serializable]
    public struct BoardData
    {
        public CardData[,] _cards;
    }

    private List<CardData> _defaultDeck;

    public Sprite[] _clubs;
    public Sprite[] _diamonds;
    public Sprite[] _hearts;
    public Sprite[] _spades;
    
    public List<CardData[]> _gameData;
    public BoardData _boardData;

    public GameObject[] _cardColumns;
    public GameObject _blankCard; //kept a template card in scene for this so the event triggers were copied on instantiate
    public GameObject _garbageHolder;
    public CardData[] _freeCells;
    public GameObject _freeCellHolder;
    public CardData[] _completeCells; //Using the name "completeCells" instead of "foundation" to refer to the top right 4 Cells throughout the project.
    public GameObject _completeCellHolder;

    private void Start()
    {
        ShuffleCards();
    }

    private void SeedColumnData()
    {
        //creates datapoints for a new game board
        _boardData = new BoardData();
        _boardData._cards = new CardData[8, 19]; //handle up to 19 in each column in case of some extremely rare scenario of an entire King-Ace Run on the bottom of an untouched line
                     
        _gameData = new List<CardData[]>();
        for (int d = 0; d < 8; d++)
        {
            _gameData.Add(new CardData[19]);
        }

        _freeCells = new CardData[4];
        _completeCells = new CardData[4];
        for (int f = 0; f < 4; f++)
        {
            _freeCells[f]._value = 0;
            _completeCells[f]._value = 0;
        }
    }

    private void SeedDefaultDeck()
    {
        //function that creates a list of the default 52 cards to be randomly picked from
        //0 value not included, it is reserved for no card/empty, also makes values line up with card numbers :)
        _defaultDeck = new List<CardData>();

        for (int a = 1; a < 14; a++)
        {
            _defaultDeck.Add(new CardData() { _suit = "Clubs", _value = a });
            _defaultDeck.Add(new CardData() { _suit = "Diamonds", _value = a });
            _defaultDeck.Add(new CardData() { _suit = "Hearts", _value = a });
            _defaultDeck.Add(new CardData() { _suit = "Spades", _value = a });
        }
    }

    private void ClearBoard()
    {
        for (int e = 0; e < 8; e++)
        {
            if (_cardColumns[e].transform.childCount > 0)
            {
                _cardColumns[e].transform.GetChild(0).transform.SetParent(_garbageHolder.transform);
            }
            for (int f = 0; f < _garbageHolder.transform.childCount; f++)
            {
                Destroy(_garbageHolder.transform.GetChild(f).gameObject);
            }
        }
    }

    private void DisplayDeckDealForDebug()
    {
        //function to make sure data set lines up with display
        for (int x = 0; x < 8; x++)
        {
            string _info = "";

            for (int y = 0; y < 19; y++)
            {
                _info += (_boardData._cards[x, y]._value).ToString() + " of " + _boardData._cards[x, y]._suit + "\n";
            }
            Debug.Log("the " + x + " column has: " + _info);
        }
    }

    public void ShuffleCards()
    {
        //function that starts a new game, deals cards out on to the game board and sets the data
        ClearBoard();
        SeedColumnData();
        SeedDefaultDeck();

        int _colCount = 0;
        int _rowCount = 0;

        for (int b = 0; b < 52; b++)
        {
            GameObject _tempCard;
            Transform _tempTransform = _cardColumns[_colCount].transform;
            CardData _tempData = new CardData();

            int _rng = Random.Range(0, _defaultDeck.Count);

            _tempData = _defaultDeck[_rng]; //selects a card randomly from the list of available choices

            for (int c = 0; c < _rowCount; c++) //iterates through the column heirarchy until it reaches the bottom spot to attach the new dealt card to
            {
                _tempTransform = _tempTransform.transform.GetChild(0);
            }

            _tempCard = Instantiate(_blankCard, _cardColumns[_colCount].transform.position, Quaternion.identity, _tempTransform); //spawns the card
            
            _tempCard.GetComponent<FreeCell_SingleCardData>()._data._value = _tempData._value;
            _tempCard.GetComponent<FreeCell_SingleCardData>()._data._suit = _tempData._suit;
            _tempCard.name = (_tempData._value) + " of " + _tempData._suit; 
            _tempCard.tag = "PlayingCard";

            if (_rowCount == 0)
            {
                _tempCard.transform.localPosition = new Vector3(0, 175, 0);
            }
            else
            {
                _tempCard.transform.localPosition = new Vector3(0, -40, 0);
            }


            switch (_tempData._suit) //chooses graphic to place to card
            {
                case "Clubs":
                    _tempCard.GetComponent<Image>().sprite = _clubs[_tempData._value - 1]; //minus 1 since arrays start at 0 
                    break;
                case "Diamonds":
                    _tempCard.GetComponent<Image>().sprite = _diamonds[_tempData._value - 1];
                    break;
                case "Hearts":
                    _tempCard.GetComponent<Image>().sprite = _hearts[_tempData._value - 1];
                    break;
                case "Spades":
                    _tempCard.GetComponent<Image>().sprite = _spades[_tempData._value - 1];
                    break;
            }

            _boardData._cards[_colCount, _rowCount] = _tempData; //sets the data for the card
            _defaultDeck.RemoveAt(_rng); //clear the selected choice from the list so it doesn't get picked again

            _colCount += 1; //moves to the next column, if at end, go back to first

            if (_colCount == 8)
            {
                _colCount = 0;
                _rowCount += 1;
            }
        }
    }
}
