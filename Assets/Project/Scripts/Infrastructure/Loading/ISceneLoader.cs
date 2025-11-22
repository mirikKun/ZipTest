using System;

namespace Project.Scripts.Infrastructure.Loading
{
    public interface ISceneLoader
    {
        void LoadScene(string name, Action onLoaded = null);
    }
}