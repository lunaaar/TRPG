using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "new_spell", menuName = "ScriptableObjects/Spell")]
public class Spell : ScriptableObject
{
    public new string name;
    public string range;

    public virtual void Cast(Character caster) { }
    public virtual void Cast(Character caster, Character target) { }

}
