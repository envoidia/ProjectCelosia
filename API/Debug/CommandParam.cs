using System;

namespace API.Debug;

public class CommandParam
{
    /// <summary>
    /// Hint text to display for this parameter
    /// </summary>
    public readonly string Hint;

    /// <summary>
    /// Valid inputs for this parameter. Empty array means anything is accepted
    /// </summary>
    public readonly string[] ValidInputs;

    /// <summary>
    /// Called before directly checking <c>ValidInputs</c>. Used to lazily evaluate large amounts of valid inputs
    /// </summary>
    public readonly Func<string[]> GetValidInputs;

    public CommandParam(string hint, string[] validInputs, Func<string[]>? getValidInputs = null)
    {
        this.Hint = hint;
        this.ValidInputs = validInputs;
        this.GetValidInputs = getValidInputs ?? this._GetValidInputs;
    }

    public CommandParam(string[] validInputs) : this(string.Join('/', validInputs), validInputs) { }

    public CommandParam(string hint) : this(hint, []) { }

    public CommandParam(string hint, Func<string[]> getValidInputs) : this(hint, [], getValidInputs) { }

    private string[] _GetValidInputs()
    {
        return this.ValidInputs;
    }
}
