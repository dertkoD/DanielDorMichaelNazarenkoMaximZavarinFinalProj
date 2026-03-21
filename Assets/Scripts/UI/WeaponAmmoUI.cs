using TMPro;
using UnityEngine;

public class WeaponAmmoUI : MonoBehaviour
{
    [SerializeField] private PlayerWeaponController playerWeaponController;
    [SerializeField] private GameObject root;
    [SerializeField] private TMP_Text ammoText;

    private void Update()
    {
        if (playerWeaponController == null || !playerWeaponController.HasWeapon)
        {
            if (root != null)
                root.SetActive(false);

            return;
        }

        Weapon weapon = playerWeaponController.CurrentWeapon;
        if (weapon == null)
        {
            if (root != null)
                root.SetActive(false);

            return;
        }

        if (root != null)
            root.SetActive(true);

        if (ammoText != null)
            ammoText.text = $"{weapon.CurrentAmmoInMagazine}/{weapon.MagazineSize}";
    }
}
