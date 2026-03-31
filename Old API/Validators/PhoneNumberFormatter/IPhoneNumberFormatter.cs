namespace WorkTrackBio.API.Validators.PhoneNumberFormatter
{
    public interface IPhoneNumberFormatter
    {
        string? FormatPhoneNumber(string? phoneNumber);
        string? CleanDocumentNumber(string? documentNumber);
    }
}

