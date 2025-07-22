using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CrypterCLI
{
    internal static class DecryptCommand
    {
        private enum InputFormat
        {
            text = 1,
            hex,
            base64
        };

        public static Command CreateCommand()
        {
            // Create the main decrypt command
            Command decryptCommand = new Command("decrypt", "Decrypts encrypted content using AES-256 decryption");

            // Create input option with alias
            Option inputOption = new Option<string>("--input", "The input file path or encrypted text to decrypt")
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
            Option passwordOption = new Option<string>("--password", "The password to use for decryption")
            {
                IsRequired = true
            };
            passwordOption.AddAlias("-p");

            // Create text flag option with alias
            Option isTextOption = new Option<bool>("--text", "Treat input as encrypted text instead of a file path")
            {
                IsRequired = false
            };
            isTextOption.AddAlias("-t");

            // Create input format option with alias
            Option formatOption = new Option<InputFormat>("--format", "The format of the encrypted input (for text input)")
            {
                IsRequired = false
            };
            formatOption.AddAlias("-f");
            formatOption.SetDefaultValue(InputFormat.base64);

            // Add all options to command
            decryptCommand.AddOption(inputOption);
            decryptCommand.AddOption(outputOption);
            decryptCommand.AddOption(passwordOption);
            decryptCommand.AddOption(isTextOption);
            decryptCommand.AddOption(formatOption);

            // Set the command handler
            decryptCommand.Handler = CommandHandler.Create<string, string, string, bool, InputFormat>(
                (input, output, password, text, format) =>
                {
                    try
                    {
                        string encryptedContent;

                        // Get content from file or use input directly
                        if (text)
                        {
                            encryptedContent = input;
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

                            encryptedContent = File.ReadAllText(input);
                        }

                        // Convert the encrypted content from specified format to bytes
                        byte[] encryptedBytes;
                        try
                        {
                            switch (format)
                            {
                                case InputFormat.text:
                                    encryptedBytes = Encoding.UTF8.GetBytes(encryptedContent);
                                    break;
                                case InputFormat.hex:
                                    encryptedBytes = Convert.FromHexString(encryptedContent);
                                    break;
                                case InputFormat.base64:
                                default:
                                    encryptedBytes = Convert.FromBase64String(encryptedContent);
                                    break;
                            }
                        }
                        catch (FormatException)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error: The input is not in valid {format} format");
                            Console.ResetColor();
                            return;
                        }

                        // Extract IV (first 16 bytes)
                        if (encryptedBytes.Length < 16)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Error: Encrypted data is too short to contain an IV");
                            Console.ResetColor();
                            return;
                        }

                        byte[] iv = new byte[16];
                        byte[] cipherText = new byte[encryptedBytes.Length - 16];

                        Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
                        Buffer.BlockCopy(encryptedBytes, iv.Length, cipherText, 0, cipherText.Length);

                        // Decrypt the content
                        string decryptedContent = DecryptStringFromBytes(cipherText, CreateKey(password), iv);

                        // Output to file or console
                        if (!string.IsNullOrEmpty(output))
                        {
                            File.WriteAllText(output, decryptedContent);
                            Console.WriteLine();
                            Console.WriteLine("Decryption Successful");
                            Console.WriteLine("====================");
                            Console.WriteLine($"Decrypted content written to: {output}");
                            Console.WriteLine($"Method: AES-256-CBC");
                        }
                        else
                        {
                            Console.WriteLine();
                            Console.WriteLine("Decryption Successful");
                            Console.WriteLine("====================");
                            Console.WriteLine($"Method: AES-256-CBC");
                            Console.WriteLine();
                            Console.WriteLine("Decrypted Content:");
                            Console.WriteLine("====================");
                            Console.WriteLine(decryptedContent);
                        }
                    }
                    catch (CryptographicException)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error: Decryption failed. This may be due to an incorrect password or corrupted data.");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error during decryption: {ex.Message}");
                        Console.ResetColor();
                    }
                }
            );

            return decryptCommand;
        }

        // Decrypt a string using AES algorithm
        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));

            string plaintext;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
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