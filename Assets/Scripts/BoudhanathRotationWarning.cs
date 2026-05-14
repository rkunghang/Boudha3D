using UnityEngine;
using TMPro;

/// <summary>
/// BoudhanathRotationWarning
/// Attach this script to your FPP Player (Capsule).
/// It detects whether the player is walking clockwise or anticlockwise
/// around the Boudhanath stupa centre, and displays a warning on screen
/// when moving anticlockwise.
/// </summary>
public class BoudhanathRotationWarning : MonoBehaviour
{
    [Header("Stupa Settings")]
    [Tooltip("Assign the centre of the Boudhanath stupa (an empty GameObject placed at the stupa's centre).")]
    public Transform stupaCentre;

    [Header("Warning UI")]
    [Tooltip("Assign your Warning Panel (UI Canvas > Panel).")]
    public GameObject warningPanel;

    [Tooltip("Assign the Text component inside the Warning Panel.")]
    public TMP_Text warningText;

    [Header("Detection Settings")]
    [Tooltip("Minimum speed (units/sec) the player must be moving before direction is judged.")]
    public float minMoveSpeed = 0.3f;

    [Tooltip("How smoothly the warning fades in/out. Lower = snappier.")]
    public float fadeDuration = 0.4f;

    // ── Private state ──────────────────────────────────────────────
    private Vector3 _previousPosition;
    private CanvasGroup _canvasGroup;        // for smooth fade
    private float _targetAlpha = 0f;
    private bool _warningActive = false;

    // ── Messages ───────────────────────────────────────────────────
    private const string WARNING_MSG =
        "Do NOT walk Anticlockwise!\n" +
        "Please walk CLOCKWISE\naround the Boudhanath Stupa.";

    // ──────────────────────────────────────────────────────────────
    void Start()
    {
        // Validation
        if (stupaCentre == null)
            Debug.LogError("[BoudhanathWarning] stupaCentre is not assigned!");

        if (warningPanel == null)
            Debug.LogError("[BoudhanathWarning] warningPanel is not assigned!");

        // Set up CanvasGroup for fade effect
        if (warningPanel != null)
        {
            _canvasGroup = warningPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
                _canvasGroup = warningPanel.AddComponent<CanvasGroup>();

            _canvasGroup.alpha = 0f;
            warningPanel.SetActive(true);   // keep active; alpha controls visibility
        }

        // Set warning message text
        if (warningText != null)
            warningText.text = WARNING_MSG;

        _previousPosition = transform.position;
    }

    // ──────────────────────────────────────────────────────────────
    void Update()
    {
        if (stupaCentre == null || warningPanel == null) return;

        DetectRotationDirection();
        FadeWarning();

        _previousPosition = transform.position;
    }

    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Uses a 2-D cross product on the XZ plane to determine whether
    /// the player moved clockwise or anticlockwise around the stupa.
    /// </summary>
    void DetectRotationDirection()
    {
        // Movement vector this frame (XZ plane only)
        Vector3 movement = transform.position - _previousPosition;
        movement.y = 0f;

        // Only judge direction if the player is actually moving
        if (movement.magnitude < minMoveSpeed * Time.deltaTime)
        {
            // Player is stationary – keep last warning state, no new judgment
            return;
        }

        // Vector from stupa centre to player (XZ plane)
        Vector3 toPlayer = transform.position - stupaCentre.position;
        toPlayer.y = 0f;

        // Cross product Y component:
        //   positive  →  movement is to the LEFT of toPlayer  →  anticlockwise
        //   negative  →  movement is to the RIGHT of toPlayer →  clockwise
        float cross = toPlayer.x * movement.z - toPlayer.z * movement.x;

        bool isAntiClockwise = cross > 0f;

        if (isAntiClockwise)
        {
            ShowWarning();
        }
        else
        {
            HideWarning();
        }
    }

    // ──────────────────────────────────────────────────────────────
    void ShowWarning()
    {
        if (!_warningActive)
        {
            _warningActive = true;
            _targetAlpha = 1f;
        }
    }

    void HideWarning()
    {
        if (_warningActive)
        {
            _warningActive = false;
            _targetAlpha = 0f;
        }
    }

    // ──────────────────────────────────────────────────────────────
    /// <summary>Smoothly fades the warning panel in or out.</summary>
    void FadeWarning()
    {
        if (_canvasGroup == null) return;

        _canvasGroup.alpha = Mathf.MoveTowards(
            _canvasGroup.alpha,
            _targetAlpha,
            Time.deltaTime / fadeDuration
        );
    }

    // ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Draw a gizmo in the Scene view so you can see the stupa centre.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (stupaCentre == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(stupaCentre.position, 1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, stupaCentre.position);
    }
}