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
                nextScore = 10 + ((level + 1) * 1);
                cost = 10 + CalculateCost(level, 10);
                break;

            case ObjectType.CirclePad:
                currentScore = 10 + (level * 1);
                nextScore = 10 + ((level + 1) * 1);
                cost = 10 + CalculateCost(level, 10);
                break;

            case ObjectType.ReversePad:
                currentScore = 15 + (level);
                nextScore = 15 + ((level + 1) * 2);
                cost = 10 + CalculateCost(level, 20);
                break;

            case ObjectType.Missile:
                currentScore = 10 + (level);
                nextScore = 10 + ((level + 1) * 2);
                cost = 15 + CalculateCost(level, 20);
                break;

            case ObjectType.Gravity:
                currentScore = 15 + (level);
                nextScore = 15 + ((level + 1) * 3);
                cost = 15 + CalculateCost(level, 20);
                break;

            case ObjectType.Teleport:
                currentScore = 15 + (level * 2);
                nextScore = 15 + ((level + 1) * 2);
                cost = 15 + CalculateCost(level, 15);
                break;

            case ObjectType.Lift:
                currentScore = 15 + (level * 3);
                nextScore = 15 + ((level + 1) * 3);
                cost = 20 + CalculateCost(level, 25);
                break;

            case ObjectType.Rotate:
                currentScore = 30 + (level * 15);
                nextScore = 30 + ((level + 1) * 15);
                cost = 30 + CalculateCost(level, 30);
                break;

            case ObjectType.Pause:
                currentScore = 15 + (level * 5);
                nextScore = 15 + ((level + 1) * 5);
                cost = 20 + CalculateCost(level, 20);
                break;
        }

        maxLevel = 5;
    }

    public int CalculateCost(int level, int cost)
    {
        if (level == 0)
            return 0;

        if (level == 1)
        {
            return cost;
        }
        else {
            return CalculateCost(level - 1, cost) + cost * level;
        }
    }
}
