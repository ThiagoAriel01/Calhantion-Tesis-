using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LavaElevator : NetworkBehaviour
{
    [SerializeField] protected float _heightTarget;
    [SerializeField] protected float _time;
    protected float _startHeight;
    protected float _midHeight;
    protected float t;

    private void Awake()
    {
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        _startHeight = transform.position.y;
        _midHeight = ((_startHeight + _heightTarget) / 2);
        GameModeManager.instance.onGameModeStart += OnStart;
    }

    void OnStart()
    {
        GameModeManager.instance.currentGameMode.onNewRound += OnNewRound;
    }

    void OnNewRound()
    {
        t = .0f;
    }

    private void Update()
    {
        if (!isServer)
            return;
        t += Time.deltaTime;
        float y = _midHeight + Mathf.Cos((t * Mathf.PI) / _time) * (-_heightTarget/2);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
