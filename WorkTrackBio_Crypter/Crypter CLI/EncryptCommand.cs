using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CrypterCLI
{
    internal static class EncryptCommand
    {
        private enum OutputFormat
        {
            text = 1,
            hex,
            base64
        };

        public static Command CreateCommand()
        {
            // Create the main encrypt command
            Command encryptCommand = new Command("encrypt", "Encrypts a file or text using AES-256 encryption");

            // Create input option with alias
            Option inputOption = new Option<string>("--input", "The input file path or text to encrypt")
            {
                IsRequired = true
            };
            inputOption.AddAlias("-i");

            // Create output option with alias
            Option outputOption = new Option<string>("--output", "The output file path (optional, if not provided will output to console)")
            {
                IsRequired = false
            };
            outputOption.AddAlias("-o");

            // Create password option with alias
            Option passwordOption = new Option<string>("--password", "The password to use for encryption")
            {
                IsRequired = true
            };
            passwordOption.AddAlias("-p");

            // Create text flag option with alias
            Option isTextOption = new Option<bool>("--text", "Treat input as text instead of a file path")
            {
                IsRequired = false
            };
            isTextOption.AddAlias("-t");

            // Create output format option with alias
            Option formatOption = new Option<OutputFormat>("--format", "The format of the encrypted output (for console output)")
            {
                IsRequired = false
            };
            formatOption.AddAlias("-f");
            formatOption.SetDefaultValue(OutputFormat.base64);

            // Add all options to command
            encryptCommand.AddOption(inputOption);
            encryptCommand.AddOption(outputOption);
            encryptCommand.AddOption(passwordOption);
            encryptCommand.AddOption(isTextOption);
            encryptCommand.AddOption(formatOption);

            // Set the command handler
            encryptCommand.Handler = CommandHandler.Create<string, string, string, bool, OutputFormat>(
                (input, output, password, text, format) =>
                {
                    try
                    {
                        string contentToEncrypt;

                        // Determine if we're encrypting direct text or a file
                        if (text)
                        {
                            contentToEncrypt = input;
                        }
                        else
                        {
                            if (!File.Exists(input))
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"Error: Input file '{input}' does not exist");
                                Console.ResetColor();
                                return;
                            }

                            contentToEncrypt = File.ReadAllText(input);
                        }

                        // Generate a random IV (Initialization Vector)
                        byte[] iv = RandomNumberGenerator.GetBytes(16);

                        // Encrypt the content
                        byte[] encryptedBytes = EncryptStringToBytes(contentToEncrypt, CreateKey(password), iv);

                        // Combine IV and encrypted content
                        byte[] result = new byte[iv.Length + encryptedBytes.Length];
                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

                        // Convert to the desired output format
                        string encryptedContent;
                        switch (format)
                        {
                            case OutputFormat.text:
                                encryptedContent = Encoding.UTF8.GetString(result);
                                break;
                            case OutputFormat.hex:
                                encryptedContent = Convert.ToHexString(result);
                                break;
                            case OutputFormat.base64:
                            default:
                                encryptedContent = Convert.ToBase64String(result);
                                break;
                        }

                        // Output to file or console
                        if (!string.IsNullOrEmpty(output))
                        {
                            File.WriteAllText(output, encryptedContent);
                            Console.WriteLine();
                            Console.WriteLine("Encryption Successful");
                            Console.WriteLine("====================");
                            Console.WriteLine($"Encrypted content written to: {output}");
                            Console.WriteLine($"Method: AES-256-CBC");
                            Console.WriteLine($"Format: {format}");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Encryption Successful");
                            Console.WriteLine("====================");
                            Console.WriteLine($"Method: AES-256-CBC");
                            Console.WriteLine($"Format: {format}");
                            Console.WriteLine();
                            Console.WriteLine("Encrypted Content:");
                            Console.WriteLine("====================");
                            Console.WriteLine(encryptedContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error during encryption: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            );

            return encryptCommand;
        }

        // Encrypt a string using AES algorithm
        private static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        // Create a 256-bit key from the provided password using SHA256
        private static byte[] CreateKey(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}