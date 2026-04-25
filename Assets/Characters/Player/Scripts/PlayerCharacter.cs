using UnityEngine;

namespace OneTry.Characters
{
    /// <summary>
    /// Root component on the PlayerMannequin prefab.
    ///
    /// Responsibilities in this iteration:
    ///   - Play the idle animation loop when the game starts.
    ///   - Expose a clean hook for future locomotion / combat controllers.
    ///
    /// The multi-part body hierarchy (see PlayerMannequin.prefab) intentionally
    /// mirrors a standard humanoid bone layout so it can be swapped for a
    /// skinned mesh with IK Retargeter later (SIGNALIS step).
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class PlayerCharacter : MonoBehaviour
    {
        [Tooltip("Speed at which the character rotates toward movement direction.")]
        [SerializeField] private float _rotationSpeed = 10f;

        private Animator _animator;
        private static readonly int _idleHash = Animator.StringToHash("Idle");

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _animator.SetTrigger(_idleHash);
        }

        /// <summary>
        /// Called by the future locomotion controller to face a direction.
        /// No-op in the mannequin/idle-only state.
        /// </summary>
        public void FaceDirection(Vector3 worldDirection)
        {
            if (worldDirection.sqrMagnitude < 0.001f) return;
            Quaternion target = Quaternion.LookRotation(worldDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation, target, Time.deltaTime * _rotationSpeed);
        }
    }
}
