using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    JumpPad = 0,
    CirclePad = 1,
    ReversePad = 2,
    Missile = 3,
    Gravity = 4,
    Teleport = 5,
    Lift = 6,
    Rotate = 7,
    Pause = 8,
}

public class ObjectData
{
    public ObjectType type{ get; private set; }
    public int maxLevel { get; private set; }
    public int level { get; private set; }
    public int currentScore { get; private set; }
    public int nextScore { get; private set; }
    public int cost { get; private set; }

    public ObjectData SetObjectType(ObjectType type)
    {
        this.type = type;
        return this;
    }

    public ObjectData SetLevel(int level)
    {
        this.level = level;
        return this;
    }

    public ObjectData SetMaxLevel(int maxLevel)
    {
        this.maxLevel = maxLevel;
        return this;
    }

    public ObjectData SetCurrentScore(int currentScore)
    {
        this.currentScore = currentScore;
        return this;
    }

    public ObjectData SetNextScore(int nextScore)
    {
        this.nextScore = nextScore;
        return this;
    }

    public ObjectData SetCost(int cost)
    {
        this.cost = cost;
        return this;
    }

    public void InitObjectData()
    {
        switch (type)
        {
            case ObjectType.JumpPad:
                currentScore = 10 + (level * 1);
                nextScore = 10 + (level * 1);
                cost = 30 + (10 * level);
                break;

            case ObjectType.CirclePad:
                currentScore = 20 + (level * 1);
                nextScore = 20 + ((level +1) * 1);
                cost = 30 + (10 * level);
                break;

            case ObjectType.ReversePad:
                currentScore = 30 + (level);
                nextScore = 30 + ((level + 1) * 2);
                cost = 30 + (15 * level);
                break;

            case ObjectType.Missile:
                currentScore = 40 + (level);
                nextScore = 40 + ((level + 1) * 2);
                cost = 30 + (15 * level);
                break;

            case ObjectType.Gravity:
                currentScore = 50 + (level);
                nextScore = 50 + ((level + 1) * 2);
                cost = 30 + (20 * level);
                break;

            case ObjectType.Teleport:
                currentScore = 60 + (level * 3);
                nextScore = 60 + ((level + 1) * 3);
                cost = 30 + (20 * level);
                break;

            case ObjectType.Lift:
                currentScore = 70 + (level * 3);
                nextScore = 70 + ((level + 1) * 3);
                cost = 30 + (20 * level);
                break;

            case ObjectType.Rotate:
                currentScore = 80 + (level * 5);
                nextScore = 80 + ((level + 1) * 5);
                cost = 30 + (30 * level);
                break;

            case ObjectType.Pause:
                currentScore = 90 + (level * 3);
                nextScore = 90 + ((level + 1) * 3);
                cost = 30 + (30 * level);
                break;
        }

        maxLevel = 5;
    }
}
