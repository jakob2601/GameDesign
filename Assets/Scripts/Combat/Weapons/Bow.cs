using UnityEngine;
using Scripts.Combats.CharacterCombats;
using Scripts.Movements.AI;

namespace Scripts.Combats.Weapons
{
    public class Bow : Weapon
    {

        public override void PerformAttack(MovementAI characterDirection, LayerMask enemyLayers)
        {
            base.Start();
            // Füge hier die Logik für den Pfeil hinzu
        }
    }
}