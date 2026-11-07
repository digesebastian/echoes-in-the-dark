using UnityEngine;

public class LockSpriteRotation : MonoBehaviour
{
    // LateUpdate runs AFTER the behavior tree updates the parent's rotation
    void LateUpdate()
    {
        // Forces the sprite to remain flat to the 2D camera, ignoring the parent's Y-rotation
        transform.rotation = Quaternion.identity;
    }
}