using System;

namespace Gyges.Game {
    public interface IWaveObject {
        event Action<IWaveObjectDestroyEventParams> onDestroy;
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