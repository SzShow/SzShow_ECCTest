using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;


namespace EncryptProject
{

	//楕円曲線上の点
	public struct Ep
	{
		//点の位置
		public BigInteger x;
		public BigInteger y;


		//コンストラスタ：初期位置を入力
		public Ep(BigInteger m)
		{
			BigInteger z;
            BigInteger y = 0;
            BigInteger x = 0;
			for (int index = 0; index < 100; index++)
			{
				x = (100 * m) + index;
				z = (x ^ 3) + (ECC.a * x) + ECC.b;//z=y^2に注意
				if (IntegerCalclator.LegendreSymbol(z, ECC.p))
				{
                    
					y = IntegerCalclator.SqrtMod(z, ECC.p);
                    break;
				}
			}
            this.x = x;
            this.y = y;

		}

		public Ep(BigInteger x, BigInteger y)
		{
			this.x = x;
			this.y = y;
		}

		#region overloaded operator

		//（非）等値演算子は点が同じ位置にあるかを評価
		public static bool operator ==(Ep P, Ep Q)
		{
			if (P.x == Q.x & P.y == Q.y) return true;
			else return false;
		}
		public static bool operator !=(Ep P, Ep Q)
		{
			if (P.x == Q.x & P.y == Q.y) return false;
			else return true;
		}

		//群演算
		public static Ep operator +(Ep P, Ep Q)
		{
			//Ep R;
			BigInteger rambda;

			//PとQを通る二点の傾きを計算
			//(P=Qならば点Pにおける楕円曲線の微分値を計算する）
			if (P == Q)
			{
				rambda = (((3 * P.x ^ 2) + ECC.a) / 2 * P.y);
			}
			else
			{
				rambda = ((Q.y - P.y) / (Q.x - P.x));
			}

			//群演算の実行
			BigInteger x = rambda ^ 2 - P.x - Q.x;
			BigInteger y = (rambda * (P.x - x) - P.y)%ECC.p;
			return new Ep(x, y);
		}

		//逆元との群演算
		public static Ep operator -(Ep P, Ep Q)
		{
			//x軸について点の位置を反転させる
			Q.y = -Q.y;
			return P + Q;
		}

		//単位元との群演算
		public static Ep operator +(Ep P, InfinitePoint O)
		{
			return P;
		}

		//a回の群演算
		public static Ep operator *(BigInteger a, Ep P)
		{
			Ep Q = P;
			String bin = IntegerCalclator.BigIntegerToBin(a);
			for (int item = bin.Length - 1; item >= 0; item--)
			{
				//桁数を上げる度に
				Q = Q + Q;
				if (bin[item] == 1)
				{
					Q = Q + P;
				}
			}
			Q += P;

			return Q;
		}

		#endregion
	}

	public class InfinitePoint
	{
	};


	public static class ECC
	{
        #region Global Variance
        static int _a;
		public static int a
		{
			get { return _a; }
			private set
			{
				_a = value;
			}
		}
        static BigInteger _b;
		public static BigInteger b
		{
			get { return _b; }
			private set
			{
				_b = value;
			}
		}
        //Epの点Pと被る場合があるので変更推奨
        static BigInteger _p;
        public static BigInteger p
		{
			get { return _p; }
			private set
			{
				_p = value;
			}
		}
        static BigInteger _Kp;
        public static BigInteger Kp
		{
			get { return _Kp; }
			private set
			{
				_Kp = value;
			}
		}

        static Ep _P;
		public static Ep P
        {
            get { return _P;}
            private set
            {
                _P = value;
            }
        }

        static Ep _B;
        public static Ep B
        {
            get { return _B; }
            private set
            {
                _B = value;
            }
        }

		#endregion

		#region Excute Function

		//パラメータの設定
		static public void SetECCParams()
		{
			//テキストファイルの読み込み
			string text = FileManager.LoadTextFile("ECCParam.bin");

			//テキストの各パラメータに対応する分割の実行
			string[] splitedText = FileManager.SplitParamText(text);

			//各テキストからパラメータ値を読み込む
			//FileManager.ExtractParams(splitedText);

			//パラメータのset
			_a = int.Parse(splitedText[0]);
			_b = BigInteger.Parse(splitedText[1]);
			_p = BigInteger.Parse(splitedText[2]);

			//確認用
			Console.WriteLine("パラメータのセットが完了しました．");
			Console.WriteLine("_a= " + splitedText[0]);
			Console.WriteLine("_b= " + splitedText[1]);
			Console.WriteLine("_p= " + splitedText[2]);

		}

        static public void SetPublicKey(Ep B, Ep P)
        {
            ECC.P = P;
            ECC.B = B;
        }

        static public void SetPrivateKey(BigInteger Kp)
        {
            ECC.Kp = Kp;
        }

		static public void Encrypt(byte[] m, out Ep M1, out Ep M2)
		{
			//平文mをEpに対応付ける
			Ep M = ECC.MsgToEp(new BigInteger(m));

			//Mに掛ける回数kを設定
			BigInteger k = IntegerCalclator.RandamBigInteger(Program.KEYLENGTH);

			//暗号文の生成
			M1 = k * ECC.P;
			Ep temp = k * ECC.B;
			M2 = temp + M;
		}

		static public byte[] Decrypt(Ep M1, Ep M2)
		{
			//平文に対応する点の計算
			Ep M = M2 - (ECC._Kp * M1);

			//楕円曲線上の点から平文への変換
			byte[] m = ECC.EpToMsg(M);
			return m;
		}

		#endregion

		#region Internal Function

		static public Ep MsgToEp(BigInteger m)
		{
			BigInteger z;
			for (int index = 0; index < 100; index++)
			{
				BigInteger x = (100 * m) + index;
				z = x ^ 3 + ECC._a * x + ECC._b;
				if (IntegerCalclator.LegendreSymbol(z, ECC._p))
				{
					BigInteger y = IntegerCalclator.SqrtMod(z, ECC._p);
					return new Ep(x, y);
				}
			}

			//エラー
			return new Ep();
		}

		static public byte[] EpToMsg(Ep M)
		{
			BigInteger m = M.x / 100;
            return m.ToByteArray();
			
		}

		#endregion

	}
}
