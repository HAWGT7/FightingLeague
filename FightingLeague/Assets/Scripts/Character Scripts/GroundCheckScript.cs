﻿using CharacterControl;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterControl
{
    public class GroundCheckScript : MonoBehaviour
    {
        [SerializeField]
        GameObject cchar;

        [SerializeField]
        GameObject p1;

        [SerializeField]
        GameObject p2;

        private void OnTriggerEnter(Collider other)
        {
            if ((p1.transform.position.x < p2.transform.position.x) && p1.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2 && p2.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1) {
                p1.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P1);
                p2.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P2);
            } else if((p1.transform.position.x > p2.transform.position.x) && p1.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1 && p2.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                p1.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P2);
                p2.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P1);
            }
            cchar.GetComponent<Animator>().applyRootMotion = true;
            cchar.GetComponent<Animator>().SetBool("airborn", false);
        }

        private void OnTriggerStay(Collider other)
        {
            Animator animator = cchar.GetComponent<Animator>();
            Rigidbody rigidbody = cchar.GetComponent<Rigidbody>();
            if (cchar.transform.position.y > 0.26)
            {
                if (cchar.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
                {
                    animator.applyRootMotion = false;
                    rigidbody.AddForce(new Vector3(-0.5f, 0), ForceMode.VelocityChange);
                }

                if (cchar.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
                {
                    animator.applyRootMotion = false;
                    rigidbody.AddForce(new Vector3(0.5f, 0), ForceMode.VelocityChange);
                }
            }

        }

        public void UpdateSide()
        {
            if ((p1.transform.position.x < p2.transform.position.x) && p1.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2 && p2.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1)
            {
                p1.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P1);
                p2.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P2);
            }
            else if ((p1.transform.position.x > p2.transform.position.x) && p1.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P1 && p2.GetComponent<CharacterStateController>().GetFacingSide() == Enums.FacingSide.P2)
            {
                p1.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P2);
                p2.GetComponent<CharacterStateController>().SetFacingSide(Enums.FacingSide.P1);
            }
            cchar.GetComponent<Animator>().applyRootMotion = true;
        }

        private void Update()
        {
            bool wf, wb;
            wf = cchar.GetComponent<Animator>().GetBool("walkingForward");
            wb = cchar.GetComponent<Animator>().GetBool("walkingBackward");
            if (wf == true && wb == true)
            {
                cchar.GetComponent<Animator>().SetBool("walkingForward", false);
                cchar.GetComponent<Animator>().SetBool("walkingBackward", false);
            }
            if (cchar.transform.position.y > 5f) cchar.transform.position = new Vector3(cchar.transform.position.x, 5f, cchar.transform.position.z);

            if (cchar.transform.position.y < 0f) cchar.transform.position = new Vector3 (cchar.transform.position.x, 0.3f);
            if (cchar.transform.position.y > 15f) cchar.transform.position = new Vector3(cchar.transform.position.x, 14f);

            if (cchar.transform.position.x < -8f) cchar.transform.position = new Vector3(-8f, cchar.transform.position.y);
            if(cchar.transform.position.x > 8f) cchar.transform.position = new Vector3(8f, cchar.transform.position.y);

            cchar.transform.position = new Vector3(cchar.transform.position.x, cchar.transform.position.y, -7.5f);
        }
    }

    
}