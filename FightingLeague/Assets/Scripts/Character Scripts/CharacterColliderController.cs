﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterControl
{
    public class CharacterColliderController : MonoBehaviour
    {

        private bool flaggedAtk = false;

        private int chosenS1 = 0;
        private int chosenS = 0;

        public void SetS1SColors(int a, int b)
        {
            if (a != 0 && a != 1) a = 0;
            if (b != 0 && b != 1) b = 0;
            chosenS1 = a;
            chosenS = b;
        }

        [SerializeField]
        private GameObject fireBallPrefab;

        [SerializeField]
        private GameObject fireBallAltPrefab;

        [SerializeField]
        private GameObject spinKickPrefab;

        [SerializeField]
        private GameObject guardBreakPrefab;

        [SerializeField]
        private GameObject reflectPrefab;

        [SerializeField]
        private GameObject heavyPrefab;

        [SerializeField]
        private GameObject beamPrefab;
        [SerializeField]
        private GameObject beamAltPrefab;

        [SerializeField]
        private GameObject otherPlayer;

        [SerializeField]
        private GameObject groundCheck;

        private bool attackingL = false;
        private bool attackingM = false;
        private bool attackingH = false;

        [SerializeField]
        private Collider cleft;
        [SerializeField]
        private Collider cright;
        [SerializeField]
        private Collider fleft;
        [SerializeField]
        private Collider fright;
        [SerializeField]
        private Collider tleft;
        [SerializeField]
        private Collider tright;
        [SerializeField]
        private Collider hleft;
        [SerializeField]
        private Collider hright;
        [SerializeField]
        private Collider laleft;
        [SerializeField]
        private Collider laright;
        [SerializeField]
        private Collider ualeft;
        [SerializeField]
        private Collider uaright;

        private Animator myAnimator;

        private Rigidbody myRigidBody;
        private CharacterStateController stateController;

        [SerializeField]
        private AudioClip kick;
        [SerializeField]
        private AudioClip mpunch;
        [SerializeField]
        private AudioClip lpunch;

        public GameObject GetOtherPlayer()
        {
            return otherPlayer;
        }

        // Use this for initialization

        private void SpawnFireBall()
        {
            Vector3 temp = myRigidBody.position;
            temp.y = temp.y + 0.7f;
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1) temp.x += 0.7f;
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2) temp.x -= 0.7f;

            if (chosenS1 == 0)
            {
                var fireBall = (GameObject)Instantiate(
                fireBallPrefab,
                temp,
                myRigidBody.rotation);

                fireBall.GetComponent<Rigidbody>().velocity = fireBall.transform.forward * 30;
                fireBall.GetComponent<FireballScript>().SetCreator(myRigidBody);

                Destroy(fireBall, 2.0f);
            }

            if (chosenS1 == 1)
            {
                var fireBall = (GameObject)Instantiate(
                fireBallAltPrefab,
                temp,
                myRigidBody.rotation);

                fireBall.GetComponent<Rigidbody>().velocity = fireBall.transform.forward * 30;
                fireBall.GetComponent<FireballScript>().SetCreator(myRigidBody);

                Destroy(fireBall, 2.0f);
            }
        }

        private void SpinKick()
        {
            Vector3 temp = myRigidBody.position;
            temp.y = temp.y + 1;
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1) temp.x += 1;
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2) temp.x -= 1;
            var spinKick = (GameObject)Instantiate(
           spinKickPrefab,
           temp,
           Quaternion.Euler(new Vector3(0,0,0)) );
           spinKick.GetComponent<SpinKickScript>().SetCreator(myRigidBody);
           GetComponent<AnimationController>().Shockwave();
           Destroy(spinKick, 0.1f);
        }

        private void GuardBreak()
        {
            Vector3 temp = myRigidBody.position;
            temp.y = temp.y + 1;
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, 0));
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
            {
                temp.x += 1.1f;
            }
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                temp.x -= 1.1f;
            }
            var guardBreak = (GameObject)Instantiate(
           guardBreakPrefab,
           temp,
           rot);
            guardBreak.GetComponent<FlurryScript>().SetCreator(myRigidBody);
            GetComponent<AnimationController>().Shockwave();
            Destroy(guardBreak, 0.30f);
        }

        private void MidDash()
        {
            float x = 7f;
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
            {
                if (myRigidBody.transform.position.x + x > 7f)
                {
                    x = 7f - myRigidBody.transform.position.x;
                    if(otherPlayer.transform.position.x > 5.5f) otherPlayer.transform.position = new Vector3(5.5f, otherPlayer.transform.position.y, otherPlayer.transform.position.z);
                }
                myRigidBody.transform.position += new Vector3(x, 0.33f, 0.0f);
            }
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                if (myRigidBody.transform.position.x - x < -7f)
                {
                    x = 7f + myRigidBody.transform.position.x;
                    if (otherPlayer.transform.position.x < -5.5f) otherPlayer.transform.position = new Vector3(-5.5f, otherPlayer.transform.position.y, otherPlayer.transform.position.z);
                } 
                myRigidBody.transform.position += new Vector3(-x, 0.33f, 0.0f);
            }
            GetComponent<AnimationController>().HorizontalTeleportFX();

            StartCoroutine(UpdateTeleport());
        }

        public void TechGrab()
        {
            ReflectBarrier();
        }
        private void ReflectBarrier()
        {
            Vector3 temp = myRigidBody.position;
            temp.y = temp.y + 1;
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, 0));
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
            {
                temp.x += 1f;
                //rot = Quaternion.Euler(new Vector3(0, 90, 0));
            }
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                temp.x -= 1f;
                //rot = Quaternion.Euler(new Vector3(0, 270, 0));
            }
            var reflect = (GameObject)Instantiate(
           reflectPrefab,
           temp,
           rot);
            GetComponent<AnimationController>().Shockwave();
            Destroy(reflect, 0.35f);
        }

        private void HeavyAtk()
        {
            Vector3 temp = myRigidBody.position;
            temp.y = temp.y + 1;
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, 0));
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
            {
                temp.x += 0.7f;
            }
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                temp.x -= 0.7f;
            }
            var heavy = (GameObject)Instantiate(
           heavyPrefab,
           temp,
           rot);
            heavy.GetComponent<HeavyScript>().SetCreator(myRigidBody);
            Destroy(heavy, 0.15f);
        }

        private void Beam()
        {
            Vector3 temp = myRigidBody.position;
            temp.y = temp.y + 1;
            Quaternion rot = Quaternion.Euler(new Vector3(90, 0, 90));
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
            {
                temp.x += beamPrefab.transform.localScale.y+.5f;
                //rot = Quaternion.Euler(new Vector3(0, 90, 0));
            }
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                temp.x -= beamPrefab.transform.localScale.y + .5f;
                //rot = Quaternion.Euler(new Vector3(0, 270, 0));
            }
            if (chosenS == 0)
            {
                var beam = (GameObject)Instantiate(
               beamPrefab,
               temp,
               rot);
                beam.GetComponent<SuperBeamScript>().SetCreator(myRigidBody);
                Destroy(beam, 1f);

                myRigidBody.constraints = RigidbodyConstraints.FreezeAll;
                stateController.SetRotLockState(true);
                StopAllCoroutines();
                StartCoroutine(FinishBeam());
            }

            if (chosenS == 1)
            {
                var beam = (GameObject)Instantiate(
               beamAltPrefab,
               temp,
               rot);
                beam.GetComponent<SuperBeamScript>().SetCreator(myRigidBody);
                Destroy(beam, 1f);

                myRigidBody.constraints = RigidbodyConstraints.FreezeAll;
                stateController.SetRotLockState(true);
                StopAllCoroutines();
                StartCoroutine(FinishBeam());
            }
        }

        IEnumerator FinishBeam()
        {
            yield return new WaitForSeconds(1f);
            myRigidBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            stateController.SetRotLockState(false);
            if (stateController.GetQueueRotate())
            {
                stateController.SetFacingSide(stateController.GetNextFace());
            }
        }

        private void VanishTP()
        {

            GetComponent<AnimationController>().VerticalTeleportFX();
            float x = 1.5f;
            Quaternion rot = Quaternion.Euler(new Vector3(0, 0, 0));
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
            {
                if (otherPlayer.transform.position.x + x > 7f)
                {
                    x = 0f;
                    if (otherPlayer.transform.position.x > 5.5f) otherPlayer.transform.position = new Vector3(5.5f, otherPlayer.transform.position.y, otherPlayer.transform.position.z);
                }
                myRigidBody.transform.position = new Vector3(otherPlayer.transform.position.x + x, myRigidBody.transform.position.y + 1.83f, myRigidBody.transform.position.z);
            }
            if (GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                if (otherPlayer.transform.position.x - x < -7f)
                {
                    x = 0f;
                    if (otherPlayer.transform.position.x < -5.5f) otherPlayer.transform.position = new Vector3(-5.5f, otherPlayer.transform.position.y, otherPlayer.transform.position.z);
                }
                myRigidBody.transform.position = new Vector3(otherPlayer.transform.position.x - x, myRigidBody.transform.position.y + 1.83f, myRigidBody.transform.position.z);
            }

            StartCoroutine(UpdateTeleport());
            

        }

        IEnumerator UpdateTeleport()
        {
            yield return new WaitForSeconds(0.1f);
            groundCheck.GetComponent<GroundCheckScript>().UpdateSide();
        }

        private void EnableL()
        {
            tleft.enabled = true;
            tright.enabled = true;
            hleft.enabled = true;
            hright.enabled = true;
            laleft.enabled = true;
            laright.enabled = true;
            ualeft.enabled = true;
            uaright.enabled = true;
            attackingL = true;
            flaggedAtk = false;
            StartCoroutine(ForceNoAtkL());
        }

        IEnumerator ForceNoAtkL()
        {
            yield return new WaitForSeconds(0.34f);
            DisableL();
            attackingL = false;
            flaggedAtk = true;
        }



        private void EnableM()
        {
            tleft.enabled = true;
            tright.enabled = true;
            hleft.enabled = true;
            hright.enabled = true;
            laleft.enabled = true;
            laright.enabled = true;
            ualeft.enabled = true;
            uaright.enabled = true;
            attackingM = true;
            flaggedAtk = false;
            StartCoroutine(ForceNoAtkM());
        }

        IEnumerator ForceNoAtkM()
        {
            yield return new WaitForSeconds(0.4f);
            DisableL();
            attackingM = false;
            flaggedAtk = true;
        }

        private void EnableH()
        {
            cleft.enabled = true;
            cright.enabled = true;
            fleft.enabled = true;
            fright.enabled = true;
            attackingH = true;
            flaggedAtk = false;
            GetComponent<AnimationController>().Shockwave();
            StartCoroutine(ForceNoAtkH());
        }

        IEnumerator ForceNoAtkH()
        {
            yield return new WaitForSeconds(0.34f);
            DisableL();
            attackingM = false;
            flaggedAtk = true;
        }

        private void DisableL()
        {
            tleft.enabled = false;
            tright.enabled = false;
            hleft.enabled = false;
            hright.enabled = false;
            laleft.enabled = false;
            laright.enabled = false;
            ualeft.enabled = false;
            uaright.enabled = false;
            attackingL = false;
            flaggedAtk = true;
            myAnimator.applyRootMotion = true;
    }

        private void DisableM()
        {
            tleft.enabled = false;
            tright.enabled = false;
            hleft.enabled = false;
            hright.enabled = false;
            laleft.enabled = false;
            laright.enabled = false;
            ualeft.enabled = false;
            uaright.enabled = false;
            attackingM = false;
            flaggedAtk = true;
            myAnimator.applyRootMotion = true;
        }

        private void DisableH()
        {
            cleft.enabled = false;
            cright.enabled = false;
            fleft.enabled = false;
            fright.enabled = false;
            attackingH = false;
            flaggedAtk = true;
            myAnimator.applyRootMotion = true;
        }

        private void Start()
        {
            myAnimator = GetComponent<AnimationController>().GetAnimator();
            myRigidBody = GetComponent<Rigidbody>();
            stateController = GetComponent<CharacterStateController>();

            DisableL();
            DisableM();
            DisableH();
        }

        private void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
            Rigidbody body = collision.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
                return;
            if((attackingL || attackingM || attackingH) && !flaggedAtk)
            {
                float dmg = 0;
                float bar = 0;
                if (attackingL)
                {
                    dmg = 500f;
                    bar = 5f;
                    GetComponent<AudioSource>().PlayOneShot(lpunch);
                }
                if (attackingM)
                {
                    dmg = 700f;
                    bar = 7f;
                    GetComponent<AudioSource>().PlayOneShot(mpunch);
                }
                if (attackingH)
                {
                    dmg = 400f;
                    bar = 4f;
                    myRigidBody.GetComponent<AnimationController>().Push(dmg);
                    GetComponent<AudioSource>().PlayOneShot(kick);
                }
                stateController.AddSuperBar(bar);
                body.GetComponent<CharacterStateController>().TakeDamage(dmg, false);
                body.GetComponent<CharacterStateController>().AddSuperBar(bar / 2);
                DisableL();
                DisableM();
                DisableH();
                flaggedAtk = true;
            }
        }
    }
}
