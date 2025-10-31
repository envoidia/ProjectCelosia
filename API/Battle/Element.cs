using API.Entity;

namespace API.Battle;

public class Element : IconEntity {
    public Element(string name, string description, string icon) : base(name, description, icon) {
        Core.Elements.Add(this);
    }
}