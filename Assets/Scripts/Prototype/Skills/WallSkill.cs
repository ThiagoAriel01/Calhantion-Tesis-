using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


[CreateAssetMenu(menuName = "Data/Skills/Wall")]
public class WallSkill : SkillData, INetworkHandler
{
    [SerializeField] protected GameObject _wall;
    [SerializeField] protected Vector3 _wallScale;
    [SerializeField] protected float _range;
    [SerializeField] protected LayerMask _layers;
    [SerializeField] protected string castPoint;

    protected struct WallMsg : NetworkMessage
    {
        public string prefab;
        public Vector3 point;
        public Vector3 forward;
        public Vector3 up;
        public Vector3 scale;
    }

    public override bool PrepareToCast(Character character)
    {
        if (base.PrepareToCast(character))
        {
            Transform point = character.GetPoint(castPoint);
            Vector3 dir = point.forward;
            PreviewManager.instance.InitPreviewCube(this,_wallScale,_range,true);
            return true;
        }
        return false;
    }

    virtual protected void Awake()
    {
        (this as INetworkHandler).RegisterHandlers();
    }

    void INetworkHandler.RegisterHandlers()
    {
        NetworkServer.RegisterHandler<WallMsg>(OnSpawnWall);
    }

    [Server]
    protected void OnSpawnWall (NetworkConnection conn, WallMsg msg)
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/" + msg.prefab),msg.point, Quaternion.LookRotation(msg.forward, msg.up));
        obj.transform.localScale = msg.scale;
        NetworkServer.Spawn(obj);
    }

    protected virtual float GetDamage()
    {
        return damage;
    }

    public override void CancelCast(Character character)
    {
        PreviewManager.instance.EndPreview();
    }

    public override bool Cast(Character character, GameObject target, Vector3 direction)
    {
        PreviewManager.instance.EndPreview();
        if (!CanCast(character))
            return false;

        Transform point = character.GetPoint(castPoint);
        Vector3 dir = point.forward;

        if (direction != Vector3.zero)
            dir = direction;

        Vector3 targetPos = point.transform.position + dir * _range;
        RaycastHit hit;
        if (Physics.Raycast(point.position, dir, out hit, _range, _layers))
        {
            base.Cast(character, target, direction);
            targetPos = hit.point;
            Vector3 v = direction;
            v.y = .0f;
            bool floor = Vector3.Angle(hit.normal, Vector3.up) < 45f;
            WallMsg msg = new WallMsg
            {
                prefab = _wall.name,
                point = hit.point,
                forward = floor ? v : Vector3.down,
                scale = _wallScale,
                up = hit.normal
            };
            NetworkClient.Send(msg);
            return true;
        }
        else
        {

            return false;
        }
    }
}