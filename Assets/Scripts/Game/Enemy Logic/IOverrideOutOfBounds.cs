namespace Gyges.Game {
    public interface IOverrideOutOfBounds {
        bool IsOutOfBounds();
        bool IsOutOfBounds(out Enemy.OutOfBoundsDirections oobDir);
    }
}