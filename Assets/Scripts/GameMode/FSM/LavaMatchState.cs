using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LavaMatchState : GameModeState
{
    protected LavaMatch _lava;

    public LavaMatchState(string pid, LavaMatch lm) : base(pid)
    {
        _lava = lm;
    }
    public override Transform ChooseSpawnLocation(PlayerState state)
    {
        return ChooseSpawnLocation();
    }

    public override Transform ChooseSpawnLocation()
    {
        Transform t = base.ChooseSpawnLocation();
        float h = _lava.lava._lavaHeight;
        float l = Mathf.Clamp(_lava.lava.transform.position.y + 20f, _lava.lava._lavaHeight, _lava.lava._lavaMaxlevel);
        while (t.transform.position.y <= l || t.transform.position.y > l + h)
        {
            t = base.ChooseSpawnLocation();
            h += 2.5f;
        }
        return t;
    }

}
