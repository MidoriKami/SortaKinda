using KamiLib.AutomaticUserInterface;

namespace SortaKinda.Models;

[Category("OrderingConfig", 1)]
public interface IOrderingConfig
{
    [BoolConfig("FillFromBottom")]
    public bool FillFromBottom { get; set; }
}