using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
[CreateAssetMenu(fileName = "Gameplay Settings", menuName = "Gyges/Settings/Gameplay")]
public class GameplaySettings : ScriptableObject
{
    [SerializeField]
    private bool _P1HUDOnRight = true;
    public bool P1HUDOnRight {
        get {
            return _P1HUDOnRight;
        }
        set {
            _P1HUDOnRight = value;
            OnP1HUDUpdate?.Invoke(_P1HUDOnRight);
        }
    }

    [SerializeField]
    private bool _streamerMode = false;
    public bool StreamerMode {
        get {
            return _streamerMode;
        }
        set {
            _streamerMode = value;
            OnStreamerModeUpdate?.Invoke(_streamerMode);
        }
    }

    public event Action<bool> OnP1HUDUpdate;
    public event Action<bool> OnStreamerModeUpdate;
}
