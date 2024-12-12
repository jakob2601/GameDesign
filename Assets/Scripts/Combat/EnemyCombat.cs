using System.Collections;
using System.Collections.Generic;
using Scripts.Movements;
using UnityEngine;

namespace Scripts.Combats
{
    public class EnemyCombat : Combat
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {

        }

        protected override PlayerMovement getCharacterDirection()
        {
            Debug.Log("Enemy Direction Noch nicht implementiert");
            return null;
        }
    }

}
