using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json.Linq;
using PianoRollMIDIConverter.Models.Utils;
using Serilog;

namespace PianoRollMIDIConverter.Models
{
    public class ConfigHandler
    {
        private readonly string filepath;
        private readonly ILogger _logger;

        public ConfigHandler(string filepath) 
        {
            this.filepath = filepath;
            _logger = LoggerSingleton.Instance.Logger;
        }

        protected async Task<Dictionary<string, object>> ConfigParser(string filepath)
        {
            var config = new Dictionary<string, object>();
            string fileDump = "42"; //numero magico per non far arrabbiare csc
            try 
            {
                fileDump = await File.ReadAllTextAsync(this.filepath);
                _logger.Information("File di configurazione caricato correttamente.");
            }
            //TODO: tutto questo blocco try/catch deve andare nel loggatore
            catch (ArgumentNullException e)
            {
            // Eccezione generata se 'this.filepath' è nullo.
            _logger.Error(e.Message, "Il percorso del file è nullo.");
            }  
            catch (ArgumentException e)
            {
            // Eccezione generata se 'this.filepath' è una stringa vuota o contiene caratteri non validi.
            _logger.Error(e.Message, "Il percorso del file non è valido.");
            }
            catch (PathTooLongException e)
            {
            // Eccezione generata se il percorso del file supera la lunghezza massima consentita.
            _logger.Error(e.Message, "Il percorso del file è troppo lungo.");
            }
            catch (DirectoryNotFoundException e)
            {
            _logger.Error(e.Message, "La directory non esiste.");
            }
            catch (UnauthorizedAccessException e)
            {
                // Eccezione generata se mancano i permessi per accedere al file o alla directory.
                _logger.Error(e.Message, "Accesso non autorizzato al file o alla directory.");
            }
            catch (FileNotFoundException e)
            {
                // Eccezione generata se il file specificato non esiste.
                _logger.Error(e.Message, "Il file specificato non esiste.");
            }
            catch (IOException e)
            {
                // Eccezione generata per errori di I/O durante la lettura del file.
                _logger.Error(e.Message, "Si è verificato un errore di I/O durante la lettura del file.");
            }
            catch (NotSupportedException e)
            {
                // Eccezione generata se il file è in un formato non supportato.
                _logger.Error(e.Message, "Il formato del file non è supportato.");
            }
            catch (SecurityException e)
            {
                // Eccezione generata per problemi di sicurezza durante l'accesso al file o alla directory.
                _logger.Error(e.Message, "Problema di sicurezza durante l'accesso al file o alla directory.");
            }
            catch (Exception e)
            {
                // Eccezione generica per gestire qualsiasi altro tipo di eccezione non prevista.
                _logger.Error(e.Message, "Si è verificato un errore durante la lettura del file: ");
            }
            string[] lines = fileDump.Split('\n');
            lines = lines.Select(line => line.Trim()).ToArray();
            JObject jsonObject = new JObject();
            _logger.Information("Instanziato oggetto JSON.");
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
            _logger.Information("Dizionario di configurazione creato correttamente.");
            return config;

        }
    }
}


