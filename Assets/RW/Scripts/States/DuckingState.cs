using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class DuckingState : GroundedState
    {
        private bool belowCeiling;
        private bool crouchHeld;

        public DuckingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            character.SetAnimationBool(character.crouchParam, true);
            speed = character.CrouchSpeed;
            rotationSpeed = character.CrouchRotationSpeed;
            character.ColliderSize = character.CrouchColliderHeight;
            belowCeiling = false;
        }

        public override void Exit()
        {
            base.Exit();
            character.SetAnimationBool(character.crouchParam, false);
            character.ColliderSize = character.NormalColliderHeight;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            crouchHeld = Input.GetButton("Fire3");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (!(crouchHeld || belowCeiling))
            {
                stateMachine.ChangeState(character.standing);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            belowCeiling = character.CheckCollisionOverlap(character.transform.position +
                Vector3.up * character.NormalColliderHeight);
        }

    }
}
