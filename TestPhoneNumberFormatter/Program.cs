using System;
using System.Linq;

namespace TestPhoneNumberFormatter
{
    public static class PhoneNumberFormatter
    {
        public static string? FormatPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return null;

            // Remover todos los caracteres no numéricos
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Si no hay exactamente 8 dígitos, retornar el original
            if (digitsOnly.Length != 8)
                return phoneNumber;

            // Formatear como nnnn-nnnn
            return $"{digitsOnly.Substring(0, 4)}-{digitsOnly.Substring(4, 4)}";
        }

        public static string? CleanDocumentNumber(string? documentNumber)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return null;

            // Remover guiones, espacios y otros caracteres no alfanuméricos
            return new string(documentNumber.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Prueba de Formateo de Números de Teléfono ===");
            
            // Pruebas de números de teléfono
            var phoneNumbers = new[] { "12345678", "1234-5678", "1234 5678", "1234.5678", "123456789", "1234567", "abc12345" };
            
            foreach (var phone in phoneNumbers)
            {
                var formatted = PhoneNumberFormatter.FormatPhoneNumber(phone);
                Console.WriteLine($"'{phone}' -> '{formatted}'");
            }
            
            Console.WriteLine("\n=== Prueba de Limpieza de Números de Documento ===");
            
            // Pruebas de números de documento
            var documentNumbers = new[] { "12345678", "1234-5678", "1234 5678", "ABC-123", "ABC 123", "ABC123" };
            
            foreach (var doc in documentNumbers)
            {
                var cleaned = PhoneNumberFormatter.CleanDocumentNumber(doc);
                Console.WriteLine($"'{doc}' -> '{cleaned}'");
            }
            
            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
