using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Numerics;

namespace EncryptProject
{
	public static class FileManager
	{
		#region LoadFunction

		//バイナリファイルの読み込み
		public static byte[] LoadBinaryFile(string str)
		{
            //ASCIIコードとして読み取り
			byte[] buf = BasicLoad(str);
			return buf;

		}

		//バイナリファイルのテキスト化
		public static string LoadTextFile(string str)
		{
			//ファイル読み込み
			byte[] buf = BasicLoad(str);

			//ストリング型への変換
			return Encoding.ASCII.GetString(buf);
		}

		//暗号文の読み取り
		public static void LoadEncryption(string str,
			out Ep M1, out Ep M2)
		{
			byte[] buf = BasicLoad(str);
            string encryption = Encoding.ASCII.GetString(buf);
            string[] allM = encryption.Split("_");
			

			M1 = new Ep(BigInteger.Parse(allM[0]), BigInteger.Parse(allM[1]));
			M2 = new Ep(BigInteger.Parse(allM[2]), BigInteger.Parse(allM[3]));
		}

		//秘密鍵の読み取り
		public static BigInteger LoadPrivateKey(string str)
		{
            byte[] buf = BasicLoad(str);
            string text = Encoding.ASCII.GetString(buf);
			return BigInteger.Parse(text);
		}

		#endregion

		#region OutputFunction

		public static void OutputEncryptFile(Ep M1, Ep M2, string path)
		{
			//BigIntegerをstring化
			string str1x = M1.x.ToString();
			string str1y = M1.y.ToString();
			string str2x = M2.x.ToString();
			string str2y = M2.y.ToString();
			string str = str1x + "_" + str1y + "_" +
				str2x + "_" + str2y;//_を目印に区切れるようにする

			//stringをByte[]に変換
			byte[] buf = Encoding.ASCII.GetBytes(str);

			//データの出力
			File.WriteAllBytes(@path, buf);
		}

		public static void OutputPublicKey(Ep B, Ep P, string path)
		{
			//BigIntegerをstring化
			string str1x = B.x.ToString();
			string str1y = B.y.ToString();
			string str2x = P.x.ToString();
			string str2y = P.y.ToString();
			string str = str1x + "_" + str1y + "_" +
				str2x + "_" + str2y;//_を目印に区切れるようにする

			//stringをByte[]に変換
			byte[] buf = Encoding.ASCII.GetBytes(str);

			//データの出力
			File.WriteAllBytes(@path, buf);
		}

		public static void OutputPrivateKey(BigInteger Kp, string path)
		{
			byte[] buf = Encoding.ASCII.GetBytes(Kp.ToString());
			File.WriteAllBytes(@path, buf);
		}

		public static void OutputDecryptedMsg(byte[] m, string path)
		{
            
			File.WriteAllBytes(@path, m);
		}
		#endregion

		#region InternalFunction

		public static byte[] BasicLoad(string str)
		{
            //出力はASCIIコードとして変換されることに注意
			FileStream fs = new FileStream(
				@str, FileMode.Open, FileAccess.Read);
			byte[] buf = new byte[(int)fs.Length];
			fs.Read(buf);
			fs.Dispose();
			return buf;
		}

		#endregion

		#region TextFunction

		public static string[] SplitParamText(string text)
		{
			string[] lines = text.Split(new string[] { "\r\n" },
				StringSplitOptions.None);
			return lines;
		}

		public static void ExtractParams(string[] texts)
		{

		}

		#endregion
	}
}
