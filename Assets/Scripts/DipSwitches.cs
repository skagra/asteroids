using UnityEngine;

public class DipSwitches : MonoBehaviour
{
    public static DipSwitches Instance { get; private set; }

    [SerializeField]
    private bool _isSoundEnabled;

    public bool IsSoundEnabled { get { return _isSoundEnabled; } }

    private void Awake()
    {
        Instance ??= this;
    }
       
}
