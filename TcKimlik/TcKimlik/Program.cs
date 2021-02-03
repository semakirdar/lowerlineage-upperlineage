using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcKimlik
{
    using System.Globalization;
    using System.Threading;
    using System;
    using System.Linq;
    using System.Text;

    class Program
    {
        static void Main()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("tr-TR");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            var isValidTcKimlikNo = false;
            var client = new TcKimlikService.KPSPublicSoapClient();
            var tckimlikNo = string.Empty;
            while (!isValidTcKimlikNo)
            {
                Console.Write("Tc Kimlik Numaranızı Girin:");
                tckimlikNo = Console.ReadLine() ?? "";
                isValidTcKimlikNo = TcDogrula(tckimlikNo);
            }

            Console.WriteLine("Üst Soy Bilgilerini Girin");
            Console.Write("{0,11}", "Ad:");
            var ad = Console.ReadLine();
            Console.Write("{0,11}", "Soyad:");
            var soyad = Console.ReadLine();
            Console.Write("{0,11}", "Doğum Yılı:");
            var dogumYili = int.Parse(Console.ReadLine() ?? string.Empty);

        newTry:
            int.TryParse(tckimlikNo.Substring(0, 5), out var ilk5);
            int.TryParse(tckimlikNo.Substring(5, 4), out var orta4);
            int.TryParse(tckimlikNo[9].ToString(), out var onuncu);
            int.TryParse(tckimlikNo.LastOrDefault().ToString(), out var sonuncu);
            var sonCikarim = 4;
            var ilk5C3 = tckimlikNo[2];
            var orta4C2 = tckimlikNo[6];
            ilk5 += 3;
            orta4 -= 1;
            if (ilk5C3 != ilk5.ToString()[2])
            {
                sonCikarim = 6;
            }

            if (orta4C2 != orta4.ToString()[1])
            {
                sonCikarim = 2;
            }

            if (sonuncu < sonCikarim)
            {
                sonuncu = sonuncu + 10 - sonCikarim;
            }
            else
            {
                sonuncu -= sonCikarim;
            }


            onuncu = Find10Th((ilk5.ToString() + orta4).Select(x => int.Parse(x.ToString())).Sum(), sonuncu);

            var test = client.TCKimlikNoDogrula(long.Parse(tckimlikNo), ad, soyad, dogumYili);
            if (!test)
            {
                Console.WriteLine("{2}: {0} - {1} değil aramaya devam ediliyor.", ad, soyad, tckimlikNo);
                tckimlikNo = ilk5.ToString() + orta4 + onuncu + sonuncu;
                goto newTry;
            }
            Console.WriteLine("{0} - {1} Bulundu: {2}", ad, soyad, tckimlikNo);
            Console.ReadLine();
        }

        public static int Find10Th(int ilk9Toplami, int sonuncu)
        {
            for (var i = 0; i < 10; i++)
            {
                if ((ilk9Toplami + i) % 10 == sonuncu)
                    return i;
            }
            return default;
        }

        public static bool TcDogrula(string tcKimlikNo)
        {
            if (tcKimlikNo.Length != 11) return false;
            var tcNo = long.Parse(tcKimlikNo);

            var atcno = tcNo / 100;
            var btcno = tcNo / 100;

            var c1 = atcno % 10;
            atcno /= 10;
            var c2 = atcno % 10;
            atcno /= 10;
            var c3 = atcno % 10;
            atcno /= 10;
            var c4 = atcno % 10;
            atcno /= 10;
            var c5 = atcno % 10;
            atcno /= 10;
            var c6 = atcno % 10;
            atcno /= 10;
            var c7 = atcno % 10;
            atcno /= 10;
            var c8 = atcno % 10;
            atcno /= 10;
            var c9 = atcno % 10;
            var q1 = (10 - ((c1 + c3 + c5 + c7 + c9) * 3 + c2 + c4 + c6 + c8) % 10) % 10;
            var q2 = (10 - ((c2 + c4 + c6 + c8 + q1) * 3 + c1 + c3 + c5 + c7 + c9) % 10) % 10;

            var returnvalue = btcno * 100 + q1 * 10 + q2 == tcNo;
            return returnvalue;
        }
    }
}