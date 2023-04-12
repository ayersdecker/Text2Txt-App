using IronOcr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text2Txt
{
    public class OCR
    {
        public async Task<string> OCRAction()
        {
            // Create a new instance of the IronTesseract class.
            var Ocr = new IronTesseract();

            // Call the SelectImage method to allow the user to select an image from their device.
            string path = await SelectImage();

            // Create a new instance of the OcrInput class using the selected image path.
            using (var Input = new OcrInput(path))
            {
                // Uncomment the following line if the image is not straight.
                // Input.Deskew();

                // Uncomment the following line if the image contains digital noise.
                // Input.DeNoise();

                // Use the Read method of the IronTesseract class to read the text from the image.
                var Result = Ocr.Read(Input);

                // Return the text from the image as a string with any line breaks removed.
                return Result.Text.Replace("\r", "").Replace("\n", " ");
            }
        }
        public async Task<string> SelectImage()
        {
            // Use the MediaPicker class to allow the user to select an image from their device.
            var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Select a photo"
            });

            // If an image is selected, return the full path of the selected image as a string.
            if (result != null)
            {
                return result.FullPath;
            }

            // If no image is selected, return null.
            return null;
        }
    }
}
