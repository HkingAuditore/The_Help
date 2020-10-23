using System.Collections.Generic;
using UnityEngine;

namespace Checkpoint
{
    public class Checkpoint : MonoBehaviour
    {
        public int hp = 100;
        public int maxHp = 100;
        public bool isSafe = false;
        
        private List<Checkpoint> _linkingCheckpoints = new List<Checkpoint>();
        
        
    }
}
