using API.Battle.State;
using API.Extensions;

namespace API.Battle.BuffEffects;

public sealed class ChangeAffinity(Element element, int change) : IBuffEffect
{
    public void OnGive(Unit self, int stacks)
    {
        this._Calc(self, change * stacks);
    }

    public void OnRemove(Unit self, int stacks)
    {
        this._Calc(self, change * -stacks);
    }

    private void _Calc(Unit self, int changeFull)
    {
        int affOld = self.GetAffinity(element);
        int affNew = affOld + changeFull;
        self.SetAffinity(element, affNew);

        LogLib.Add("LogChangeAff".FormatLang([self.FormatName(),
            element.GetName(), affOld.Format(), affNew.Format()]));
    }
}