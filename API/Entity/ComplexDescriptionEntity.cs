using System.Text;
using API.Battle;
using API.Graphics;

namespace API.Entity;

public abstract class ComplexDescriptionEntity(string name, string description, string icon)
    : IconEntity(name, description, icon) {
    public string[] DescArgs { get; init; }
    public IconEntity[] DescInclusions { get; init; }

    // Force inheritors to reimplement
    public override string Description { get; set; }

    public virtual string GetPartialDesc() {
        StringBuilder partialDesc = new(string.Format(base.Description, this.DescArgs));
        if (this.DescInclusions.Length > 0) {
            partialDesc.Append('\n');
        }

        foreach (IconEntity entity in this.DescInclusions) {
            string color = entity is Skill ? Colors.Skill : Colors.Buff;
            partialDesc.Append("\n/c[white](").Append(entity.GetNameWithIcon(color)).Append("/c[white]: ")
                .Append(entity.Description.Replace("\n", ". ")).Append("/c[white])");
        }

        return partialDesc.ToString();
    }
}