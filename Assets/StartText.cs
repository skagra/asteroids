using UnityEngine;
using UnityEngine.UI;

public class StartText : MonoBehaviour
{
    private Text _startText;
    private float _flashDelta = 0;
    private float _flashPeriod = 0.5f;

    private void Awake()
    {
        _startText = GetComponent<Text>();
    }

    private void Update()
    {
        if (_flashDelta < _flashPeriod)
        {
            _flashDelta += Time.deltaTime;
        }
        else
        {
            _flashDelta = 0f;
            _startText.enabled = !_startText.enabled;
        }
    }
}
