namespace Project.Scripts.Infrastructure.Progress
{
    public interface IGameProgressService
    {
        int GetZipLevelIndex();
        void SetZipLevelIndex(int index);
    }
}