using System.Text;
using API.Battle;
using API.Graphics;

namespace API.Entity;

public abstract class ComplexDescriptionEntity(string name, string keyDescription, string icon)
    : IconEntity(name, keyDescription, icon) {
    public string[] DescriptionArgs { get; init; } = [];
    public IconEntity[] DescriptionInclusions { get; init; } = [];

    // Force inheritors to reimplement
    public abstract override string GetDescription();

    public virtual string GetPartialDescription() {
        StringBuilder partialDescription = new(string.Format(this.GetDescription(), this.DescriptionArgs));
        if (this.DescriptionInclusions.Length > 0) {
            partialDescription.Append('\n');
        }

        foreach (IconEntity entity in this.DescriptionInclusions) {
            string color = entity is Skill ? Colors.Skill : Colors.Buff;
            partialDescription.Append("\n/c[white](").Append(entity.GetName(color)).Append("/c[white]: ")
                .Append(entity.GetDescription().Replace("\n", ". ")).Append("/c[white])");
        }

        return partialDescription.ToString();
    }
}