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



