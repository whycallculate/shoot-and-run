using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class ModelGetData : MonoBehaviour
{
    [SerializeField]public Avatar avatar;
    [SerializeField]public Rig modelRig;
    [SerializeField]public Transform modelRoot;
    [SerializeField]public MultiAimConstraint modelBodyAim;
    [SerializeField]public MultiAimConstraint modelHeadAim;
    [SerializeField]public MultiAimConstraint modelRHandAim;
    [SerializeField]public TwoBoneIKConstraint modelLHandIK;
    [SerializeField]public SkinnedMeshRenderer Body;
    [SerializeField]public SkinnedMeshRenderer Feet;
    [SerializeField]public SkinnedMeshRenderer Head;
    [SerializeField]public SkinnedMeshRenderer Legs;
    [SerializeField]public GameObject[] weaponList;

}
