using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    private int _reverseCount = 0;
    public int ReverseCount
    {
        get => _reverseCount;
        set
        {
            _reverseCount = value;
        }
    }
    private bool _canPut = false;
    public bool CanPut
    {
        get => _canPut;
        set
        {
            _canPut = value;
            OnChangeCellState();
        }
    }
    public CellState _cellState = CellState.None;
    public CellState CellColorState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnChangeCellState();
        }
    }
    private void OnValidate()
    {
        OnChangeCellState();
    }
    public void OnChangeCellState()
    {
        if (!_canPut)
        {
            GetComponent<Image>().color = Color.green;
        }
        if (_cellState == CellState.White)
        {
            GetComponent<Image>().color = Color.white;
        }
        if (_cellState == CellState.Black)
        {
            GetComponent<Image>().color = Color.black;
        }
        if (_cellState == CellState.None)
        {
            GetComponent<Image>().color = Color.green;
        }
        if (_canPut)
        {
            GetComponent<Image>().color = Color.red;
        }
    }
    public enum CellState
    {
        White,
        Black,
        None
    }
}
