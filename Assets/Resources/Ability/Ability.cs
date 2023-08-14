using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new_ability", menuName = "ScriptableObjects/Ability")]
public class Ability : ScriptableObject
{
    public new string name;
    public int range;

    public virtual void ShowTargets() { }

    public virtual void Effect() { }
}
