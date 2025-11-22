namespace Project.Scripts.Infrastructure.Progress
{
    public class GameProgressService : IGameProgressService
    {
        private int _zipLevelIndex;
        public int GetZipLevelIndex()
        {
            return _zipLevelIndex;
        }

        public void SetZipLevelIndex(int index)
        {
            _zipLevelIndex=index;
        }
    }
}