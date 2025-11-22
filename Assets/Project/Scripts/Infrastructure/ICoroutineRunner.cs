using System.Collections;
using UnityEngine;

namespace Project.Scripts.Infrastructure
{
    public interface ICoroutineRunner
    {
        Coroutine StartCoroutine(IEnumerator load);
    }
}