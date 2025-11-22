using Project.Scripts.Progress.Data;

namespace Project.Scripts.Progress.Provider
{
    public interface IProgressProvider
    {
        ProgressData ProgressData { get; }
        void SetProgressData(ProgressData data);
    }
}