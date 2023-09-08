using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class Reversi : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Cell _cellPrefab;
    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;
    [SerializeField]
    private GameObject _discPrefab;
    [SerializeField]
    private Text _timerText;
    [SerializeField]
    private Text _blackCountText;
    [SerializeField]
    private Text _whiteCountText;
    [SerializeField]
    private string _recode = "";
    [SerializeField]
    private InputField _resultString;
    [SerializeField]
    private GameObject _resultPanel;
    [SerializeField]
    private GameObject _pickDifficultyPanel;
    [SerializeField]
    private Text _resultText;
    [SerializeField]
    private Text _winnerText;
    [SerializeField]
    private ToggleGroup _timeLimitSetting;
    private Cell[,] _cells;
    List<Cell> _canPutAllCell;
    private List<Cell> _reversDiscs;
    private bool _sand;
    private bool _turn = true;
    public bool _normal = false;
    private int _whiteCount;
    private int _blackCount;
    private int _count;
    private int _rows = 8;
    private int _columns = 8;
    private float _timer;
    private int _timeLimit;

    private GameState _gameState = GameState.PickDifficulty;
    private void Awake()
    {
        GenerateBoard();

        _cells[3, 3]._disc.SetActive(true);
        _cells[3, 3].CellColorState = Cell.CellState.Black;
        _cells[4, 4]._disc.SetActive(true);
        _cells[4, 4].CellColorState = Cell.CellState.Black;
        _cells[4, 3]._disc.SetActive(true);
        _cells[4, 3].CellColorState = Cell.CellState.White;
        _cells[3, 4]._disc.SetActive(true);
        _cells[3, 4].CellColorState = Cell.CellState.White;
    }
    private void Start()
    {
        SarchCanPutCell();
        _canPutAllCell = new();
        _resultString.text = _recode;
        _whiteCountText.text = "Åú 2";
        _blackCountText.text = "Åú 2";
    }
    private void Update()
    {
        if (_gameState == GameState.InGame)
        {
            _timer += Time.deltaTime;
            _timerText.text = (_timeLimit - _timer).ToString("00");
            if (_timeLimit - _timer < 0)
            {
                PutRandom();
                EnemyMove();
            }
        }
    }
    public void GenerateBoard()
    {
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        _cells = new Cell[_rows, _columns];
        _reversDiscs = new List<Cell>();

        var parent = _gridLayoutGroup.transform;
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cell.name = $"{r},{c}";
                _cells[r, c] = cell;
                var disc =Instantiate(_discPrefab, cell.transform);
                cell._disc = disc;
                disc.SetActive(false);
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        var cell = eventData.pointerCurrentRaycast.gameObject;
        int tmprow = 0;
        int tmpcolum = 0;
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                if (_cells[r, c] == cell.GetComponent<Cell>())
                {
                    tmprow = r;
                    tmpcolum = c;
                    break;
                }
            }
        }
        if (eventData.button == PointerEventData.InputButton.Left && _gameState == GameState.InGame && _turn)
        {
            if (_cells[tmprow, tmpcolum].CanPut)
            {
                PutDisc(tmprow, tmpcolum);
                Invoke("EnemyMove", 1f);
            }
        }
    }
    private void EnemyMove()
    {
        int Row = -1;
        int Column = -1;

        foreach (var cell in _cells)
        {
            if (cell.CanPut)
            {
                _canPutAllCell.Add(cell);
            }
        }
        if (_canPutAllCell.Count > 0 && !_normal)
        {
            int randomIndex = Random.Range(0, _canPutAllCell.Count);
            Cell randomCell = _canPutAllCell[randomIndex];
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    if (randomCell == _cells[r, c])
                    {
                        Row = r;
                        Column = c;
                    }
                }
            }
            _canPutAllCell.Clear();
        }
        else if (_canPutAllCell.Count > 0 && _normal)
        {
            int MaxReverseCount = 0;
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    if (MaxReverseCount < _cells[r, c].ReverseCount)
                    {
                        Row = r;
                        Column = c;
                        MaxReverseCount = _cells[r, c].ReverseCount;
                    }
                }
            }
        }

        if (Row != -1 && Column != -1)
        {
            PutDisc(Row, Column);
        }
    }
    private void PutRandom()
    {
        int Row = -1;
        int Column = -1;

        foreach (var cell in _cells)
        {
            if (cell.CanPut)
            {
                _canPutAllCell.Add(cell);
            }
        }
        if (_canPutAllCell.Count > 0 && !_normal)
        {
            int randomIndex = Random.Range(0, _canPutAllCell.Count);
            Cell randomCell = _canPutAllCell[randomIndex];
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    if (randomCell == _cells[r, c])
                    {
                        Row = r;
                        Column = c;
                    }
                }
            }
            _canPutAllCell.Clear();
        }

        if (Row != -1 && Column != -1)
        {
            PutDisc(Row, Column);
        }
    }
    private void PutDisc(int Row, int Colum)
    {
        var Disc = _cells[Row, Colum]._disc;
        Disc.SetActive(true);
        _timer = 0;
        if (_turn)
        {
            _cells[Row, Colum].CellColorState = Cell.CellState.Black;
            SarchReverceDisc(Row, Colum, Cell.CellState.Black);
            _recode += (Row * 10 + Colum).ToString();
            _resultString.text = _recode;
        }
        else
        {
            _cells[Row, Colum].CellColorState = Cell.CellState.White;
            SarchReverceDisc(Row, Colum, Cell.CellState.White);
            _recode += (Row * 10 + Colum).ToString();
            _resultString.text = _recode;
        }

        foreach (var cell in _cells)
        {
            cell.CanPut = false;
            cell.ReverseCount = 0;
            if (cell.CellColorState == Cell.CellState.White) _whiteCount++;
            if (cell.CellColorState == Cell.CellState.Black) _blackCount++;
        }

        _whiteCountText.text = "Åú" + _whiteCount.ToString();
        _blackCountText.text = "Åú" + _blackCount.ToString();

        _whiteCount = 0;
        _blackCount = 0;

        _turn = !_turn;
        SarchCanPutCell();

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                if (_cells[r, c].CanPut)
                {
                    return;
                }
            }
        }

        _turn = !_turn;
        SarchCanPutCell();

        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                if (_cells[r, c].CanPut)
                {
                    EnemyMove();
                    return;
                }
            }
        }

        GameEnd();
    }
    private void SarchCanPutCell()
    {
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                if (!_turn && _cells[r, c].CellColorState == Cell.CellState.White)
                {
                    CheckNeighborCell(r, -1, c, -1, Cell.CellState.White);
                    CheckNeighborCell(r, -1, c, 0, Cell.CellState.White);
                    CheckNeighborCell(r, -1, c, +1, Cell.CellState.White);
                    CheckNeighborCell(r, 0, c, -1, Cell.CellState.White);
                    CheckNeighborCell(r, 0, c, +1, Cell.CellState.White);
                    CheckNeighborCell(r, 1, c, -1, Cell.CellState.White);
                    CheckNeighborCell(r, 1, c, 0, Cell.CellState.White);
                    CheckNeighborCell(r, 1, c, +1, Cell.CellState.White);
                }
                else if (_turn && _cells[r, c].CellColorState == Cell.CellState.Black)
                {
                    CheckNeighborCell(r, -1, c, -1, Cell.CellState.Black);
                    CheckNeighborCell(r, -1, c, 0, Cell.CellState.Black);
                    CheckNeighborCell(r, -1, c, 1, Cell.CellState.Black);
                    CheckNeighborCell(r, 0, c, -1, Cell.CellState.Black);
                    CheckNeighborCell(r, 0, c, 1, Cell.CellState.Black);
                    CheckNeighborCell(r, 1, c, -1, Cell.CellState.Black);
                    CheckNeighborCell(r, 1, c, 0, Cell.CellState.Black);
                    CheckNeighborCell(r, 1, c, 1, Cell.CellState.Black);
                }
            }
        }
    }
    private void CheckNeighborCell
        (int Row, int DirRow, int Column, int DirColumn, Cell.CellState Color)
    {
        if (Row + DirRow < 0 || Column + DirColumn < 0
            || Row + DirRow >= _rows || Column + DirColumn >= _columns)
        {
            _sand = false;
        }

        if (Row + DirRow >= 0 && Column + DirColumn >= 0
        && Row + DirRow < _rows && Column + DirColumn < _columns)
        {
            if (_cells[Row + DirRow, Column + DirColumn].CellColorState != Color
                && _cells[Row + DirRow, Column + DirColumn].CellColorState != Cell.CellState.None)
            {
                _count++;
                _sand = true;
                CheckNeighborCell(Row + DirRow, DirRow, Column + DirColumn, DirColumn, Color);
            }
            else if (_cells[Row + DirRow, Column + DirColumn].CellColorState == Cell.CellState.None
                && _sand == true)
            {
                _sand = false;
                _cells[Row + DirRow, Column + DirColumn].CanPut = true;
                _cells[Row + DirRow, Column + DirColumn].ReverseCount += _count;
                _count = 0;
            }
            else
            {
                _sand = false;
            }
        }
    }
    private void SarchReverceDisc(int Row, int Column, Cell.CellState Color)
    {
        CheckNeighborDisc(Row, -1, Column, -1, Color);
        CheckNeighborDisc(Row, -1, Column, 0, Color);
        CheckNeighborDisc(Row, -1, Column, 1, Color);
        CheckNeighborDisc(Row, 0, Column, -1, Color);
        CheckNeighborDisc(Row, 0, Column, 1, Color);
        CheckNeighborDisc(Row, 1, Column, -1, Color);
        CheckNeighborDisc(Row, 1, Column, 0, Color);
        CheckNeighborDisc(Row, 1, Column, 1, Color);
    }
    private void CheckNeighborDisc(int Row, int DirRow, int Column, int DirColumn, Cell.CellState Color)
    {
        if (Row + DirRow >= 0 && Column + DirColumn >= 0
            && Row + DirRow < _rows && Column + DirColumn < _columns)
        {
            if (_cells[Row + DirRow, Column + DirColumn].CellColorState != Color
            && _cells[Row + DirRow, Column + DirColumn].CellColorState != Cell.CellState.None)
            {
                _sand = true;
                _reversDiscs.Add(_cells[Row + DirRow, Column + DirColumn]);
                CheckNeighborDisc(Row + DirRow, DirRow, Column + DirColumn, DirColumn, Color);
            }
            else if (_cells[Row + DirRow, Column + DirColumn].CellColorState == Color
                && _sand == true)
            {
                foreach (var disc in _reversDiscs)
                {
                    disc.CellColorState = Color;
                }
                _sand = false;
            }
            else
            {
                _sand = false;
            }
        }
        _sand = false;
        _reversDiscs.Clear();
    }
    private void GameEnd()
    {
        _gameState = GameState.GameEnd;
        _resultPanel.SetActive(true);
        foreach (var cell in _cells)
        { 
            if (cell.CellColorState == Cell.CellState.White) _whiteCount++;
            if (cell.CellColorState == Cell.CellState.Black) _blackCount++;
        }
        _resultText.text = _blackCount.ToString() + " -- " + _whiteCount.ToString();
        if (_whiteCount == _blackCount)
        {
            _winnerText.text = "Draw";
        }
        else
        {
            _winnerText.text = _blackCount > _whiteCount ? "çïÇÃèüÇøÅI" : "îíÇÃèüÇøÅI";
        }
    }
    public void ReproductionRecode()
    {
        _gameState = GameState.Recode;
        var data = _resultString.text.Trim();
        if (data.Length % 2 == 0)
        {
            for (int r = 0; r < _rows; r++)
            {
                for (int c = 0; c < _columns; c++)
                {
                    _cells[r, c].CellColorState = Cell.CellState.None;
                    _cells[r, c]._disc.SetActive(false);
                }
            }

            _cells[3, 3]._disc.SetActive(true);
            _cells[3, 3].CellColorState = Cell.CellState.Black;
            _cells[4, 4]._disc.SetActive(true);
            _cells[4, 4].CellColorState = Cell.CellState.Black;
            _cells[4, 3]._disc.SetActive(true);
            _cells[4, 3].CellColorState = Cell.CellState.White;
            _cells[3, 4]._disc.SetActive(true);
            _cells[3, 4].CellColorState = Cell.CellState.White;

            _resultString.text = "";
            _recode = "";


            StartCoroutine(MakeDistance(data));

        }
        _gameState = GameState.InGame;
    }
    public void ModeEasy()
    {
        string selectedLabel = _timeLimitSetting.ActiveToggles()
            .First().GetComponentsInChildren<Text>()
            .First(t => t.name == "Label").text;
        _timeLimit = int.Parse(selectedLabel);
        _normal = false;
        _gameState = GameState.InGame;
        _pickDifficultyPanel.SetActive(false);
        _turn = true;
    }
    public void ModeNormal()
    {
        string selectedLabel = _timeLimitSetting.ActiveToggles()
            .First().GetComponentsInChildren<Text>()
            .First(t => t.name == "Label").text;
        _timeLimit = int.Parse(selectedLabel);
        _normal = true;
        _gameState = GameState.InGame;
        _pickDifficultyPanel.SetActive(false);
        _turn = true;
    }
    public void Title()
    {
        SceneManager.LoadScene("Osero");
    }
    IEnumerator MakeDistance(string data)
    {      
        for (int i = 0; i < data.Length; i += 2)
        {
            string PutPosRow = data[i].ToString();
            string PutPosColumn = data[i + 1].ToString();

            PutDisc(int.Parse(PutPosRow), int.Parse(PutPosColumn));
            yield return new WaitForSeconds(1.0f);
        }
    }
    private enum GameState
    {
        Recode,
        PickDifficulty,
        InGame,
        GameEnd
    }
}
