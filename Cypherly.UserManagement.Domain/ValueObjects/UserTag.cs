using Cypherly.Domain.Common;

namespace Cypherly.UserManagement.Domain.ValueObjects;

public class UserTag : ValueObject
{
    public string Tag { get; private set; } = null!;

    public UserTag() {} // For EF Core

    private UserTag(string tag)
    {
        Tag = tag;
    }

    public static UserTag Create(string displayName)
    {
        var tagGuid = Guid.NewGuid();
        var bytes = tagGuid.ToByteArray();
        var numberPortion = Math.Abs(BitConverter.ToInt32(bytes, 0) % 1000000);
        var letterPortion = (char)('A' + (bytes[4] % 26));
        return new UserTag($"{displayName}#{numberPortion:D6}{letterPortion}");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Tag;
    }
}