using InTheHand.Net.Sockets;
using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Media;
using System.Reflection;
//using System.Linq;
using System.Windows.Forms;
using USBClassLibrary;




namespace ZeroTrip

// Cuando pulso stop no se debe inicializar todo en pantalla.
// Quitar barra deslizante con diferencia


{
    public partial class frPrincipal : Form
    {
        public VblesGlobales Gb = new VblesGlobales();

        // datos de medias y tiempos de un tramo cronometrado
        public ZeroTrip.ZeroTripBBDDDataSet.DatosDataTable tbDatosTr = new ZeroTripBBDDDataSet.DatosDataTable();
        // datos generales del tramo
        public ZeroTrip.ZeroTripBBDDDataSet.TramosDataTable tbInfoTr = new ZeroTripBBDDDataSet.TramosDataTable();
        // datos de las incidencias y cruces del tramo
        public ZeroTrip.ZeroTripBBDDDataSet.IncidenciasDataTable tbIncidenciasTr = new ZeroTripBBDDDataSet.IncidenciasDataTable();

        // datos de las incidencias y cruces del tramo
        public ZeroTrip.ZeroTripBBDDDataSet.LogDataTable tbLogTr = new ZeroTripBBDDDataSet.LogDataTable();

        // Array con tres columnas: la primera es calibre, la segunda es el número de pulsos hasta el que aplica ese calibre y la tercera es la distancia equivalente
        public double[,] anCalibres = new double[1, 3];


        public bool bHayTramo = false; // nos indica si tenemos un tramo cargado
        public bool bEnCompeticion = false; // nos indica si estamos en tiempo de tramo cronometrado
        public DateTime dtSalidaTr; // hora de salida a tramo cronometrado
        public TimeSpan tsCrono = new TimeSpan(); // Crono en segundos
        public TimeSpan tsCronoMil = new TimeSpan(); // Crono en milesimas
        public DateTime dtCrona = new DateTime();
        public int nSegundoAnterior;
        public int nDifMetros;  //Diferencia en metros entre la distancia real y la teorica.
        public int nSectorIdeal; // Sector del tramo en el que nos debemos encontrar, que se corresponde con una velocidad en cambios de medias
        public int nSigIncidecia; // Numero de la siguente incidencia, es como el de distancia pero para incidencias
        public string szDireccionCruce; // Nos indica en formato horario la direccion a tomar en un cruce, se envia al terminal
        public int nTramoCron;
        public short nCalcarTramo;

        public double dbCalibreActivo;
        public double dbDistIdeal; // Distancia ideal en la que debemos encontrarnos para una velocidad y tiempo conocidos.
        public Int32 nDistIdeal; // Lo mismo pero como entero expresado en metros.
        public Int32 nDistRealAnt; // Distancia real en la que hice una recalibración, que guardo por si me equivoco y tengo que volver a recalibrar.
        public double dbDistReal; // Distancia real en la que nos encontramos segun la lectura de la sonda.

        public Int32 nDistReal; // Lo mismo pero como entero expresado en metros.
        public Int32 nSigCM; // distancia a la que se hara el siguiente cambio de media
        public Int32 nFaltaCruce; // distancia para el siguiente cruce
        public Int32 nDifPorRecalibre; // Contiene la diferencia en metros entre la distancia que tenemos por real y la recalibración que introducimos
        public Int32 nCorrecionMetros; // Acumula la correción de metros que meta pulsando botones

        public double dbVelActual, dbVelSiguiente;
        double dbPulsos = 0, dbPulsosAnt = 0;
        public string szTipoTramo = "";

        string szEnvCuentaAtras = "";
        string szEnvDistancia = "";
        // string szEnvDistancia2 = "";
        string szVelocidad = "";

        public TimeSpan tsZero = new TimeSpan();
        public double dbSegundoAnterior = 0.99;  // Variable para saber si nos movemos en el mismo segundo para evitar repeticiones de sonidos.
        public bool bAvisar = false;
        public bool bReCargaTramo = false;

        //Para tramos a calcar
        public short nCalcarSegundos, nCalcarRegs;
        public int nCalcarHasta, nCalcarDesde, nCalcarParcial;
        public decimal dbCalcarVelocidad;
        public DateTime dtmCalcarTParcial = DateTime.Today, dtmCalcarTAcumulado = DateTime.Today;


        SoundPlayer simpleSound = new SoundPlayer(@"c:\Windows\Media\chimes.wav");

        GestionConfig config = new GestionConfig(Application.StartupPath + @"\ConfigZeroTrip.exe.config");

        //USB Detect
        public bool MyUSBARDConnected;
        public bool MyUSBPDAConnected;
        //public bool MyUSBAndroidConnected;

        private USBClassLibrary.USBClass USBPort = new USBClass();
        private USBClassLibrary.USBClass.DeviceProperties USBDeviceProperties = new USBClass.DeviceProperties();


        // Variables para la conexión por Bluetooth
        private BTLibrary.Enviar BLTObj = new BTLibrary.Enviar();
        public BluetoothDeviceInfo[] listaDevices;


        public frPrincipal()
        {

            InitializeComponent();

            //BT Detect. Consultamos la lista de dispositivos emparejados con este ordenador
            listaDevices = BLTObj.listPaired();


            //USB Detect
            USBPort.USBDeviceAttached +=
                new USBClass.USBDeviceEventHandler(USBPort_USBDeviceAttached);
            USBPort.USBDeviceRemoved +=
              new USBClass.USBDeviceEventHandler(USBPort_USBDeviceRemoved);

            //if (USBClass.GetUSBDevice(2341, 43, ref USBDeviceProperties, true))
            if (USBClass.GetUSBDevice(Convert.ToUInt16(config.GetVID()), Convert.ToUInt16(config.GetPID()), ref USBDeviceProperties, true))

                {
                //My Device is connected

                cbPortARD.Text = USBDeviceProperties.COMPort;
                MyUSBARDConnected = true;
                //MyUSBAndroidConnected = true;

                AbrirPuertoSonda(USBDeviceProperties.COMPort, USBDeviceProperties.FriendlyName);
            }


            USBPort.RegisterForDeviceChange(true, this);
            //     RegisterHidNotification();

        }

        //********************************************************************************************************************

        private void frPrincipal_Load(object sender, EventArgs e)
        {
            // TODO: esta línea de código carga datos en la tabla 'dsLog.Log' Puede moverla o quitarla según sea necesario.
            //this.logTableAdapter.Fill(this.dsLog.Log);
            // TODO: esta línea de código carga datos en la tabla 'ZeroTripBBDDDataSet.Datos' Puede moverla o quitarla según sea necesario.
            //this.datosTableAdapter.Fill(this.dsDatos.Datos);

            BaseDeDatos();

            btStop.Enabled = false;
            btStart.Enabled = false;
            btStart.LookAndFeel.SkinName = "The Asphalt World";
            btStop.LookAndFeel.SkinName = "The Asphalt World";

            lbCuentaAtras.Text = "";
            lbVariable.Text = "";
            lbTipoTramo.Text = "";
            lbHoraSalida.Text = "";

            rgMedidor.EditValue = "Terra";

            //Deshabilitamos el editor de navegación de los grid
            //gcMedias.EmbeddedNavigator.Enabled = false;
            //gcAdd.Enabled = false;

            string[] ports = SerialPort.GetPortNames();
            cbPortPDA.Properties.Items.Clear();

            nCorrecionMetros = 0;
            lbCorreccion.Text = "0";
            
            foreach (string port in ports)
            {
                cbPortPDA.Properties.Items.Add(port);
            }

            // Cargamos el combo con los nombres de los dispositivos Bluetooth emparejados.
            foreach (BluetoothDeviceInfo dev in listaDevices)
            {
                cbBLTDevs.Properties.Items.Add(dev.DeviceName);
            }

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            lbVersion.Text = fileVersionInfo.ProductVersion;

            Inicializar();

            gcMedias.EmbeddedNavigator.Enabled = true;
            gcIncidencias.EmbeddedNavigator.Enabled = true;

            teTPaso.Visible = false;
            tePrueba.Time = DateTime.Now;
            teFechaInicio.Time = DateTime.Now;

            btReset.Focus();
            //recConnetionBLT();

            //Habilitamos el timer auxiliar para que se inicie el proceso de sincronización y arranque del timer principal.
            tmAux.Enabled = true;


            //GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

            //// Do not suppress prompt, and wait 1000 milliseconds to start.
            //watcher.TryStart(false, TimeSpan.FromMilliseconds(3000));

            //GeoCoordinate coord = watcher.Position.Location;

            //   teDirBBDD.Text = Gb.sDirectorioDatos;
        }

        //********************************************************************************************************************

        private void recConnetionBLT()
        {
            BLTObj.localListener.BeginAcceptBluetoothClient(new AsyncCallback(BLTObj.AcceptConnection), BLTObj.localListener);
            if (BLTObj.remoteDevice.Connected )
            {
                string a = BLTObj.remoteDevice.RemoteMachineName;
                BLTObj.localListener.BeginAcceptBluetoothClient(new AsyncCallback(BLTObj.AcceptConnection), BLTObj.localListener);
            }
        }

        #region CONTROLES

        //-----------------------------------------------------------------------------------

        private void btSalir_Click(object sender, EventArgs e)
        {
            Gb.Util.fnSalida();
        }

        //-----------------------------------------------------------------------------------

        private void xtTabControl_Click(object sender, EventArgs e)
        {
            CargaHora();
        }

        //-----------------------------------------------------------------------------------

        private void cbTramosRace_SelectedIndexChanged(object sender, EventArgs e)
        // Detectamos un cambio en el combo que almacena los nombres de los tramos para la carrera.
        // Es decir, queremos cargar un tramo nuevo en memoria.
        {
            if (cbTramosRace.Text != "")
            {
                nTramoCron = Convert.ToInt16(cbTramosRace.Text.Substring(6));

                CargaTramo(Convert.ToInt16(nTramoCron));

                btStart.Enabled = true;
                btStart.LookAndFeel.SkinName = "Money Twins";
                btStart.Select();
                nCorrecionMetros = 0;
                lbCorreccion.Text = "0";
                ResetContador();
                chkCalcar.Checked = false;
                Gb.bTramoACalcar = false;
                nCalcarTramo = 0;
                cbTramoACalcar.Enabled = false;
            }
        }

        //-----------------------------------------------------------------------------------

        private void btStop_Click(object sender, EventArgs e)
        {
            btStart.Enabled = true;
            btStart.LookAndFeel.SkinName = "Money Twins";

            btStop.Enabled = false;
            btStop.LookAndFeel.SkinName = "The Asphalt World";

            Inicializar();

        }

        //-----------------------------------------------------------------------------------

        private void btStart_Click(object sender, EventArgs e)
        {

            //CargaTramo();

            //   bHayTramo = true;

            btStart.Enabled = false;
            btStart.LookAndFeel.SkinName = "The Asphalt World";

            btStop.Enabled = true;
            btStop.LookAndFeel.SkinName = "Money Twins";
            btStop.Focus();

            //   nSectorIdeal = 0;
            Arrancar();
        }

        //-----------------------------------------------------------------------------------

        private void btReset_Click(object sender, EventArgs e)
        {
            ResetContador();
        }

        //-----------------------------------------------------------------------------------
   
        private void btRecalibrar_Click(object sender, EventArgs e)
        {
            //if (Util.EsNumerico(tbRecalibre.Text))
            {
                Int32 nDistRealMedidor;


                if (rgCalibre.Text == "Biciclometro")
                    nDistRealMedidor = Convert.ToInt32((dbCalibreActivo / 1000) * dbPulsos);
                else
                    nDistRealMedidor = Convert.ToInt32((dbPulsos * 1000) / dbCalibreActivo);

                //nDistRealAnt = nDistReal;
                nDistRealAnt = nDistRealMedidor;
                
                // Si la diferencia por recalibre es positiva es que iba perdiendo metros, y si es negativa es que iba ganando
                //nDifPorRecalibre = Int32.Parse(teRecalibre.Text.Replace(".", "")) - nDistReal;
                nDifPorRecalibre = Int32.Parse(teRecalibre.Text.Replace(".", "")) - nDistRealMedidor;

                lbDifPorRecal.Text = nDifPorRecalibre.ToString();
                
                // Ponemos a cero la posible correccion.
                nCorrecionMetros = 0;
                lbCorreccion.Text = "0";

                GrabarLog("RECALIBRACION / Informado: " + teRecalibre.Text + " | Dist Arduino: " + nDistRealMedidor.ToString() + " | Mostrado: " + lbDistReal.Text + " | Dif: " + nDifPorRecalibre.ToString());
             
            }
        }

        //-----------------------------------------------------------------------------------

        private void btEliminarReajuste_Click(object sender, EventArgs e)
        {
            Int32 nDistRealMedidor;

            nDistRealMedidor = Convert.ToInt32((dbCalibreActivo / 1000) * dbPulsos);

            // Eliminamos toda posible recalibración que huieramos hecho.
            nDifPorRecalibre = 0;

            lbDifPorRecal.Text = nDifPorRecalibre.ToString();

            // Ponemos a cero la posible correccion.
            nCorrecionMetros = 0;
            lbCorreccion.Text = "0";
            GrabarLog("Eliminamos recalibración: " + teRecalibre.Text + " | Dist Arduino: " + nDistRealMedidor.ToString() + " | Mostrado: " + lbDistReal.Text + " | Dif: " + nDifPorRecalibre.ToString());

        }
 
        //-----------------------------------------------------------------------------------

        private void btRERecalibra_Click(object sender, EventArgs e)
            // He hecho un recalibrado y me he equivocado. En dDistRealAnt tengo la distancia real en la que estábamos en el momento de recalibre
            //Aplico sobre esa distancia.
        {
            nDifPorRecalibre = Int32.Parse(teRecalibre.Text.Replace(".", "")) - nDistRealAnt;
            lbDifPorRecal.Text = nDifPorRecalibre.ToString();
            // Ponemos a cero la posible correccion.
            GrabarLog("RE-Recalibracion / Informado: " + teRecalibre.Text + " | Dist Arduino " + nDistReal.ToString() + " | Mostrado: " + lbDistReal.Text + " | Dist Real Ant: " + nDistRealAnt.ToString() +
                " | Nuevo Dif: " + nDifPorRecalibre.ToString());
            //teRecalibre.Focus();

        }

        //-----------------------------------------------------------------------------------
        
        private void cbTramoACalcar_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si cambia el el contenido del combo y está activado el combo de tramo a calcar, entonces queremos calcar
            // grabaremos el tramo destino como si fuera a tablas.
            if (cbTramoACalcar.Text != "" && chkCalcar.Checked)
            {
                // Si no confirmamos, nos salimos.
                if (!Util.AvisoConRespuesta("¿Deseas calcar este tramo con el seleccionado?", "Tenemos una duda."))
                {
                    nCalcarTramo = 0;
                    Gb.bTramoACalcar = false;
                    cbTramoACalcar.ResetText();
                    return;
                }

                nCalcarTramo = Convert.ToInt16(cbTramoACalcar.Text.Substring(6));
                Gb.bTramoACalcar = true;
                nCalcarRegs = 0;
                datosTableAdapter.BorrraTramo(nCalcarTramo);
                tramosTableAdapter.GetData(nCalcarTramo);

              //  tramosTableAdapter.Update(nCalcarTramo, "Tablas");
            }
        }

        //-----------------------------------------------------------------------------------
        
        private void chkCalcar_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCalcar.Checked)
            {
                cbTramoACalcar.Enabled = true;
            }
            else
            {
                cbTramoACalcar.Enabled = false;
            }

            nCalcarTramo = 0;
            Gb.bTramoACalcar = false;
            cbTramoACalcar.ResetText();

        }

        //-----------------------------------------------------------------------------------

        private void btSigCM_Click(object sender, EventArgs e)

        //TODO: Aquí deberiamos gestionar los cambios de media en referencias externas.
        //El cambio se debe aplicar en la distancia teórica, pues puede que haya corrido mucho y vaya adelantado
        {

            decimal dbVelocidad, dbVel;
            DateTime dtmTParcial = DateTime.Today, dtmTAcumulado = DateTime.Today;

            NumberFormatInfo provider = new NumberFormatInfo();

            if (nSectorIdeal < tbDatosTr.Rows.Count)
            { 

                provider.NumberDecimalSeparator = ",";
                provider.NumberGroupSeparator = ".";
                provider.NumberGroupSizes = new int[] { 3 };

                dbVelocidad = decimal.Parse(tbDatosTr[nSectorIdeal-1].Velocidad.ToString(), provider);

                //GrabarLog("CAMBIO REF.EXTERNAS/ Velocidad aplicada: " + dbVelocidad.ToString() + " | Hasta: " + Convert.ToInt32(double.Parse(lbDistReal.Text.ToString(), provider) * 1000).ToString() );
                GrabarLog("CAMBIO REF.EXTERNAS/ Velocidad aplicada: " + dbVelocidad.ToString() + " | Hasta: " + nDistReal.ToString());
                //tbDatosTr[nSectorIdeal-1].Hasta = Convert.ToInt32(decimal.Parse(teSigCMRE.Text.ToString(), provider));
                //tbDatosTr[nSectorIdeal - 1].Hasta = Convert.ToInt32(double.Parse(lbDistReal.Text.ToString(), provider)*1000);
                tbDatosTr[nSectorIdeal - 1].Hasta = nDistReal;
                tbDatosTr[nSectorIdeal-1].Parcial = tbDatosTr[nSectorIdeal-1].Hasta - tbDatosTr[nSectorIdeal-1].Desde;
                dtmTParcial = Util.Tiempo(tbDatosTr[nSectorIdeal-1].Parcial, dbVelocidad);
                tbDatosTr[nSectorIdeal - 1].TiempoParcial = dtmTParcial;

                if (nSectorIdeal == 1)
                {
                    tbDatosTr[nSectorIdeal - 1].TiempoAcum = dtmTParcial;
                }
                else
                {
                    dtmTAcumulado = dtmTParcial.Add(Convert.ToDateTime(tbDatosTr[nSectorIdeal - 2].TiempoAcum).TimeOfDay);
                    tbDatosTr[nSectorIdeal - 1].TiempoAcum = dtmTAcumulado;
                }

                // Como ya hemos pasado la referencia, cambiamos a la siguiente media
                dbVelocidad = decimal.Parse(tbDatosTr[nSectorIdeal].Velocidad.ToString(), provider);
                teVelRE.Text = tbDatosTr[nSectorIdeal].Velocidad.ToString("00.##");

                //tbDatosTr[nSectorIdeal].Desde = Convert.ToInt32(double.Parse(lbDistReal.Text.ToString(), provider) * 1000);
                tbDatosTr[nSectorIdeal].Desde = nDistReal;
                tbDatosTr[nSectorIdeal].Parcial = tbDatosTr[nSectorIdeal].Hasta - tbDatosTr[nSectorIdeal].Desde;
               // tbDatosTr[nSectorIdeal].Velocidad = Convert.ToDouble(dbVelocidad);

                dtmTParcial = Util.Tiempo(tbDatosTr[nSectorIdeal].Parcial, dbVelocidad);
                tbDatosTr[nSectorIdeal].TiempoParcial = dtmTParcial;

                dtmTAcumulado = dtmTParcial.Add(Convert.ToDateTime(tbDatosTr[nSectorIdeal - 1].TiempoAcum).TimeOfDay);
                //tbDatosTr[nSectorIdeal].TiempoAcum = dtmTAcumulado;

                if (nSectorIdeal > 1)

                {
                    dtmTAcumulado = dtmTParcial.Add(Convert.ToDateTime(tbDatosTr[nSectorIdeal - 1].TiempoAcum).TimeOfDay);
                    tbDatosTr[nSectorIdeal].TiempoAcum = dtmTAcumulado;
                }

                if (Gb.bFreeze)
                {
                    Gb.bFreeze = false;
                }

                btFreeze_Click(sender, e);

                foreach (DataRow drFila in tbDatosTr)
                {
                    // Grabamos en BBDD las distancias que hemos ido poniendo para cada referencia
                    {

                        dbVel = decimal.Parse(drFila["Velocidad"].ToString(), provider);
                        datosTableAdapter.ModificaFila((int)drFila["Desde"], (int)drFila["Hasta"], (int)drFila["Parcial"], 
                            (decimal)dbVel, (DateTime)drFila["TiempoParcial"], (DateTime)drFila["TiempoAcum"], (short)drFila["IdTramo"], (short)drFila["IdDato"]);

                    }
                }
            }

        }

        //-----------------------------------------------------------------------------------

        private void chkBRecalAuto_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBRecalAuto.Checked)
                teRecalibre.BackColor = Color.LightGreen;
            else
                teRecalibre.BackColor = Color.Crimson;


                if (chkBRecalAuto.Checked && nSigIncidecia > 0 && nSigIncidecia != 9999)
                {
                    CambiaDistRecalibre();
                }
        }

        //-----------------------------------------------------------------------------------
        
        private void btSigRecalibre_Click(object sender, EventArgs e)
        {
            if (nSigIncidecia > 0 && nSigIncidecia != 9999)
                CambiaDistRecalibre();
            else
                teSigRecalibre.Text = "--"; 
        }

        //-----------------------------------------------------------------------------------

        private void CambiaDistRecalibre()

            // Pone en el Text Box para recalibrar, la siguiente distancia del dataSet de incidencias.
        {

            teRecalibre.Text = tbIncidenciasTr[nSigIncidecia - 1].Posicion.ToString().Replace(".", "");
            if (nSigIncidecia < tbIncidenciasTr.Count)
                teSigRecalibre.Text = tbIncidenciasTr[nSigIncidecia].Posicion.ToString().Replace(".", "");
            else
                teSigRecalibre.Text = "--";       
        }

        //-----------------------------------------------------------------------------------

        private void btFreeze_Click(object sender, EventArgs e)
        {
            if (Gb.bFreeze)
            {
                Gb.bFreeze = false;
                lbFreeze.Text = "0".ToString();
            }
            else
            {
                Gb.bFreeze = true;
                lbFreeze.Text = (dbDistReal / 1000).ToString("00.#00").Substring(0, 6);
            }
        }

        //-----------------------------------------------------------------------------------

        private void frPrincipal_KeyDown(object sender, KeyEventArgs e)
        {
           if (Control.ModifierKeys == Keys.Shift)
            { int a = 1; }

            switch (e.KeyCode)
            {
                case (Keys.Add):
                    btFreeze_Click(sender, e);
                    break;
                case (Keys.Multiply):
                    btRecalibrar_Click(sender, e);
                    break;
                case (Keys.Divide):
                    btSigRecalibre_Click(sender, e);
                    break;
                case (Keys.F1):
                    btStart_Click(sender, e);
                    break;
                case (Keys.F2):
                    btBloqMetros_Click(sender, e);
                    break;
                case (Keys.F12):
                    btStop_Click(sender, e);
                    break;
                case (Keys.F3):
                    btSigCM_Click(sender, e);
                    break;
                case (Keys.Y):
                    btInicio_Click(sender, e);                  
                    break;
                case (Keys.Q):
                    if (btTiempoSector.Enabled)
                    {
                        btTiempoSector_Click(sender, e);
                    }
                    break;
                case (Keys.D):
                    if (btMas1.Enabled)
                    {
                        btMas1_Click(sender, e);
                    }
                    break;
                case (Keys.W):
                    if (btMas10.Enabled)
                    {
                        btMas10_Click(sender, e);
                    }
                    break;
                case (Keys.A):
                    if (btMenos1.Enabled)
                    {
                        btMenos1_Click(sender, e);
                    }
                    break;
                case (Keys.S):
                    if (btMas10.Enabled)
                    {
                        btMenos10_Click(sender, e);
                    }
                    break;
                case (Keys.M):
                    if(rgDecaMetro.Text == "Metros")
                        rgDecaMetro.EditValue = "Decametro";
                    else
                        rgDecaMetro.EditValue = "Metros";
                    //rgDecaMetro_SelectedIndexChanged(sender, e);
                    break;
                   
                default:
                    break;
                    //MessageBox.Show("Function F1 frPrincipal_KeyDown");

            }

        }

        //-----------------------------------------------------------------------------------

        private void teRecalibre_Enter(object sender, EventArgs e)
        {
            teRecalibre.SelectionStart = 0;
            teRecalibre.SelectionLength = teRecalibre.Text.Length;
        }

        //-----------------------------------------------------------------------------------

        private void btMas1_Click(object sender, EventArgs e)
        {
            nCorrecionMetros = nCorrecionMetros + 1;
            lbCorreccion.Text = nCorrecionMetros.ToString();
            GrabarLog("Correccion metros " + nCorrecionMetros.ToString());
        }

        //-----------------------------------------------------------------------------------

        private void btMas10_Click(object sender, EventArgs e)
        {
            nCorrecionMetros = nCorrecionMetros + 10;
            lbCorreccion.Text = nCorrecionMetros.ToString();
            GrabarLog("Correccion metros " + nCorrecionMetros.ToString());
        }

        //-----------------------------------------------------------------------------------

        private void btMenos1_Click(object sender, EventArgs e)
        {
            nCorrecionMetros = nCorrecionMetros - 1;
            lbCorreccion.Text = nCorrecionMetros.ToString();
            GrabarLog("Correccion metros " + nCorrecionMetros.ToString());
        }

        //-----------------------------------------------------------------------------------

        private void btMenos10_Click(object sender, EventArgs e)
        {
            nCorrecionMetros = nCorrecionMetros - 10;
            lbCorreccion.Text = nCorrecionMetros.ToString();
            GrabarLog("Correccion metros " + nCorrecionMetros.ToString());
        }
        
        //-----------------------------------------------------------------------------------

        private void btBloqMetros_Click(object sender, EventArgs e)
        {
            if (btBloqMetros.Tag.ToString() == "Cerrado")
            {
                btBloqMetros.Image = ZeroTrip.Properties.Resources.lock_open;
                btBloqMetros.Tag = "Abierto";
                btMas1.Enabled = true;
                btMas10.Enabled = true;
                btMenos10.Enabled = true;
                btMenos1.Enabled = true;
                btMas1Min.Enabled = true;
                btMenos1Min.Enabled = true;
            }
            else
            {
                btBloqMetros.Image = ZeroTrip.Properties.Resources._lock;
                btBloqMetros.Tag = "Cerrado";
                btMas1.Enabled = false;
                btMas10.Enabled = false;
                btMenos10.Enabled = false;
                btMenos1.Enabled = false;
                btMas1Min.Enabled = false;
                btMenos1Min.Enabled = false;
            }
        }

        //-----------------------------------------------------------------------------------

        private void btMas1Min_Click(object sender, EventArgs e)
        {
            DateTime dtHoraMas1Min = ((DateTime)tbInfoTr.Rows[0]["HoraSalida"]).AddMinutes(1);
            tramosTableAdapter.Update(tbInfoTr.Rows[0]["Tramo"].ToString(), tbInfoTr.Rows[0]["TipoTramo"].ToString(), dtHoraMas1Min, (short)nTramoCron);
            CargaTramo(Convert.ToInt16(nTramoCron));
        }

        //-----------------------------------------------------------------------------------

        private void btMenos1Min_Click(object sender, EventArgs e)
        {
            DateTime dtHoraMas1Min = ((DateTime)tbInfoTr.Rows[0]["HoraSalida"]).AddMinutes(-1);
            tramosTableAdapter.Update(tbInfoTr.Rows[0]["Tramo"].ToString(), tbInfoTr.Rows[0]["TipoTramo"].ToString(), dtHoraMas1Min, (short)nTramoCron);
            CargaTramo(Convert.ToInt16(nTramoCron));
        }


        private void btTiempoSector_Click(object sender, EventArgs e)
        {
            // Tenemos que tomar el tiempo empleado en el primer sector y aplicarlo a los demás.

            //Como ya hemos hecho todo, ocultamos el boton
            btTiempoSector.Enabled = false;
            btTiempoSector.Visible = false;
        }

        #endregion CONTROLES


        //********************************************************************************************************************

        #region TIMER

        private void tmAux_Tick(object sender, EventArgs e)
        {
            int mili = DateTime.Now.Millisecond;

            if (DateTime.Now.Millisecond < 10)
            {
                tmAux.Enabled = false;
                tmCrono.Enabled = true;
            }
        }

        //-----------------------------------------------------------------------------------

        private void tmCrono_Tick(object sender, EventArgs e)
        {

            // Pintamos la hora actual
            lbHora.Text = Convert.ToString(DateTime.Now.ToLongTimeString());

            DateTime dtmTiempoParcial;


            //  if (bEnCompeticion)
            //   CalcDistReal();


            CalcDistReal();

            if (bHayTramo) // solo hacemos cosas si hemos arrancado el tramo.
            {

                CalcularCrono(ref tsCrono, ref tsCronoMil);

                if (bEnCompeticion)
                    CalcDistIdeal(true);



                // nDifMetros = (nDistReal + nDifPorRecalibre) - nDistIdeal;
                nDifMetros = nDistReal - nDistIdeal;

                if (bEnCompeticion)
                {
                    if (tsCrono.Seconds != nSegundoAnterior) // muestro la diferencia cada segundo porque al ser en metros va muy rápido.
                    {
                      
                        //if (dPulsos < 1)
                        //    a = 1;
                        if (nDifMetros < -10) {
                            lbDiferencia.ForeColor = Color.Blue;
                         //   lbDiferencia.BackColor = Color.Coral;
                        }
                        else if (nDifMetros < -5) { 
                            lbDiferencia.ForeColor = Color.Blue;
                        }
                        else if (nDifMetros >= -5 && nDifMetros < 5) {
                            lbDiferencia.ForeColor = Color.LimeGreen;
                            lbDiferencia.BackColor = Color.Transparent;
                        }
                        else if (nDifMetros > 5 ) {
                            lbDiferencia.ForeColor = Color.Red;
                        }
                        else{
                            lbDiferencia.ForeColor = Color.Red;
                           // lbDiferencia.BackColor = Color.DarkTurquoise;
                        }

                        lbDiferencia.Text = ((double)nDifMetros).ToString();

                        if (Gb.bTramoACalcar)
                        {
                            
                            if (nCalcarSegundos >= 10)
                            { 
                                //Grabamos 
                                nCalcarRegs += 1;
                                dtmCalcarTAcumulado = Convert.ToDateTime(lbCrono.Text.ToString());
                                string a = tsCronoMil.ToString();
                                //dtmCalcarTAcumulado=dtmCalcarTAcumulado.Subtract(dtmCalcarTAcumulado.TimeOfDay);
                                //dtmCalcarTAcumulado = Convert.ToDateTime(Math.Abs(tsCronoMil.Milliseconds));
                                dtmTiempoParcial = dtmCalcarTAcumulado.Subtract(dtmCalcarTParcial.TimeOfDay);
                                datosTableAdapter.Insert(nCalcarTramo,
                                    Convert.ToInt16(nCalcarRegs),
                                    nCalcarDesde,
                                    nDistReal,
                                    nDistReal - nCalcarDesde,
                                    ((Convert.ToDouble(nDistReal - nCalcarDesde) / 1000) / (dtmCalcarTAcumulado.Subtract(dtmCalcarTParcial.TimeOfDay).TimeOfDay.TotalHours)),
                                    dtmTiempoParcial,
                                    dtmCalcarTAcumulado,
                                     cbTipo.Text
                                    );

                                dtmCalcarTParcial = dtmCalcarTAcumulado;
                                nCalcarDesde = nDistReal;
                                nCalcarSegundos = 0;
                            }
                            nCalcarSegundos += 1;
                        
                        }

                    } // if (tsCrono.Seconds != nSegundoAnterior)
                }
                else
                {
                    lbDiferencia.ForeColor = Color.Green;
                    lbDiferencia.BackColor = Color.Transparent;
                }

                nSegundoAnterior = tsCrono.Seconds;

            }

            if (PSeriePDA.IsOpen)
                EnviarTerminal();

            if (BLTObj.remoteDevice.Connected)
                EnviarBluetooth();

        }

        #endregion TIMER

        //********************************************************************************************************************

        #region VARIOS

        public void CalcularCrono(ref TimeSpan tsCrSeg, ref TimeSpan tsCrMil)
        {
           
            DateTime dtHoraActual = DateTime.Now;

            tsCrMil = dtSalidaTr.Subtract(dtHoraActual);

            dtHoraActual = dtHoraActual.AddTicks(dtHoraActual.Ticks % 10000000 * -1);

            // Calculamos la diferencia con la hora acutal para hacer el cronometro.

            //// OOOOJJJJOOO esto no vale para cuando cambiamos de dia.
            TimeSpan tsSalida = dtSalidaTr.TimeOfDay;
            TimeSpan tsActual = DateTime.Now.TimeOfDay;

            double dbSec = DateAndTime.DateDiff(DateInterval.Second, dtSalidaTr, dtHoraActual);

            //tsAux = dtSalidaTr.Subtract( dtHoraActual);
            tsCrSeg = TimeSpan.FromSeconds(DateAndTime.DateDiff(DateInterval.Second, dtSalidaTr, dtHoraActual));


            //if (!bEnCompeticion) //Cuando no estemos en competición no entramos aqui
            
            {

                switch (tsCrSeg.CompareTo(tsZero))
                {
                    case -1:

                        lbCrono.ForeColor = Color.Red;
                        break;
                    case 0:
                        //tsAux = tsSalida.Subtract(tsActual);
                        lbCrono.ForeColor = Color.Purple;
                        if (!Gb.bSalidaAvisada && Gb.bSonido)
                            Sonidos("Salida");
                        bEnCompeticion = true;
                        break;
                    case 1:
                        //tsAux = tsActual.Subtract(tsSalida); 
                        if (rgDiaNoche.Text == "Dia")
                            lbCrono.ForeColor = Color.DarkBlue;
                        else
                            lbCrono.ForeColor = Color.LightBlue;
                        //lbCrono.ForeColor = Color.YellowGreen;
                        bEnCompeticion = true;


                        break;
                    default:
                        break;
                }

                //tsCrSeg = tsAux;

            }

            lbCrono.Text = tsCrSeg.ToString(@"hh\:mm\:ss");

        }

        //-----------------------------------------------------------------------------------

        public void EnviarTerminal()
        {
            // Enviamos datos al terminal externo

            //      Random rndObj = new Random();
            //      string szDistancia = rndObj.Next(100).ToString() + "," + rndObj.Next(100).ToString();
            //      string szCD = rndObj.Next(1, 10).ToString();
            // 00,63;00:00:47;1:36:44;00,03;-596;50;0;Tipo Incidencia;00.000;50;

            // Envio al terminal de información
            try
            {
                PSeriePDA.Write(szEnvDistancia + ";"  //Distancia ideal
                    + tsCrono.ToString() + ";"          // Crono
                    + DateTime.Now.ToLongTimeString() + ";"         // Hora actual
                    //  + szEnvDistancia2 + ";"             // Punto de siguiente paso horario: hitos, viñetas, 
                    + lbDistReal.Text + ";"             // Punto de siguiente paso horario: hitos, viñetas, tablas
                    //  + szEnvCuentaAtras + ";"                // Cuenta atras
                    + lbDiferencia.Text + ";"                // Cuenta atras / Diferencia de metros
                    + szVelocidad + ";"          // Velocidad actual
                    + szDireccionCruce + ";"      // direccion a tomar en cruce
                    + lbTipoIncidencia.Text + ";" // Tipo de incidencia
                    //+ nFaltaCruce + ";"          // distancia hasta el siguiente cruce
                    + lbDistAInci.Text + ";"     // distancia hasta el siguiente cruce
                    + dbVelSiguiente + ";"               // siguiente velocidad
                    + lbFaltaCam.Text        // distancia para el cambio de media
                 //   + Gb.nLongTramo.ToString()
                    );                  
            }
            catch (TimeoutException)
            {
                if (PSeriePDA.IsOpen)
                {

                    PSeriePDA.Close();
                    Util.AvisoConRespuesta("No hay nada escuchando por el puerto " + PSeriePDA.PortName, "Error en puerto");
                }
            }
        }

        //-----------------------------------------------------------------------------------

        public void EnviarBluetooth()
        {
            // Enviamos datos al terminal externo

            // 00,63;00:00:47;1:36:44;00,03;-596;50;0;Tipo Incidencia;00.000;50;

            string szSigInci = "00,00";

            try
            {
                
                if (nSigIncidecia != 0 && nSigIncidecia != 9999)
                {
                    szSigInci = (Convert.ToDouble(tbIncidenciasTr[nSigIncidecia - 1].Posicion)/1000).ToString("00.#00").Substring(0, 5); 
                }
                else
                { szSigInci = "--,--"; }

                string szVel = szVelocidad.Length > 5 ? szVelocidad.Substring(0, 5) : szVelocidad;
                string szCrono = tsCrono.ToString().Contains("-") ? "-" + tsCrono.ToString().Substring(4) : tsCrono.ToString();
                string szTramo, szDifMetros;

                if (cbTramosRace.Text == "")
                    szTramo = "XX-Tipo";
                else
                {
                    if (lbTipoTramo.Text.Length > 6)
                        szTramo = cbTramosRace.Text.Substring(6, 2) + "-" + lbTipoTramo.Text.Substring(0, 7);
                    else
                        szTramo = cbTramosRace.Text.Substring(6, 2) + "-" + lbTipoTramo.Text;
                }

                if (nSectorIdeal != 9999)
                    szDifMetros = lbDiferencia.Text;
                else
                    szDifMetros = "Fin";


                string a = ( szTramo + ";"  // Nombre del tramo
                    + DateTime.Now.ToLongTimeString() + ";"     // Hora actual
                    + szCrono + ";"                             // Crono
                    // + lbDistReal.Text + ";"                     //Distancia REAL
                    + (dbDistReal / 1000).ToString("00.#00").Substring(0, 5) + ";"
                    + szEnvDistancia + ";"                      //Distancia ideal
                    //+ lbDiferencia.Text + ";"                   // Cuenta atras / Diferencia de metros
                    + szDifMetros + ";"                   // Cuenta atras / Diferencia de metros
                    + szVel + ";"          // Velocidad actual
                    + lbDistActVel.Text + ";"                   //Velocidad HASTA
                    + dbVelSiguiente.ToString("00.#00").Substring(0, 5) + ";"               // siguiente velocidad

                    + szSigInci + ";"                           // Posicion siguiente incidencia
                    + szDireccionCruce + ";"                     // direccion a tomar en cruce
                    + lbTipoIncidencia.Text + ";"               // Tipo de incidencia
                    + lbDistAInci.Text + ";"                      // distancia hasta el siguiente cruce
                    + Gb.nLongTramo.ToString("#,##0") +               // Longitud del tramo
                    "\n");
       
                bool result = BLTObj.EnviarDatos(BLTObj.remoteDevice, a); 

            }
            catch (TimeoutException)
            {
                if (PSeriePDA.IsOpen)
                {

                    PSeriePDA.Close();
                    Util.AvisoConRespuesta("No hay nada escuchando por el puerto " + PSeriePDA.PortName, "Error en puerto");
                }
            }
        }  

        //-----------------------------------------------------------------------------------

        public void CalcDistIdeal(bool bShowCambio)
        {
            // Llamamos Tramo Cronometrado TC al que figura en el rutometro como tal.
            // Llamamos Sector Cronometrado SC a la distancia que ha de recorrerse dentro de un 
            // TC a una velocidad dada.

            // En "dbDistIdeal" acumulamos la distancia ideal en la que deberiamos encontrarnos 
            //  segun el tiempo transcurrido en el TC, que sera igual a la empleada en los sectores
            //  anteriores mas la del sector actual.

            // 1º Debemos obtener el número total de segundos que llevamos en el TC. En 
            // "dtCrono" y en "tsCrono" tenemos el crono.


            double dbSegTC = tsCrono.TotalMilliseconds / 1000;
            TimeSpan tsAux;
            string szAux, szTipoSector;

            nSectorIdeal = SectorParaDistancia(nSectorIdeal);
            szAux = "";

            if (szTipoTramo == "Varias")
                if(nSectorIdeal == 9999)
                    szTipoSector = szTipoTramo;
                else
                {
                    szTipoSector = tbDatosTr[nSectorIdeal - 1].TipoTramo;
                }
            else
                szTipoSector = szTipoTramo;

            //lbAux.Text = nSectorIdeal.ToString();

            if (nSectorIdeal != 9999)
            {
                switch (szTipoSector)
                {
                    case "Tablas":
                    case "HitosH":

                        tsAux = TimeSpan.Parse(tbDatosTr[nSectorIdeal - 1].TiempoAcum.ToLongTimeString());
                        lbCuentaAtras.Text = (Convert.ToInt32(tsAux.TotalSeconds - tsCrono.TotalSeconds)).ToString();
                        lbVariable.Text = tbDatosTr[nSectorIdeal - 1].Hasta.ToString("#,##0");
                        // szEnvCuentaAtras = lbCuentaAtras.Text;

                        if (szTipoTramo == "Tablas")
                        {
                            szVelocidad = (tbDatosTr[nSectorIdeal - 1].Velocidad).ToString("00.##");
                            lbVelocidad.Text = szVelocidad;

                            //Pintamos siguiente velocidad y donde el cambio de media
                            if (nSectorIdeal < Convert.ToInt32(tbDatosTr.Rows.Count))
                            {
                                dbVelSiguiente = Convert.ToDouble(tbDatosTr[nSectorIdeal].Velocidad);
                                lbSigVelocidad.Text = dbVelSiguiente.ToString("00.##");
                                lbSigVel.Text = lbSigVelocidad.Text;
                                lbActVelocidad.Text = lbVelocidad.Text;
                                lbDistActVel.Text = tbDatosTr[nSectorIdeal - 1].Hasta.ToString("#,##0");
                                nSigCM = Convert.ToInt32(tbDatosTr[nSectorIdeal - 1].Hasta);
                            }
                            else
                            {
                                lbSigVelocidad.Text = "Fin";
                                lbSigVel.Text = "";
                                lbDistActVel.Text = "Enlace";
                                nSigCM = 0;
                            }

                            dbVelActual = Convert.ToDouble(tbDatosTr[nSectorIdeal - 1].Velocidad);

                            DistanciaIdeal();

                        }
                        break;

                    case "Viñetas":
                    case "Hitos":
                    case "Sectores":
                    case "HitosK":

                        tsAux = TimeSpan.Parse(tbDatosTr[nSectorIdeal - 1].TiempoAcum.ToLongTimeString());
                        lbCuentaAtras.Text = (tsAux.TotalSeconds - tsCrono.TotalSeconds).ToString();
                        lbVariable.Text = tbDatosTr[nSectorIdeal - 1].Hasta.ToString("#,##0");
                        szEnvCuentaAtras = lbCuentaAtras.Text;
                        //     szEnvDistancia2 = lbVariable.Text;

                        dbVelActual = Convert.ToDouble(tbDatosTr[nSectorIdeal - 1].Velocidad);
                        szVelocidad = dbVelActual.ToString("00.##");
                        lbVelocidad.Text = szVelocidad;

                        //Pintamos siguiente velocidad y donde el cambio de media
                        if (nSectorIdeal < Convert.ToInt32(tbDatosTr.Rows.Count))
                        {
                            dbVelSiguiente = Convert.ToDouble(tbDatosTr[nSectorIdeal].Velocidad);
                            lbSigVelocidad.Text = dbVelSiguiente.ToString("00.##");
                            lbSigVel.Text = lbSigVelocidad.Text;
                            lbActVelocidad.Text = lbVelocidad.Text;
                            lbDistActVel.Text = tbDatosTr[nSectorIdeal - 1].Hasta.ToString("#,##0");
                            nSigCM = Convert.ToInt32(tbDatosTr[nSectorIdeal - 1].Hasta);
                        }
                        else
                        {
                            lbSigVelocidad.Text = "Fin";
                            lbSigVel.Text = "";
                            lbDistActVel.Text = "Enlace";
                            nSigCM = 0;
                        }

                        DistanciaIdeal();

                        lbFaltaCambio.Text = (((tbDatosTr[nSectorIdeal - 1].Hasta - nDistIdeal) / 10) + 1).ToString() + "0";
                        lbFaltaCam.Text = lbFaltaCambio.Text;

                       // Sonidos("CambioMedia");

                        break;

                    case "Medias":
                    case "RefExternas":

                        dbVelActual = Convert.ToDouble(tbDatosTr[nSectorIdeal - 1].Velocidad);
                        szVelocidad = dbVelActual.ToString("00.##");
                        lbVelocidad.Text = szVelocidad;
                        szEnvCuentaAtras = "";
                        //     szEnvDistancia2 = "";


                        if (nSectorIdeal < Convert.ToInt32(tbDatosTr.Rows.Count))
                        {
                            dbVelSiguiente = Convert.ToDouble(tbDatosTr[nSectorIdeal].Velocidad);
                            lbSigVelocidad.Text = dbVelSiguiente.ToString("00.##");
                            lbSigVel.Text = lbSigVelocidad.Text;
                            lbActVelocidad.Text = szVelocidad;
                            lbDistActVel.Text = tbDatosTr[nSectorIdeal - 1].Hasta.ToString("#,##0");
                            lbDistSigVel.Text = tbDatosTr[nSectorIdeal].Hasta.ToString("#,##0");
                            nSigCM = Convert.ToInt32(tbDatosTr[nSectorIdeal - 1].Hasta);
                        }
                        else
                        {
                            lbSigVelocidad.Text = "Fin";
                            lbSigVel.Text = "";
                            lbDistActVel.Text = "Enlace";
                            nSigCM = 0;
                        }

                        DistanciaIdeal();

                        lbFaltaCambio.Text = (((tbDatosTr[nSectorIdeal - 1].Hasta - nDistIdeal) / 10) + 1).ToString() + "0";
                        lbFaltaCam.Text = lbFaltaCambio.Text;

                        Sonidos("CambioMedia");

                        break;

                    default:
                        break;
                }


                szAux = (Convert.ToDouble(nDistIdeal) / 1000).ToString("00.#00");

                if (Gb.bMetros)
                {
                    // Al metro
                    lbDistTeorica.Text = szAux.Substring(0, 6);
                    szEnvDistancia = szAux.Substring(0, 5);
                }
                else
                {
                    // A la decena
                    lbDistTeorica.Text = szAux.Substring(0, 5);
                    szEnvDistancia = szAux.Substring(0, 5);
                }

            }
            else //Se acabo el tramo
            {
                bEnCompeticion = false;
                lbDistTeorica.Text = "Final";
                lbCuentaAtras.Text = "";
                lbVelocidad.Text = "";
                lbVariable.Text = "";
               // lbDiferencia.Text = "0";


            }
            //Si siguiente incidencia = 9999, quiere decir que no hay mas incidencias

            if (nSigIncidecia != 9999)
            {
                nSigIncidecia = SiguienteIncidencia(nSigIncidecia);
                if (nSigIncidecia != 9999)
                {
                    Sonidos("Cruce");

                    if (chkBRecalAuto.Checked)
                    {
                        teRecalibre.Text = tbIncidenciasTr[nSigIncidecia - 1].Posicion.ToString().Replace(".", "");
                        if (nSigIncidecia < tbIncidenciasTr.Count)
                            teSigRecalibre.Text = tbIncidenciasTr[nSigIncidecia].Posicion.ToString().Replace(".", "");
                        else
                            teSigRecalibre.Text = "--";
                    }
                }
                else
                {
                    szDireccionCruce = "0";
                    picOrientacion.Visible = false;
                    label19.Visible = false;
                    label20.Visible = false;
                    lbDistAInci.Text = "";
                    lbTipoIncidencia.Text = "";
                    lbDistAInci.Visible = false;
                    lbTipoIncidencia.Visible = false;
                    lbComenInci.Visible = false;
                }
            }
            else
            {
                szDireccionCruce = "0";
                picOrientacion.Visible = false;
                label19.Visible = false;
                label20.Visible = false;
                lbDistAInci.Text = "";
                lbTipoIncidencia.Text = "";
                lbDistAInci.Visible = false;
                lbTipoIncidencia.Visible = false;
                lbComenInci.Visible = false;
            }
        }

        //-----------------------------------------------------------------------------------

        public void DistanciaIdeal()
        {
            //Son todo variable globales a esta clase

            if (nSectorIdeal == 1)
            {
                // Todavia no ha habido cambio de media. Calculamos la posición teorica a partir del tiempo que llevamos.

                // dbVelocidad = (float)((Convert.ToDouble(nParcial) / 1000) / (dtmTParcial.TimeOfDay.TotalHours));

                dbDistIdeal = (dbVelActual * Math.Abs(tsCronoMil.TotalHours));
                nDistIdeal = Convert.ToInt32(dbDistIdeal * 1000);

            }
            else
            {
                // Ya ha habido cambio de media. Calculamos la posición teorica a partir del tiempo que llevamos.

                // Primero recuperamos el Tiempo acumulado correspondiente al sector ideal anterior en dbA
                double dbA = TimeSpan.Parse(tbDatosTr[nSectorIdeal - 2].TiempoAcum.TimeOfDay.ToString()).TotalHours;
                // y ahora calculamos la distancia ideal sólo correspondiente al tiempo teorico ideal del sector ideal actual
                dbDistIdeal = (dbVelActual * (Math.Abs(tsCronoMil.TotalHours) - dbA)) * 1000;  // y Lo ponemos en metros

                //    Sumamos esta distancia ideal del sector actual ideal con la distancia del anterior sector y tenemos la distancia ideal actual
                nDistIdeal = Convert.ToInt32(Convert.ToInt32(tbDatosTr[nSectorIdeal - 2].Hasta) + dbDistIdeal);

            }

            // Si el terra marca de menos, lo que tenemos que hacer restar el valor indicado en la barra, que para este caso será negativo.
            //Esto lo manejo ahora en el CaldDistReal, junto con la diferencia por recalibración
            // nDistIdeal += zoCoreccion.Value;

        }

        //-----------------------------------------------------------------------------------

        public void CalcDistReal()
        {
            string szCadena;

            nDifMetros = 0;

            try
            {
#if DEBUG

                if (bEnCompeticion)
                    //if (bHayTramo && bEnCompeticion)
                    {
                    Random r = new Random();

                    lbPulsos.Visible = true;
                    
                    dbPulsos = dbPulsos + r.Next(1,2); //+ r.NextDouble()

                } //if (bEnCompeticion)

#else

                // DEBUG EN RELEASE QUITAR COMENTARIOS
                if (PSerieARD.IsOpen)
                {

                    if (PSerieARD.BytesToRead > 2 && PSerieARD.BytesToRead < 4096)
                    {
                        //lbPulsos.Visible = true;

                         szCadena = PSerieARD.ReadExisting();
                        string szcopia = szCadena;
                        
                        //Contamos cuantos \n hay
                        int cont2 = szCadena.Length - szCadena.Replace("\n", "").Length;

                        if (szCadena.Length > 2 && szCadena[szCadena.Length - 1] == '\n')
                        {
                            string[] szA = (szCadena.Replace("\r\n", " ")).Split(new Char[] { });
                            //BUENO
                            if (szA.Length < 2)
                                dbPulsos = double.Parse(szA[0]);
                            else
                                dbPulsos = double.Parse(szA[szA.Length - 2]);
                            //FIN BUENO

                            //if (szA.Length < 2)
                            //{
                            //    string[] szB = szA[0].Split(':');
                            //    dbPulsos = double.Parse(szB[0]);
                            //}
                            //else
                            //{
                            //    string[] szB = szA[0].Split(':');
                            //    dbPulsos = double.Parse(szB[szB.Length - 2]);
                            //   
                            //}


                            // DEBUG EN 
                            //{if (bEnCompeticion)
                            //    {
                            //        { 
                            //            Random r = new Random();
                            //            dbPulsos = dbPulsos + r.Next(1, 2); //+ r.NextDouble()
                            // HASTA AQUI
                        }
                    }
                    // DEBUG EN RELEASE QUITAR COMENTARIOS
                    else
                        PSerieARD.DiscardInBuffer();
                }

#endif

                if (dbPulsos > dbPulsosAnt)
                {
                    // a veces ocurre que se deja de leer un dígito del número de pulsos.Por ello preguntamos que si es menor
                    //que el anterior, no se pinte. También le meto aquí la diferencia por recalibrado para que ya la lleve incluida.
                    // y también la posible corrección de metros de la barra o por botones

                    // Para ver que está pasando
                    //lbPulsos.Text = nSectorIdeal.ToString();
                    lbPulsos.Text = dbPulsos.ToString();


                    if (rgCalibre.Text == "Biciclometro")
                        dbDistReal = ((dbCalibreActivo / 1000) * dbPulsos) + (Convert.ToDouble(nDifPorRecalibre)) + Convert.ToDouble(nCorrecionMetros);
                    else
                        dbDistReal = ((dbPulsos * 1000) / dbCalibreActivo) + (Convert.ToDouble(nDifPorRecalibre)) + Convert.ToDouble(nCorrecionMetros);

                    //if (!Gb.bFreeze) // Si congelamos, no modificamos la etiqueta con la distancia
                    //{
                    if (Gb.bMetros)
                        //Al metro
                        lbDistReal.Text = (dbDistReal / 1000).ToString("00.#00").Substring(0, 6);
                    else
                        // a la decena
                        lbDistReal.Text = (dbDistReal / 1000).ToString("00.#00").Substring(0, 5);
                    //}

                    nDistReal = Convert.ToInt32(dbDistReal);
                    dbPulsosAnt = dbPulsos;

                    if (dbPulsos % 100 == 0) //bueno
                        GrabarLog("Pulsos: " + dbPulsos.ToString());
                }
            }
            catch (Exception ex)
            {
 //PSerieARD.DiscardInBuffer();
                Util.AvisoConEx(ex, "Puerto " + PSeriePDA.PortName + " no disponible o no existe", "Error en puerto");
               
            }

        }


        //-----------------------------------------------------------------------------------

        private void Inicializar()
        {
            KeyPreview = true;
            bHayTramo = false;
            bEnCompeticion = false;
            lbCuentaAtras.Text = "";
            lbDistTeorica.Text = "00,000";
            lbDistReal.Text = "00,000";
          
            lbDiferencia.Text = "0";
            lbVariable.Text = "";
            lbVelocidad.Text = "";
            lbLitVariable.Text = "";
            lbLitCuentaAtras.Text = "";

            lbActVelocidad.Text = "";
            lbDistActVel.Text = "";
            lbDistSigVel.Text = "";
            lbFaltaCambio.Text = "";
            lbFaltaCam.Text = "";
            lbSigVelocidad.Text = "";
            lbSigVel.Text = "";

            lbFaltaCambio.Visible = false;
            label8.Visible = false;

            nCorrecionMetros = 0;
            lbCorreccion.Text = "0";

            picOrientacion.Visible = false;
            label19.Visible = false;
            label20.Visible = false;
            lbDistAInci.Visible = false;
            lbTipoIncidencia.Visible = false;
            lbComenInci.Visible = false;

            //picOrientacion.Image = "";

            dbSegundoAnterior = 0.99;
            bAvisar = false;
            nSigIncidecia = 0;
            Gb.nLongTramo = 0;

            if (Gb.anAvCM != null)
                Array.Clear(Gb.anAvCM, 0, Gb.anAvCM.Length);
            if (Gb.anAvInc != null)
                Array.Clear(Gb.anAvInc, 0, Gb.anAvInc.Length);
            if (anCalibres != null)
            {
                anCalibres = (double[,]) ResizeArray(anCalibres, new int[] { 1, 3 });
               // anCalibres.Initialize();
                
                Array.Clear(anCalibres, 0, anCalibres.Length);
            }

            lbSigCMRE.Visible = false;
            teSigCMRE.Visible = false;
            lbVelRE.Visible = false;
            teVelRE.Visible = false;
            btSigCM.Visible = false;
            btSigCMManual.Visible = false;

            //Abrimos el fichero con los datos de configuracion

            chkSonido.Checked = config.GetSonido();
            Gb.bSonido = config.GetSonido();
            Gb.bLog = config.GetLog();

            chkSonido100.Checked = config.GetSonidoMetros();

            rgDiaNoche.EditValue = config.GetDiaNoche();

            rbTamanioRueda.EditValue = config.GetTamanioRueda();

            if (PSerieARD.IsOpen)
            {
                String szCadena;
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

            DiaNoche(rgDiaNoche.Text);

            teDistCM.Text = config.GetAvisoCM().ToString();
            teDistCruce.Text = config.GetAvisoCruces();
            teCorreccion.Text = config.GetMaxCorreccion();
            teDistHitos.Text = config.GetDistanciaHitos();
            teDistTablas.Text = config.GetDistanciaTablas();

            teVID.Text = config.GetVID();
            tePID.Text = config.GetPID();


            // CALIBRES Recuperamos los datos de los calibres guardados y establecemos el calibre activo
            teCal1.Text = config.GetCal1().ToString();
            teCal2.Text = config.GetCal2().ToString();
            teCal3.Text = config.GetCal3().ToString();
            //teDistOrg.Text = config.GetDistOrg().ToString();
            teCalMopuBici.Text = config.GetCalMopuBici().ToString();
            teCalMopuTerra.Text = config.GetCalMopuTerra().ToString();

            if (config.GetSelCal1())
            {
                btCal1.Image = ZeroTrip.Properties.Resources.tick;
                btCal2.Image = ZeroTrip.Properties.Resources.cross;
                btCal3.Image = ZeroTrip.Properties.Resources.cross;

                dbCalibreActivo = (double)config.GetCal1();
            }

            if (config.GetSelCal2())
            {
                btCal1.Image = ZeroTrip.Properties.Resources.cross;
                btCal2.Image = ZeroTrip.Properties.Resources.tick;
                btCal3.Image = ZeroTrip.Properties.Resources.cross;

                dbCalibreActivo = (double)config.GetCal2();
            }

            if (config.GetSelCal3())
            {
                btCal1.Image = ZeroTrip.Properties.Resources.cross;
                btCal2.Image = ZeroTrip.Properties.Resources.cross;
                btCal3.Image = ZeroTrip.Properties.Resources.tick;

                dbCalibreActivo = (double)config.GetCal3();
            }

            // Calculamos la distancia entre hitos a partir del calibre activo y del configurado como MOPU

 
            // Enviaremos el calibre si hiciera falta para calcular la velocidad en la tarjeta.
            //if (PSerieARD.IsOpen)
            //{
            //    PSerieARD.Write(((int)dbCalibreActivo).ToString());
            //    String szCadena = PSerieARD.ReadLine();
            //}
            // CALIBRES

            lbDiferencia.Text = "0";
            nDifMetros = 0;
            lbDiferencia.ForeColor = Color.LimeGreen;
            lbDiferencia.BackColor = Color.Transparent;

            tHor.Visible = false;
            tMin.Visible = false;
            tSec.Visible = false;

            rgSonda.EditValue = config.GetSonda();
            rgCalibre.EditValue = config.GetTipoMedidor();
            rgDecaMetro.EditValue = config.GetDecaMetros();

            if (rgDecaMetro.Text == "Metros")
                Gb.bMetros = true;
            else
                Gb.bMetros = false;

            if (rgCalibre.EditValue.ToString() == "Biciclometro")
            {
                teDistHitos.Text = ((1000 * Convert.ToInt64(dbCalibreActivo)) / Convert.ToInt64(config.GetCalMopuBici())).ToString();
            }
            else
            {
                teDistHitos.Text = ((1000 * Convert.ToInt64(config.GetCalMopuTerra())) / Convert.ToInt64(dbCalibreActivo)).ToString();
            }


            lbPulsos.Text = "0";

            rgAcumParc.EditValue = "Segundo";

            chkCalcar.Checked = false;
            Gb.bTramoACalcar = false;
            Gb.bFreeze = false;
            nDistRealAnt = 0;

            btMas1.Enabled = false;
            btMas10.Enabled = false;
            btMenos10.Enabled = false;
            btMenos1.Enabled = false;
            btMenos1Min.Enabled = false;
            btMas1Min.Enabled = false;

            btBloqMetros.Image = ZeroTrip.Properties.Resources._lock;
            btBloqMetros.Text = "Cerrado";

            //Sólo mostramos y habilitamos est botón cuando se cargue un tramo a Sectores.
            btTiempoSector.Enabled = false;
            btTiempoSector.Visible = false;


        }

        //-----------------------------------------------------------------------------------

        private void Arrancar()
        {

            bHayTramo = true;
            nSectorIdeal = 0;
            nSigIncidecia = 0;
            Gb.bSalidaAvisada = false;


            nCalcarDesde = 0;
            nCalcarHasta = 0;
            nCalcarSegundos = 0;
            dtmCalcarTParcial = DateTime.MinValue;
            lbDiferencia.Text = "0";
        }

        //-----------------------------------------------------------------------------------
            
        private static Array ResizeArray(Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        } 

        #endregion VARIOS


        #region DETECTAR usb

        protected override void WndProc(ref Message m)
        {
            USBPort.ProcessWindowsMessage(ref m);

            base.WndProc(ref m);
        }

        private void USBPort_USBDeviceAttached(object sender,
             USBClass.USBDeviceEventArgs e)
        {
            if (!MyUSBARDConnected)
            {
                //if (USBClass.GetUSBDevice(2341, 43, ref USBDeviceProperties, true) || // la 43 es la mia, la de Javier es 42, y la R4 es 69
                //    USBClass.GetUSBDevice(2341, 42, ref USBDeviceProperties, true))
                    if (USBClass.GetUSBDevice(Convert.ToUInt16(config.GetVID()), Convert.ToUInt16(config.GetPID()), ref USBDeviceProperties, true))
                    {
                    //My Device is connected
                    cbPortARD.Text = USBDeviceProperties.COMPort;
                    MyUSBARDConnected = true;

                    AbrirPuertoSonda(USBDeviceProperties.COMPort, USBDeviceProperties.FriendlyName);

                }
            }

            //if (!MyUSBAndroidConnected)
            //{
            //    if (USBClass.GetUSBDevice(2717, "FF48", ref USBDeviceProperties, true))
            //    {
            //        //My Device is connected
            //        cbPortARD.Text = USBDeviceProperties.COMPort;
            //        MyUSBARDConnected = true;
            //        if (USBDeviceProperties.COMPort != null)
            //        {
            //            AbrirPuertoSonda(USBDeviceProperties.COMPort, USBDeviceProperties.FriendlyName);
            //        }



            //    }
            //}
            //if (!MyUSBPDAConnected)
            //{
            //    if (USBClass.GetUSBDevice(1131, 1004, ref USBDeviceProperties, true))
            //    {
            //        //My Device is connected
            //        cbPortPDA.Text = USBDeviceProperties.COMPort;
            //        MyUSBPDAConnected = true;

            //        AbrirPuertoPDA(USBDeviceProperties.COMPort, USBDeviceProperties.FriendlyName);



            //    }
            //}
        }

        private void USBPort_USBDeviceRemoved(object sender,
                     USBClass.USBDeviceEventArgs e)
        {
            if (!USBClass.GetUSBDevice(2341, 43, ref USBDeviceProperties, false))
            {
                //My Device is removed
                MyUSBARDConnected = false;
                if (PSerieARD.IsOpen)
                {
                    Util.AvisoConError("Se ha perdido la conexión a la sonda en  " + USBDeviceProperties.FriendlyName, "Sonda desconectada");
                    //if (PSerieARD.IsOpen)
                    //  PSerieARD.Close();
                }
            }

            if (!USBClass.GetUSBDevice(1131, 1004, ref USBDeviceProperties, false))
            {
                //My Device is removed
                MyUSBPDAConnected = false;
                if (PSeriePDA.IsOpen)
                {
                    // Quito este mensaje porque se me activa cuando bajo la tapa del PC

                   // Util.AvisoConError("Se ha perdido la conexión con la PDA  " + USBDeviceProperties.FriendlyName, "PDA desconectada");
                    //if (PSerieARD.IsOpen)
                    //  PSerieARD.Close();
                }
            }
        }
        #endregion DETECTAR usb


 
 
    }

   
}
