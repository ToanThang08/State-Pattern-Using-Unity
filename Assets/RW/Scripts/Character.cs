using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Character : MonoBehaviour
    {
        #region Variables


#pragma warning disable 0649
        [SerializeField]
        private Transform handTransform;
        [SerializeField]
        private Transform sheathTransform;
        [SerializeField]
        private Transform shootTransform;
        [SerializeField]
        private CharacterData data;
        [SerializeField]
        private LayerMask whatIsGround;
        [SerializeField]
        private Collider hitBox;
        [SerializeField]
        private Animator anim;
        [SerializeField]
        private ParticleSystem shockWave;
#pragma warning restore 0649
        [SerializeField]
        private float meleeRestThreshold = 10f;
        [SerializeField]
        private float diveThreshold = 1f;
        [SerializeField]
        private float collisionOverlapRadius = 0.1f;

        private GameObject currentWeapon;
        private Quaternion currentRotation;
        private int horizonalMoveParam = Animator.StringToHash("H_Speed");
        private int verticalMoveParam = Animator.StringToHash("V_Speed");
        private int shootParam = Animator.StringToHash("Shoot");
        private int hardLanding = Animator.StringToHash("HardLand");
        #endregion

        #region Properties

        public float NormalColliderHeight => data.normalColliderHeight;
        public float CrouchColliderHeight => data.crouchColliderHeight;
        public float DiveForce => data.diveForce;
        public float JumpForce => data.jumpForce;
        public float MovementSpeed => data.movementSpeed;
        public float CrouchSpeed => data.crouchSpeed;
        public float RotationSpeed => data.rotationSpeed;
        public float CrouchRotationSpeed => data.crouchRotationSpeed;
        public GameObject MeleeWeapon => data.meleeWeapon;
        public GameObject ShootableWeapon => data.staticShootable;
        public float DiveCooldownTimer => data.diveCooldownTimer;
        public float CollisionOverlapRadius => collisionOverlapRadius;
        public float DiveThreshold => diveThreshold;
        public float MeleeRestThreshold => meleeRestThreshold;
        public int isMelee => Animator.StringToHash("IsMelee");
        public int crouchParam => Animator.StringToHash("Crouch");

        public float ColliderSize
        {
            get => GetComponent<CapsuleCollider>().height;

            set
            {
                GetComponent<CapsuleCollider>().height = value;
                Vector3 center = GetComponent<CapsuleCollider>().center;
                center.y = value / 2f;
                GetComponent<CapsuleCollider>().center = center;
            }
        }

        #endregion

        #region Methods

        public void Move(float speed, float rotationSpeed)
        {
            Vector3 targetVelocity = speed * transform.forward * Time.deltaTime;
            targetVelocity.y = GetComponent<Rigidbody>().velocity.y;
            GetComponent<Rigidbody>().velocity = targetVelocity;

            GetComponent<Rigidbody>().angularVelocity = rotationSpeed * Vector3.up * Time.deltaTime;

            if (targetVelocity.magnitude > 0.01f || GetComponent<Rigidbody>().angularVelocity.magnitude > 0.01f)
            {
                SoundManager.Instance.PlayFootSteps(Mathf.Abs(speed));
            }

            anim.SetFloat(horizonalMoveParam, GetComponent<Rigidbody>().angularVelocity.y);
            anim.SetFloat(verticalMoveParam, speed * Time.deltaTime);
        }

        public void ResetMoveParams()
        {
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            anim.SetFloat(horizonalMoveParam, 0f);
            anim.SetFloat(verticalMoveParam, 0f);
        }

        public void ApplyImpulse(Vector3 force)
        {
            GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
        }

        public void SetAnimationBool(int param, bool value)
        {
            anim.SetBool(param, value);
        }

        public void TriggerAnimation(int param)
        {
            anim.SetTrigger(param);
        }

        public void Shoot()
        {
            TriggerAnimation(shootParam);
            GameObject shootable = Instantiate(data.shootableObject, shootTransform.position, shootTransform.rotation);
            shootable.GetComponent<Rigidbody>().velocity = shootable.transform.forward * data.bulletInitialSpeed;
            SoundManager.Instance.PlaySound(SoundManager.Instance.shoot, true);
        }

        public bool CheckCollisionOverlap(Vector3 point)
        {
            return Physics.OverlapSphere(point, CollisionOverlapRadius, whatIsGround).Length > 0;
        }

        public void Equip(GameObject weapon = null)
        {
            if (weapon != null)
            {
                currentWeapon = Instantiate(weapon, handTransform.position, handTransform.rotation, handTransform);
            }
            else
            {
                ParentCurrentWeapon(handTransform);
            }
        }

        public void DiveBomb()
        {
            TriggerAnimation(hardLanding);
            SoundManager.Instance.PlaySound(SoundManager.Instance.hardLanding);
            shockWave.Play();
        }

        public void SheathWeapon()
        {
            ParentCurrentWeapon(sheathTransform);
        }

        public void Unequip()
        {
            Destroy(currentWeapon);
        }

        public void ActivateHitBox()
        {
            hitBox.enabled = true;
        }

        public void DeactivateHitBox()
        {
            hitBox.enabled = false;
        }

        private void ParentCurrentWeapon(Transform parent)
        {
            if (currentWeapon.transform.parent == parent)
            {
                return;
            }

            currentWeapon.transform.SetParent(parent);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
        #endregion

        #region MonoBehaviour Callbacks


        #endregion
    }
}
