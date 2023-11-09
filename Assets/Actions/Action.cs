using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newBaseAction", menuName = "Actions/Action")]
public class Action : ScriptableObject
{
    //. Green
    //? Red
    
    /*?
        Actions:
        --------
        Actions serve the purpose as the main =Scriptable Object= that all functions characters can
        perform on their turn derive from. Specifically, there are 3 overhead types of Actions:
        - Weapons
        - Spells
        - Abilities
        You can read more about those specific type of actions in their associated classes.
        
        What matters at this overhead. Is since all the Actions are derived from this class, we can
        know guaranteed that they all have base functionality guaranteed.
     
        Currently, there are two functions that every Action has:
        - performAction()
        - showActionRange()


        For Reference:
        ---------------------------
        The current heirarchy is:
        - Weapons
            - Melee Weapon
            - Ranged Weapon
        - Spell
            - Self Spell
            - AOE Spell
            - Single Friendly Spell
        - Ability
            - Self Ability

        * The current plan I think. Is that I use the specific Scripts here (MeleeWeapon, etc). To
        * define how generically all Melee weapons for example work. But will probably need
        * one more layer deep for all the actual weapons (Axe, Sword, Rapier, etc).
        * 
        * The throught process is that right now I would need it anyway for how spells work regardless.
        * And then it allows me to define the calcuations for how each type of weapon would do damage.
        * The extension is then you can have other battle axes and stuff, but maybe an axe does dmg
        * with a different stat then a dagger.

        ** There will be more added as I continue to add more.

     */

    public new string name;
    public int range;
    public int damage;
    public ActionType actionType;
    public ActionTargets actionTargets;

    public enum ActionType { Weapon, Spell, Ability }
    public enum ActionTargets { SingleEnemy, SingleAlly, AOE, Self, MultipleEnemies, MultipleAllies }
    public enum DamageType { Holy, Necrotic, Fire, Water, Lightning, Nature }


    /*?
        performAction(Stats stats, Character target)
        ------
        The performAction funciton serves as what the game calls when any action well... needs to be
        performed. This can be a spell being cast, a weapon being attacked with, or an ability being
        used.
        
        stats - this is a reference to the Stats object of the character who is performing the action
        target - this is a reference to the character that is the target of the action.
     */
    public virtual void performAction(Stats stats, Character target) { }

    public virtual List<GridTile> showActionRange(List<GridTile> movementTiles, GridTile start, int movementRange)
    {
        //. This is the old default show method. Just here for reference incase needed again.
        
        /**
         * foreach (var tile in attackTiles)
        {
            if (tile.status.Equals("Friendly"))
            {
                CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.friendlyTile);
            }
            else if (tile.status.Equals("Enemy") ||
                pathFinder.findPath(start, tile).Sum(t => t.movementPenalty) == range + movementRange)
            {
                CursorMovement.instance.attackRangeTilemap.SetTile(tile.gridPosition, CursorMovement.instance.attackTile);
            }
        }
         */

        return new List<GridTile>();
    }
}
