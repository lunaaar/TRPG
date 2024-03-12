using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    /*.
        Stat:
        =====

        This class represents the statistics that are held within the Character 'stats' array.
        
        The main reason this exists is because in Unity. You cannot have a dictionary exist within
        the editor. So having a list of these objects allows for them to persist.

        That is the only reason. I could technically add additionally functionality. But
        no need. Just need Gets and Sets.
        
     */
    
    
    [System.Serializable]
    public class Stat
    {
        public string key;
        public float value;

        public Stat(string k, float v)
        {
            key = k; value = v;
        }
    }

    public List<Stat> stats;
    public List<Stat> baseStats;

    public void Start()
    {
        foreach(Stat s in stats.GetRange(2, stats.Count - 2))
        {
            baseStats.Add(new Stat(s.key, s.value));
        }
    }

    public void SetStats(string key, float value)
    {
        foreach(Stat s in stats)
        {
            if (s.key.Equals(key))
            {
                s.value = value;
                return;
            }
        }

        stats.Add(new Stat(key, value));
    }

    public float contains(string key)
    {
        foreach(Stat s in stats)
        {
            if (s.key.Equals(key))
            {
                return s.value;
            }
        }

        //. Return something that should never be returned.
        return -1;
    }

    public float baseStatContains(string key)
    {
        foreach (Stat s in baseStats)
        {
            if (s.key.Equals(key))
            {
                return s.value;
            }
        }

        return 0;
    }
}



