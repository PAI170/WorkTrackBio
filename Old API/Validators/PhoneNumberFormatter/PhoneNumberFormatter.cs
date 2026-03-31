namespace WorkTrackBio.API.Validators.PhoneNumberFormatter
{
    public class PhoneNumberFormatter : IPhoneNumberFormatter
    {
        public string? FormatPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length != 8)
                return phoneNumber;

            return $"{digitsOnly.Substring(0, 4)}-{digitsOnly.Substring(4, 4)}";
        }

        public string? CleanDocumentNumber(string? documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return null;

            return new string(documentNumber.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }
    }
}

