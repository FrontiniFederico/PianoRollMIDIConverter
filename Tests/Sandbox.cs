using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Collections.Generic;
using Emgu.CV.Util;
using DynamicData;
using System.Linq;

string filepath = "C:\\Users\\Federico\\Desktop\\test_linea.png";

Mat image = new Mat(filepath, ImreadModes.Color);
CvInvoke.GaussianBlur(image, image, new Size(5, 5), 0);
CvInvoke.MedianBlur(image, image, 5);
CvInvoke.ConvertScaleAbs(image, image, 2.8, -120);
CvInvoke.CvtColor(image, image, ColorConversion.Bgr2Gray);

CvInvoke.Imshow("Greyscale image", image);

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

// Calcola i centroidi dei contorni
List<PointF> centroids = new List<PointF>();
foreach (var contour in filteredContours)
{
        float centerX = 0;
        float centerY = 0;

        foreach (var point in contour)
        {
                centerX += point.X;
                centerY += point.Y;
        }

        if (contour.Count > 0)
        {
                centerX /= contour.Count;
                centerY /= contour.Count;
        }

        centroids.Add(new PointF(centerX, centerY));
}

// Filtra i contorni rimuovendo quelli troppo lontani dagli altri
List<List<Point>> filteredContoursWithDistance = new List<List<Point>>();
for (int i = 0; i < filteredContours.Count; i++)
{
        bool keepContour = true;
        for (int j = i + 1; j < filteredContours.Count; j++)
        {
                double deltaX = centroids[i].X - centroids[j].X;
                double deltaY = centroids[i].Y - centroids[j].Y;

                // Imposta una soglia separata per le coordinate X e Y
                double maxDeltaX = 600;
                double maxDeltaY = 1000;

                if (Math.Abs(deltaX) > maxDeltaX || Math.Abs(deltaY) > maxDeltaY)
                {
                        keepContour = false;
                        break; // Esci dal ciclo interno se il contorno è troppo lontano
                }
        }
        if (keepContour)
        {
                filteredContoursWithDistance.Add(filteredContours[i]);
        }
}

// Disegna i contorni filtrati in rosso sull'immagine risultante
CvInvoke.CvtColor(image, resultImage, ColorConversion.Bgr2Bgra); // Converti l'immagine in BGR (a 3 canali con canale alpha)
for (int i = 0; i < filteredContoursWithDistance.Count; i++)
{
        Point[] contourPoints = filteredContoursWithDistance[i].ToArray();
        CvInvoke.Polylines(resultImage, contourPoints, true, new MCvScalar(0, 0, 255), 2);
}


// Visualizza l'immagine risultante con i contorni
CvInvoke.Imshow("Contour Detection", resultImage);

// Attendere che l'utente prema un tasto per chiudere la finestra
CvInvoke.WaitKey(0);

// Converti i contorni in liste di punti
List<List<Point>> contoursAsPoints = filteredContoursWithDistance.ToList();

// Interpola i contorni
var interpolatedContours = new List<List<Point>>();
for (int i = 0; i < contoursAsPoints.Count - 1; i++)
{
        var interpolatedContour = InterpolateBetweenContours(contoursAsPoints[i], contoursAsPoints[i + 1]);
        interpolatedContours.Add(interpolatedContour);
}

// Disegna i contorni interpolati sull'immagine risultante
CvInvoke.CvtColor(image, resultImage, ColorConversion.Bgr2Bgra);
foreach (var contour in interpolatedContours)
{
        var contourArray = contour.ToArray();
        CvInvoke.Polylines(resultImage, new VectorOfPoint(contourArray), true, new MCvScalar(0, 0, 255), 2);
}

// Visualizza l'immagine risultante con i contorni interpolati
CvInvoke.Imshow("Interpolated Contours", resultImage);

// Attendere che l'utente prema un tasto per chiudere la finestra
CvInvoke.WaitKey(0);

static List<Point> InterpolateBetweenContours(List<Point> contour1, List<Point> contour2)
{
        List<Point> interpolationPoints = new List<Point>();

    // Aggiungi il punto finale del primo contorno
        interpolationPoints.Add(contour1[contour1.Count - 1]);

    // Aggiungi il punto iniziale del secondo contorno
        interpolationPoints.Add(contour2[0]);

        return interpolationPoints;
}
