using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] protected string defaultSkillTrigger = "skillA";
    protected Animator animador;
    protected PlayerScript plr;
    protected PlayerSkills plrSkills;

    public delegate void AnimationDelegate(string trigger);
    public AnimationDelegate onPlayAnimation;

    protected Renderer[] meshes;

    [System.Serializable]
    protected class MeshMateriales
    {
        public Renderer mesh;
        public Material[] mats;
    }

    protected List<MeshMateriales> _materiales = new List<MeshMateriales>();

    public Animator animachion
    {
        get
        {
            if (animador == null)
                animador = GetComponent<Animator>();
            return animador;
        }
    }

    private void Awake()
    {
        meshes = GetComponentsInChildren<Renderer>();
        foreach (var item in meshes)
        {
            _materiales.Add(new MeshMateriales() { mesh = item, mats = item.materials });
        }
    }

    public void ShowMeshes (bool val)
    {
        foreach (Renderer m in meshes)
        {
            m.enabled = val;
        }
    }

    public void ChangeMaterial (Material sharedMaterial)
    {
        foreach (var item in meshes)
        {
            Material[] marray = new Material[item.materials.Length];
            for (int i = 0; i < marray.Length; i++)
            {
                marray[i] = sharedMaterial;
            }
            item.materials = marray;
        }
    }

    public void ResetMaterial()
    {
        for (int i = 0; i < _materiales.Count; i++)
        {
            Material[] marray = new Material[_materiales[i].mats.Length];
            for (int k = 0; k < marray.Length; k++)
            {
                marray[k] = _materiales[i].mats[k];
            }
            _materiales[i].mesh.materials = marray;
        }
        //Debug.LogError("RESETED");
    }

    public void Initiar()
    {
        enabled = true;
        animador = GetComponent<Animator>();
        plr = GetComponentInParent<PlayerScript>();
        plrSkills = GetComponentInParent<PlayerSkills>();
        plr.onJump += OnJump;
        plr.onJumpAir += OnJumpAir;
        plrSkills.onSkillCast += OnSkillCast;
    }

    void OnSkillCast(SkillData skill)
    {
        PlayAnimation(defaultSkillTrigger);
    }

    void OnJump(PlayerScript p_plr)
    {
        PlayAnimation("jump");
    }

    void OnJumpAir(PlayerScript p_plr)
    {
        PlayAnimation("jumpFlip");
    }

    public void PlayAnimation (string p_anim)
    {
        if (animador==null)
            animador = GetComponent<Animator>();
        animador.ResetTrigger(p_anim);
        animador.SetTrigger(p_anim);
        onPlayAnimation?.Invoke(p_anim);
    }

    private void Update()
    {
        Vector3 relativeVelocity = plr.transform.InverseTransformDirection(plr.velocity);
        relativeVelocity /= plr.moveSpeed;
        animador.SetFloat("forward", relativeVelocity.z, 0.25f, Time.deltaTime);
        float spdMult = relativeVelocity.magnitude <= 0.01f ? 1f : relativeVelocity.magnitude;
        animador.SetFloat("speedMult", spdMult);
        animador.SetFloat("right", relativeVelocity.x * 5f, 0.25f, Time.deltaTime);
        animador.SetFloat("forwardInput", plr.relativeInput.z);
        animador.SetFloat("rightInput", plr.relativeInput.x);
        animador.SetBool("isGrounded", plr.isGrounded);
        animador.SetBool("isGliding", plr.isGliding);
        animador.SetBool("battle",plr.freezeRotation);
        Vector2 move;
        move.x = animador.GetFloat("right");
        move.y = animador.GetFloat("forward");
        animador.SetBool("idle", move.magnitude <= 0.75f);
    }

    private void OnAnimatorMove()
    {
        if (animador == null)
            return;

        //plr.AddDelta(animador.deltaPosition, animador.deltaRotation,animador.velocity);
    }
}
