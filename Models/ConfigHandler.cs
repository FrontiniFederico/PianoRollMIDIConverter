using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PianoRollMIDIConverter.Models;

// string test = "C:\\Users\\Federico\\Desktop";
// ConfigHandler Handle = new ConfigHandler(test);
// Dictionary<string, object> testo = Handle.FileParser(test);


namespace PianoRollMIDIConverter.Models
{
    public class ConfigHandler
    {
        private readonly string filepath;

        public ConfigHandler(string filepath) 
        {
            this.filepath = filepath;
        }

        public Dictionary<string, object> FileParser(string filepath)
        {
            var config = new Dictionary<string, object>();
            try 
            {
                string fileDump = File.ReadAllText(filepath);
            }
            catch(Exception e)
            {
                //roba
                //TODO: vorrei mettere un loggatore qui
            }
        return config;
        }
    }
}


