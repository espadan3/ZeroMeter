using System;
using System.Data;

namespace ZeroMeter
{
	/// <summary>
	/// Descripción breve de VblesGlobales.
	/// </summary>
	public class VblesGlobales
	{
		public Utiles Util = new Utiles();
		public DataSet dsTramosC;
		public DataSet dsControles;
		public DataSet dsPersonas;
		public DataSet dsRallyes;
        public int nAnio;
        public int[] anAvInc;
        public int[] anAvCM;
        public int nLongTramo;
        public bool bSalidaAvisada;
        public string sDirectorioDatos;
        public bool bTramoACalcar;
        public bool bSonido;
        public bool bLog;
        public bool bMetros;
        public bool bFreeze;
        

        public struct SYSTEMTIME
            {
            public short Year ;
            public short Month ;
            public short DayOfWeek ;
            public short Day ;
            public short Hour ;
            public short Minute ;
            public short Second ;
            public short MilliSecond ;
            }




//		public System.Windows.Forms.OpenFileDialog ofdVentana;
//		public int nIndice;
//	
		public VblesGlobales()
		{
			//
			// TODO: agregar aquí la lógica del constructor
			//

		}
	}
}
