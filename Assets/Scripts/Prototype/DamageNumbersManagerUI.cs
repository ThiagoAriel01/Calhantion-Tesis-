using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumbersManagerUI : MonoBehaviour
{
    [SerializeField] protected DamageNumberUI baseDamageNumber;
    [SerializeField] protected RectTransform canva;
    protected PlayerScript player;
    protected DamageNumberUI[] numbers;

    public void SetTarget (PlayerScript pplr)
    {
        player = pplr;
        numbers = new DamageNumberUI[64];
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = Instantiate(baseDamageNumber, canva.transform);
        }

        Character.onCharacterDamaged -= OnCharacterDamaged;
        Character.onCharacterDamaged += OnCharacterDamaged;
    }

    void OnCharacterDamaged(Character victim, GameObject attacker, SHitInfo info)
    {
        if (player == null)
            return;
        if (attacker == player.gameObject && !victim.invunerable)
        {
            SpawnDamageNumber((int)info.dmg,info.point,victim.gameObject);
        }
    }

    void SpawnDamageNumber(int dmg, Vector3 point, GameObject victim)
    {
        try
        {
            DamageNumberUI t = GetDamageNumber(victim);
            Vector3 dir = player.camara.transform.position - point;
            //t.transform.position = point + dir.normalized * 3f + Vector3.up;
            //t.transform.rotation = Quaternion.LookRotation(dir);
            t.gameObject.SetActive(true);
            t.value += dmg;
            t.victim = victim;
            t.Bump();
        }
        catch
        {

        }
    }

    DamageNumberUI GetDamageNumber(GameObject victim)
    {
        for (int i = 0; i < numbers.Length; i++)
        {
            if (!numbers[i].gameObject.activeSelf || (victim!=null && numbers[i].victim == victim))
                return numbers[i];
        }
        return null;
    }

}