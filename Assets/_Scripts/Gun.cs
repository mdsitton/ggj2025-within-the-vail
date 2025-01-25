using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    #region Properties
    [Header("References")]
    public GameObject reloadCanvas;
    public UnityEngine.UI.Image reloadProgressBar;
    public GameObject hitEnemyVFX;
    public GameObject hitWallVFX;
    public GameObject muzzleFlashVFX;
    public AudioSource shootSFX;
    public Transform firePoint;

    public LayerMask enemyLayerMask;
    public LayerMask wallLayerMask;

    public InputActionReference useAction;

    [Header("Stats")]
    [Min(1)]
    public int maxAmmo;
    public int currAmmo = 0;
    [Min(0.1f)]
    public float fireDelay = 0.2f;
    [Min(0.1f)]
    public float reloadDelay = 2f;

    [Header("Modes")]
    public bool auto = false;
    public bool safety = false;

    private float reloadStarted = -1; // Time.time that reload started.
    private bool reloading = false; // Is currently reloading?
    private float nextFire = -1; // Next Time.time available to fire.
    private bool triggerHeld = false; // Is trigger still held since last fire?
    #endregion

    void Start()
    {
        // Initialize here if needed
        EndReload();

        useAction.action.started += Reload;
    }

    void Update()
    {
        if (CanFire() && auto && triggerHeld)
            Fire();

        if (reloading)
        {
            if  (Time.time > reloadStarted + reloadDelay)
                EndReload();
            else
            {
                if (reloadProgressBar)
                    reloadProgressBar.fillAmount = (Time.time - reloadStarted) / reloadDelay;
            }
        }
    }

    public void Reload (InputAction.CallbackContext context)
    {
        Reload();
    }

    public void Reload ()
    {
        if (reloading)
            return;

        reloadStarted = Time.time;
        reloading = true;

        if (reloadCanvas != null)
        {
            reloadCanvas.SetActive(true);

            if (reloadProgressBar != null)
                reloadProgressBar.fillAmount = 0;
        }
    }

    public void EndReload()
    {
        reloadStarted = -1;
        reloading = false;

        currAmmo = maxAmmo;

        if (reloadCanvas != null)
        {
            reloadCanvas.SetActive(false);
        }
    }

    public void TriggerDown ()
    {
        if (CanFire() && (auto? true : !triggerHeld))
        {
            Fire();
            triggerHeld = true;
        }
    }

    public void TriggerUp()
    {
        triggerHeld = false;
    }

    public void Fire ()
    {
        // Spend ammo & time
        nextFire = Time.time + fireDelay;
        currAmmo--;

        // MuzzleFlash FX
        if (muzzleFlashVFX != null)
            Instantiate (muzzleFlashVFX, firePoint.position, firePoint.rotation);
        if (shootSFX != null)
            shootSFX.Play();

        RaycastHit hit;
        UnityEngine.Debug.DrawRay(firePoint.position, firePoint.forward * 10, Color.red, 0.1f);
        // Fire
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 300, enemyLayerMask))
        {
            // Hit Enemy FX
            if (hitEnemyVFX != null)
                Instantiate(hitEnemyVFX, hit.point, Quaternion.LookRotation(hit.normal));

            // Hit Enemy Logic
            //TODO
        }
        else if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 300, wallLayerMask))
        {
            // Hit Wall FX
            if (hitWallVFX != null)
                Instantiate(hitWallVFX, hit.point, Quaternion.LookRotation(hit.normal));
        }

    }

    public bool CanFire ()
    {
        return Time.time >= nextFire && currAmmo > 0 && safety == false && reloading == false;
    }

    public void SetAutoMode (bool auto)
    {
        this.auto = auto;
    }

    public void SetSafety (bool safety)
    {
        this.safety = safety;
    }
}