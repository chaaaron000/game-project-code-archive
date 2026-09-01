using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace Nostal.Map
{
    public class NostalgiaTile : NetworkBehaviour
    {
        [SerializeField] private List<Transform> m_playerSpawnPositions = new List<Transform>();
        [SerializeField] private List<Transform> m_itemPositions = new List<Transform>();
        [SerializeField] private List<Transform> m_exitPositions = new List<Transform>();
        [SerializeField] private List<Transform> m_mobPositions = new List<Transform>();
        [SerializeField] private List<AngrySpawner> m_angrySpawners = new List<AngrySpawner>();
        [SerializeField] private List<FemaleSpawner> m_femaleSpawners = new List<FemaleSpawner>();

        public List<Transform> PlayerSpawnPositions => m_playerSpawnPositions;
        public List<Transform> ItemPositions => m_itemPositions;
        public List<Transform> ExitPositions => m_exitPositions;
        public List<Transform> MobPositions => m_mobPositions;
        public List<AngrySpawner> AngrySpawners => m_angrySpawners;
        public List<FemaleSpawner> FemaleSpawners => m_femaleSpawners;
    }
}