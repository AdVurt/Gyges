using System;

namespace Gyges.Game {
    public interface IWaveObject {
        event Action<IWaveObjectDestroyEventParams> OnDestroy;
    }

    public struct IWaveObjectDestroyEventParams {
        public IWaveObject waveObject;
        public bool killedByPlayer;
    }
}