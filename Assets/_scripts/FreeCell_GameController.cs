using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class FreeCell_GameController : MonoBehaviour
{
    [System.Serializable]
    public struct PlayerStats
    {
        public int _totalScore;
        public float _totalTime;
        public int _gamesPlayed;
        public int _gamesWon;

        public PlayerStats(int totalScore, float totalTime, int gamesPlayed, int gamesWon)
        {
            _totalScore = totalScore;
            _totalTime = totalTime;
            _gamesPlayed = gamesPlayed;
            _gamesWon = gamesWon;
        }
    }

    public GameObject _victoryWindow;
    public GameObject _menuWindow;

    public Text _victoryScoreText;
    public Text _victoryTimeText;
    public Text _totalScoreText;
    public Text _totalTimeText;
    public Text _gamesPlayedText;
    public Text _totalWinsText;
    public Text _gameClockText;
    public Text _scoreText;

    public PlayerStats _playerStats;

    private bool _keepTime;
    private float _gameClockTime;
    private int _score;
    private string _statsSavePath;


    private void Start()
    {
        _keepTime = true;
        LoadPlayerStats();
        _statsSavePath = Application.persistentDataPath;
        _playerStats._gamesPlayed += 1;
    }

    private void Update()
    {
        if (_keepTime == true)
        {
            _gameClockText.text = ConvertClockTime(_gameClockTime);
            _gameClockTime += Time.deltaTime;
            _playerStats._totalTime += Time.deltaTime;
        }
    }

    private string ConvertClockTime(float _time)
    {
        string _clockString;
        int _gameClockTotal = (int)_time;
        int _gameClockMinutes = _gameClockTotal / 60;
        int _gameClockSeconds = _gameClockTotal - (_gameClockMinutes * 60);
        _clockString = "Time: " + _gameClockMinutes + ":" + _gameClockSeconds;

        return _clockString;
    }

    private void LoadPlayerStats()
    {
        if (!Directory.Exists(Application.persistentDataPath)) //check if player stats exists, if not create
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            _playerStats = new PlayerStats
            {
                _totalScore = 0,
                _totalTime = 0,
                _gamesPlayed = 0,
                _gamesWon = 0
            };
            SavePlayerStats();
        }
        else //if so, load
        {
            var _metaFileInfo = Directory.GetFiles(Application.persistentDataPath);

            if (_metaFileInfo.Length > 0)
            {
                string _dataAsJson = File.ReadAllText(_metaFileInfo[0]);
                PlayerStats _tempObj = JsonUtility.FromJson<PlayerStats>(_dataAsJson);
            }
        }
    }

    private void SavePlayerStats()
    {
        //developer note: I should have used binary since Json allows manual editing of file data so you could change player stats
        string json = JsonUtility.ToJson(_playerStats);
        string path = Application.persistentDataPath + "/PlayerStats.json";

        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(json);
            }
        }
    }

    public void ResetGameStats()
    {
        _gameClockTime = 0;
        _gameClockText.text = "Time: 0:0";
        _score = 0;
        _scoreText.text = "Score: 0";
        _keepTime = true;
        _victoryWindow.SetActive(false);
        _playerStats._gamesPlayed += 1;
        _menuWindow.SetActive(false);
    }

    public void ChangeScore(int _amount)
    {
        _score += _amount;
        _scoreText.text = "Score: " + _score.ToString();
        _playerStats._totalScore += _amount;

        if (_score == 364) //amount of points a win is worth with each card being worth number value of card
        {
            Debug.Log("You win!");
            _playerStats._gamesWon += 1;
            _keepTime = false;
            _victoryWindow.SetActive(true);

            _victoryScoreText.text = "Score: " + _score.ToString();
            _victoryTimeText.text = "Time: " + ConvertClockTime(_gameClockTime);
            _totalScoreText.text = "Total Score: " + _playerStats._totalScore;
            _totalTimeText.text = "Total " + ConvertClockTime(_playerStats._totalTime); ;
            _gamesPlayedText.text = "Games Played: " + _playerStats._gamesPlayed;
            _totalWinsText.text = "Wins: " + _playerStats._gamesWon;
        }
    }
}
