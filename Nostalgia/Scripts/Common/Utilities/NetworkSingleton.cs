using Fusion;
using UnityEngine;

namespace Nostal.Util
{
    public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
    {
        public static T Instance { get; private set; }

        public override void Spawned()
        {
            base.Spawned();

            if (Instance != null)
            {
                Runner.DestroySingleton<GameManager>();
                Runner.Despawn(gameObject.GetComponent<NetworkObject>());
                return;
            }
            
            Instance = this as T;
            Runner.MakeDontDestroyOnLoad(gameObject);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}