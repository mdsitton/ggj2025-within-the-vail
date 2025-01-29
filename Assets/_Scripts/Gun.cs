using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
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
    public AudioSource dryFireSFX;
    public AudioSource clipEjectSFX;
    public AudioSource cockSFX;
    public Transform firePoint;
    public GameObject casingPrefab;
    public GameObject clipPrefab;
    public Transform casingPoint; // Casing ejection spawn
    public Transform clipPoint; // Clip ejection spawn

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
    [Min(0.1f)]
    public float casingEjectionForce = 1f;
    [Min(0.1f)]
    public float bulletImpactForce = 3f;
    [Min(1)]
    public int damage = 10;

    [Header("Modes")]
    public bool auto = false;
    public bool safety = false;

    [HideInInspector]
    public UnityEvent onFire;
    public UnityEvent onHitWall;
    public UnityEvent onHitEnemy;
    public UnityEvent onDryFire;
    public UnityEvent onReload;
    public UnityEvent onEndReload;

    private float reloadStarted = -1; // Time.time that reload started.
    private bool reloading = false; // Is currently reloading?
    private float nextFire = -1; // Next Time.time available to fire.
    private bool triggerHeld = false; // Is trigger still held since last fire?

    private Rigidbody rb;
    #endregion
    
    void Start()
    {
        // Initialize here if needed
        useAction.action.started += Reload;

        rb = GetComponent<Rigidbody>();
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

        if (clipEjectSFX != null)
            clipEjectSFX.Play();

        if (clipPrefab != null && clipPoint != null)
            if (Instantiate(clipPrefab, clipPoint.position, clipPoint.rotation).TryGetComponent<Rigidbody>(out var clipRB))
            {
                Debug.Log(rb.velocity);
                clipRB.velocity = rb.velocity*10;
            }

        onReload.Invoke();
    }

    public void EndReload()
    {
        reloadStarted = -1;
        reloading = false;

        currAmmo = maxAmmo;

        if (reloadCanvas != null)
            reloadCanvas.SetActive(false);
        if (cockSFX != null)
            cockSFX.Play();

        onEndReload.Invoke();
    }

    public void TriggerDown ()
    {
        if (CanFire() && (auto? true : !triggerHeld))
        {
            Fire();
            triggerHeld = true;
        }
        else if (JustOutOfAmmo())
        {
            dryFireSFX.Play();
            onDryFire.Invoke();
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

        // Casing FX
        if (casingPrefab != null && casingPoint != null)
        {
            if (Instantiate(casingPrefab, casingPoint.position, casingPoint.rotation).TryGetComponent<Rigidbody>(out var casingRB))
                casingRB.AddForce(casingPoint.right * casingEjectionForce);
        }

        // Raycast Logic
        RaycastHit hit;
        UnityEngine.Debug.DrawRay(firePoint.position, firePoint.forward * 10, Color.red, 0.1f);
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 70, enemyLayerMask))
        {
            // Hit Enemy Splatter FX
            if (hitEnemyVFX != null)
                Instantiate(hitEnemyVFX, hit.point, Quaternion.LookRotation(hit.normal));

            // Hit Enemy Push Back
            if (hit.collider.attachedRigidbody != null)
                hit.collider.attachedRigidbody.AddForce(-hit.normal * bulletImpactForce);

            // Hit Enemy Logic
            hit.collider.GetComponentInParent<ITarget>()?.Hit(damage, PlayerManager.instance);

            onHitEnemy.Invoke();
        }
        else if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, 70, wallLayerMask))
        {
            // Hit Wall FX
            if (hitWallVFX != null)
                Instantiate(hitWallVFX, hit.point, Quaternion.LookRotation(hit.normal));

            // Hit Wall Push Back
            if (hit.collider.attachedRigidbody != null)
                hit.collider.attachedRigidbody.AddForce(-hit.normal * bulletImpactForce);

            onHitWall.Invoke();
        }

        onFire.Invoke();
    }

    public bool CanFire ()
    {
        return Time.time >= nextFire && currAmmo > 0 && safety == false && reloading == false;
    }

    public bool JustOutOfAmmo ()
    {
        return Time.time >= nextFire && currAmmo <= 0 && safety == false && reloading == false;
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