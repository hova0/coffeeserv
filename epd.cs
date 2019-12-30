using System;
using System.Collections.Generic;
using System.Text;
using Unosquare.RaspberryIO.Abstractions;
using System.Diagnostics;


namespace coffeeserv
{
    public class epd
    {
        public const int WIDTH = 176;
        public const int HEIGHT = 264;

        private IGpioPin busypin = Unosquare.RaspberryIO.Pi.Gpio[(int)BCM_Pins.BUSY_PIN];
        private IGpioPin dcpin = Unosquare.RaspberryIO.Pi.Gpio[(int)BCM_Pins.DC_PIN];

        public epd()
        {
            // copied directly from python
            busypin.PinMode = GpioPinDriveMode.Input;
            dcpin.PinMode = GpioPinDriveMode.Output;

            reset();
            send_command(EPD_Commands.POWER_ON);
            Console.WriteLine("Waiting until  idle");


            wait_until_idle();
            Console.WriteLine("Setting initial parameters");
            send_command(EPD_Commands.PANEL_SETTING);
            send_data(new byte[] { 0xAF });   //KW-BF   KWR-AF    BWROTP 0f
            System.Threading.Thread.Sleep(100);
            send_command(EPD_Commands.PLL_CONTROL);
            send_data(new byte[] { 0x3A }); // 3A 100HZ   29 150Hz 39 200HZ    31 171HZ
            System.Threading.Thread.Sleep(100);
            send_command(EPD_Commands.POWER_SETTING);
            send_data(new byte[] {0x03, //# VDS_EN, VDG_EN
                0x00, //# VCOM_HV, VGHL_LV[1], VGHL_LV[0]
                0x2b,  //# VDH
                0x2b,  //# VDL
                0x09}); //# VDHR
            System.Threading.Thread.Sleep(100);
            send_command(EPD_Commands.BOOSTER_SOFT_START);
            send_data(new byte[] { 0x07, 0x07, 0x17 });
            System.Threading.Thread.Sleep(100);

            send_command(EPD_Commands.POWER_OPTIMIZATION);
            send_data(new byte[] { 0x60, 0xA5 });
            send_command(EPD_Commands.POWER_OPTIMIZATION);
            send_data(new byte[] { 0x89, 0xA5 });
            send_command(EPD_Commands.POWER_OPTIMIZATION);
            send_data(new byte[] { 0x90, 0x00 });
            send_command(EPD_Commands.POWER_OPTIMIZATION);
            send_data(new byte[] { 0x93, 0x2A });
            send_command(EPD_Commands.POWER_OPTIMIZATION);
            send_data(new byte[] { 0x73, 0x41 });
            System.Threading.Thread.Sleep(100);
            send_command(EPD_Commands.VCM_DC_SETTING_REGISTER);
            send_data(new byte[] { 0x12 });
            send_command(EPD_Commands.VCOM_AND_DATA_INTERVAL_SETTING);
            send_data(new byte[] { 0x87 });  //  define by OTP
            System.Threading.Thread.Sleep(100);
            set_lut();
            System.Threading.Thread.Sleep(100);
            send_command(EPD_Commands.PARTIAL_DISPLAY_REFRESH);
            send_data(new byte[] { 0x00 });
            System.Threading.Thread.Sleep(100);


        }

        public void reset()
        {
            Unosquare.RaspberryIO.Pi.Gpio[(int)BCM_Pins.RST_PIN].Write(GpioPinValue.High);
            delay(200);
            Unosquare.RaspberryIO.Pi.Gpio[(int)BCM_Pins.RST_PIN].Write(GpioPinValue.Low);
            delay(200);

            Unosquare.RaspberryIO.Pi.Gpio[(int)BCM_Pins.RST_PIN].Write(GpioPinValue.High);
            delay(200);

        }

        public void send_command(EPD_Commands c)
        {
            dcpin.Write(GpioPinValue.Low);
            Unosquare.RaspberryIO.Pi.Spi.Channel0.Write(new byte[] { (byte)c });
        }
        public void send_command(byte c)
        {
            dcpin.Write(GpioPinValue.Low);
            Unosquare.RaspberryIO.Pi.Spi.Channel0.Write(new byte[] { c });
        }

        public void send_data(byte[] data)
        {
            
            //if (data.Length > 256)
            //{
                //byte[] buf = new byte[256];
                //int i = 0;
                //for (i = 0; i < data.Length - 256; i += 256)
                //{
                //    Buffer.BlockCopy(data, i, buf, 0, 256);
                //    Unosquare.RaspberryIO.Pi.Spi.Channel0.Write(buf);
                //}
                //buf = new byte[data.Length - i];
                //Buffer.BlockCopy(data, i, buf, 0, (data.Length - i));
                //Unosquare.RaspberryIO.Pi.Spi.Channel0.Write(buf);

            //}
            //else
            //    Unosquare.RaspberryIO.Pi.Spi.Channel0.Write(data);
            byte[] buf = new byte[1];
            foreach(byte b in data) {
                buf[0] = b;
                dcpin.Write(GpioPinValue.High);
                Unosquare.RaspberryIO.Pi.Spi.Channel0.Write(buf);
            }
            //dcpin.Write(GpioPinValue.Low);
        }

        public void send_data(byte data)
        {
            dcpin.Write(GpioPinValue.High);
            Unosquare.RaspberryIO.Pi.Spi.Channel0.SendReceive(new byte[] { data });
        }

        public void wait_until_idle()
        {
            // The python loop is "while(epdconfig.digital_read(self.busy_pin) == 0):      # 0: idle, 1: busy"
            // Which doesn't make sense if you loop while it's idle ???

            //Console.WriteLine($"[Waituntilidle] Pin Number B{busypin.BcmPinNumber}/{busypin.PhysicalPinNumber} Status : {busypin.Read()} ");

            while (busypin.Read() != true)
                System.Threading.Thread.Sleep(100); //delay(100);
                                                    //Console.WriteLine($"[Waituntilidle] no longer busy");
        }

        public static void delay(int ms)
        {
            System.Threading.Thread.Sleep(ms);
            //Unosquare.RaspberryIO.Pi.Timing.SleepMilliseconds((uint)ms);
        }

        public void Clear()
        {
            int size = (WIDTH * HEIGHT) / 8;
            //byte[] displaybuffer = new byte[size];
            byte[] empty = new byte[] { 0xFF };

            send_command(EPD_Commands.DATA_START_TRANSMISSION_1);
            for (int i = 0; i < size; i++)
                send_data(empty);
            send_command(EPD_Commands.DATA_STOP);

            send_command(EPD_Commands.DATA_START_TRANSMISSION_2);
            for (int i = 0; i < size; i++)
                send_data(empty);
            send_command(EPD_Commands.DATA_STOP);
            send_command(EPD_Commands.DISPLAY_REFRESH);

            wait_until_idle();
        }

        public static byte[] GetBlankBuffer()
        {
            byte[] b = new byte[((WIDTH/8) * HEIGHT)];
            for (int i = 0; i < ((WIDTH/8) * HEIGHT); i++)
                b[i] = 0x00;
            return b;
        }

        public void Display(byte[] displaybuffer)
        {
            int size = (WIDTH * HEIGHT) / 8;
            if (displaybuffer.Length < size)
                throw new Exception("Invalid display buffer size.  Too small!");
            //for(int i = 0; i < size; i++)
            //    displaybuffer[i] = (byte)(~displaybuffer[i]);   //Reverse

            send_command(EPD_Commands.DATA_START_TRANSMISSION_1);
            //black image data

            send_data(displaybuffer);
            //for(int i = 0; i < size; i++)
            //    send_data((byte)~displaybuffer[i]);
            send_command(EPD_Commands.DATA_STOP);

            send_command(EPD_Commands.DATA_START_TRANSMISSION_2);
            // red image data
            for (int i = 0; i < size; i++)
                send_data(0x00);
            send_command(EPD_Commands.DATA_STOP);

            //delay(5);
            send_command(EPD_Commands.DISPLAY_REFRESH);
            //delay(5);
            wait_until_idle();
        }


        public void Sleep()
        {
            send_command(0x50);
            send_data(0xF7);
            send_command(0x02);
            send_command(0x07);
            send_data(0xA5);

        }

        private void set_lut()
        {
            byte[] lut_vcom_dc = new byte[]
            {
                0x00, 0x00,
                0x00, 0x1A, 0x1A, 0x00, 0x00, 0x01,
                0x00, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x00, 0x0E, 0x01, 0x0E, 0x01, 0x10,
                0x00, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
                0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
                0x00, 0x23, 0x00, 0x00, 0x00, 0x01
            };

            byte[] lut_ww = new byte[]
            {
                0x90, 0x1A, 0x1A, 0x00, 0x00, 0x01,
                0x40, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
                0x80, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
                0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
                0x00, 0x23, 0x00, 0x00, 0x00, 0x01
            };

            // R22H    r
            byte[] lut_bw = new byte[]
            {
                0xA0, 0x1A, 0x1A, 0x00, 0x00, 0x01,
                0x00, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
                0x90, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0xB0, 0x04, 0x10, 0x00, 0x00, 0x05,
                0xB0, 0x03, 0x0E, 0x00, 0x00, 0x0A,
                0xC0, 0x23, 0x00, 0x00, 0x00, 0x01
            };

            //# R23H    w
            byte[] lut_bb = new byte[]
            {
                0x90, 0x1A, 0x1A, 0x00, 0x00, 0x01,
                0x40, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
                0x80, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
                0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
                0x00, 0x23, 0x00, 0x00, 0x00, 0x01
            };
            //# R24H    b
            byte[] lut_wb = new byte[]
            {
                0x90, 0x1A, 0x1A, 0x00, 0x00, 0x01,
                0x20, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x84, 0x0E, 0x01, 0x0E, 0x01, 0x10,
                0x10, 0x0A, 0x0A, 0x00, 0x00, 0x08,
                0x00, 0x04, 0x10, 0x00, 0x00, 0x05,
                0x00, 0x03, 0x0E, 0x00, 0x00, 0x0A,
                0x00, 0x23, 0x00, 0x00, 0x00, 0x01
            };
            send_command(EPD_Commands.LUT_FOR_VCOM);

            send_data(lut_vcom_dc);
            send_command(EPD_Commands.LUT_WHITE_TO_WHITE);
            send_data(lut_ww);
            send_command(EPD_Commands.LUT_BLACK_TO_WHITE);
            send_data(lut_bw);
            send_command(EPD_Commands.LUT_WHITE_TO_BLACK);
            send_data(lut_bb);
            send_command(EPD_Commands.LUT_BLACK_TO_BLACK);
            send_data(lut_wb);


        }

    }
}
