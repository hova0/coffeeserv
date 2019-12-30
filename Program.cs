using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace coffeeserv
{
    internal class Program
    {
        private static void dumppins()
        {
            for (int x = 0; x <= 24; x++)
            {
                if (x == 17 || x == 25 || x == 8 || x == 24)
                {
                    Console.WriteLine(
                        $"* Pin Number B{Pi.Gpio[x].BcmPinNumber}/{Pi.Gpio[x].PhysicalPinNumber} Status : {Pi.Gpio[x].Read()} ");
                }
                else
                {
                    Console.WriteLine(
                        $"  Pin Number B{Pi.Gpio[x].BcmPinNumber}/{Pi.Gpio[x].PhysicalPinNumber} Status : {Pi.Gpio[x].Read()} ");
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Startup");
            Pi.Init<BootstrapWiringPi>();

            Pi.Gpio[(int) BCM_Pins.BUSY_PIN].PinMode = GpioPinDriveMode.Input;
            Pi.Gpio[(int) BCM_Pins.CS_PIN].PinMode = GpioPinDriveMode.Output;
            Pi.Gpio[(int) BCM_Pins.DC_PIN].PinMode = GpioPinDriveMode.Output;
            Pi.Gpio[(int) BCM_Pins.RST_PIN].PinMode = GpioPinDriveMode.Output;
            IGpioPin key1 = Pi.Gpio[5];
            key1.PinMode = GpioPinDriveMode.Input;
            Pi.Spi.Channel0Frequency = 2_000_000;


            Console.WriteLine("Dumping pins");

            Console.WriteLine("GPIO initialized");
            epd e = new epd();
            Console.WriteLine("Initialization complete");
            //e.Sleep();
            epd.delay(100);
            Console.WriteLine("Generating Image...");
            byte[] display = epd.GetBlankBuffer();
            Console.WriteLine("Image dimensions : {0}", display.Length);
            for (int i = 0; i < display.Length; i++)
            {
                int posx = (i * 8) % epd.WIDTH;
                int posy = (i * 8) / epd.WIDTH ;
                display[i] = 0x00;
                if (posy >= 26 && posy <= 32)   // Makes a line
                    display[i] = 0xFF;
                if(posy >= 32 && posy <= 64)
                {
                    if(posx == posy)
                        display[i] = 0xFF;
                }
                ////Dump  image to console
                //if((i * 8) % epd.WIDTH == 0) { 
                //    Console.WriteLine();
                //    Console.Write("{0}:", posy);
                //}
                //Console.Write(display[i].ToString("X2"));
            }
            Console.WriteLine("Displaying Image...");

            e.Display(display);

          
            Console.WriteLine("Image one displayed");
            epd.delay(500);

            e.Sleep();
            
            

            //Console.WriteLine("Sleeping.  Press Key 1 to exit.");
            //while (key1.Read() == true)
            //    epd.delay(100);
            ////e.Clear();
            //Console.WriteLine("Key 1 pressed, exiting.");

            //e.Clear();
            //epd.delay(100);
            //Console.WriteLine("Sleeping");
            //e.Sleep();
            
        }
        byte[] masks = new byte[] {
        0x80,
        0x40,
        0x20,
        0x10,
        0x08,
        0x04,
        0x02,
        0x01
    };
        public bool GetPixel(byte[] buffer,System.Drawing.Size size, int x, int y)
        {
            int bytesperline = size.Width / 8;
            int offset = x / 8 + (y * bytesperline);
            byte target = buffer[offset];
            int maskindex = x % 8;
            return (target & masks[maskindex]) != 0;
        }

        // Define other methods and classes here
        public void SetPixel(byte[] buffer,System.Drawing.Size size, int x, int y, bool ison)
        {

            int bytesperline = size.Width / 8;
            int maskindex = x % 8;
            //Console.WriteLine(masks[maskindex].ToString("X2"));
            int offset = x / 8 + (y * bytesperline);
            byte target = buffer[offset];
            if (ison)
            {
                target = (byte)(target | masks[maskindex]);
            }
            else
            {
                target = (byte)(target & (~masks[maskindex]));
            }
            buffer[offset] = target;
            //byte mask = (byte) ( 
        }

        public byte[] ConvertToBW(System.Drawing.Bitmap bmp)
        {
            var bmpsize = bmp.Size;
            byte[] data = new byte[bmpsize.Height * (bmpsize.Width / 8)];
            for(int y = 0; y < bmpsize.Height; y++)
            {
                for(int x = 0; x < bmpsize.Width; x++)
                {
                    var pix = bmp.GetPixel(x, y);
                    if(pix.GetBrightness() > .5)
                    {
                        // Black?  Set bit
                        int lookup = y * epd.WIDTH + (x / 8);
                        byte dest = data[lookup];
                        dest |= (byte)(0x80 >> (x % 8));    // Bits filled left to right
                        data[lookup] = dest;
                    }

                }
            }
            return data;
        }


    }


    //public class BootstrapPi : Unosquare.RaspberryIO.Abstractions.IBootstrap
    //{
    //    public void Bootstrap()
    //    {
    //        // ??

    //    }
    //}
}