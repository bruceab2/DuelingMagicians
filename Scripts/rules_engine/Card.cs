using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Card : MonoBehaviour
{
    
    private string name, description, flavor;
    private int cost, player, type, counter, counterReduction;
    private LinkedList<int> effect, modifies, target;
    private bool isStealing, isCoinFlip;

    public Card(string name, string description, string flavor, int cost, int player, int type, int effect, int modifies, int target, 
                bool isStealing, bool isCoinFlip, int counter = 0, int counterReduction = -1)
    {
        this.name = name;
        this.description = description;
        this.flavor = flavor;
        this.cost = cost;
        this.player = player;
        this.type = type;
        this.effect = new LinkedList<int>();
        this.effect.AddLast(effect);
        this.modifies = new LinkedList<int>();
        this.modifies.AddLast(modifies);
        this.target = new LinkedList<int>();
        this.target.AddLast(target);
        this.isStealing = isStealing;
        this.isCoinFlip = isCoinFlip;
        this.counter = counter;
        this.counterReduction = counterReduction;
    }

    public int getCounter()
    {
        return counter;
    }

    public int getCounterReduction()
    {
        return counterReduction;
    }

    public void reduceCounter()
    {
        counter--;
    }

    public bool getIsStealing()
    {
        return isStealing;
    }

    public bool getIsCoinFlip()
    {
        return isCoinFlip;
    }

    public string getName()
    {
        return name;
    }

    public string getDescription()
    {
        return description;
    }

    public string getFlavor()
    {
        return flavor;
    }

    public int getCost()
    {

        return cost;
    }

    public int getPlayer()
    {
        return player;
    }
    
    public int getType()
    {
        return type;
    }

    public string getStringType()
    {
        if (type == (int) Constants.Types.audience)
        {
            return "Audience";
        } else if (type == (int)Constants.Types.favor)
        {
            return "Favor";
        } else if (type == (int)Constants.Types.sabatoge)
        {
            return "Sabotage";
        } else if (type == (int)Constants.Types.sleight)
        {
            return "Sleight";
        } else if (type == (int)Constants.Types.trick)
        {
            return "Trick";
        }

        return "ERROR";
    }

    public LinkedList<int> getEffect()
    {
        return effect;
    }

    public LinkedList<int> getModifies()
    {
        return modifies;
    }

    public LinkedList<int> getTarget()
    {
        return target;
    }

    public void addRule(int e, int m, int t)
    {
        effect.AddLast(e);
        modifies.AddLast(m);
        target.AddLast(t);
    }

    public override string ToString()
    {
        string ret = "";

        ret += "name: " + name + "\n" + "desc: " + description + "\n" + "falvor: " + flavor + "\n" + "cost: " + cost.ToString() + "\n" + "player: " + player.ToString() + "\n" + "type: " + type.ToString() + "\n";

        LinkedList<int>.Enumerator e = effect.GetEnumerator(), m = modifies.GetEnumerator(), t = target.GetEnumerator();
        e.MoveNext();
        m.MoveNext();
        t.MoveNext();
        for (int i = 0; i < effect.Count; i++)
        {
            ret += "effect: " + e.Current.ToString() + "\n" + "mods: " + m.Current.ToString() + "\n" + "tar: " + t.Current.ToString() + "\n";
            e.MoveNext();
            m.MoveNext();
            t.MoveNext();

        }

        return ret;
    }

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}