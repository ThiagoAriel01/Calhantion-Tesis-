using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewManager : MonoBehaviour
{

    [SerializeField] protected GameObject dot;
    [SerializeField] protected GameObject box;
    [SerializeField] protected float spaceBetween;
    [SerializeField] protected Transform dotsPivot;
    [SerializeField] protected LayerMask layers;
    protected GameObject radiusDot;
    protected float radiusDotRange;
    protected float radiusRay;
    protected bool _box;
    protected Vector3 boxscale;
    protected SkillData previewing;
    protected Transform plr;
    protected PlayerScript plrScript;
    GameObject[] dots = new GameObject[150];

    static protected PreviewManager _instance;

    static public PreviewManager instance
    {
        get
        {
            return _instance;
        }
    }

    public void Init(PlayerScript p_plr)
    {
        _instance = this;
        plrScript = p_plr;
        plr = plrScript.aimTransform;
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i] = Instantiate(dot.gameObject);
            dots[i].transform.SetParent(dotsPivot);
            dots[i].gameObject.SetActive(true);
            dots[i].transform.localPosition = Vector3.zero;
        }
        radiusDot = Instantiate(dot.gameObject);
        radiusDot.gameObject.SetActive(false);
        radiusDot.transform.localPosition = Vector3.zero;

        dotsPivot.gameObject.SetActive(false);
        box.gameObject.SetActive(false);
    }

    public void InitPreview(SkillData skill, Vector3 worldDir, float gravity, float radius, float speed, float range, float proyectileRadius)
    {
        InitPreview(skill, worldDir, radius,range);
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].transform.localPosition = new Vector3(0, 0f, i * spaceBetween);
        }
        radiusRay = proyectileRadius;
        box.gameObject.SetActive(false);
        radiusDot.gameObject.SetActive(true);
        dotsPivot.gameObject.SetActive(true);
    }

    public void InitPreviewCube(SkillData skill,Vector3 boxScale, float range, bool aligntoworld)
    {
        radiusDotRange = range;
        previewing = skill;
        this.boxscale = boxScale;
        _box = true;
        box.gameObject.SetActive(true);
    }


    public void InitPreview(SkillData skill, Vector3 worldDir, float radius, float range)
    {
        box.gameObject.SetActive(false);
        _box = false;
        radiusRay = 0.0f;
        dotsPivot.gameObject.SetActive(false);
        radiusDot.gameObject.SetActive(true);
        radiusDotRange = range;
        radiusDot.transform.localScale = Vector3.one * radius / 2;
    }

    private void Update()
    {
        UpdatePreview();
    }

    public void UpdatePreview()
    {
        if (previewing = null)
            return;

        if (!_box)
        {
            dotsPivot.transform.position = plr.transform.position;
            dotsPivot.transform.rotation = plr.transform.rotation;

            if (radiusDot.activeSelf)
            {
                RaycastHit hit;
                if (Physics.Raycast(plrScript.camara.transform.position, plrScript.camara.transform.forward, out hit, radiusDotRange, layers))
                {
                    radiusDot.transform.position = hit.point;
                }
                else
                {
                    radiusDot.transform.position = plrScript.camara.transform.position + plrScript.camara.transform.forward * radiusDotRange;
                }
            }
        }
        else
        {
            box.transform.localScale = boxscale;
            RaycastHit hitbo;
            if (Physics.Raycast(plrScript.camara.transform.position, plrScript.camara.transform.forward, out hitbo, radiusDotRange, layers))
            {
                box.gameObject.SetActive(true);
                box.transform.position = hitbo.point;
                Vector3 v = plrScript.camara.transform.forward;
                v.y = .0f;
                if (Vector3.Angle(hitbo.normal,Vector3.up)<45f)
                    box.transform.rotation = Quaternion.LookRotation(v, hitbo.normal);
                else
                    box.transform.rotation = Quaternion.LookRotation(Vector3.down, hitbo.normal);
            }
            else
            {
                box.gameObject.SetActive(false);
            }
        }
    }

    public void EndPreview()
    {
        previewing = null;
        dotsPivot.gameObject.SetActive(false);
        radiusDot.gameObject.SetActive(false);
        box.gameObject.SetActive(false);
        _box = false;
    }
}
