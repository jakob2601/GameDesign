using System.Collections;
using System.Collections.Generic;
using Scripts.Combats.Weapons;
using Scripts.Movements;
using Scripts.Movements.AI;
using UnityEngine;
using MyGame;

namespace Scripts.Combats.CharacterCombats
{
    public class EnemyCombat : Combat
    {
        [SerializeField] private Transform player;
        [SerializeField] private ContactDamage contactDamage;
        [SerializeField] public float startAttackRange = 1f; // Reichweite, in der der Gegner den Spieler versucht anzugreifen 
        [SerializeField] protected Sword sword;


        public override MovementAI getCharacterMovement()
        {
            return transform.root.GetComponentInChildren<EnemyMovementAI>();
        }

        public void SetPlayer(Transform playerTransform)
        {
            player = playerTransform;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            // Get contact damage component
            contactDamage = GetComponentInChildren<ContactDamage>();
            if (contactDamage != null)
            {
                contactDamage.SetEnemyLayer(enemyLayer);
            }
            else
            {
                Debug.LogWarning("ContactDamage component not found on " + gameObject.name);
            }

            // Get sword component - now optional
            sword = GetComponentInChildren<Sword>();
            if (sword != null)
            {
                sword.SetEnemyLayer(enemyLayer);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void CheckCombatInput()
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.position);

                if (distanceToPlayer <= startAttackRange)
                {
                    // Enable contact damage if available
                    if (HasWeapon(WeaponTypes.ContactDamage) && contactDamage != null)
                    {
                        contactDamage.SetIsEnabled(true);
                    }

                    // Stelle sicher, dass der Gegner nur dann angreift, wenn er nicht bereits im Angriff ist
                    if (combatEnabled && !isAttacking)
                    {
                        gotInput = true;
                        isSwordAttack = HasWeapon(WeaponTypes.Sword);
                        lastInputTime = Time.time;
                    }
                }
                else
                {
                    if (HasWeapon(WeaponTypes.ContactDamage) && contactDamage != null)
                    {
                        contactDamage.SetIsEnabled(false);
                    }
                }
            }
        }




        protected override void CheckAttacks()
        {
            if (gotInput && !isAttacking)
            {
                if (HasWeapon(WeaponTypes.Sword) && isSwordAttack)
                {
                    gotInput = false;
                    isAttacking = true;
                    isFirstAttack = !isFirstAttack;
                    characterAnimation.SetSwordAttackAnimation(true);
                    SoundManager.PlaySound(SoundType.SWING);
                }
                else
                {
                    // No valid weapon for attacking
                    gotInput = false;
                }
            }

            if (Time.time >= lastInputTime + inputTimer)
            {
                gotInput = false;
            }
        }

        public override void FinishSwordAttack()
        {
            isAttacking = false;
            isSwordAttack = false;
            StartCoroutine(AttackCooldown(1.0f)); // 1 Sekunde Wartezeit zwischen Angriffen
        }

        private IEnumerator AttackCooldown(float cooldown)
        {
            combatEnabled = false; // Angriff vorübergehend deaktivieren
            yield return new WaitForSeconds(cooldown);
            combatEnabled = true;  // Nach Ablauf der Zeit wieder aktivieren
        }


        public override void FinishBowAttack()
        {
            isAttacking = false;
            isBowAttack = false;
        }

        public void CancelAttack()
        {
            isAttacking = false;
            gotInput = false;
            isSwordAttack = false;
            isBowAttack = false;

            // Animation zurücksetzen
            characterAnimation.SetSwordAttackAnimation(false);
            characterAnimation.SetBowAttackAnimation(false);
        }

    }

}
