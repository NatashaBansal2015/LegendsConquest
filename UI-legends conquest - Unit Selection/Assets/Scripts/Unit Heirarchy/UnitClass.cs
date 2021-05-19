using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public abstract class UnitClass : MonoBehaviour
{
    [SerializeField] protected string name;
    [SerializeField] protected int maxHP;
    [SerializeField] protected int maxStamina = 1;
    [SerializeField] protected int attackRange;

    [SerializeField] protected int atk, def;
    protected int currentHP;
    protected int currentStamina;
    protected bool noSpecialUsed = true;
    protected Unit unit;
    public Text TextField;

    //movement
    protected List<Node> currentPath = new List<Node>(); //used to animate the unit moving from one tile to another. Must have a list so Update() doesnt freak out
    protected static float movementSpeed = 5.5f; // the speed at which a unit will move from 1 tile to another

    //getter for unit class name
    public string getName() {
        return name;
    }

    //getter for current stamina
    public int getCurrentStam() {
        return currentStamina;
    }

    //getter for current HP
    public int getCurrentHP() {
        return currentHP;
    }

    //getter for no special used
    public bool getNoSpecialUsed() {
        return noSpecialUsed;
    }

    //calcs damage this unit will take ans set its hp
    public void takeDamage(Unit attacker) {
        currentHP = currentHP - (attacker.getUnitClass().GetComponent<UnitClass>().getCurrentAttack() - getCurrentDefence());
        TextField.text = currentHP.ToString();
    }

    //reduces current stamina to 0
    //only gets called when we attack with this unit so it stops ends their turn
    public void reduceStamina() {
        currentStamina = 0;
    }

    //destroys the game object
    public void destroy() {
        Destroy(gameObject);
    }

    //sets path if the cavalier uses their special
    public void setPath(List<Node> newPath) {
        currentPath = newPath;
    }

    //move unit to a new location, consumes stamina, and readys path to be used to move unit visually
    public void setPathReduceStaminaAndRelocateUnit(List<Node> path) {
        
        currentPath = path;
        currentStamina = currentStamina - currentPath.Count + 1; //+1 because path includes the tile unit is currently standing on
        unit.setPosition(currentPath[currentPath.Count - 1]); //sets unit to new location logically
    }

    //getters that can differ based on class
    public abstract int getCurrentAttack(); //grabs the current value of this units attack
    public abstract int getCurrentDefence(); //grabs the current value of this units defence
    public abstract int getCurrentRange(); // grabs the current attack range of the unit

    public abstract void initialize(Unit unit);//sets the unit's stats to its initial values

    public abstract void special(); //The Classes special action only that class can take

    public abstract void upkeep(); //values and effects that need to be reset/tweeked at the start of the turn

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
    }
}
