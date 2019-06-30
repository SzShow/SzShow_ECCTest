using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace EncryptProject
{

	//特殊な整数演算を実装するクラス
	static class IntegerCalclator
	{
		#region Public Function

		//BigInteger内でランダムな数値を設定
        //lengthで指定したビット長の整数値を入れたかったが，
        //思ったより手間がかかりそうだったので断念
        //スピードを度外視すれば，BigInteger同士の演算で
        //実現はできる？
		public static BigInteger RandamBigInteger(int length)
		{
            Random rnd = new Random();
            //StringBuilder bins = new StringBuilder();
            //         for (int index = 0; index<length; index++)
            //         {
            //             bins.Append(rnd.Next(0, 1).ToString());//値を0~9に制限
            //         }
            //Byte[] Ubytes = new byte[length + 1];
            //bytes.CopyTo(Ubytes, 1);
            //string str = bins.ToString();
            BigInteger bigInteger = new BigInteger(rnd.Next(1, 845));
            return bigInteger;
		}

		//BigIntegerを２進数で表現した文字列に変換
		public static string BigIntegerToBin(BigInteger a)
		{
			string bin = "";
			BigInteger temp = a;
			BigInteger q = 0;

			//tempを2で割り続け，1以下になるまで繰り返す．
			while (temp > 1)
			{
				//余りの値をbinの右端に連結
				temp = BigInteger.DivRem(temp, 2, out q);
				bin = String.Concat(q.ToString(), bin);
			}
			bin = String.Concat(temp.ToString(), bin);
			return bin;
		}

		//ルシャンドル記号
		//aが素数pでの平方剰余であるかの判定
        //オイラーの規準を使用
		public static bool LegendreSymbol(BigInteger a, BigInteger p)
		{
			BigInteger s = BigInteger.ModPow(a, (p - 1) / 2, p);
			if (s == 1) { return true; }
			else { return false; }
		}

		//pを法とした時のaの平方根を計算
		public static BigInteger SqrtMod(BigInteger a, BigInteger p)
		{
			//初期設定
			BigInteger g = GetQNP(p);//平方非剰余となる最初の整数を探す
			BigInteger t = p - 1;
			int rem = (int)(t % 2);//素数-1は必ず偶数になるので0
			int r = 0;
			while (rem == 0)
			{
				//素数p-1を奇数になるまで割り続ける
				t = t / 2;
				rem = (int)(t % 2);
				r += 1;
			}
			BigInteger h = (g ^ t) % p;
			BigInteger b = (a ^ t) % p;
			BigInteger z = SqrtModSub(p, h, b, r);
			return (a ^ ((t + 1) / 2) * z) % p;


		}

		#endregion


		#region Internal Function

		//pを法とした平方非剰余を求める
		private static int GetQNP(BigInteger p)
		{
			for (int q = 2; q < p - 1; q++)
			{
				if (!LegendreSymbol(q, p)) return q;
			}
			return 0;
		}

		private static BigInteger SqrtModSub(BigInteger p, BigInteger h,
			BigInteger b, int r)
		{
			BigInteger z = 1;
			h = h ^ (p - 2) % p;//pを法としたモジュラ逆数を返す
			for (int i = r - 2; i > 0; i--)
			{
				if ((int)(b ^ 2 ^ i % p) == -1)
				{
					z = z * h % p;
					b = b * (h ^ 2) % p;
				}
				h = h ^ 2 % p;
			}
			return z;
		}

		#endregion

	}
}
