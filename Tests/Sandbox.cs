using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
// using PianoRollMIDIConverter.Models;
using Newtonsoft.Json.Linq;

string path = "C:\\Users\\Federico\\Desktop\\test.txt";
// ConfigHandler Handle = new(test);
// Dictionary<string, object> testo = Handle.FileParser(test);
string fileDump = await File.ReadAllTextAsync(path);
// using(var sr = new StreamReader(path))
// {
//     string text = sr.ReadToEnd();
//     Console.Write(text);
// }
string[] lines = fileDump.Split('\n');
lines = lines.Select(line => line.Trim()).ToArray();
Console.Write("ciao");
JObject jsonObject = new JObject();

        foreach (string line in lines)
        {
            // Ignora le righe che iniziano con '[' o sono vuote
            if (line.StartsWith("[") || string.IsNullOrWhiteSpace(line))
                continue;

            // Dividi la riga in chiave e valore
            string[] parts = line.Split('=');

            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                // Aggiungi la coppia chiave-valore all'oggetto JObject
                jsonObject[key] = JToken.FromObject(value);
            }
        }

        // Converti l'oggetto JObject in un dizionario di stringhe
        Dictionary<string, string> dictionary = jsonObject.ToObject<Dictionary<string, string>>();

        // Ora puoi utilizzare il dizionario come preferisci
        foreach (var kvp in dictionary)
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }

