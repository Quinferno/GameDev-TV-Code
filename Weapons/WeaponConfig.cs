using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Core;
using RPG.Saving.Inventories;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] AnimatorOverrideController weaponAnimationOverride;
        [SerializeField] Weapon equippedPrefab = null;
        [SerializeField] float weaponBaseDamage = 5;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] float weaponRange = 2f;
        [SerializeField] bool isRightHanded = true;//most weapons will be right-handed but some may need to be left-handed for simplicity and anims.
        [SerializeField] bool isSpellbook = false;
        [SerializeField] Projectile projectile = null;//Holds object this weapon launches (and will be null for most weapons)
        [SerializeField] ElementalDamage[] elementalDamages;

        const string weaponName = "Weapon";
        public Weapon SpawnWeapon(Transform rightHandTransform, Transform leftHandTransform, Transform spellbookTransform, Animator animator)
        {
            DestroyOldWeapon(rightHandTransform, leftHandTransform, spellbookTransform);

            Weapon weapon = null;

            if (equippedPrefab != null)//Only tries to make prefab if it has one. Mostly for used Unarmed.
            {
                Transform handTransform = GetTransforms(rightHandTransform, leftHandTransform, spellbookTransform);

                weapon = Instantiate(equippedPrefab, handTransform);

                weapon.name = weaponName;
            }

            if (weaponAnimationOverride != null)
            {
            animator.runtimeAnimatorController = weaponAnimationOverride;
            }
            else
            {
                var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
                if(overrideController != null)
                {
                    animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
                }
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHandTransform, Transform leftHandTransform, Transform spellbookTransform)
        {
            Transform oldWeapon = rightHandTransform.Find("Weapon");
            if(oldWeapon == null) oldWeapon = leftHandTransform.Find("Weapon");
            if(oldWeapon == null) oldWeapon = spellbookTransform.Find("Weapon");
            if(oldWeapon == null) return;

            oldWeapon.name = "DESTROYING WEAPON";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransforms(Transform rightHandTransform, Transform leftHandTransform, Transform spellbookTransform)
        {
            Transform handTransform;
            
            if (isSpellbook == true) 
            {
                handTransform = spellbookTransform;
                return handTransform;
            }
            if (isRightHanded == false) handTransform = leftHandTransform;
            else handTransform = rightHandTransform;
            return handTransform;
        }
        public IEnumerable<ElementalDamage> GetElementalDamages()
        {
            foreach (ElementalDamage elementalDamage in elementalDamages)
            {
                yield return elementalDamage;
            }
        }
        public bool HasProjectile()
        {
            return projectile != null;//returns true if projectile isn't null
        }
        public void LaunchProjectile(Transform rightHandTransform, Transform leftHandTransform, Transform spellbookTransform, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetTransforms(rightHandTransform, leftHandTransform, spellbookTransform).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }
        public float GetDamage()
        {
            return weaponBaseDamage;
        }

        public float GetRange()
        {
            return weaponRange;
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stats stat)
        {
            if (stat == Stats.PhysicalDamage)
            {
                yield return weaponBaseDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stats stat)
        {
            if (stat == Stats.PhysicalDamage)
            {
                yield return percentageBonus;
            }
        }
    }
}
