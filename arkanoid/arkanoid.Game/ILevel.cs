using SiliconStudio.Xenko.Engine;

namespace arkanoid
{
    public interface ILevel
    {
        void ClearAll();
        void Prepare();
        bool HasRemainingBrick();
        void SignalDeletedBrick(Entity e);
    }
}