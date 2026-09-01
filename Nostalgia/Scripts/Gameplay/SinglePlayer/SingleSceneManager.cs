using Nostal.Util;
using UnityEngine.SceneManagement;

namespace Nostal.Single
{
    [System.Serializable]
    public enum NostalSingleLevel : int
    {
        None = 0,
        Tutorial = 6,
        LevelOne = 7,
        LevelTwo = 8,
        Chase = 9,
    };

    public class SingleSceneManager : Singleton<SingleSceneManager>
    {
        public void LoadScene(NostalSingleLevel level)
        {
            SceneManager.LoadScene((int)level);
        }
    }
}
