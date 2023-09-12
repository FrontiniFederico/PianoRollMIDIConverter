using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using PianoRollMIDIConverter.Models;
using Newtonsoft.Json.Linq;

namespace PianoRollMIDIConverter.Models
{
    public class ConfigHandler
    {
        private readonly string filepath;

        public ConfigHandler(string filepath) 
        {
            this.filepath = filepath;
        }

        protected async Task<Dictionary<string, object>> ConfigParser(string filepath)
        {
            var config = new Dictionary<string, object>();
            string fileDump = "42"; //numero magico per non far arrabbiare csc
            try 
            {
                fileDump = await File.ReadAllTextAsync(this.filepath);
            }
            //TODO: tutto questo blocco try/catch deve andare nel loggatore
            catch (ArgumentNullException e)
            {
            // Eccezione generata se 'this.filepath' è nullo.
            Console.WriteLine(e.Message, "Il percorso del file è nullo."); 
            }  
            catch (ArgumentException e)
            {
            // Eccezione generata se 'this.filepath' è una stringa vuota o contiene caratteri non validi.
            Console.WriteLine(e.Message, "Il percorso del file non è valido.");
            }
            catch (PathTooLongException e)
            {
            // Eccezione generata se il percorso del file supera la lunghezza massima consentita.
            Console.WriteLine(e.Message, "Il percorso del file è troppo lungo.");
            }
            catch (DirectoryNotFoundException e)
            {
                // Eccezione generata se la directory contenente il file non esiste.
                Console.WriteLine(e.Message, "La directory non esiste.");
            }
            catch (UnauthorizedAccessException e)
            {
                // Eccezione generata se mancano i permessi per accedere al file o alla directory.
                Console.WriteLine(e.Message, "Accesso non autorizzato al file o alla directory.");
            }
            catch (FileNotFoundException e)
            {
                // Eccezione generata se il file specificato non esiste.
                Console.WriteLine(e.Message, "Il file specificato non esiste.");
            }
            catch (IOException e)
            {
                // Eccezione generata per errori di I/O durante la lettura del file.
                Console.WriteLine(e.Message, "Si è verificato un errore di I/O durante la lettura del file.");
            }
            catch (NotSupportedException e)
            {
                // Eccezione generata se il file è in un formato non supportato.
                Console.WriteLine(e.Message, "Il formato del file non è supportato.");
            }
            catch (SecurityException e)
            {
                // Eccezione generata per problemi di sicurezza durante l'accesso al file o alla directory.
                Console.WriteLine(e.Message, "Problema di sicurezza durante l'accesso al file o alla directory.");
            }
            catch (Exception e)
            {
                // Eccezione generica per gestire qualsiasi altro tipo di eccezione non prevista.
                Console.WriteLine(e.Message, "Si è verificato un errore durante la lettura del file: ");
            }
            string[] lines = fileDump.Split('\n');
            lines = lines.Select(line => line.Trim()).ToArray();
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
            config = jsonObject.ToObject<Dictionary<string, object>>();
            return config;
        }
    }
}


