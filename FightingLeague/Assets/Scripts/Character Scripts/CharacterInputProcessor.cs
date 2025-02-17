﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterControl
{
    public class CharacterInputProcessor : MonoBehaviour
    {
        private CharacterStateController stateController;
		private int charID;

        private Rigidbody myRigidbody;

        private float horizontalInput;

        private float verticalInput;

		

		private AnimatorParameters animatorParameters;

		private Enums.Inputs lastInput;

		private Enums.Inputs latestDirection;

		private Enums.FacingSide nextFace;

		private bool airborn;

		private Enums.AttackState lastAtk = Enums.AttackState.none;

		private String horizontalBtn;
		private String verticalBtn;

		[SerializeField]
		private AnimationController animControl;

		[SerializeField]
		private List<Enums.AttackState> attackStates;
		[SerializeField]
		private FiniteStateMachineState motionStateMachine;

		// Use this for initialization
		void Start()
        {
            myRigidbody = GetComponent<Rigidbody>();
            stateController = GetComponent<CharacterStateController>();
			animControl = GetComponent<AnimationController>();
			latestDirection = Enums.Inputs.Neutral;
			airborn = false;
			animControl.SetRigidBody(myRigidbody);
			animatorParameters = new AnimatorParameters(animControl.GetAllBoolTriggerAnimatorParameters());
			motionStateMachine = GetComponent<FiniteStateMachineState>();
			attackStates = new List<Enums.AttackState>();
			charID = stateController.GetPlayerID();
			if(PlayerPrefs.GetInt("P1Arrows") == 1)
			{
				horizontalBtn = "HorizontalArrows";
				verticalBtn = "VerticalArrows";
			}
			else
			{
				horizontalBtn = "HorizontalP";
				verticalBtn = "VerticalP";
			}
		}

		// Update is called once per frame
		void Update()
        {
            horizontalInput = Input.GetAxisRaw(horizontalBtn + charID.ToString());
            verticalInput = Input.GetAxisRaw(verticalBtn + charID.ToString());

            switch ((int)horizontalInput)
            {
                case 1:
                    if (verticalInput > 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Right, Enums.NumPad.Up);
                    }
                    else if (verticalInput == 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Right, Enums.NumPad.Neutral);
                    }
                    else if (verticalInput < 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Right, Enums.NumPad.Down);
                    }
                    break;

                case 0:
                    if (verticalInput > 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Neutral, Enums.NumPad.Up);
                    }
                    else if (verticalInput == 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Neutral, Enums.NumPad.Neutral);
                    }
                    else if (verticalInput < 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Neutral, Enums.NumPad.Down);
                    }
                    break;

                case -1:
                    if (verticalInput > 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Left, Enums.NumPad.Up);
                    }
                    else if (verticalInput == 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Left, Enums.NumPad.Neutral);
                    }
                    else if (verticalInput < 0)
                    {
                        TranslateDirectionalInput(Enums.NumPad.Left, Enums.NumPad.Down);
                    }
                    break;

            }

            //Attacks
            if (Input.GetButtonDown("LightP" + charID.ToString()))
            {
                stateController.SetCharState(Enums.CharState.attacking);
                attackStates.Add(Enums.AttackState.light);
            }
            if (Input.GetButtonDown("MediumP" + charID.ToString()))
            {
                stateController.SetCharState(Enums.CharState.attacking);
				attackStates.Add(Enums.AttackState.medium);
            }
            if (Input.GetButtonDown("HeavyP" + charID.ToString()))
            {
                stateController.SetCharState(Enums.CharState.attacking);
				attackStates.Add(Enums.AttackState.heavy);
            }

            if (Input.GetButtonDown("CancelP" + charID.ToString()))
            {
                stateController.CancelSpecial();
            }

            if (Input.GetButtonDown("FireballP" + charID.ToString()))
            {
                animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "special1" }));
            }
            if (Input.GetButtonDown("SpinKickP" + charID.ToString()))
            {
                animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "special2" }));
            }
            if (Input.GetButtonDown("SuperP" + charID.ToString()))
            {
                if (stateController.GetSuperBar() > 49)
                {
                    stateController.Super();
                    stateController.UpdateUI(false);
                    stateController.SetLastAtk(Enums.AttackState.super);
                    stateController.SetCharState(Enums.CharState.attacking);
                }
            }
            if (Input.GetButtonDown("JSCP" + charID.ToString()))
            {
                animControl.Jump();
                if (stateController.GetSuperBar() > 49)
                {
                    stateController.Super();
                    stateController.UpdateUI(false);
                    stateController.SetLastAtk(Enums.AttackState.super);
                    stateController.SetCharState(Enums.CharState.attacking);
                }
            }
        }

		public void TranslateDirectionalInput(Enums.NumPad xAxis, Enums.NumPad yAxis)
		{

			if (stateController.GetFacingSide() == Enums.FacingSide.P1)
			{
				if (yAxis == Enums.NumPad.Down)
				{
					if (xAxis == Enums.NumPad.Left)
					{
						lastInput = Enums.Inputs.DownBack;

					}
					else if (xAxis == Enums.NumPad.Right)
					{
						lastInput = Enums.Inputs.DownFront;

					}
					else if (xAxis == Enums.NumPad.Neutral)
					{
						lastInput = Enums.Inputs.Down;
					}
				}
				else if (yAxis == Enums.NumPad.Neutral)
				{

					if (xAxis == Enums.NumPad.Left)
					{
						lastInput = Enums.Inputs.Back;

					}
					else if (xAxis == Enums.NumPad.Right)
					{
						lastInput = Enums.Inputs.Front;

					}
					else if (xAxis == Enums.NumPad.Neutral)
					{
						lastInput = Enums.Inputs.Neutral;
					}
				}
			}
			else if (stateController.GetFacingSide() == Enums.FacingSide.P2)
			{
				if (yAxis == Enums.NumPad.Down)
				{
					if (xAxis == Enums.NumPad.Left)
					{
						lastInput = Enums.Inputs.DownFront;
					}
					else if (xAxis == Enums.NumPad.Right)
					{
						lastInput = Enums.Inputs.DownBack;
					}
					else if (xAxis == Enums.NumPad.Neutral)
					{
						lastInput = Enums.Inputs.Down;
					}
				}
				else if (yAxis == Enums.NumPad.Neutral)
				{
					if (xAxis == Enums.NumPad.Left)
					{
						lastInput = Enums.Inputs.Front;
					}
					else if (xAxis == Enums.NumPad.Right)
					{
						lastInput = Enums.Inputs.Back;
					}
					else if (xAxis == Enums.NumPad.Neutral)
					{
						lastInput = Enums.Inputs.Neutral;
					}
				}
			}

			if (yAxis == Enums.NumPad.Up)
			{
				lastInput = Enums.Inputs.Up;
			}

			lastInput = motionStateMachine.PerformTransition(lastInput, attackStates);


			if (attackStates.Count == 0)
			{
				switch (lastInput)
				{
					case Enums.Inputs.Back:
						animControl.WalkBwd();
						break;

					case Enums.Inputs.DownBack:
						animControl.WalkBwd();
						break;

					case Enums.Inputs.Down:
						break;

					case Enums.Inputs.DownFront:
						animControl.WalkFwd();
						break;

					case Enums.Inputs.Front:
						animControl.WalkFwd();
						break;

					case Enums.Inputs.Up:
						animControl.Jump();
						break;

					case Enums.Inputs.Neutral:

						break;
				}
			}
			else
			{
				switch (lastInput)
				{
					case Enums.Inputs.Light:
						animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "lightAttack" }));
						break;

					case Enums.Inputs.Medium:
						animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "mediumAttack" }));
						break;

					case Enums.Inputs.Heavy:
						animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "heavyAttack" }));
						break;

					case Enums.Inputs.Special1:
						animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "special1" }));
						SetLastAtk(Enums.AttackState.special1);
						stateController.SetCharState(Enums.CharState.attacking);
						break;

					case Enums.Inputs.Special2:
						animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "special2" }));
						SetLastAtk(Enums.AttackState.special2);
						stateController.SetCharState(Enums.CharState.attacking);
						break;

					case Enums.Inputs.Super:
						if (stateController.GetSuperBar() > 49)
						{
                            stateController.Super();
							stateController.UpdateUI(false);
							stateController.SetLastAtk(Enums.AttackState.super);
							stateController.SetCharState(Enums.CharState.attacking);
						}
						else
						{
							animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "special1" }));
						}
						break;

					case Enums.Inputs.Vanish:
						if (stateController.GetSuperBar() > 9)
						{
                            stateController.Vanish();
                            stateController.UpdateUI(false);
                        }
                        else
                        {
                            animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "special2" }));
                        }
                        break;

					case Enums.Inputs.GuardBreak:
						animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "guardBreak" }));
						break;

					case Enums.Inputs.Dash:
                        if (stateController.GetSuperBar() > 4)
                        {
                            stateController.ForwardDash();
                            stateController.UpdateUI(false);
                        }
						break;

					case Enums.Inputs.Reflect:
						animControl.TriggerAnimatorParameters(animatorParameters.FindAnimatorParameter(new string[] { "reflect" }));
						break;
				}
				animControl.TurnAnimatorParametersOff(animatorParameters.FindAnimatorParameter(new string[] { "walkingForward", "walkingBackward"}));
			}
			ResetEnumState();
		}

		public void SetLastAtk(Enums.AttackState atk)
		{
			lastAtk = atk;
		}

		private void ResetEnumState()
		{
			attackStates = new List<Enums.AttackState>();
			SetLastAtk(Enums.AttackState.none);

		}
	}
}

