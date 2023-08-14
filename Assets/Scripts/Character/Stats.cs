using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
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

    public void SetStats(string key, float value)
    {
        foreach(Stat s in stats)
        {
            if (s.key.Equals(key))
            {
                s.value = value;
                break;
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

        return 0;
    }
}



