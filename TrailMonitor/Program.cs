using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        while (true)
        {
            bool detected = DetectPinkColor();
            if (detected)
            {
                await SendDiscordAlert();
            }
            await Task.Delay(500); // check every 500ms
        }
    }

    static bool DetectPinkColor()
    {
        int pinkPixelCount = 0;  // pixels detected counter

        using (Bitmap screenshot = CaptureScreen())
        {
            for (int x = 0; x < screenshot.Width; x += 5)  
            {
                for (int y = 0; y < screenshot.Height; y += 5)
                {
                    Color pixel = screenshot.GetPixel(x, y);
                    if (IsPink(pixel))
                    {
                        pinkPixelCount++;
                        if (pinkPixelCount >= 10)  // 10 pix minimum
                            return true;
                    }
                }
            }
        }
        return false;
    }

    static Bitmap CaptureScreen()
    {
        Rectangle bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
        Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
        }
        return bitmap;
    }

    static bool IsPink(Color color)
    {
        return color.R >= 240 && color.R <= 255 &&  // color variation fix
               color.G >= 0 && color.G <= 15 &&     
               color.B >= 240 && color.B <= 255;    
    }

    static DateTime lastAlertTime = DateTime.MinValue;

    static async Task SendDiscordAlert()
    {
        if ((DateTime.Now - lastAlertTime).TotalSeconds < 5)
            return; // prevent discord message spam

        lastAlertTime = DateTime.Now;

        string webhookUrl = "";
        string message = "🚨 Trail-Mapper bot '**Qati**' detected a nether trail! Coordinates saved <a:ahhhh:422347143983005696>";

        using (HttpClient client = new HttpClient())
        {
            var jsonPayload = "{\"content\":\"" + message + "\"}";
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            await client.PostAsync(webhookUrl, content);
        }
    }
}
