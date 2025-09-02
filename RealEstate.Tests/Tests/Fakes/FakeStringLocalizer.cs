using Microsoft.Extensions.Localization;

namespace RealEstate.Tests.Tests.Fakes;

public class FakeStringLocalizer<T> : IStringLocalizer<T>
{
    public LocalizedString this[string name] => new(name, name, false);
    public LocalizedString this[string name, params object[] arguments] => new(name, name, false);

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        System.Array.Empty<LocalizedString>();

    public IStringLocalizer WithCulture(System.Globalization.CultureInfo culture) => this;
}
