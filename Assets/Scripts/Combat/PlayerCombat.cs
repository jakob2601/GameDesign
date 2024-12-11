using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Movement;

namespace Scripts.Combat
{
    public class PlayerCombat : Combat
    {
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        void Update()
        {
            // Synchronisiere die Position des Visualisierungssprites mit attackPoint
            if (debugAttackRangeVisualizer != null && attackPoint != null)
            {
                debugAttackRangeVisualizer.transform.position = attackPoint.position;
            }

            // Steuerung der Angriffsvisualisierung
            if (debugAttackRangeVisualizer != null && ifDebug)
            {
                if (Input.GetKey(KeyCode.R) || Input.GetMouseButton(0))
                {
                    debugAttackRangeVisualizer.SetActive(true); // Visualisierung anzeigen
                }
                else
                {
                    debugAttackRangeVisualizer.SetActive(false); // Visualisierung ausblenden
                }
            }

            // Angriff ausführen
            if (Time.time >= nextAttackTime)
            {
                if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(0))
                {
                    Attack();
                    nextAttackTime = Time.time + 1f / attackRate;
                }
            }
        }

        protected override TopDownPlayerController getPlayerDirection()
        {
            return GetComponent<TopDownPlayerController>();
        }

    }

}
