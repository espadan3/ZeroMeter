using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Data;
using System.Windows.Forms;
//using System.Net;
//using System.Net.Sockets;

namespace ZeroTrip
{
    partial class frPrincipal
    {


        [DllImport("kernel32.dll")]

        private static extern bool SetLocalTime(ref VblesGlobales.SYSTEMTIME st);

        List<byte> recibido = new List<byte>();

        VblesGlobales.SYSTEMTIME st = new VblesGlobales.SYSTEMTIME();

        //GestionConfig config = new GestionConfig(Application.StartupPath + @"\ConfigZeroTrip.exe.config");

        //*********************************************************************************************************        

        #region INICIALIZACION Y CARGA

        public void CargaHora()
        {

            teHora.Time = DateTime.Now.AddSeconds(10);
            tePrueba.Time = DateTime.Now;

        }

        //-----------------------------------------------------------------------------------

        private void BaseDeDatos()
        {


#if DEBUG
#if PORTATIL                  
            String dataName = @"D:\Mis documentos\Visual Studio 2017\Proyectos\ZeroTrip_2.0\";
            String fileName = "ZeroTripBBDD.accdb"; 
#else
#if PC
            String fileName = "ZeroTripBBDD.accdb";
            // String dataName = @"D:\Mis documentos\Visual Studio 2017\Proyectos\ZeroTrip_2.0\";
            szDirectorio = System.AppDomain.CurrentDomain.BaseDirectory; ;

            String dataName = szDirectorio.Substring(0, szDirectorio.IndexOf("ZeroTrip") + 8);
            int n = szDirectorio.IndexOf("ZeroTrip");
#else
#if NANO
            String fileName = "ZeroTripBBDD.accdb";
            String dataName = @"C:\Users\Miguel Angel\Documents\Visual Studio 2017\Proyectos\ZeroTrip_2.0\";
#else

            //String fileName = FichConfig.GetFicheroDeDatos();
            String fileName = "ZeroTripBBDD.accdb";
            String dataName = @"D:\Mis documentos\Visual Studio 2010\Projects\ZeroTrip\";


#endif
            //#else
            //            String fileName = FichConfig.GetFicheroDeDatos();
            //            String dataName = @"D:\Mis documentos\Visual Studio 2017\Proyectos\ACERATrece\Datos\";
#endif
#endif
#else
            String fileName = "ZeroTripBBDD.accdb";
            String dataName = System.AppDomain.CurrentDomain.BaseDirectory;
#endif

            //OOOOJJJJJOOOOOO Falta manejar el nombre del fichero para cogerlo del fichero de configuracion.
            Gb.sDirectorioDatos = dataName;
            AppDomain currentDomain = AppDomain.CurrentDomain;

            String setappname = "DataDirectory";
            currentDomain.SetData(setappname, dataName);

            setappname = "Fichero";
            currentDomain.SetData(setappname, fileName);

        }

        //-----------------------------------------------------------------------------------

        private void CargaTramo(short nTr)
        {

            tbInfoTr = tramosTableAdapter.GetData(nTr);
            if (tbInfoTr.Rows.Count == 0)
            {
                Util.AvisoInformativo("No existe este tramo", "Error en tramo.");
                return;
            }

            szTipoTramo = tbInfoTr.Rows[0]["TipoTramo"].ToString();

            datosTableAdapter.Fill(tbDatosTr, nTr);
            incidenciasTableAdapter.Fill(tbIncidenciasTr, nTr);

            Gb.anAvCM = new int[tbDatosTr.Rows.Count + 1];
            Gb.anAvInc = new int[tbIncidenciasTr.Rows.Count + 1];

            if (tbDatosTr.Rows.Count == 0)
            {
                Util.AvisoInformativo("No existen datos para este tramo", "Error en tramo.");
                return;
            }
            // Obtenemos la longitud del tramo
            Gb.nLongTramo = Convert.ToInt32(tbDatosTr[tbDatosTr.Count-1]["Hasta"]);
            //Obtenemos la hora de inicio del tramo
            dtSalidaTr = Convert.ToDateTime(tbInfoTr.Rows[0]["HoraSalida"]);

            dtSalidaTr = dtSalidaTr.AddTicks(dtSalidaTr.Ticks % 10000000 * -1);

            lbHoraSalida.Text = dtSalidaTr.TimeOfDay.ToString();
            lbTipoTramo.Text = tbInfoTr[0].TipoTramo.ToString();
            
            switch (tbInfoTr[0].TipoTramo)
            {
                case "Tablas":
                    lbLitVariable.Visible = false;
                    lbVariable.Visible = false;
                    lbCuentaAtras.Visible = false;
                    lbLitCuentaAtras.Visible = false;
                    lbVariable.Text = tbDatosTr[0].Hasta.ToString();
                    lbLitCuentaAtras.Text = "Cuenta Atrás";
                    lbLitVariable.Text = "Siguiente Distancia";
                    lbActVelocidad.Text = "";
                    lbSigVelocidad.Text = "";
                    lbVelocidad.Text = "";
                    lbDistActVel.Text = "";
                    lbSigCMRE.Visible = false;
                    teSigCMRE.Visible = false;
                    btSigCM.Visible = false;
                    lbSigVelRE.Visible = false;
                    teSigVelRE.Visible = false;
                    break;

                case "Hitos":
                    lbLitVariable.Visible = true;
                    lbVariable.Visible = true;
                    lbCuentaAtras.Visible = true;
                    lbLitCuentaAtras.Visible = true;
                    lbVariable.Text = tbDatosTr[0].Hasta.ToString();
                    lbLitVariable.Text = "Siguiente Hito";
                    lbVelocidad.Text = "";
                    lbSigVelocidad.Text = "";
                    lbDistActVel.Text = "";
                    lbSigCMRE.Visible = false;
                    teSigCMRE.Visible = false;
                    btSigCM.Visible = false;
                    lbSigVelRE.Visible = false;
                    teSigVelRE.Visible = false;

                    break;

                case "Medias":
                    lbLitVariable.Visible = false;
                    lbVariable.Visible = false;
                    lbCuentaAtras.Visible = false;
                    lbLitCuentaAtras.Visible = false;
                    lbVelocidad.Text = "";
                    lbActVelocidad.Text = tbDatosTr[0].Velocidad.ToString("00.##");
                    lbDistActVel.Text = tbDatosTr[0].Hasta.ToString("#,##0");
                    lbSigCMRE.Visible = false;
                    teSigCMRE.Visible = false;
                    btSigCM.Visible = false;
                    lbSigVelRE.Visible = false;
                    teSigVelRE.Visible = false;
                    break;

                case "Viñetas":
                    lbLitVariable.Visible = false;
                    lbVariable.Visible = false;
                    lbCuentaAtras.Visible = false;
                    lbLitCuentaAtras.Visible = false;
                    //lbVariable.Text = tbDatosTr[0].Hasta.ToString();
                    //lbLitCuentaAtras.Text = "Cuenta Atrás";
                    //lbLitVariable.Text = "Siguiente Viñeta";
                    lbVelocidad.Text = "";
                    lbSigVelocidad.Text = tbDatosTr[0].Velocidad.ToString("00.##");
                    lbDistActVel.Text = tbDatosTr[0].Hasta.ToString("#,##0");
                    lbSigCMRE.Visible = false;
                    teSigCMRE.Visible = false;
                    btSigCM.Visible = false;
                    lbSigVelRE.Visible = false;
                    teSigVelRE.Visible = false;
                    break;

                case "Sectores":
                    lbLitVariable.Visible = false;
                    lbVariable.Visible = false;
                    lbCuentaAtras.Visible = false;
                    lbLitCuentaAtras.Visible = false;
                    //lbVariable.Text = tbDatosTr[0].Hasta.ToString();
                    //lbLitVariable.Text = "Siguiente Distancia";
                    //lbVelocidad.Text = "";
                    lbSigVelocidad.Text = tbDatosTr[0].Velocidad.ToString("00.##");
                    lbDistActVel.Text = tbDatosTr[0].Hasta.ToString("#,##0");
                    lbSigCMRE.Visible = false;
                    teSigCMRE.Visible = false;
                    btSigCM.Visible = false;
                    lbSigVelRE.Visible = false;
                    teSigVelRE.Visible = false;
                    break;

                case "RefExternas":
                    lbLitVariable.Visible = false;
                    lbVariable.Visible = false;
                    lbCuentaAtras.Visible = false;
                    lbLitCuentaAtras.Visible = false;
                    ////lbVariable.Text = tbDatosTr[0].Hasta.ToString();
                    ////lbLitVariable.Text = "Siguiente Distancia";
                    lbVelocidad.Text = "";
                    lbActVelocidad.Text = tbDatosTr[0].Velocidad.ToString("00.##");
                    lbDistActVel.Text = tbDatosTr[0].Hasta.ToString("#,##0");
                    lbSigCMRE.Visible = true;
                    teSigCMRE.Visible = true;
                    btSigCM.Visible = true;
                    lbSigVelRE.Visible = true;
                    teSigVelRE.Visible = true;
                    btSigCM.Text = "2ª Media";
                    break;

                default:
                    break;

            }





            lbHoraSalida.Text = dtSalidaTr.TimeOfDay.ToString();


        }

        #endregion INICIALIZACION Y CARGA

        //*********************************************************************************************************        


        private void btConnectBLT_Click(object sender, EventArgs e)
        {
            // Al pulsar este botón, lo que hacemos es ponernos en modo listener de forma asincrona. El otro dispositivo debe conectar con nosotros
            if (cbBLTDevs.SelectedItem != null) { 
                BLTObj.remoteDeviceNameAdmited = cbBLTDevs.SelectedItem.ToString();

                BLTObj.localListener.BeginAcceptBluetoothClient(new AsyncCallback(BLTObj.AcceptConnection), BLTObj.localListener);
            }
        }

        //---------------------------------------------------------------------------------------------------------

        private void btSincronizar_Click(object sender, EventArgs e)
        {
            tmCrono.Stop();

             DateTime dtHora = Convert.ToDateTime(teHora.Text.ToString());
             //dtHora = GetNetworkTime();
            // VblesGlobales.SYSTEMTIME st = new VblesGlobales.SYSTEMTIME();
            st.Year = short.Parse(dtHora.Year.ToString());
            st.Month = short.Parse(dtHora.Month.ToString());
            st.Day = short.Parse(dtHora.Day.ToString());
            st.Hour = short.Parse(dtHora.Hour.ToString());
            st.Minute = short.Parse(dtHora.Minute.ToString());
            st.Second = short.Parse(dtHora.Second.ToString());
            st.MilliSecond = 300;
            SetLocalTime(ref st);

            tmCrono.Start();

        }
   
        //---------------------------------------------------------------------------------------------------------

        private void AbrirPuertoPDA(string szPuerto, string szNombre)
        {
            try
            {
                PSeriePDA.Close();
                PSeriePDA.PortName = cbPortPDA.Text;
                if (PSeriePDA.IsOpen)
                    PSeriePDA.Close();

                PSeriePDA.Open();
                PSeriePDA.WriteTimeout = 100;

            }
            catch (Exception ex)
            {
                Util.AvisoConEx(ex, "Puerto " + PSeriePDA.PortName + " no disponible o no existe", "Error en puerto");

            }
        }
        //---------------------------------------------------------------------------------------------------------

        private void btOpenPortPDA_Click(object sender, EventArgs e)
        {
            /*
            try
            {
                PSeriePDA.Close();
                PSeriePDA.PortName = cbPortPDA.Text;
                if (PSeriePDA.IsOpen)
                    PSeriePDA.Close();

                PSeriePDA.Open();
                PSeriePDA.WriteTimeout = 500;
                
            }
            catch (Exception ex)
            {
                Util.AvisoConEx(ex, "Puerto " + PSeriePDA.PortName + " no disponible o no existe", "Error en puerto");

            }
              */

            AbrirPuertoPDA(cbPortPDA.Text, "Apertura de puerto PDA manualmente.");
        }

        //---------------------------------------------------------------------------------------------------------

        private void ResetContador()
        {
            if (Util.AvisoConRespuesta("¿Quieres poner a Cero la distancia?", "Reset Distancia."))
            {
                lbDistReal.Text = "00,00";
                lbDiferencia.Text = "0";
                lbDifPorRecal.Text = "0";
                nDifPorRecalibre = 0;
                nDistRealAnt = 0;
                teRecalibre.Text = "0";
                teSigRecalibre.Text = "0";
                nDistReal = 0;

                String szCadena = "Z";
#if DEBUG
                dbPulsosAnt = 0;
                dbPulsos = 0;


#endif

                if (PSerieARD.IsOpen)
                {
                    dbPulsosAnt = 0;
                    dbPulsos = 0;
                    PSerieARD.Write(szCadena);
                    szCadena = PSerieARD.ReadLine();
                }
                else
                    Util.AvisoInformativo("No está establecida la conexión con la sonda.", "Error de conexión.");
            }
        }

        //---------------------------------------------------------------------------------------------------------

        private void btEnviar_Click(object sender, EventArgs e)
        {

            String szCadena = "R";

            PSerieARD.Write(szCadena);

            szCadena = PSerieARD.ReadLine();

            //PuertoSerie.Write(szCadena, 0, szCadena.Length);
            //PuertoSerie.WriteLine(DateTime.Now.ToString());
        }

        //---------------------------------------------------------------------------------------------------------

        private void PuertoSerie_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int bytes = PSeriePDA.BytesToRead;

            byte[] buffer = new byte[bytes];

            PSeriePDA.Read(buffer, 0, bytes);
            foreach (byte elem in buffer)
            {
                recibido.Add(elem);
            }
        }

        //---------------------------------------------------------------------------------------------------------

        private void AbrirPuertoSonda(string szPuerto, string szNombre)
        {
            try
            {
                PSerieARD.Close();
                PSerieARD.PortName = szPuerto;
                if (PSerieARD.IsOpen)
                    PSerieARD.Close();
                PSerieARD.ReadBufferSize = (Int32)Math.Pow(2, 16);
                PSerieARD.Open();
                if (PSerieARD.IsOpen)
                {
                    Util.AvisoInformativo("Abierta conexión a sonda en " + szNombre, "Sonda detectada");
                }
                ResetContador();
                PSerieARD.WriteTimeout = 500;


            }
            catch (Exception ex)
            {
                Util.AvisoConEx(ex, "Puerto " + PSerieARD.PortName + " no disponible o no existe", "Error en puerto");

            }
        }

        //---------------------------------------------------------------------------------------------------------

        private void btOpenPortARD_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    PSerieARD.Close();
            //    PSerieARD.PortName = cbPortARD.Text;
            //    if (PSerieARD.IsOpen)
            //        PSerieARD.Close();
            //    PSerieARD.ReadBufferSize = (Int32)Math.Pow(2, 16);
            //    PSerieARD.Open();
            //    ResetContador();
            //    PSerieARD.WriteTimeout = 500;

            //}
            //catch (Exception ex)
            //{
            //    Util.AvisoConEx(ex, "Puerto " + PSerieARD.PortName + " no disponible o no existe", "Error en puerto");

            //}
            // cbPortARD.Text = USBDeviceProperties.COMPort;
            MyUSBARDConnected = true;

            AbrirPuertoSonda(cbPortARD.Text, "Apertura de puerto sonda manualmente.");
        }

        //---------------------------------------------------------------------------------------------------------

        private void ARD_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }

        //---------------------------------------------------------------------------------------------------------

        public int SectorParaDistancia(int nSectorAct)
        {
            // Devuelve el sector dentro de un tramo en el que nos encontramos en función del tiempo de crono transcurrido
            TimeSpan tsTiempoAcum;

            if (nSectorAct != 0)
                nSectorAct = nSectorAct - 1;

            for (int nInd = nSectorAct; nInd <= tbDatosTr.Rows.Count - 1; nInd++)
            {
                tsTiempoAcum = (Convert.ToDateTime(tbDatosTr[nInd].TiempoAcum)).TimeOfDay;
                if (tsCrono <= tsTiempoAcum)
                {
                 //   lbPulsos.Text = tbDatosTr[nInd].IdDato.ToString();
                    return (tbDatosTr[nInd].IdDato);
                }
            }


            //switch (szTipoTramo)
            //{
            //    case "Tablas":
            //        foreach (ZeroTrip.ZeroTripBBDDDataSet.DatosRow rwDato in tbDatosTr)
            //        {
            //            tsTiempoAcum = (Convert.ToDateTime(rwDato.TiempoAcum)).TimeOfDay;
            //            if (tsCrono <= tsTiempoAcum)
            //                return (rwDato.IdDato);

            //        }
            //        return (9999);
            //        // break;
            //    case "Medias":
            //    case "RefExternas":
            //        for (int nInd = nSectorAct; nInd <= tbDatosTr.Rows.Count - 1; nInd++)
            //        {
            //            tsTiempoAcum = (Convert.ToDateTime(tbDatosTr[nInd].TiempoAcum)).TimeOfDay;
            //            if (tsCrono <= tsTiempoAcum)
            //                return (tbDatosTr[nInd].IdDato);

            //        }
            //        //foreach (ZeroTrip.ZeroTripBBDDDataSet.DatosRow rwDato in tbDatosTr)
            //        //{
            //        //    tsTiempoAcum = (Convert.ToDateTime(rwDato.TiempoAcum)).TimeOfDay;
            //        //    if (tsCrono <= tsTiempoAcum)
            //        //        return (rwDato.IdDato);

            //        //}
            //        return (9999);
            //        // break;
            //    case "Hitos":
            //    case "HitosH":
            //        foreach (ZeroTrip.ZeroTripBBDDDataSet.DatosRow rwDato in tbDatosTr)
            //        {
            //            tsTiempoAcum = (Convert.ToDateTime(rwDato.TiempoAcum)).TimeOfDay;
            //            if (tsCrono <= tsTiempoAcum)
            //                return (rwDato.IdDato);

            //        }
            //        return (9999);
            //        // break;
            //    case "Viñetas":
            //    case "Sectores":
            //    case "HitosK":
            //        for (int nInd = nSectorAct; nInd <= tbDatosTr.Rows.Count - 1; nInd++)
            //        {
            //            tsTiempoAcum = (Convert.ToDateTime(tbDatosTr[nInd].TiempoAcum)).TimeOfDay;
            //            if (tsCrono <= tsTiempoAcum)
            //                return (tbDatosTr[nInd].IdDato);
            //        }

            //        return (9999);
            //        // break;

            //    default:
            //        break;

            //}


            return (9999);
        }

        //---------------------------------------------------------------------------------------------------------

        private int SiguienteIncidencia(int nSigInc)
        {
            if (nSigInc != 0)
                nSigInc = nSigInc - 1;

            for (int nInd = nSigInc; nInd <= tbIncidenciasTr.Rows.Count - 1; nInd++)
            {
                //nSigInc = (Convert.ToInt16(tbIncidenciasTr[nInd].Posicion));
                //if (nDistIdeal <= Convert.ToInt32(tbIncidenciasTr[nInd].Posicion))
                    if (nDistReal <= Convert.ToInt32(tbIncidenciasTr[nInd].Posicion))
                    return (tbIncidenciasTr[nInd].IdIncidencia);

            }
            return (9999);

        }

        //---------------------------------------------------------------------------------------------------------

        private void btInicio_Click(object sender, EventArgs e)

        // Boton de ayuda en las pruebas para modificar la hora de inicio de un tramo por la que escribamos en pantalla.
        {
            dtSalidaTr = Convert.ToDateTime((tePrueba.Time).AddSeconds(8));
            dtSalidaTr = dtSalidaTr.AddTicks(dtSalidaTr.Ticks % 10000000 * -1);
            //tsSalida = tePrueba;
        }

        //-----------------------------------------------------------------------------------

        private void Sonidos(string szSituacion)
        {

            // if (dbSegundoAnterior != tsCrono.TotalSeconds && chkSonido.Checked == true)
            if (Gb.bSonido)
            {
                switch (szSituacion)
                {
                    case "Salida":
                        {
                            simpleSound.SoundLocation = Application.StartupPath.ToString() + @"\Sonidos\Salida.wav";
                            simpleSound.Play();
                            Gb.bSalidaAvisada = true;
                            break;
                        }
                    case "CambioMedia":
                        {
                            // if (((nSigCM - nDistIdeal) <= Convert.ToInt32(teDistCM.Text)) && Gb.anAvCM[nSectorIdeal] == 0)
                            if (((nSigCM - nDistReal) <= Convert.ToInt32(teDistCM.Text)) && Gb.anAvCM[nSectorIdeal] == 0)
                            {
                                //if (bAvisar)
                                {
                                    simpleSound.SoundLocation = Application.StartupPath.ToString() + @"\Sonidos\CambioMedia.wav";
                                    simpleSound.Play();
                                    Gb.anAvCM[nSectorIdeal] = 1;
                                    bAvisar = false;
                                }
                            }
                            //else
                            //{
                            //    bAvisar = true;
                            //}
                            break;
                        }
                    case "Cruce":
                        {


                            // if (((tbIncidenciasTr[nSigIncidecia - 1].Posicion - nDistIdeal) <= Convert.ToInt32(teDistCruce.Text)))
                            // Si la distancia al siguiente cruce es menor que la configurada para emitir señal
                            if (((tbIncidenciasTr[nSigIncidecia - 1].Posicion - nDistReal) <= Convert.ToInt32(teDistCruce.Text)))
                            {
                                if (Gb.anAvInc[nSigIncidecia] == 0)
                                {
                                    simpleSound.SoundLocation = Application.StartupPath.ToString() + @"\Sonidos\Incidencia.wav";
                                    picOrientacion.Visible = true;
                                    lbTipoIncidencia.Visible = true;
                                    label19.Visible = true;
                                    label20.Visible = true;
                                    lbDistAInci.Visible = true;
                                    lbComenInci.Visible = true;
                                    //if (chkBRecalAuto.Checked)
                                    //{
                                    //    teRecalibre.Text = tbIncidenciasTr[nSigIncidecia - 1].Posicion.ToString().Replace(".", "");
                                    //    teSigRecalibre.Text = tbIncidenciasTr[nSigIncidecia].Posicion.ToString().Replace(".", "");
                                    //}

                                    switch (tbIncidenciasTr[nSigIncidecia - 1].Orientacion)
                                    {
                                        case "1":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._1;
                                            break;
                                        case "2":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._2;
                                            break;
                                        case "3":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._3;
                                            break;
                                        case "4":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._4;
                                            break;
                                        case "5":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._5;
                                            break;
                                        case "6":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._6;
                                            break;
                                        case "7":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._7;
                                            break;
                                        case "8":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._8;
                                            break;
                                        case "9":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._9;
                                            break;
                                        case "10":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._10;
                                            break;
                                        case "11":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._11;
                                            break;
                                        case "12":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources._12;
                                            break;
                                        case "C1":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C1;
                                            break;
                                        case "C2":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C2;
                                            break;
                                        case "C9":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C9;
                                            break;
                                        case "C3":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C3;
                                            break;
                                        case "C4":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C4;
                                            break;
                                        case "C5":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C5;
                                            break;
                                        case "C7":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C1;
                                            break;
                                        case "C8":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C2;
                                            break;
                                        case "C10":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C10;
                                            break;
                                        case "C11":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C11;
                                            break;
                                        case "C12":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.C12;
                                            break;
                                        case "R1":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R1;
                                            break;
                                        case "R2":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R2;
                                            break;
                                        case "R3":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R3;
                                            break;
                                        case "R4":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R4;
                                            break;
                                        case "R5":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R5;
                                            break;
                                        case "R7":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R7;
                                            break;
                                        case "R8":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R8;
                                            break;
                                        case "R9":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R9;
                                            break;
                                        case "R10":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R10;
                                            break;
                                        case "R11":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R11;
                                            break;
                                        case "R12":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.R12;
                                            break;
                                        case "S3":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.S3;
                                            break;
                                        case "S9":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.S9;
                                            break;
                                        case "T3":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.T3;
                                            break;
                                        case "T9":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.T9;
                                            break;
                                        case "TS2":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.TS2;
                                            break;
                                        case "TS3":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.TS3;
                                            break;
                                        case "TS4":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.TS4;
                                            break;
                                        case "TS8":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.TS8;
                                            break;
                                        case "TS9":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.TS9;
                                            break;
                                        case "TS10":
                                            picOrientacion.Image = ZeroTrip.Properties.Resources.TS10;
                                            break;
                                        default:
                                            picOrientacion.Visible = false;
                                            break;

                                    }

                                    szDireccionCruce = tbIncidenciasTr[nSigIncidecia - 1].Orientacion;
                                    lbTipoIncidencia.Text = tbIncidenciasTr[nSigIncidecia - 1].Descripcion;

                                    simpleSound.Play();
                                    Gb.anAvInc[nSigIncidecia] = 1;
                                }
                                lbDistAInci.Text = ((Convert.ToInt32(tbIncidenciasTr[nSigIncidecia - 1].Posicion) - nDistReal) / 10).ToString() + "0";
                            }
                            else
                            {
                                szDireccionCruce = "0";
                                picOrientacion.Visible = false;
                                label19.Visible = false;
                                label20.Visible = false;
                                lbDistAInci.Visible = false;
                                lbDistAInci.Text = "";
                                lbTipoIncidencia.Visible = false;
                                lbTipoIncidencia.Text = "";
                                lbComenInci.Visible = false;
                            }

                            break;
                        }
                    case "100m":
                        {
                            simpleSound.SoundLocation = Application.StartupPath.ToString() + @"\Sonidos\100m.wav";
                            simpleSound.Play();

                            break;
                        }
                    default:
                        break;

                }


            }

            dbSegundoAnterior = Convert.ToInt32(tsCrono.TotalSeconds);


        }

        //-----------------------------------------------------------------------------------

        private void teCorreccion_EditValueChanged(object sender, EventArgs e)
        {
            if (teCorreccion.Text.Length > 1)
            {
                zoCoreccion.Properties.Maximum = Convert.ToInt32(teCorreccion.Text);
                zoCoreccion.Properties.Minimum = Convert.ToInt32(teCorreccion.Text) * -1;

                config.SetMaxCorreccion(teCorreccion.Text);
            }
        }

        //-----------------------------------------------------------------------------------

        private void GrabarLog(string szDatos)
        {
            if (Gb.bLog && bEnCompeticion)
            {
                try
                {
                    DateTime dtAux;
                    dtAux = new DateTime(tsCrono.Ticks);
                    logTableAdapter.Insert(nTramoCron, DateTime.Now, nDistIdeal, (nDifPorRecalibre + nDistReal), (dtAux), Convert.ToDecimal(dbVelActual), szDatos);
                }
                catch (Exception ex)
                {
                    Util.AvisoConEx(ex, "Error fantasma en Grabar Log", "Error en puerto");
                }
            }
        }

        //-----------------------------------------------------------------------------------


        private void DiaNoche(string szDiaNoche)
        {
            if (szDiaNoche == "Dia")
            {
                xtTabControl.LookAndFeel.UseWindowsXPTheme = false;

                //Cajas de la pantalla de utilidades
                groupControl1.LookAndFeel.UseWindowsXPTheme = false;
                groupControl2.LookAndFeel.UseWindowsXPTheme = false;
                groupControl3.LookAndFeel.UseWindowsXPTheme = false;
                groupControl4.LookAndFeel.UseWindowsXPTheme = false;
                groupControl5.LookAndFeel.UseWindowsXPTheme = false;
                groupControl6.LookAndFeel.UseWindowsXPTheme = false;

                label13.ForeColor = System.Drawing.Color.Navy;
                label14.ForeColor = System.Drawing.Color.Navy;
                label12.ForeColor = System.Drawing.Color.Navy;
                label5.ForeColor = System.Drawing.Color.Navy;
                label16.ForeColor = System.Drawing.Color.Navy;
                label7.ForeColor = System.Drawing.Color.Navy;
                label9.ForeColor = System.Drawing.Color.Navy;
                label19.ForeColor = System.Drawing.Color.Navy;
                label20.ForeColor = System.Drawing.Color.Navy;
                label30.ForeColor = System.Drawing.Color.Navy;
                label10.ForeColor = System.Drawing.Color.Navy;
                label11.ForeColor = System.Drawing.Color.Navy;
                label15.ForeColor = System.Drawing.Color.Navy;
                label18.ForeColor = System.Drawing.Color.Navy;
                label31.ForeColor = System.Drawing.Color.Navy;
                label26.ForeColor = System.Drawing.Color.Navy;
                label27.ForeColor = System.Drawing.Color.Navy;
                label28.ForeColor = System.Drawing.Color.Navy;
                label32.ForeColor = System.Drawing.Color.Navy;
                label17.ForeColor = System.Drawing.Color.Navy;
                label21.ForeColor = System.Drawing.Color.Navy;
                lb99.ForeColor = System.Drawing.Color.Navy;
                label22.ForeColor = System.Drawing.Color.Navy;
                label23.ForeColor = System.Drawing.Color.Navy;
                label24.ForeColor = System.Drawing.Color.Navy;
                label25.ForeColor = System.Drawing.Color.Navy;

                label1.ForeColor = System.Drawing.Color.Navy;
                label2.ForeColor = System.Drawing.Color.Navy;
                label3.ForeColor = System.Drawing.Color.Navy;
                label4.ForeColor = System.Drawing.Color.Navy;
                label29.ForeColor = System.Drawing.Color.Navy;

                lbSigCMRE.ForeColor = System.Drawing.Color.Navy;
                lbSalida.ForeColor = System.Drawing.Color.Navy;
                lbDifPorRecal.ForeColor = System.Drawing.Color.Navy;
                lbCorreccion.ForeColor = System.Drawing.Color.Navy;
                chkSonido.ForeColor = System.Drawing.Color.Navy;
                chkSonido100.ForeColor = System.Drawing.Color.Navy;
                chkLog.ForeColor = System.Drawing.Color.Navy;

                rgDiaNoche.ForeColor = System.Drawing.Color.Navy;

                lbTipoTramo.ForeColor = System.Drawing.Color.Navy;
                chkCalcar.ForeColor = System.Drawing.Color.Navy;
                chkBRecalAuto.ForeColor = System.Drawing.Color.Navy;

                scContenedor.Panel1.BackColor = System.Drawing.Color.LightBlue;
                


            }

            else
            {
                xtTabControl.LookAndFeel.UseWindowsXPTheme = true;
                xtTabControl.BackColor = System.Drawing.Color.Teal;

                xtTPCarrera.BackColor = System.Drawing.Color.DarkSlateGray;
                xtTPUtils.BackColor = System.Drawing.Color.DarkSlateGray;
                xtTPTramos.BackColor = System.Drawing.Color.DarkSlateGray;

                //Cajas de la pantalla de utilidades
                groupControl1.LookAndFeel.UseWindowsXPTheme = true;
                groupControl2.LookAndFeel.UseWindowsXPTheme = true;
                groupControl3.LookAndFeel.UseWindowsXPTheme = true;
                groupControl4.LookAndFeel.UseWindowsXPTheme = true;
                groupControl5.LookAndFeel.UseWindowsXPTheme = true;
                groupControl6.LookAndFeel.UseWindowsXPTheme = true;

                groupControl1.BackColor = System.Drawing.Color.DarkSlateGray;
                groupControl2.BackColor = System.Drawing.Color.DarkSlateGray;
                groupControl3.BackColor = System.Drawing.Color.DarkSlateGray;
                groupControl4.BackColor = System.Drawing.Color.DarkSlateGray;
                groupControl5.BackColor = System.Drawing.Color.DarkSlateGray;
                groupControl6.BackColor = System.Drawing.Color.DarkSlateGray;

                label13.ForeColor = System.Drawing.Color.White;
                label14.ForeColor = System.Drawing.Color.White;
                label12.ForeColor = System.Drawing.Color.White;
                label5.ForeColor = System.Drawing.Color.White;
                label16.ForeColor = System.Drawing.Color.White;
                label7.ForeColor = System.Drawing.Color.White;
                label9.ForeColor = System.Drawing.Color.White;
                label19.ForeColor = System.Drawing.Color.White;
                label20.ForeColor = System.Drawing.Color.White;
                label10.ForeColor = System.Drawing.Color.White;
                label11.ForeColor = System.Drawing.Color.White;
                label15.ForeColor = System.Drawing.Color.White;
                label18.ForeColor = System.Drawing.Color.White;
                label31.ForeColor = System.Drawing.Color.White;
                label30.ForeColor = System.Drawing.Color.White;
                label26.ForeColor = System.Drawing.Color.White;
                label27.ForeColor = System.Drawing.Color.White;
                label28.ForeColor = System.Drawing.Color.White;
                label32.ForeColor = System.Drawing.Color.White;
                label17.ForeColor = System.Drawing.Color.White;
                label21.ForeColor = System.Drawing.Color.White;
                lb99.ForeColor = System.Drawing.Color.White;
                label22.ForeColor = System.Drawing.Color.White;
                label23.ForeColor = System.Drawing.Color.White;
                label24.ForeColor = System.Drawing.Color.White;
                label25.ForeColor = System.Drawing.Color.White;

                label1.ForeColor = System.Drawing.Color.White;
                label2.ForeColor = System.Drawing.Color.White;
                label3.ForeColor = System.Drawing.Color.White;
                label4.ForeColor = System.Drawing.Color.White;
                label29.ForeColor = System.Drawing.Color.White;

                chkSonido.ForeColor = System.Drawing.Color.White;
                chkSonido100.ForeColor = System.Drawing.Color.White;
                chkLog.ForeColor = System.Drawing.Color.White;

                rgDiaNoche.ForeColor = System.Drawing.Color.White;

                lbSigCMRE.ForeColor = System.Drawing.Color.White;
                lbSalida.ForeColor = System.Drawing.Color.White;
                lbDifPorRecal.ForeColor = System.Drawing.Color.White;
                lbCorreccion.ForeColor = System.Drawing.Color.White;
                lbTipoTramo.ForeColor = System.Drawing.Color.White;
                chkCalcar.ForeColor = System.Drawing.Color.White;
                chkBRecalAuto.ForeColor = System.Drawing.Color.White;

                scContenedor.Panel1.BackColor = System.Drawing.Color.Teal;
                

                
            
            }
        }

        //-----------------------------------------------------------------------------------

        #region Calculadora Velocidad

        private void btVelocidad_Click(object sender, EventArgs e)
        {
            if ((tbDistancia.Text == "") || teTiempo.Time == DateTime.MinValue)
            {
                MessageBox.Show("Falta información para calcular la Velocidad.", "Velocidad");

            }
            else
            {
                tbVelocidad.Text = ((Convert.ToDouble(tbDistancia.Text) / 1000) / (Convert.ToDouble(teTiempo.Time.TimeOfDay.TotalSeconds) / (double)3600)).ToString();
            }
        }

        private void btTiempo_Click(object sender, EventArgs e)
        {
            teTiempo.Time = System.DateTime.MinValue;
            teTiempo.Time = teTiempo.Time.AddSeconds((Convert.ToDouble(tbDistancia.Text) / 1000) / (Convert.ToDouble(tbVelocidad.Text)) * 3600);

        }

        private void btEspacio_Click(object sender, EventArgs e)
        {
            if ((tbVelocidad.Text == "") || (teTiempo.Time == DateTime.MinValue))
            {
                MessageBox.Show("Falta información para calcular el Espacio.", "Velocidad");

            }
            else
                tbDistancia.Text = (Convert.ToInt32((Convert.ToDouble(tbVelocidad.Text) / 3600) * (Convert.ToDouble(teTiempo.Time.TimeOfDay.TotalSeconds) * 1000))).ToString();
        }


        private void btLimpiar_Click(object sender, EventArgs e)
        {
            tbDistancia.Text = "0";
            tbVelocidad.Text = "0";
            teTiempo.Time = DateTime.MinValue;
        }

        #endregion Calculadora Velocidad


        #region Calculadora Calibre

        private void btCalcCal_Click(object sender, EventArgs e)
        {

            if (!Util.IsNumeric(tbCalActual.Text) || tbCalActual.Text == "0" ||
                !Util.IsNumeric(tbReferencia.Text) || tbReferencia.Text == "0" ||
                !Util.IsNumeric(tbDistRecorrida.Text) || tbDistRecorrida.Text == "0")
            {
                Util.AvisoConError("Falta algún dato para el cálculo.", "Recalibración.");
                return;
            }

            if (rgMedidor.EditValue.ToString() == "Terra")
            {
                tbCalNuevo.Text = ((Convert.ToInt32(tbCalActual.Text.Replace(".", "")) * Convert.ToInt32(tbDistRecorrida.Text.Replace(".", ""))) / Convert.ToInt32(tbReferencia.Text.Replace(".", ""))).ToString();
            }
            else
            {
                tbCalNuevo.Text = ((Convert.ToInt32(tbCalActual.Text.Replace(".", "")) * Convert.ToInt32(tbReferencia.Text.Replace(".", ""))) / Convert.ToInt32(tbDistRecorrida.Text.Replace(".", ""))).ToString();
            }


        }


        #endregion Calculadora Calibre


        #region Calibracion

        private void chkSonido_CheckedChanged(object sender, EventArgs e)
        {
            config.SetSonido(chkSonido.Checked);
            Gb.bSonido = chkSonido.Checked;
        }

        private void chkSonido100_CheckedChanged(object sender, EventArgs e)
        {
            config.SetSonidoMetros(chkSonido100.Checked);
        }

        private void chkLog_CheckedChanged(object sender, EventArgs e)
        {
            config.SetLog(chkLog.Checked);
            Gb.bLog = chkLog.Checked;
        }

        private void rgDiaNoche_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.SetDiaNoche(rgDiaNoche.Text);

            if (rgDiaNoche.Text == "Dia")
            {
                DiaNoche("Dia");
            }
            else
            {
                DiaNoche("Noche");
            }
        }

        private void teDistCM_EditValueChanged(object sender, EventArgs e)
        {
            config.SetAvisoCM(teDistCM.Text);
        }

        private void teDistCruce_EditValueChanged(object sender, EventArgs e)
        {
            config.SetAvisoCruces(teDistCruce.Text);
        }

        private void teDistHitos_EditValueChanged(object sender, EventArgs e)
        {
            config.SetDistanciaHitos(teDistHitos.Text);
        }

        private void teDistTablas_EditValueChanged(object sender, EventArgs e)
        {
            config.SetDistanciaTablas(teDistTablas.Text);
        }


        private void btCal1_Click(object sender, EventArgs e)
        {
            if (!config.GetSelCal1())
            {

                btCal1.Image = ZeroTrip.Properties.Resources.tick;
                btCal2.Image = ZeroTrip.Properties.Resources.cross;
                btCal3.Image = ZeroTrip.Properties.Resources.cross;

                config.SetSelCal1(true);
                config.SetSelCal2(false);
                config.SetSelCal3(false);

            }
            dbCalibreActivo = (double)config.GetCal1();
            GrabarLog("Calibre activo " + dbCalibreActivo.ToString());
            if (bEnCompeticion) GuardaCalibre();
        }

        private void btCal2_Click(object sender, EventArgs e)
        {
            if (!config.GetSelCal2())
            {
                btCal1.Image = ZeroTrip.Properties.Resources.cross;
                btCal2.Image = ZeroTrip.Properties.Resources.tick;
                btCal3.Image = ZeroTrip.Properties.Resources.cross;

                config.SetSelCal1(false);
                config.SetSelCal2(true);
                config.SetSelCal3(false);

            }
            dbCalibreActivo = (double)config.GetCal2();
            GrabarLog("Calibre activo " + dbCalibreActivo.ToString());
            if (bEnCompeticion) GuardaCalibre();
        }

        private void btCal3_Click(object sender, EventArgs e)
        {
            if (!config.GetSelCal3())
            {
                btCal1.Image = ZeroTrip.Properties.Resources.cross;
                btCal2.Image = ZeroTrip.Properties.Resources.cross;
                btCal3.Image = ZeroTrip.Properties.Resources.tick;

                config.SetSelCal1(false);
                config.SetSelCal2(false);
                config.SetSelCal3(true);

            }
            dbCalibreActivo = (double)config.GetCal3();
            GrabarLog("Calibre activo " + dbCalibreActivo.ToString());
            if (bEnCompeticion) GuardaCalibre();
        }

        public void GuardaCalibre()
        {
            if (anCalibres.GetLength(0) == 1)
            {
                anCalibres[0, 0] = dbCalibreActivo;
                anCalibres[0, 1] = dbPulsos;
                anCalibres[0, 2] = dbDistIdeal;
            }
            else
            {
                
                anCalibres[anCalibres.GetLength(0) - 1, 0] = dbCalibreActivo;
                anCalibres[anCalibres.GetLength(0) - 1, 1] = dbPulsos;
                anCalibres[anCalibres.GetLength(0) - 1, 2] = dbDistIdeal;
                
            }
            anCalibres = (double[,])ResizeArray(anCalibres, new int[] { anCalibres.GetLength(0)+1, 3 });

        }

        private void teCal1_EditValueChanged(object sender, EventArgs e)
        {
            config.SetCal1(int.Parse(teCal1.Text.Replace(".", "")));
        }

        private void teCal2_EditValueChanged(object sender, EventArgs e)
        {
            config.SetCal2(int.Parse(teCal2.Text.Replace(".", "")));
        }

        private void teCal3_EditValueChanged(object sender, EventArgs e)
        {
            config.SetCal3(int.Parse(teCal3.Text.Replace(".", "")));
        }

        private void rgCalibre_EditValueChanged(object sender, EventArgs e)
        {
            config.SetTipoMedidor(rgCalibre.Text);
            //   string a = rgMedidor.EditValue.ToString();
            //   int B = rgMedidor.SelectedIndex;
        }

        private void rgDecaMetro_SelectedIndexChanged(object sender, EventArgs e)
        {
            config.SetDecaMetros(rgDecaMetro.Text);

            if (rgDecaMetro.Text == "Metros")
                Gb.bMetros = true;
            else
                Gb.bMetros = false;
        }


        private void rbTamanioRueda_SelectedIndexChanged(object sender, EventArgs e)
 
        {
            String szCadena;

            if (PSerieARD.IsOpen)
            {
                switch (rbTamanioRueda.Text)
                { 

                    case "L":
                        szCadena = "A"; // pone 40 msg de retardo para ruedas más grandes
                        break;
                    case "M":
                        szCadena = "B"; // pone 30 msg de retardo para ruedas más pequeñas
                        break;
                    case "S":
                        szCadena = "C"; // pone 22 msg de retardo para ruedas mucho más pequeñas para hasta 140 Km/h
                        break;
                    default:
                        szCadena = "B";
                        break;
                }

                //Con calibre 880
                // 40 ms hasta 75 Km/h
                // 30 ms hasta 100 Km/h
                // 25 ms hasta 125 Km/h
                // 20 ms hasta 150 Km/h

                //    szCadena = "C"; // este no lo usamos todavia
                //    break;

                PSerieARD.Write(szCadena);
                szCadena = PSerieARD.ReadLine();
            }

            config.SetTamanioRueda(rbTamanioRueda.Text);
        }


        #endregion Calibracion

    }
}
