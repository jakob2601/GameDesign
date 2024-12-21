using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Movements.AI;
using Scripts.Combats.Weapons;
using Scripts.Movements;

namespace Scripts.Combats.CharacterCombats
{
    public class PlayerCombat : Combat
    {
        
        private Sword sword;

        protected override void Start()
        {
            base.Start();
            sword = GetComponent<Sword>();
        }

        // Update is called once per frame
        void Update()
        {

            // Angriff ausführen
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
                {   
                    sword.PerformAttack(playerDirection, enemyLayers);
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        }

        public override MovementAI getCharacterDirection()
        {
            return GetComponent<PlayerMovementAI>();
        }

    }

}
