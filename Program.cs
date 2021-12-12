using System;
using OpenCvSharp;
using System.Numerics;
namespace opencv_anonymousface
{
    class Program
    {
        private const string Classifier_file="data/haarcascade_frontalface_default.xml";
        private const string Mask_file="data/mask.png";
        static void Main(string[] args)
        {
            var input_path=args[0];
            
            Mat image = Cv2.ImRead(input_path,ImreadModes.Unchanged);
            Mat mask_image = Cv2.ImRead(Mask_file,ImreadModes.Unchanged);
            Mat out_image=Cv2.ImRead(input_path,ImreadModes.Unchanged);;

            Cv2.CvtColor(out_image,out_image,ColorConversionCodes.RGB2RGBA);

            var Cascade = new CascadeClassifier(Classifier_file);
            Rect[] faces = Cascade.DetectMultiScale(image, 1.31, 3, 0, new Size(20, 20));

            for (int i = 0; i < faces.Length; i++)
            {

                Mat temp1=Mat.Zeros(mask_image.Height,mask_image.Width,MatType.CV_16UC4);
                Cv2.Resize(mask_image,temp1,new Size(faces[i].Width,faces[i].Height),0,0,InterpolationFlags.Lanczos4);

                var rect = new Rect(faces[i].X, faces[i].Y, faces[i].Width, faces[i].Height);
                Mat[] channels=new Mat[4];

	            Cv2.Split(temp1,out channels);

                var mask_of_mask_image=new Mat();
                channels[3].CopyTo(mask_of_mask_image);

                Cv2.Threshold(
                    channels[3],
                    mask_of_mask_image,
                    200, 255,ThresholdTypes.Binary
                );

                //Cv2.Co(new Mat(out_image,rect),temp1,temp3,mask_of_mask_image2);
                temp1.CopyTo(out_image[rect],mask_of_mask_image);
            }

            Cv2.CvtColor(out_image,out_image,ColorConversionCodes.BGR2BGRA);

            Cv2.ImWrite(args[1],out_image);
        }
    }
}
