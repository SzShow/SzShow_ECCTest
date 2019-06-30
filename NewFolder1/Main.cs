using System;
using System.IO;
using System.Numerics;
using System.Text;
using System.Security.Cryptography;

namespace EncryptProject
{

	#region enum


	enum EExcuteMode
	{
		MakeKey,
		Encrypt,
		Decrypt,
        ECDMakeKey,
        ECDEncrypt,
        ECDDecrypt
            
	}

	enum EDebugMode:byte
	{
		FileReadTest = 1,//ファイル読み取りテスト
		RandomBintTest = 2,//BigInteger乱数のテスト
		MakeEpTest = 3,//BigIntegerの２進数変換テスト
        SqrtModTest = 4//平方剰余の計算テスト
	}

	#endregion

	class Program
	{
		#region InitalParams

        //実行モードの設定
		const EExcuteMode MODE = EExcuteMode.MakeKey;
		const bool ISDEBUGMODE = false;//デバッグモードの設定
		public const int KEYLENGTH = 190;
        public const string PUBLICKEYNAME = "PublicKey.bin";

		#endregion

		//Main関数からどの処理に進むのかを分岐
		static void Main(string[] args)
		{
			//初期設定
			Program program = new Program();
			Console.WriteLine("楕円曲線のパラメータをセット…");
			ECC.SetECCParams();//楕円曲線のパラメータ初期設定

			//デバッグの分岐
			if (ISDEBUGMODE)
			{
				//モードの入力
				Console.WriteLine("モードを入力してください");
				Console.WriteLine("1:ファイル読み取りのテスト");
                Console.WriteLine("2:BigInteger乱数生成テスト");
                Console.WriteLine("3:楕円曲線上の有理点生成テスト");
                Console.WriteLine("4.平方剰余の計算テスト");
				byte mode = byte.Parse(Console.ReadLine());

				//モードによる分岐
				switch ((EDebugMode)mode)
				{
					//ファイル読み取りテスト
					case EDebugMode.FileReadTest:
						program.FileReadTest(args[0]);
						break;
                    case EDebugMode.RandomBintTest:
                        program.RandomBIntTest();
                        break;
                    case EDebugMode.MakeEpTest:
                        program.MakeEpTest();
                        break;
                    case EDebugMode.SqrtModTest:
                        program.SqrtModTest();
                        break;
				}
			}
			else
			{
				//モードによる分岐
				switch (MODE)
				{
					case EExcuteMode.MakeKey:
						Console.WriteLine("鍵生成プログラムを起動...");
						program.MakeKey();
						break;
					case EExcuteMode.Encrypt:
						Console.WriteLine("暗号化プログラムを起動...");
						program.EncryptMode(args[0]);
						break;
					case EExcuteMode.Decrypt:
						Console.WriteLine("復号プログラムを起動...");
						program.DecryptMode(args[0]);
						break;

                }

			}
		}

		#region MainPrograms

		//公開鍵と秘密鍵をランダムに生成する
		public void MakeKey()
		{
			//ランダムに公開鍵Pを生成
			BigInteger p = IntegerCalclator.
				RandamBigInteger(KEYLENGTH);
			Ep P = new Ep(p);

			//ランダムに秘密鍵Kpを生成
			BigInteger Kp = IntegerCalclator.
				RandamBigInteger(KEYLENGTH);

			//PとKpから公開鍵Bを生成
			//（通常の乗算ではなく
			//Kp回の楕円曲線上の群演算をしている事に注意）
			Ep B = Kp * P;

			//出力
			FileManager.OutputPublicKey(B, P, "PublicKey.bin");
			FileManager.OutputPrivateKey(Kp, "PrivateKey.bin");
		}

		public void EncryptMode(string pathin)
		{
            //公開鍵の読み取り
            Ep P, B;
            FileManager.LoadEncryption("PublicKey.bin", out P, out B);
            ECC.SetPublicKey(B, P);

			//バイナリファイルの読み取り
			byte[] m = FileManager.LoadBinaryFile(pathin);

			//暗号文の生成
			Ep M1, M2;
			ECC.Encrypt(m, out M1, out M2);

            //点の位置のチェック

			//暗号文の出力
			FileManager.OutputEncryptFile(M1, M2, string.Concat(pathin, "_enc"));
		}

		//復号
		public void DecryptMode(string arg)
		{
			//暗号文の読み込み
			FileManager.LoadEncryption(string.Concat(arg, "_enc"),
				out Ep M1, out Ep M2);

			//秘密鍵の読み込み
			BigInteger Kp = FileManager.LoadPrivateKey("PrivateKey.bin");
            ECC.SetPrivateKey(Kp);

			//復号の実行
			byte[] m = ECC.Decrypt(M1, M2);

			//復号文の出力
			FileManager.OutputDecryptedMsg(m, string.Concat(arg, "_dec"));
		}

		#endregion

		#region Debug
		public void FileReadTest(string arg)
		{
			//初期設定

			//バイナリファイル読み取り
			byte[] Data;
			Data = FileManager.BasicLoad(arg);

			Console.WriteLine(Encoding.ASCII.GetString(Data));
			Console.WriteLine("Complete.");

		}

        //BigInteger型乱数のテスト
        public void RandomBIntTest()
        {
            BigInteger p = IntegerCalclator.RandamBigInteger(KEYLENGTH);
            Console.Write("BigIntegerNumber: ");
            Console.WriteLine(p.ToString());
            string b = IntegerCalclator.BigIntegerToBin(p);
            Console.Write("Binary: ");
            Console.WriteLine(b);

            Console.WriteLine("Complete.");
        }

        //Ep対応テスト
        //失敗率は1/(2^100)であることに注意
        public void MakeEpTest()
        {
            BigInteger p = IntegerCalclator.RandamBigInteger(KEYLENGTH);
            Ep P = new Ep(p);
            Console.Write("Ep.x: ");
            Console.WriteLine(P.x.ToString());
            Console.Write("Ep.y: ");
            Console.WriteLine(P.y.ToString());
        }

        //平方剰余計算テスト
        public void SqrtModTest()
        {
            Console.Write("a= ");
            string strA = Console.ReadLine();
            BigInteger a = BigInteger.Parse(strA);
            Console.Write("p= ");
            string strP = Console.ReadLine();
            BigInteger p = BigInteger.Parse(strP);
            if (IntegerCalclator.LegendreSymbol(a, p))
            {
                BigInteger x = IntegerCalclator.SqrtMod(a, p);
                Console.Write("Answer: ");
                Console.WriteLine(x.ToString());
            }
            else Console.WriteLine("aはpの倍数か平方非剰余です");
        }

		#endregion

	}
}
