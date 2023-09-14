using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;


namespace PianoRollMIDIConverter.Models
{

    public class ImageHandler
    {   
        private readonly Mat image;

        public ImageHandler(string filepath) 
        {
            // Carica un'immagine di esempio
            this.image = new Mat(filepath, ImreadModes.Color);
        }
    }
}

