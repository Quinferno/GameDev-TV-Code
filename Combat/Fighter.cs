using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Saving.Utils;
using RPG.Saving.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable//Not Dark Souls combat; more like Baldur's Gate or Diablo
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Transform spellbookTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        Health target;
        Equipment equipment;
        public WeaponConfig currentWeaponConfig;
        float timeSinceLastAttack = Mathf.Infinity;//Number is infinitely high, so you always start ready to attack
        LazyValue<Weapon> currentWeapon;
        private void Awake()
        {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if(equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if(weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start()
        {                           
            currentWeapon.ForceInit();
        }
        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if(target == null) return;
            if(target.IsDead()) return;

            if (!GetIsInRange(target.transform))
            {
                GetComponent<Mover>().MoveTo(target.transform.position, 1f);//1f for 100% max speed
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehavior();
            }
        }
        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        } 

        public Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.SpawnWeapon(rightHandTransform, leftHandTransform, spellbookTransform, animator);
        }
        private void AttackBehavior()
        {
            transform.LookAt(target.transform);
            if(timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("StopAttack");
            GetComponent<Animator>().SetTrigger("Attack");//Triggers hit event, which deals damage
        }

        void Hit() // Anim Event
        {
            if(target == null) {return;}

            float damage = GetComponent<BaseStats>().GetStat(Stats.PhysicalDamage);//eventually can check weapon for type and the modifiers the weapon should pull from via bools

            if(currentWeapon.value != null)
            {
                currentWeapon.value.OnHit();
            }

            if(currentWeaponConfig.HasProjectile() != false)
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, spellbookTransform, target, gameObject, damage);
                return;
            }

            else
            {
                target.TakeDamage(gameObject, damage);
                foreach (ElementalDamage elementalDamage in currentWeaponConfig.GetElementalDamages())
                {
                    damage = elementalDamage.amount;
                    float boosts = 0;
                    foreach (IElementalDamageProvider provider in GetComponents<IElementalDamageProvider>())
                    {
                        foreach (float amount in provider.GetElementalDamageBoost(elementalDamage.damageType))
                        {
                            boosts += amount;
                        }
                    }
                    boosts /= 100f;
                    
                    float resistances = 0;
                    foreach (IElementalResistanceProvider provider in target.GetComponents<IElementalResistanceProvider>())
                    {
                        foreach (float amount in provider.GetElementalResistance(elementalDamage.damageType))
                        {
                            resistances += amount;
                        }
                    }

                    resistances /= 100f;
                    damage += damage * boosts;
                    damage -= damage * resistances;
                    if (damage <= 0) continue;
                    target.TakeDamage(gameObject, damage);
                }
            }
            
            target.TakeDamage(gameObject, damage);
        }
        void Shoot() // Anim Event, probably unneeded
        {
            Hit();
        }
        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.transform.position) < currentWeaponConfig.GetRange();
        }
        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null) {return false;}
            if(GetComponent<Mover>().CanMoveTo(combatTarget.transform.position) == false && !GetIsInRange(combatTarget.transform))
                {
                    return false;
                }
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public Health GetTarget()
        {
            return target;
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("StopAttack");
        }
        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
}
