using UnityEngine;

public class ChaityaAudioTrigger : MonoBehaviour
{
    public Camera playerCamera;       // Assign your main camera here
    public float rayDistance = 10f;   // Adjust based on your scene scale
    public AudioSource chaityaAudio;  // Assign the AudioSource from Buddha Chaitya

    void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("BuddhaChaitya"))
            {
                if (!chaityaAudio.isPlaying)
                {
                    chaityaAudio.Play();
                }
            }
            else
            {
                if (chaityaAudio.isPlaying)
                {
                    chaityaAudio.Stop();
                }
            }
        }
        else
        {
            if (chaityaAudio.isPlaying)
            {
                chaityaAudio.Stop();
            }
        }
    }
}
