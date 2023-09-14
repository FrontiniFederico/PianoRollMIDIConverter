using System;
using System.Drawing;
// using PianoRollMIDIConverter.Models;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections.Generic;
using Emgu.CV.Util;
using DynamicData;


string filepath = "C:\\Users\\Federico\\Desktop\\test_linea.png";
// ImageHandler test = new ImageHandler(filepath);
// if(test != null){
//     Console.WriteLine("tutto ok");
// }
// else{
//     Console.WriteLine("dio bo");
// }
// Mat image = new(filepath, ImreadModes.Color);
// Carica l'immagine
// Mat image = CvInvoke.Imread(filepath, ImreadModes.Color);


Mat image = new Mat(filepath, ImreadModes.Color);
CvInvoke.GaussianBlur(image, image, new Size(5, 5), 0);
// CvInvoke.ConvertScaleAbs(image, image, 3.2, -40);
CvInvoke.MedianBlur(image, image, 5);
// CvInvoke.BilateralFilter(image, image, 9, 75, 75);
CvInvoke.ConvertScaleAbs(image, image, 2.8, -120);
CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Gray);

CvInvoke.Imshow("immagine in scala di grigi", image);

// Attendere che l'utente prema un tasto per chiudere la finestra
CvInvoke.WaitKey(0);

Mat binaryImage = new Mat();
CvInvoke.Threshold(image, binaryImage, 128, 255, ThresholdType.Binary);


// Trova i contorni nell'immagine
List<List<Point>> contours = new List<List<Point>>();
using (VectorOfVectorOfPoint contoursVector = new VectorOfVectorOfPoint())
{
        CvInvoke.FindContours(binaryImage, contoursVector, null, RetrType.List, ChainApproxMethod.ChainApproxSimple);
        for (int i = 0; i < contoursVector.Size; i++)
        {
                VectorOfPoint contour = contoursVector[i];
                contours.Add(new List<Point>(contour.ToArray()));
        }
}

//TEST FILTRAGGIO


// Filtra i contorni in base all'area o alla forma
double areaThreshold = 0.8; // Valore soglia per l'area
double shapeThreshold = 0.9; // Valore soglia per la forma
List<List<Point>> filteredContours = new List<List<Point>>();

foreach (var contour in contours)
{
        double contourArea = CvInvoke.ContourArea(new VectorOfPoint(contour.ToArray()));
        double perimeter = CvInvoke.ArcLength(new VectorOfPoint(contour.ToArray()), true);
        double expectedCircleArea = (perimeter * perimeter) / (4 * Math.PI);

        VectorOfPoint contourVector = new VectorOfPoint(contour.ToArray());
        RotatedRect boundingBox = CvInvoke.MinAreaRect(contourVector);
        double width = boundingBox.Size.Width;
        double height = boundingBox.Size.Height;
        double aspectRatio = Math.Max(width / height, height / width);

        if ((contourArea / expectedCircleArea) > areaThreshold && aspectRatio > shapeThreshold)
        {
                filteredContours.Add(contour);
        }
        }




// Crea un'immagine vuota per disegnare i contorni
Mat resultImage = new Mat(image.Size, DepthType.Cv8U, 3);

// Disegna i contorni sull'immagine risultante
CvInvoke.CvtColor(image, resultImage, ColorConversion.Bgr2Bgra); // Converti l'immagine in BGR (a 3 canali con canale alpha)
// CvInvoke.DrawContours(resultImage, contours, -1, new MCvScalar(0, 0, 255), 2); // Disegna i contorni in rosso

// Disegna i contorni in rosso FUNZIA
// for (int i = 0; i < contours.Count; i++)
// {
//         Point[] contourPoints = new Point[contours[i].Count];
//         for (int j = 0; j < contours[i].Count; j++)
//         {
//                 contourPoints[j] = new Point((int)contours[i][j].X, (int)contours[i][j].Y);
//         }
// CvInvoke.Polylines(resultImage, contourPoints, true, new MCvScalar(0, 0, 255), 2);
// }

// Disegna i contorni filtrati in rosso sull'immagine risultante
CvInvoke.CvtColor(image, resultImage, ColorConversion.Bgr2Bgra); // Converti l'immagine in BGR (a 3 canali con canale alpha)
for (int i = 0; i < filteredContours.Count; i++)
{
        Point[] contourPoints = filteredContours[i].ToArray();
        CvInvoke.Polylines(resultImage, contourPoints, true, new MCvScalar(0, 0, 255), 2);
}



// Visualizza l'immagine risultante con i contorni
CvInvoke.Imshow("Contour Detection", resultImage);

// Attendere che l'utente prema un tasto per chiudere la finestra
CvInvoke.WaitKey(0);