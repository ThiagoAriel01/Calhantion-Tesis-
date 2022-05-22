using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSkills : NetworkBehaviour
{

    public delegate void PlayerSkillsSKILLDelegate(SkillData skill);
    public delegate void PlayerSkillsDelegate(PlayerSkills plr);
    public PlayerSkillsSKILLDelegate onSkillCast;
    public PlayerSkillsDelegate onSkillTab;

    [SerializeField] protected SkillData[] startingSkills;
    [SerializeField] protected bool lookOnCast = false;
    protected bool looking;
    [SerializeField] protected float lookTime = 2f;
    [SerializeField] protected LayerMask layers;
    protected PlayerScript character;
    protected float t;
    protected bool s;
    private int currentSkill;
    protected SkillData[] skills;
    protected SkillData holding;
    protected SkillData canceled;
    protected bool tabbed;
    protected bool _silenced;
    static protected int skillsPerTab=4;

    static public int SkillsPerTab
    {
        get => skillsPerTab;
    }

    public bool Tabbed
    { 
        get => tabbed;
    }

    public bool silenced
    {
        get
        {
            return _silenced;
        }
        set
        {
            _silenced = value;
        }
    }

    public PlayerScript Char
    {
        get
        {
            return character;
        }
    }

    public SkillData GetSkill (int index)
    {
        try
        {
            return skills[index];
        }
        catch
        {
            throw;
        }
    }

    private void Awake()
    {
        character = GetComponent<PlayerScript>();
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GetComponent<ProtoPlayerMP>().onRespawn += (x) => OnRespawn();
        InitSkills();
    }

    void OnRespawn()
    {
        foreach (var item in skills)
        {
            item.ForceCancel(character);
        }
        CancelCast();
        if (!LoadoutManager.pendingChange)
            return;
        InitSkills();
    }

    public float tdelay
    {
        get
        {
            return t;
        }
        set
        {
            t = value;
        }
    }

    public void InitSkills()
    {
        skills = new SkillData[11];
        startingSkills = LoadoutManager.instance.GetSkills();
        for (int i = 0; i < startingSkills.Length; i++)
        {
            skills[i] = SkillUtlity.InstantiateSkill(startingSkills[i]);
        }
        Debug.LogError("iniTSKILLS");
        LoadoutManager.pendingChange = false;
    }

    public SkillData CurrentSkill
    {
        get
        {
            return skills[currentSkill];
        }
    }

    private void Update()
    {
        if (!_silenced)
            OnInput();
        character.freezeRotation = looking;
        if (looking)
        {
            Vector3 v = character.camara.transform.forward;
            v.y = .0f;
            Quaternion q = Quaternion.LookRotation(v);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, Time.deltaTime * 600 * Mathf.Deg2Rad);
        }
        for (int i = 0; i < skills.Length; i++)
        {
            skills[i]?.SkillUpdate(character,Time.deltaTime);
        }
    }

    void OnInput()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            tabbed = !tabbed;
            onSkillTab?.Invoke(this);
            CancelCast();
        }


        if (character.freeze)
            return;

        if (Input.mouseScrollDelta.y > 0.5f)
        {
           // NextSkill();
        }
        else if (Input.mouseScrollDelta.y < -0.5f)
        {
            //PreviousSkill();
        }

        t -= Time.deltaTime;

        if (holding!=null && Input.GetButtonDown("Fire2"))
        {
            CancelCast();
        }

        /*for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].IsUsing)
                return;
        }*/
        int offset = tabbed ? skillsPerTab : 0;
        if (!Cursor.visible || Cursor.lockState == CursorLockMode.Locked)
            InputCast("Fire1", 0);
        //InputCast("Fire2", 1);
        InputCast("SkillQ", 2 + offset);
        InputCast("SkillR", 3 + offset);
        InputCast("SkillF", 4 + offset);
        InputCast("SkillZ", 5 + offset);
        InputCast("SkillShift", 10);

        character.casting = holding != null;
    }

    void CancelCast()
    {
        if (holding!=null)
            holding.CancelCast(character);
        canceled = holding;
        holding = null;
        return;
    }

    void InputCast(string button, int skill)
    {

        if (skills[skill].CurrentCooldown > 0 || skills[skill].IsUsing)
        {
            if (Input.GetButtonDown(button) && (holding == null))
            {
                skills[skill].ForceCancel(character);
                t = skills[skill].cancelPostDelay;
                return;
            }
        }

        if (t > 0 || skills[skill].IsUsing)
            return;

        if (!skills[skill].CastsPreview)
        {
            if (Input.GetButtonDown(button) && (holding == null))
            {
                CastSkill(skill);
            }
            return;
        }

        if (Input.GetButton(button) && (holding == null) && canceled != skills[skill])
        {
            if (PrepareSkill(skill))
                holding = skills[skill];
        }
        if (Input.GetButtonUp(button))
        {
            if (holding == skills[skill])
            {
                CastSkill(skill);
                holding = null;
            }
            canceled = null;
        }
    }

    void NextSkill()
    {
        int cS = currentSkill + 1;
        if (cS > skills.Length - 1)
            currentSkill = 0;
        else
            currentSkill++;
    }

    void PreviousSkill()
    {
        int cS = currentSkill - 1;
        if (cS < 0)
            currentSkill = skills.Length - 1;
        else
            currentSkill--;
    }

    public void ReduceAllCooldowns(float amount)
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].CanReduceCooldown)
                skills[i].CurrentCooldown -= amount;
        }
    }

    public void ZeroAllCooldowns()
    {
        for (int i = 0; i < skills.Length; i++)
        {
            if (skills[i].CanReduceCooldown)
                skills[i].CurrentCooldown = 0;
        }
    }

    public void ResetCooldowns (string sk)
    {
        if (sk.ToLower() == skills[5].name.ToLower())
        {
            skills[5].CurrentCooldown = 0.0f;
        }
        else
        {
            ResetPrimaryCooldowns();
        }
    }

    public void ResetPrimaryCooldowns()
    {
        for (int i = 2; i <= 4; i++)
        {
            skills[i].CurrentCooldown = 0.0f;
        }
    }

    public bool CastSkill(int skillIndex)
    {
        Vector3 dir = character.camara.transform.position + character.camara.transform.forward * 100f;
        dir -= character.aimTransform.position;
        RaycastHit hit;
        if (Physics.Raycast(character.camara.transform.position + character.camara.transform.forward*2f, character.camara.transform.forward, out hit, 200f, layers))
        {
            dir = hit.point - character.aimTransform.position;
        }

        bool b = skills[skillIndex].Cast(character,null, dir.normalized);
        Debug.Log(b);
        if (b)
        {
            onSkillCast?.Invoke(skills[skillIndex]);
            character.CancelGlide();
            if (!character.isGrounded)
                character.AddJump(skills[skillIndex].RecoilVertical);
            character.velocity += -character.camara.transform.forward  * skills[skillIndex].Recoil;
            t = skills[skillIndex].PostDelay;
            if (lookOnCast)
            {
                looking = true;
                CancelInvoke("LookEnd");
                Invoke("LookEnd", lookTime);
            }
        }
        return b;
    }

    void LookEnd()
    {
        looking = false;
    }

    public bool PrepareSkill(int skillIndex)
    {
        bool b = skills[skillIndex].PrepareToCast(character);
        Debug.Log(b);
        return b;
    }
}
