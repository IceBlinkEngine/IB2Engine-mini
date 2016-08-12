using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceBlink2mini
{
    public class BitmapStringConversion
    {
        //List<string> stringBitmap = new List<string>();
        Dictionary<int, char> IntToB64 = new Dictionary<int, char>();
        Dictionary<char, int> B64ToInt = new Dictionary<char, int>();
        //public string workingDir = "";
        public List<ImageData> listImages = new List<ImageData>();

        public BitmapStringConversion()
        {
            fillIntToB64();
            fillB64ToInt();
        }
        public void fillIntToB64()
        {
            IntToB64.Add(0, '0');
            IntToB64.Add(1, '1');
            IntToB64.Add(2, '2');
            IntToB64.Add(3, '3');
            IntToB64.Add(4, '4');
            IntToB64.Add(5, '5');
            IntToB64.Add(6, '6');
            IntToB64.Add(7, '7');
            IntToB64.Add(8, '8');
            IntToB64.Add(9, '9');
            IntToB64.Add(10, 'A');
            IntToB64.Add(11, 'B');
            IntToB64.Add(12, 'C');
            IntToB64.Add(13, 'D');
            IntToB64.Add(14, 'E');
            IntToB64.Add(15, 'F');
            IntToB64.Add(16, 'G');
            IntToB64.Add(17, 'H');
            IntToB64.Add(18, 'I');
            IntToB64.Add(19, 'J');
            IntToB64.Add(20, 'K');
            IntToB64.Add(21, 'L');
            IntToB64.Add(22, 'M');
            IntToB64.Add(23, 'N');
            IntToB64.Add(24, 'O');
            IntToB64.Add(25, 'P');
            IntToB64.Add(26, 'Q');
            IntToB64.Add(27, 'R');
            IntToB64.Add(28, 'S');
            IntToB64.Add(29, 'T');
            IntToB64.Add(30, 'U');
            IntToB64.Add(31, 'V');
            IntToB64.Add(32, 'W');
            IntToB64.Add(33, 'X');
            IntToB64.Add(34, 'Y');
            IntToB64.Add(35, 'Z');
            IntToB64.Add(36, 'a');
            IntToB64.Add(37, 'b');
            IntToB64.Add(38, 'c');
            IntToB64.Add(39, 'd');
            IntToB64.Add(40, 'e');
            IntToB64.Add(41, 'f');
            IntToB64.Add(42, 'g');
            IntToB64.Add(43, 'h');
            IntToB64.Add(44, 'i');
            IntToB64.Add(45, 'j');
            IntToB64.Add(46, 'k');
            IntToB64.Add(47, 'l');
            IntToB64.Add(48, 'm');
            IntToB64.Add(49, 'n');
            IntToB64.Add(50, 'o');
            IntToB64.Add(51, 'p');
            IntToB64.Add(52, 'q');
            IntToB64.Add(53, 'r');
            IntToB64.Add(54, 's');
            IntToB64.Add(55, 't');
            IntToB64.Add(56, 'u');
            IntToB64.Add(57, 'v');
            IntToB64.Add(58, 'w');
            IntToB64.Add(59, 'x');
            IntToB64.Add(60, 'y');
            IntToB64.Add(61, 'z');
            IntToB64.Add(62, '+');
            IntToB64.Add(63, '/');
        }
        public void fillB64ToInt()
        {
            B64ToInt.Add('0', 0);
            B64ToInt.Add('1', 1);
            B64ToInt.Add('2', 2);
            B64ToInt.Add('3', 3);
            B64ToInt.Add('4', 4);
            B64ToInt.Add('5', 5);
            B64ToInt.Add('6', 6);
            B64ToInt.Add('7', 7);
            B64ToInt.Add('8', 8);
            B64ToInt.Add('9', 9);
            B64ToInt.Add('A', 10);
            B64ToInt.Add('B', 11);
            B64ToInt.Add('C', 12);
            B64ToInt.Add('D', 13);
            B64ToInt.Add('E', 14);
            B64ToInt.Add('F', 15);
            B64ToInt.Add('G', 16);
            B64ToInt.Add('H', 17);
            B64ToInt.Add('I', 18);
            B64ToInt.Add('J', 19);
            B64ToInt.Add('K', 20);
            B64ToInt.Add('L', 21);
            B64ToInt.Add('M', 22);
            B64ToInt.Add('N', 23);
            B64ToInt.Add('O', 24);
            B64ToInt.Add('P', 25);
            B64ToInt.Add('Q', 26);
            B64ToInt.Add('R', 27);
            B64ToInt.Add('S', 28);
            B64ToInt.Add('T', 29);
            B64ToInt.Add('U', 30);
            B64ToInt.Add('V', 31);
            B64ToInt.Add('W', 32);
            B64ToInt.Add('X', 33);
            B64ToInt.Add('Y', 34);
            B64ToInt.Add('Z', 35);
            B64ToInt.Add('a', 36);
            B64ToInt.Add('b', 37);
            B64ToInt.Add('c', 38);
            B64ToInt.Add('d', 39);
            B64ToInt.Add('e', 40);
            B64ToInt.Add('f', 41);
            B64ToInt.Add('g', 42);
            B64ToInt.Add('h', 43);
            B64ToInt.Add('i', 44);
            B64ToInt.Add('j', 45);
            B64ToInt.Add('k', 46);
            B64ToInt.Add('l', 47);
            B64ToInt.Add('m', 48);
            B64ToInt.Add('n', 49);
            B64ToInt.Add('o', 50);
            B64ToInt.Add('p', 51);
            B64ToInt.Add('q', 52);
            B64ToInt.Add('r', 53);
            B64ToInt.Add('s', 54);
            B64ToInt.Add('t', 55);
            B64ToInt.Add('u', 56);
            B64ToInt.Add('v', 57);
            B64ToInt.Add('w', 58);
            B64ToInt.Add('x', 59);
            B64ToInt.Add('y', 60);
            B64ToInt.Add('z', 61);
            B64ToInt.Add('+', 62);
            B64ToInt.Add('/', 63);
        }
        /*public ImageData ConvertBitmapToImageData(string filenameNoExt, string pathandfile)
        {
            bitmap = new Bitmap(pathandfile);
            ImageData imd = new ImageData();
            imd.name = filenameNoExt;
            imd.width = bitmap.Width;
            imd.height = bitmap.Height;
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color clr = bitmap.GetPixel(x, y);
                    imd.pixelData.Add(IntToB64[clr.A / 4].ToString() + IntToB64[clr.R / 4].ToString() + IntToB64[clr.G / 4].ToString() + IntToB64[clr.B / 4].ToString());
                }
            }
            return imd;
        }*/
        public Bitmap ConvertImageDataToBitmap(ImageData imd)
        {
            Bitmap bitmap = new Bitmap(imd.width, imd.height, PixelFormat.Format32bppArgb);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    string str = imd.pixelData[y * imd.width + x];
                    Color clr = Color.FromArgb(B64ToInt[str[0]] * 4, B64ToInt[str[1]] * 4, B64ToInt[str[2]] * 4, B64ToInt[str[3]] * 4);
                    bitmap.SetPixel(x, y, clr);
                }
            }
            return bitmap;
        }
    }
}
