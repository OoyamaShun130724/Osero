using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class reversi : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    Cell _cellPrefab;
    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;
    [SerializeField]
    private GameObject _discPrefab;
    private Cell[,] _cells;
    List<Cell> _canPutAllCell;
    private List<Cell> _reversDiscs;
    private List<List<Cell.CellState>> _gameRecord;
    private bool _sand;
    private bool _turn = true;
    private int _count;
    private int _rows = 8;
    private int _columns = 8;
    private string _recode = "54536265222524";
    private List<string> _procedure;
    private void Awake()
    {
        GenerateBoard();

        _cells[3, 3].CellColorState = Cell.CellState.Black;
        _cells[4, 4].CellColorState = Cell.CellState.Black;
        _cells[4, 3].CellColorState = Cell.CellState.White;
        _cells[3, 4].CellColorState = Cell.CellState.White;
    }
    private void Start()
    {
        SarchCanPutCell();
        _canPutAllCell = new();

        //_procedure = new();
        //for (int i = 0; i < _recode.Length; i+=2)
        //{
        //    string PutPosRow =_recode[i].ToString();
        //    string PutPosColumn = _recode[i + 1].ToString();

        //    PutDisc(int.Parse(PutPosRow), int.Parse(PutPosColumn));
        //}
    }
    public void GenerateBoard()
    {
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        _gridLayoutGroup.constraintCount = _columns;

        _cells = new Cell[_rows, _columns];
        _reversDiscs = new List<Cell>();
        _gameRecord = new();

        var parent = _gridLayoutGroup.transform;
        for (int r = 0; r < _rows; r++)
        {
            for (int c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cell.name = $"{r},{c}";
                _cells[r, c] = cell;
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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (_cells[tmprow, tmpcolum].CanPut)
            {
                PutDisc(tmprow, tmpcolum);
                EnemyMove();
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
        if (_canPutAllCell.Count > 0)
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
        //var Recode = new List<Cell.CellState>();

        if (_turn)
        {
            _cells[Row, Colum].CellColorState = Cell.CellState.Black;
            SarchReverceDisc(Row, Colum, Cell.CellState.Black);
        }
        else
        {
            _cells[Row, Colum].CellColorState = Cell.CellState.White;
            SarchReverceDisc(Row, Colum, Cell.CellState.White);
        }

        foreach (var cell in _cells)
        {
            cell.CanPut = false;
            cell.ReverseCount = 0;
            //Recode.Add(cell.CellColorState);
        }

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
        //_gameRecord.Add(Recode);
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
        //if (_count == 0)
        //{
        //    _turn = !_turn;

        //    SarchCanPutCell();
        //}
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
        Debug.Log("owa");
    }
    private enum GameState
    {

    }
}
