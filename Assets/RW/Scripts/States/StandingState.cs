using UnityEngine;

namespace RayWenderlich.Unity.StatePatternInUnity
{
    public class StandingState : GroundedState
    {
        private bool jump;
        private bool crouch;

        public StandingState(Character character, StateMachine stateMachine) : base(character, stateMachine)
        {
        }

        public override void Enter()
        {
            base.Enter();
            speed = character.MovementSpeed;
            rotationSpeed = character.RotationSpeed;
            crouch = false;
            jump = false;
        }

        public override void HandleInput()
        {
            base.HandleInput();
            crouch = Input.GetButtonDown("Fire3");
            jump = Input.GetButtonDown("Jump");
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (crouch)
            {
                stateMachine.ChangeState(character.ducking);
            }
            else if (jump)
            {
                stateMachine.ChangeState(character.jumping);
            }
        }

    }
}
