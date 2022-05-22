using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory : MonoBehaviour
{
    [SerializeField] protected PlayerCamera _cam;
    [SerializeField] protected PlayerHUD _hud;
    [SerializeField] protected DamageNumbersManagerUI _dmgNumbers;
    [SerializeField] protected PreviewManager _preview;
    private static PlayerFactory _instance;

    private void Awake()
    {
        Application.targetFrameRate = 144;
        _instance = this;
    }

    public static PlayerFactory instance
    {
        get
        {
            return _instance;
        }
    }

    static public void CreateClientSide(PlayerScript p_plr)
    {
        Instantiate(_instance._hud, Vector3.zero, Quaternion.identity).Init(p_plr);
        Instantiate(_instance._cam, p_plr.transform.position, p_plr.transform.rotation).SetTarget(p_plr);
        Instantiate(_instance._dmgNumbers, p_plr.transform.position, p_plr.transform.rotation).SetTarget(p_plr);
        Instantiate(_instance._preview, Vector3.zero, Quaternion.identity).Init(p_plr);
    }
}
