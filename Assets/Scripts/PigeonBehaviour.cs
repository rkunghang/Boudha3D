using UnityEngine;

public class PigeonBehavior : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public float flyDistance = 5f;
    public float returnDistance = 10f;
    public float flySpeed = 3f;
    public Transform[] checkpoints; // Assign in Inspector

    private Vector3 startPosition;
    private bool isFlying = false;
    private int currentCheckpoint = 0;

    void Start()
    {
        startPosition = transform.position;

        // 🟢 Step 1: Randomize flight height and speed for natural variation
        flySpeed += Random.Range(-1f, 1f);      // ±1 unit speed variation
        transform.position += Vector3.up * Random.Range(-1.5f, 1.5f); // ±1.5 m height variation
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        // Player near → take off
        if (!isFlying && distance < flyDistance)
        {
            isFlying = true;
            animator.SetBool("isFlying", true);
            animator.SetBool("isLanding", false);
        }
        // Player far → land
        else if (isFlying && distance > returnDistance)
        {
            isFlying = false;
            animator.SetBool("isFlying", false);
            animator.SetBool("isLanding", true);
        }

        // Flying between checkpoints
        if (isFlying)
        {
            if (checkpoints.Length > 0)
            {
                Transform target = checkpoints[currentCheckpoint];
                transform.position = Vector3.MoveTowards(transform.position, target.position, flySpeed * Time.deltaTime);

                // Make pigeon face its flight direction
                Vector3 direction = (target.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);

                    // 🟢 Step 3: Smooth turning for natural flight
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 1.5f);
                }

                // When close enough, pick a random next checkpoint
                if (Vector3.Distance(transform.position, target.position) < 0.5f)
                {
                    currentCheckpoint = Random.Range(0, checkpoints.Length);
                }
            }
        }
        // Landing movement
        else if (animator.GetBool("isLanding"))
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, flySpeed * Time.deltaTime);

            // Reset landing flag when idle
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("BirdRig_Standing"))
            {
                animator.SetBool("isLanding", false);
            }
        }
    }
}
