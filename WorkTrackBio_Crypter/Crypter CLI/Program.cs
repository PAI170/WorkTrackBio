using System;
using System.Collections.Generic;
using System.CommandLine;
using CrypterCLI;

// Create the root command
RootCommand rootCommand = new RootCommand("Crypter CLI - Encrypt, decrypt, and hash data");

// Add the commands to the root command
rootCommand.AddCommand(EncryptCommand.CreateCommand());
rootCommand.AddCommand(DecryptCommand.CreateCommand());
rootCommand.AddCommand(HashCommand.CreateCommand());

// Check if no arguments were provided
if (args.Length == 0)
{
    // Start interactive mode
    RunInteractiveMode();
}
else
{
    // Invoke the root command with the provided arguments
    rootCommand.Invoke(args);
}

// Interactive mode function
static void RunInteractiveMode()
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("  CRYPTER CLI - MODO INTERACTIVO");
    Console.WriteLine("=================================");
    Console.WriteLine();

    Console.WriteLine("Selecciona una operación:");
    Console.WriteLine("1. Hash de contraseña");
    Console.WriteLine("2. Encriptar texto/archivo");
    Console.WriteLine("3. Desencriptar texto/archivo");
    Console.WriteLine();

    Console.Write("Opción (1-3): ");
    string? option = Console.ReadLine();

    switch (option)
    {
        case "1":
            RunInteractiveHash();
            break;
        case "2":
            RunInteractiveEncrypt();
            break;
        case "3":
            RunInteractiveDecrypt();
            break;
        default:
            Console.WriteLine("Opción no válida. Ejecutando modo hash por defecto.");
            RunInteractiveHash();
            break;
    }
}

static void RunInteractiveHash()
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("      HASH DE CONTRASEÑA");
    Console.WriteLine("=================================");
    Console.WriteLine();

    // Get password to hash
    Console.Write("Ingresa la contraseña a hashear: ");
    string? password = ReadPasswordMasked();

    if (string.IsNullOrEmpty(password))
    {
        Console.WriteLine("La contraseña no puede estar vacía.");
        return;
    }

    // Get iterations
    Console.Write("Número de iteraciones (recomendado: 50000, presiona Enter para usar este valor): ");
    string? iterInput = Console.ReadLine();
    int iterations = 50000; // Default value

    if (!string.IsNullOrEmpty(iterInput) && int.TryParse(iterInput, out int customIter))
    {
        iterations = customIter;
    }

    // Create the arguments array
    string[] hashArgs = { "hash", password, "--iterations", iterations.ToString() };

    // Execute the hash command
    var rootCmd = new RootCommand();
    rootCmd.AddCommand(HashCommand.CreateCommand());
    rootCmd.Invoke(hashArgs);

    Console.WriteLine();
    Console.WriteLine("Presiona cualquier tecla para salir...");
    Console.ReadKey();
}

static void RunInteractiveEncrypt()
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("       ENCRIPTAR DATOS");
    Console.WriteLine("=================================");
    Console.WriteLine();

    // Ask if input is text or file
    Console.Write("¿Deseas encriptar un texto o un archivo? (t/f): ");
    bool isText = Console.ReadLine()?.ToLower() == "t";

    string input;
    if (isText)
    {
        Console.Write("Ingresa el texto a encriptar: ");
        input = Console.ReadLine() ?? "";
    }
    else
    {
        Console.Write("Ingresa la ruta del archivo a encriptar: ");
        input = Console.ReadLine() ?? "";
        if (!System.IO.File.Exists(input))
        {
            Console.WriteLine($"Error: El archivo '{input}' no existe.");
            return;
        }
    }

    // Get password
    Console.Write("Ingresa la contraseña para encriptar: ");
    string? password = ReadPasswordMasked();

    // Ask for output file (optional)
    Console.Write("Ruta del archivo de salida (opcional, presiona Enter para mostrar en pantalla): ");
    string? output = Console.ReadLine();

    // Create the arguments array
    var encryptArgs = new List<string> { "encrypt", "--input", input, "--password", password ?? "" };

    if (isText)
        encryptArgs.Add("--text");

    if (!string.IsNullOrEmpty(output))
        encryptArgs.AddRange(new[] { "--output", output });

    // Execute the encrypt command
    var rootCmd = new RootCommand();
    rootCmd.AddCommand(EncryptCommand.CreateCommand());
    rootCmd.Invoke(encryptArgs.ToArray());

    Console.WriteLine();
    Console.WriteLine("Presiona cualquier tecla para salir...");
    Console.ReadKey();
}

static void RunInteractiveDecrypt()
{
    Console.Clear();
    Console.WriteLine("=================================");
    Console.WriteLine("       DESENCRIPTAR DATOS");
    Console.WriteLine("=================================");
    Console.WriteLine();

    // Ask if input is text or file
    Console.Write("¿El contenido encriptado está como texto o en un archivo? (t/f): ");
    bool isText = Console.ReadLine()?.ToLower() == "t";

    string input;
    if (isText)
    {
        Console.Write("Ingresa el texto encriptado: ");
        input = Console.ReadLine() ?? "";
    }
    else
    {
        Console.Write("Ingresa la ruta del archivo encriptado: ");
        input = Console.ReadLine() ?? "";
        if (!System.IO.File.Exists(input))
        {
            Console.WriteLine($"Error: El archivo '{input}' no existe.");
            return;
        }
    }

    // Get password
    Console.Write("Ingresa la contraseña para desencriptar: ");
    string? password = ReadPasswordMasked();

    // Ask for output file (optional)
    Console.Write("Ruta del archivo de salida (opcional, presiona Enter para mostrar en pantalla): ");
    string? output = Console.ReadLine();

    // Create the arguments array
    var decryptArgs = new List<string> { "decrypt", "--input", input, "--password", password ?? "" };

    if (isText)
        decryptArgs.Add("--text");

    if (!string.IsNullOrEmpty(output))
        decryptArgs.AddRange(new[] { "--output", output });

    // Execute the decrypt command
    var rootCmd = new RootCommand();
    rootCmd.AddCommand(DecryptCommand.CreateCommand());
    rootCmd.Invoke(decryptArgs.ToArray());

    Console.WriteLine();
    Console.WriteLine("Presiona cualquier tecla para salir...");
    Console.ReadKey();
}

// Helper method to read password with masking
static string? ReadPasswordMasked()
{
    var password = new System.Text.StringBuilder();
    ConsoleKeyInfo key;

    do
    {
        key = Console.ReadKey(true);

        // Ignore any key out of range
        if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
        {
            password.Append(key.KeyChar);
            Console.Write("*");
        }
        else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
        {
            password.Remove(password.Length - 1, 1);
            Console.Write("\b \b");
        }
    } while (key.Key != ConsoleKey.Enter);

    Console.WriteLine();
    return password.ToString();
}