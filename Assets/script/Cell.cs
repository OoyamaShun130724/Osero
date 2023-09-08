using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Cell : MonoBehaviour
{
    
    public GameObject _disc;
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
            _disc.transform.DORotate(new Vector3(1, 0, 0) * 90f, 1f);
        }
        if (_cellState == CellState.Black)
        {
            _disc.transform.DORotate(new Vector3(1, 0, 0) * -90f, 1f);
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
