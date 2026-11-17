using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterAudio : MonoBehaviour
{
    [Header("FMOD Setup")]
    public EventReference footstepEvent;
    public bool isPlayer = false; 
    
    [Header("Volume Setup")]
    [Range(0f, 15f)] public float volumeMultiplier = 1f;

    [Header("Surface Detection")]
    [Tooltip("The physics layer containing your floor zones")]
    public LayerMask floorLayer;
    private int currentSurfaceIndex = 0; // Made private so it is strictly controlled by the script

    [Header("Timing (Stopwatch)")]
    [Tooltip("Seconds between footsteps while walking")]
    public float walkStepInterval = 0.45f;
    
    private float stepTimer = 0f;
    private bool wasMoving = false; 
    private float currentMomentum = 0f;
    private Rigidbody2D rb;
    private FMOD.Studio.EventDescription footstepDesc; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Preload the audio samples into RAM so they play instantly without lag
        footstepDesc = RuntimeManager.GetEventDescription(footstepEvent);
        footstepDesc.loadSampleData();
    }

    void Update()
    {
        float currentVelocity = rb.linearVelocity.magnitude;
        bool isMoving = currentVelocity > 0.1f;
        
        
        int fmodSpeedParam = 1; 
        float targetInterval = walkStepInterval;

        if (currentVelocity > 6.5f) // Sprinting
        {
            fmodSpeedParam = 2;
            targetInterval = walkStepInterval * 0.6f; 
        }
        else if (isMoving && currentVelocity < 2.5f) // Sneaking
        {
            fmodSpeedParam = 0;
            targetInterval = walkStepInterval * 1.5f; 
        }

        // 3. Dynamic Momentum Swell (Player Only)
        if (isPlayer)
        {
            float targetMomentum = 0f;
            float growthRate = 3.0f; // Fast decay when stopped

            if (isMoving)
            {
                targetMomentum = (fmodSpeedParam == 2) ? 1.0f : ((fmodSpeedParam == 1) ? 0.75f : 0.2f);
                growthRate = (fmodSpeedParam == 2) ? 0.6f : 0.4f; 
            }
            
            currentMomentum = Mathf.MoveTowards(currentMomentum, targetMomentum, Time.deltaTime * growthRate);
        }

        // 4. THE TIME TRACKER & MICRO-STEP FIX
        if (isMoving)
        {
            if (!wasMoving)
            {
                PlayFootstep(fmodSpeedParam); // Instantly play the first step
                stepTimer = 0f;               
            }
            else
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= targetInterval)
                {
                    PlayFootstep(fmodSpeedParam);
                    stepTimer = 0f; 
                }
            }
        }
        else 
        {
            stepTimer = 0f;
        }

        wasMoving = isMoving;
    }
    private void UpdateSurface()
    {
        // Checks a tiny point exactly at the character's feet
        Collider2D floorCollider = Physics2D.OverlapPoint(rb.position, floorLayer);
    
        if (floorCollider != null)
        {
            if (floorCollider.CompareTag("Concrete")) currentSurfaceIndex = 0;
            else if (floorCollider.CompareTag("Wood")) currentSurfaceIndex = 1;
            else if (floorCollider.CompareTag("Metal")) currentSurfaceIndex = 2;
            else if (floorCollider.CompareTag("Ground")) currentSurfaceIndex = 3;
            else if (floorCollider.CompareTag("Rock")) currentSurfaceIndex = 4;
        }
    }

    private void PlayFootstep(int speedParam)
    {
        UpdateSurface();
        FMOD.Studio.EventInstance footstep = RuntimeManager.CreateInstance(footstepEvent);
        RuntimeManager.AttachInstanceToGameObject(footstep, gameObject); 
        
        footstep.setParameterByName("Surface", currentSurfaceIndex);
        footstep.setParameterByName("Speed", speedParam);
        footstep.setParameterByName("Momentum", isPlayer ? currentMomentum : 0f);
        
        // Brute-force the volume natively in the engine mix
        footstep.setVolume(volumeMultiplier);
        
        footstep.start();
        footstep.release();
    }

    void OnDestroy()
    {
        // Free the RAM when the character dies or the scene changes
        if (footstepDesc.isValid())
        {
            footstepDesc.unloadSampleData();
        }
    }
}