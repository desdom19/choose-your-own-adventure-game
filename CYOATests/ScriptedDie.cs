using System.Collections.Generic;

// A Die that returns preset rolls in order, so tests can check exact results.
public class ScriptedDie : Die
{
    private readonly Queue<int> _rolls;

    public ScriptedDie(params int[] rolls)
    {
        _rolls = new Queue<int>(rolls);
    }

    public override int Roll(int sides)
    {
        if (_rolls.Count == 0)
            throw new InvalidOperationException($"ScriptedDie ran out of rolls (asked for a d{sides}).");

        int roll = _rolls.Dequeue();

        if (roll < 1 || roll > sides)
            throw new InvalidOperationException($"ScriptedDie roll {roll} is not possible on a d{sides}.");

        return roll;
    }
}
