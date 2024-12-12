using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lives : MonoBehaviour
{
    public event Action OnPlayerDeath;

    // Values customisable in the Unity Inspector
    [Header("General Settings")]
    [SerializeField]
    private int _startLives;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject _lifePrefab;

    [Header("References")]
    [SerializeField]
    private EventHub _eventHub;
    [SerializeField]
    private AudioHub _audioHub;

    private float _spriteWidth;
    private int _currentLives = 0;
    private readonly List<GameObject> _lives = new();

    private void Awake()
    {
        _spriteWidth = _lifePrefab.GetComponent<Image>().sprite.rect.width;
        _eventHub.OnExtraLifeThresholdPassed += ExtraLife;
    }

    private void Start()
    {
        for (var i = 0; i < _startLives; i++)
        {
            AddLife();
        }
        _eventHub.OnPlayerExploding += LifeLost;
    }

    private void LifeLost()
    {
        RemoveLife();
    }

    private void ExtraLife()
    {
        _audioHub.PlayExtraShip();
        AddLife();
    }

    private void AddLife()
    {
        var newLife = Instantiate<GameObject>(_lifePrefab);

        _lives.Add(newLife);

        newLife.transform.SetParent(transform);

        newLife.transform.localPosition = new Vector2((_currentLives * _spriteWidth), 0);

        newLife.SetActive(true);

        _currentLives++;
    }

    private void RemoveLife()
    {
        _currentLives--;
        if (_currentLives >= 0)
        {
            Destroy(_lives[_currentLives]);
            _lives.RemoveAt(_currentLives);
        }
        if (_currentLives == 0)
        {
            OnPlayerDeath.Invoke();
        } // TODO  else
    }
}
