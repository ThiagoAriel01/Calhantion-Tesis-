using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "Data/Skills/Sun Beam Skill")]
public class SunBeamSkill : ProyectileSkillData
{
    [SerializeField] protected NetworkIdentity _previewPrefab;
    protected NetworkIdentity _previewInstance;
    protected bool _canceled;

    public override bool PrepareToCast(Character character)
    {
        if (base.PrepareToCast(character)) 
        {
            _canceled = false;
            Transform point = character.GetPoint(castPoint);
            Vector3 dir = point.forward;
            SpawnerMP.instance.SpawnPrefab(_previewPrefab.name, point.position, point.rotation, character.GetComponent<NetworkIdentity>(), OnSpawned);
            return true;
        }
        return false;
    }

    void OnSpawned(NetworkIdentity ni)
    {
        _previewInstance = ni;
        if (_canceled)
        {
            RemovePreview();
            return;
        }
    }

    void RemovePreview()
    {
        if (_previewInstance == null)
            return;
        SpawnerMP.instance.DestroyObject(_previewInstance);
        _previewInstance = null;
    }

    public override bool Cast(Character character)
    {
        return base.Cast(character);
    }
    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        if(base.Cast(character, target, direction))
        {
            RemovePreview();
            _canceled = true;
            return true;
        }
        return false;
    }

    public override void CancelCast(Character character)
    {
        RemovePreview();
        _canceled = true;
        base.CancelCast(character);
    }
}