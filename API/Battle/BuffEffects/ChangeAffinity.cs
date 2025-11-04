using API.Extensions;
using API.Graphics;

namespace API.Battle.BuffEffects;

public class ChangeAffinity(Element element, int change) : IBuffEffect {
    public void OnGive(Unit self, uint stacks) {
        this.Calc(self, (int) (change * stacks));
    }

    public void OnRemove(Unit self, uint stacks) {
        this.Calc(self, (int) (change * -stacks));
    }

    private void Calc(Unit self, int changeFull) {
        int affOld = self.GetAffinity(element);
        int affNew = affOld + changeFull;
        self.SetAffinity(element, affNew);

        BattleHandlerLib.AppendToLog(string.Format(Lang.LogChangeAff, self.FormatName(),
            element.GetName(Colors.Element), affOld.Format(), affNew.Format()));
    }
}