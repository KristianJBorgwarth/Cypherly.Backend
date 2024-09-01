using Cypherly.Domain.Common;

namespace Cypherly.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Address { get; private set; }

    public Email() { } //For EF Core

    // Private constructor to prevent instantiation outside the Create method
    private Email(string address)
    {
        Address = address;
    }

    public static Result<Email> Create(string address)
    {
        try
        {
            var mailAddress = new System.Net.Mail.MailAddress(address);
            return Result.Ok(new Email(mailAddress.Address));
        }
        catch
        {
            return Result.Fail<Email>(Errors.General.UnspecifiedError("Invalid email address."));
        }
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Address;
    }

    public override string ToString()
    {
        return Address;
    }
}