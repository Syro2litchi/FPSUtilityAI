using UnityEngine;

namespace ZombieFPS.Director
{
    /// <summary>
    /// Director that does nothing. Replace it with your own IAIDirector implementation.
    /// </summary>
    public class MyDirector : MonoBehaviour, IAIDirector
    {
        public void OnGameStart(DirectorContext context, DirectorActions actions)
        {
        }

        public void Tick(DirectorContext context, DirectorActions actions, float deltaTime)
        {
        }
    }
}
