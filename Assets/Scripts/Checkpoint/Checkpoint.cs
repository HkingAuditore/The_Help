using System.Collections.Generic;
using UnityEngine;

namespace Checkpoint
{
    public class Checkpoint : MonoBehaviour
    {
        public int hp = 100;
        public int maxHp = 100;
        public bool isSafe = false;
        public int index;
        public int strength = 10;
        public float radius = 5;
        public float recovery = 1;
        public List<Checkpoint> linkedCheckpoints = new List<Checkpoint>();
    
        public Vector3 GetPosition_2D() => new Vector3(transform.position.x,0,transform.position.z);

        private List<Checkpoint> _linkingCheckpoints = new List<Checkpoint>();
        
        
    }
}
