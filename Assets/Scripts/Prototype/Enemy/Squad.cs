using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Squad Data")]
public class Squad : ScriptableObject
{
    [SerializeField] public string id;
    [SerializeField] public Vector3[] positions;
    [SerializeField] public float spacing = 1;
    [SerializeField] public float squadSpacing = 10f;
    protected List<Enemy> enemies = new List<Enemy>();
    protected bool _isInstance;

    public void Init(Enemy e)
    {
        if (_isInstance)
            return;
        AddEnemy(e);
        _isInstance = true;
    }

    public int Indexof(Enemy e)
    {
        return enemies.IndexOf(e);
    }

    public bool isInstance
    {
        get
        {
            return _isInstance;
        }
    }

    public int squadSize
    {
        get
        {
            return positions.Length;
        }
    }

    public void AddEnemy(Enemy e)
    {
        enemies.Add(e);
        e.onDie -= OnDie;
        e.onDie += OnDie;
    }

    public void AddEnemies(Enemy[] es)
    {
        foreach (Enemy e in es)
        {
            AddEnemy(e);
        }
    }

    public Enemy Leader
    {
        get
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i] != null)
                    return enemies[i];
            }
            return enemies[0];
        }
    }

    public Vector3 SampleFormationPosition(Enemy e)
    {
        int index = Indexof(e);
        if (index == -1)
            return e.transform.position;
        Vector3 worldPos = Leader.transform.TransformPoint(positions[index] * spacing);
        return worldPos;
    }

    void OnDie(Entity e, SHitInfo info)
    {
        enemies.Remove(e as Enemy);
    }
    static public Squad InstantiateSquad(Enemy e,Squad s)
    {
        Squad squad = ScriptableObject.Instantiate(s);
        squad.Init(e);
        return squad;
    }

    virtual public void OnUpdate (Enemy e,float deltaTime)
    {
        if (enemies.Count >= squadSize || e != Leader)
            return;
        e.inSquad = false;
        List<Entity> allies = e.Sight.GetAlliesSortedByDistance();
        foreach (Entity ally in allies)
        {
            Enemy allyE = ally as Enemy;
            Squad merged = MergeSquad(this, (allyE).currentSquad);
            if (merged!=null)
            {
                e.currentSquad = merged;
                allyE.currentSquad = merged;
                e.OnSquadMerged(merged);
                allyE.OnSquadMerged(merged);
            }
        }
    }

    public List<Enemy> enemiesInSquad
    {
        get
        {
            return enemies;
        }
    }

    static public Squad MergeSquad (Squad a, Squad b)
    {
        if (a== null || b==null || a.id != b.id || !a.isInstance || !b.isInstance || a.enemies.Count + b.enemies.Count > a.squadSize)
            return null;
        a.AddEnemies(b.enemies.ToArray());    
        return a;
    }

}
