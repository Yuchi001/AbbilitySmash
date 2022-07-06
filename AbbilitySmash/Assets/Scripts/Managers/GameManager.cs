using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class GameManager : MonoBehaviour
{
    #region
    public static GameManager Instance;
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion
    public CardManager CardManager 
    {
        get
        {
            if (_cardManager == null)
                _cardManager = FindObjectOfType<CardManager>();

            return _cardManager;
        }
    }
    private CardManager _cardManager;
    public Transform Canvas
    {
        get
        {
            if (_canvas == null)
                _canvas = FindObjectOfType<Canvas>().transform;

            return _canvas;
        }
    }
    private Transform _canvas;

    public Transform Hand
    {
        get
        {
            if (_hand == null)
                _hand = Canvas.Find("Hand");

            return _hand;
        }
    }
    private Transform _hand;

    public Transform Passive
    {
        get
        {
            if (_passive == null)
                _passive = Canvas.Find("Passive");

            return _passive;
        }
    }
    private Transform _passive;

    public void ClearLog()
    {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }
}
