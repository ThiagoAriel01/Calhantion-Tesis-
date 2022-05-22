using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(menuName = "Data/Skills/Rest Skill")]
public class RestSkill : SkillData
{
    [SerializeField] protected BuffData _buffToGive;
    [SerializeField] protected float radius;
    [SerializeField] protected float duration;
    [SerializeField] protected Explosion explosion;
    [SerializeField] protected float rate = 0.0f;
    [SerializeField] protected string castPoint;

    public struct RestMsg : NetworkMessage
    {
        public GameObject character;
    }

    private void Awake()
    {
        NetworkServer.RegisterHandler<RestMsg>(OnRestCast);
    }

    [Server]
    void OnRestCast(NetworkConnection conn, RestMsg msg)
    {
        Character c = msg.character.GetComponent<Character>();
        if (c!=null && _buffToGive != null)
            c.AddBuff(_buffToGive);
    }

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        if (!base.Cast(character, target, direction))
            return false;

        Transform point = character.GetPoint(castPoint);
        Vector3 dir = point.forward;

        if (direction != Vector3.zero)
            dir = direction;

        Vector3 targetPos = character.transform.position;

        ProyectileFactory.RequestSpawnExplosion(explosion, character.gameObject, targetPos, Vector3.zero, damage, damage, radius / 2, rate, duration,this.name);
        //Explosion e = Instantiate(explosion, targetPos, Quaternion.identity);
        // e.gameObject.SetActive(true);
        //e.Init(damage, damage, radius / 2, character.gameObject, rate,duration);
        character.GetComponent<ProtoPlayerMP>().ClientAddBuff(_buffToGive.name);
        return true;
    }
}
