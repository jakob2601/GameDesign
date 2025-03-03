using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Healths
{
    [CreateAssetMenu(fileName = "DropConfiguration", menuName = "Game/Drop Configuration")]
    public class DropConfiguration : ScriptableObject
    {
        public List<DropReward> possibleRewards = new List<DropReward>();
        public float nothingDropChance = 0.3f;
    }
}