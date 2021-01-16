using System;
using UnityEngine;

namespace Gyges.Game {
    public interface IWaveObject {

        bool StartLogicBeforeGameplay { get; }
        event Action<IWaveObjectDestroyEventParams> onDestroy;
        Vector2 Velocity { get; }
        bool IsOutOfBounds();
        bool IsOutOfBounds(out Enemy.OutOfBoundsDirections oobDir);
        Transform GetTransform();
    }

    public struct IWaveObjectDestroyEventParams {
        public IWaveObject waveObject;
        public bool killedByPlayer;
        public int bounty;

        public IWaveObjectDestroyEventParams(IWaveObject waveObject, bool killedByPlayer, int bounty) {
            this.waveObject = waveObject;
            this.killedByPlayer = killedByPlayer;
            this.bounty = bounty;
        }
    }
}