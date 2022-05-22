using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] protected float followSpeed;
    [SerializeField] protected Transform pivot;
    [SerializeField] protected Camera _camera;
    [SerializeField] protected LayerMask _clipLayers;
    protected float _defaultCamDistance;
    protected float yRot;
    protected PlayerScript plr;

    private void Awake()
    {
        _defaultCamDistance = _camera.transform.localPosition.z;
        //plr = FindObjectOfType<PlayerScript>();
    }

    public void SetTarget (PlayerScript pplr)
    {
        plr = pplr;
        plr.GetComponent<ProtoPlayerMP>().onRespawn += OnRespawn;
    }

    void OnRespawn(ProtoPlayerMP mp)
    {
        Vector3 v = mp.transform.forward;
        v.y=.0f;
        transform.rotation = Quaternion.LookRotation(v);
    }

    private void LateUpdate()
    {
        if (plr == null)
            return;

        transform.position = Vector3.Lerp(transform.position, plr.transform.position, Time.deltaTime * followSpeed);


        if (Cursor.lockState == CursorLockMode.None)
            return;

        float sens = TesisGameOptions.instance.mouseSens;
        float x = Input.GetAxis("Mouse X") * sens;
        float y = -Input.GetAxis("Mouse Y") * sens;
        transform.Rotate(0,x,0);
        yRot += y;
        yRot = Mathf.Clamp(yRot, -80, 80f);
        Vector3 angles = pivot.localEulerAngles;
        angles.x = yRot;
        pivot.localEulerAngles = angles;

        RaycastHit hit;
        if (Physics.SphereCast(pivot.transform.position, 0.1f, (_camera.transform.position - pivot.transform.position).normalized, out hit, -_defaultCamDistance, _clipLayers))
        {
            _camera.transform.localPosition = new Vector3(0, 0, -hit.distance);
        }
        else
        {
            _camera.transform.localPosition = new Vector3(0, 0, _defaultCamDistance);
        }
    }
}
