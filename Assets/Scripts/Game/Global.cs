using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Gyges {
    public static class Global {

        public static bool enableGameLogic = true;
        private static bool _paused = false;
        public static bool Paused {
            get {
                return _paused;
            }
            set {
                if (_paused == value)
                    return;
                _paused = value;
                OnPausedChanged?.Invoke(_paused);
            }
        }

        public static event Action<bool> OnPausedChanged;

    }
}