using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace Nostal
{
    public class GameplayEventManager : NetworkBehaviour
    {
        public static event UnityAction<bool> GameOver;
        public static event UnityAction<PlayerRef> PlayerChaseStarted;
        public static event UnityAction<PlayerRef> PlayerChaseEnded;
        public static event UnityAction<PlayerRef> PlayerRevived;
        public static event UnityAction<PlayerRef> JumpscareEnded;
        public static event UnityAction StoperItemStarted;
        public static event UnityAction StoperItemEnded;
        public static event UnityAction ChaseMapReset;
        public static event UnityAction ChaseMapClear;
        public static event UnityAction TutorialMapReset;

        [Rpc]
        public static void OnGameOverRPC(NetworkRunner runner, bool bIsClear)
        {
            GameOver?.Invoke(bIsClear); 
        }

        [Rpc]
        public static void OnPlayerChaseStartedRPC(NetworkRunner runner, PlayerRef chasedPlayerRef)
        {
            PlayerChaseStarted?.Invoke(chasedPlayerRef);
        }
        
        [Rpc]
        public static void OnPlayerChaseEndedRPC(NetworkRunner runner, PlayerRef chasedPlayerRef)
        {
            PlayerChaseEnded?.Invoke(chasedPlayerRef);
        }

        [Rpc]
        public static void OnPlayerRevivedRPC(NetworkRunner runner, PlayerRef revivedPlayerRef)
        {
            PlayerRevived?.Invoke(revivedPlayerRef);
        }

        [Rpc]
        public static void OnJumpscareEndedRPC(NetworkRunner runner, PlayerRef scaredPlayerRef)
        {
            JumpscareEnded?.Invoke(scaredPlayerRef);
        }

        [Rpc]
        public static void OnStoperItemStartedRPC(NetworkRunner runner)
        {
            StoperItemStarted?.Invoke();
        }

        [Rpc]
        public static void OnStoperItemEndedRPC(NetworkRunner runner)
        {
            StoperItemEnded?.Invoke();
        }

        [Rpc]
        public static void OnChaseMapResetRPC(NetworkRunner runner)
        {
            ChaseMapReset?.Invoke();
        }

        [Rpc]
        public static void OnChaseMapClearRPC(NetworkRunner runner)
        {
            ChaseMapClear?.Invoke();
        }
        
        [Rpc]
        public static void OnTutorialMapResetRPC(NetworkRunner runner)
        {
            TutorialMapReset?.Invoke();
        }   
    }
}