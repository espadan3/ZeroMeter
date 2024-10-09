using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;

    
using System.Windows.Forms;

namespace ZeroTrip
{
	/// <summary>
	/// Descripción breve de Utiles.
	/// </summary>
	/// 


    public class Utiles
    {

        //GestionConfig secEspeciales = new GestionConfig(Application.StartupPath + @"\ConfigACERATrece.exe.config");

        public Utiles()
        {
            //
            // TODO: agregar aquí la lógica del constructor
            //
        }

        public DateTime Tiempo(int nEspacio, decimal fVelocidad)
        { 
            Int64 nMiliseg = Convert.ToInt64(nEspacio/ (fVelocidad/3600));
            DateTime tsAux;

            //DateTime dtmTParcial = DateTime.Today,
            //DateTime dtFormHora = new DateTime();
            DateTime dtFormHora = DateTime.Today;
            tsAux = dtFormHora.AddMilliseconds(Convert.ToDouble(nMiliseg));
            return (tsAux);
        }

        public string[] DevuelveCronos(string linea, bool bEsBlunik)
        {
            /// 
            /// Recibe una linea leida del fichero que se genera cuando se vuelcan los cronos, y  devuelve tokems si 
            /// contiene tiempos,  o null si no los contiene.
            /// 

            string[] cadena;

            string szTarget = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ-";
            char[] chAnyOf = szTarget.ToCharArray();

            string szDelimStr = "\t";
            char[] chDelimiter = szDelimStr.ToCharArray();
            string szDelimStrBl = ",";
            char[] chDelimiterBl = szDelimStrBl.ToCharArray();

            /// Se trata de averiguar si la linea recibida corresponde a una picada de crono
            /// Si no lo es, devolvemos null, y si lo es devolvemos la posición y la segunda columna de crono separados por una @

            if (linea.IndexOfAny(chAnyOf) >= 0 || linea == "")
            {
                return (null);
            }

            if (bEsBlunik)
                cadena = linea.Split(chDelimiterBl);
            else
                cadena = linea.Split(chDelimiter);
            
            return (cadena);
        }


        public void AñadeFila(ListView lvLista, string szPicada, string szEntrePicadas, string szTeorico)
        {
            ///
            /// Inserta en el listview lvVista los parametros recibidos, que es la información de una picada del cronometrador
            /// 

            string szAux;

            ListViewItem lviFila = new ListViewItem();

            szAux = szPicada.Trim();

            lviFila.Text = " ";

            if (szAux.Length == 1)
                lviFila.SubItems.Add("0" + szAux);
            else
                lviFila.SubItems.Add(szAux);

            lviFila.SubItems.Add(szEntrePicadas);

            lviFila.SubItems.Add(szTeorico);
            lvLista.Items.Add(lviFila);



        }


        public void fnSalida()
        {
            ///
            /// Cerramos la aplicación después de preguntar si queremos hacerlo así.
            /// 

            DialogResult dr = MessageBox.Show("¿Estás seguro que quieres salir?", "¿Hemos terminado?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }//if
        }


        public void AvisoConEx(InvalidCastException ex, string szMensaje, string szCabecera)
        {
            szMensaje += "\n\n" + ex.Message + "\n";
            szMensaje += "\n" + ex.Source + "\n";
            szMensaje += "\n" + ex.StackTrace + "\n";

            MessageBox.Show(szMensaje, szCabecera,
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
        }

        public void AvisoConEx(Exception ex, string szMensaje, string szCabecera)
        {
            szMensaje += "\n\n" + ex.Message + "\n";
            szMensaje += "\n" + ex.Source + "\n";
            szMensaje += "\n" + ex.StackTrace + "\n";

            //MessageBox.Show(szMensaje, szCabecera,
            MessageBox.Show(ex.Message, szCabecera,
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Stop);
        }

        public void AvisoConError(string szMensaje, string szCabecera)
        {

            MessageBox.Show(szMensaje, szCabecera,
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }

        public bool AvisoConRespuesta(string szMensaje, string szCabecera)
        {
            DialogResult result;

            result = (MessageBox.Show(szMensaje, szCabecera,
                System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Exclamation) );

            if (result == DialogResult.Yes)
            {
                return (true);
            }
            else
            {
                return (false);
            }

        }

        public void AvisoInformativo(string szMensaje, string szCabecera)
        {

            MessageBox.Show(szMensaje, szCabecera,
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        public void SizeColumnsToContent(DataGrid dataGrid, int nRowsToScan, string szFormatoFecha)
        {
            // Create graphics object for measuring widths.
            Graphics Graphics = dataGrid.CreateGraphics();

            // Define new table style.
            DataGridTableStyle tableStyle = new DataGridTableStyle();

            try
            {

                //Salvo la configuración hecha en tiempo de diseño
                tableStyle.HeaderFont = dataGrid.HeaderFont;

                // System.Drawing.Size minSiza = dataGrid.MinimumSize;

                //Determino el ancho del la primera columna que no contiene datos.
                tableStyle.RowHeaderWidth = 25;

                DataTable dataTable = (DataTable)dataGrid.DataSource;

                if (-1 == nRowsToScan)
                {
                    nRowsToScan = dataTable.Rows.Count;
                }
                else
                {
                    // Can only scan rows if they exist.
                    nRowsToScan = System.Math.Min(nRowsToScan, dataTable.Rows.Count);
                }

                // Clear any existing table styles.
                dataGrid.TableStyles.Clear();


                // Use mapping name that is defined in the data source.
                tableStyle.MappingName = dataTable.TableName;

                // Now create the column styles within the table style.
                DataGridTextBoxColumn columnStyle;




                //Le asigno 43 al ancho de fila por la primera columna y la posible barra de deslizamiento.
                // Despues le ire sumando el ancho de cada columna.
                dataGrid.Width = 43;

                int iWidth;

                for (int iCurrCol = 0;
                    iCurrCol < dataTable.Columns.Count; iCurrCol++)
                {
                    DataColumn dataColumn = dataTable.Columns[iCurrCol];

                    columnStyle = new DataGridTextBoxColumn();

                    columnStyle.TextBox.Enabled = true;

                    columnStyle.HeaderText = dataColumn.ColumnName;
                    columnStyle.MappingName = dataColumn.ColumnName;

                    // Set width to header text width.
                    iWidth = (int)(Graphics.MeasureString(columnStyle.HeaderText, dataGrid.Font).Width);

                    // Change width, if data width is
                    // wider than header text width.
                    // Check the width of the data in the first X rows.
                    DataRow dataRow;
                    for (int iRow = 0; iRow < nRowsToScan; iRow++)
                    {

                        dataRow = dataTable.Rows[iRow];

                        if (null != dataRow[dataColumn.ColumnName])
                        {
                            int iColWidth = (int)(Graphics.MeasureString
                                (dataRow.ItemArray[iCurrCol].ToString(),
                                dataGrid.Font).Width);
                            iWidth = (int)System.Math.Max(iWidth, iColWidth);
                            string szAux = dataRow.ItemArray[iCurrCol].GetType().ToString();


                            switch (szAux.ToString())
                            {
                                case "System.DateTime":
                                    // Mostramos solo la hora en formato hh:mm:ss
                                    columnStyle.Format = szFormatoFecha;
                                    columnStyle.Alignment = HorizontalAlignment.Center;
                                    break;
                                case "System.String":
                                    // Mostramos solo la hora en formato hh:mm:ss
                                    columnStyle.Alignment = HorizontalAlignment.Left;
                                    break;
                                case "System.Boolean":
                                    // Mostramos solo la hora en formato hh:mm:ss
                                    columnStyle.Alignment = HorizontalAlignment.Center;
                                    break;
                                default:
                                    columnStyle.Alignment = HorizontalAlignment.Center;

                                    break;


                            }
                        }
                    }

                    if (tableStyle.HeaderFont.Bold)
                        columnStyle.Width = iWidth + 20;
                    else
                        columnStyle.Width = iWidth + 10;



                    // Add the new column style to the table style.
                    tableStyle.GridColumnStyles.Add(columnStyle);
                    tableStyle.AlternatingBackColor = Color.GhostWhite;
                    dataGrid.Width += columnStyle.Width;
                }
                // Add the new table style to the data grid.
                tableStyle.HeaderBackColor = Color.LightBlue;
                dataGrid.TableStyles.Add(tableStyle);

            }
            catch (InvalidCastException e)
            {
                //MessageBox.Show(e.Message);
                this.AvisoConEx(e, "Pasa que.....", " En redimensionamiento");
            }
            finally
            {
                Graphics.Dispose();
            }
        }

        public bool EsCadena(string szCadena)
        {
            /// Se trata de averiguar si la cadena recibida contiene algun caracter no numerico.
            /// 
            string szTarget = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ/-,.;:_*+";
            char[] chAnyOf = szTarget.ToCharArray();

            if (szCadena.IndexOfAny(chAnyOf) >= 0)
                return (true);

            return (false);

        }

        public bool EsAlfabetico(string szCadena)
        {
            /// Se trata de averiguar si la cadena recibida contiene algun caracter no numerico.
            /// 
            string szTarget = "abcdefghijklmnopqrstuvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] chAnyOf = szTarget.ToCharArray();

            if (szCadena.IndexOfAny(chAnyOf) >= 0)
                return (true);

            return (false);

        }

        public bool EsNumerico(string szCadena)
        {
            /// Se trata de averiguar si la cadena recibida contiene algun caracter no numerico.
            /// 
            string szTarget = "1234567890.,";
            char[] chAnyOf = szTarget.ToCharArray();
            int a = szCadena.IndexOfAny(chAnyOf);
            if (szCadena.IndexOfAny(chAnyOf) == szCadena.Length)
                return (true);

            return (false);

        }

        public bool QuiereCambiar(bool bPreguntar)
        {
            string szMensaje = "Algún campo de la fila ha sido modificado.\n ¿Desea consolidar los cambios?";
            string szCabecera = "¿Desea Modificar?";

            DialogResult a;
            if (bPreguntar)
            {

                a = MessageBox.Show(szMensaje, szCabecera,
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                return (a == DialogResult.Yes ? true : false);
            }
            else
                return (true);
        }

        public bool QuiereBorrar(bool bPreguntar, string szId)
        {
            string szMensaje = "Te dispones a borrar el registro correspondiente a \n\n" + szId + "\n\n ¿Seguro que quieres BORRARLO?";
            string szCabecera = "¿Desea Modificar?";

            // DialogResult a;
            if (bPreguntar)
            {

                DialogResult a = MessageBox.Show(szMensaje, szCabecera,
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                return (a == DialogResult.Yes ? true : false);
            }
            else
                return (true);
        }

        public bool QuiereBorrarEstoYMas(bool bPreguntar, string szId)
        {
            string szMensaje = "Te dispones a borrar el registro correspondiente a \n\n" + szId
                + "\n\n Junto con este registro se BORRARA INFORMACION DE OTRAS TABLAS.\n\n ¿Seguro que quieres BORRARLO TODO?";
            string szCabecera = "¿Desea Modificar?";

            // DialogResult a;
            if (bPreguntar)
            {

                DialogResult a = MessageBox.Show(szMensaje, szCabecera,
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

                return (a == DialogResult.Yes ? true : false);
            }
            else
                return (true);
        }

        public bool QuiereContinuar(bool bPreguntar, string szTexto)
        {
            //string szMensaje = "Te dispones a borrar el registro correspondiente a \n\n" + szId + "\n\n ¿Seguro que quieres BORRARLO?";
            string szCabecera = "¿Desea Continuar?";

            // DialogResult a;
            if (bPreguntar)
            {

                DialogResult a = MessageBox.Show(szTexto, szCabecera,
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                return (a == DialogResult.Yes ? true : false);
            }
            else
                return (true);
        }

        //----------------------------------------------------------

        public void CrearColumnas(AdvBandedGridView bgvGrid, GridBand bandPrincipal, int nTramo, int nControl, int nSituacion, 
            bool bPuntosEnSegundos, bool bPorSectores)
        /// PARAMETROS DE ENTRADA
        /// bgvGrid
        /// bandPrincipal
        /// nTramo
        /// nControl: Si es mayor de 100 quiere decir que es el ultimo control de un tr por sectores, y su número se halla diviendo por 100.
        /// nSituacion
        /// bPuntosEnSegundos: si es true, se debe mostrar los puntos dividos por un divisor luego habrá decimales. Usamos el formato 'f'
        /// bPorSectores: número de controles válidos en el tramo.
        {

            ///Crea las columnas de CONTROLES para un TRAMO

            GridBand gbNueva = new GridBand();

            int nControlAux = nControl;

            if (nControl > 100)
            {
                nControl /= 100;
                nControlAux = 99;
            }


            gbNueva.AppearanceHeader.Options.UseTextOptions = true;
            gbNueva.AppearanceHeader.Options.UseFont = true;

            gbNueva.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gbNueva.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);

            if (bPorSectores)
                switch (nControlAux)
                {
                    case 1:
                        gbNueva.Caption = ("Inicio Tr" + "\n" + nSituacion.ToString("Inicio"));
                        break;
                    case 2:
                        gbNueva.Caption = ("Sector 1" + "\n" + "Referencia");
                        break;
                    case 99:
                        gbNueva.Caption = ("Total Tr");   // + "\n" + nSituacion.ToString("Inicio"));
                        break;
                    default:
                        gbNueva.Caption = ("Sector " + (nControl - 1).ToString() + "\n" + nSituacion.ToString("n0"));
                        break;


                }
            else
            if (nSituacion == 0)
                gbNueva.Caption = ("Ctr " + nTramo.ToString() + "." + nControl.ToString());
            else
            {
                gbNueva.RowCount = 2;
                gbNueva.Caption = ("Ctr " + nTramo.ToString() + "." + nControl.ToString() + "\n" + nSituacion.ToString("n0"));
            }

            gbNueva.Name = "Control" + nControl.ToString();
            bandPrincipal.Children.Add(gbNueva);


            //GridColumn unbColumn;
            BandedGridColumn unbColumn;


            /// COLUMNA DE PUNTOS POR CONTROL
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucPuntos" + nControl.ToString());
            unbColumn.Caption = "Ptos";
            unbColumn.Name = "P_Control_" + nControl.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 50;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            if (bPuntosEnSegundos)
                unbColumn.DisplayFormat.FormatString = "f1";
            else
                unbColumn.DisplayFormat.FormatString = "d";

            gbNueva.Columns.Add(unbColumn);


            ///COLUMNA DE ADELANTO O RETRASO
            ///
            unbColumn = bgvGrid.Columns.AddField("ucA-R" + nControl.ToString());
            unbColumn.Caption = "";
            unbColumn.Name = "R_Control_" + nControl.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            unbColumn.OptionsFilter.AllowFilter = false;

            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
            unbColumn.Width = 8;
            //unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);




        }

        //----------------------------------------------------------

        public void CrearColumnasTR(AdvBandedGridView bgvGrid, GridBand bandPrincipal, int nTramo, bool bPuntosEnSegundos, int nCrtsValidos)
            /// PARAMETROS DE ENTRADA
            /// bgvGrid
            /// bandPrincipal
            /// nTramo
            /// bPuntosEnSegundos: si es true, se debe mostrar los puntos dividos por un divisor luego habrá decimales. Usamos el formato 'f'
            /// nCrtValidos: número de controles válidos en el tramo.

        {

            GridBand gbNueva = new GridBand();

            gbNueva.AppearanceHeader.Options.UseTextOptions = true;
            gbNueva.AppearanceHeader.Options.UseFont = true;
            gbNueva.RowCount = 2;

            gbNueva.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gbNueva.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);


            gbNueva.Caption = ("Tramo " + nTramo.ToString() + "\n" + nCrtsValidos.ToString() + " Ctrs.");
            gbNueva.Name = "Tramo" + nTramo.ToString();
            bandPrincipal.Children.Add(gbNueva);

            ///COLUMNA DE PUNTOS
            ///
            //GridColumn unbColumn;
            BandedGridColumn unbColumn;
            unbColumn = bgvGrid.Columns.AddField("ucPtT" + nTramo.ToString());
            unbColumn.Caption = "Ptos";
            unbColumn.Name = "P_Tramo_" + nTramo.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 50;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            if (bPuntosEnSegundos)
                unbColumn.DisplayFormat.FormatString = "f1";
            else
                unbColumn.DisplayFormat.FormatString = "d";

            gbNueva.Columns.Add(unbColumn);

            ///COLUMNA DE POSICION
            ///
            unbColumn = bgvGrid.Columns.AddField("ucPsT" + nTramo.ToString());
            unbColumn.Caption = "Pos";
            unbColumn.Name = "R_Tramo_" + nTramo.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            unbColumn.OptionsFilter.AllowFilter = false;

            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
            unbColumn.Width = 30;
            //unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);



        }

        //----------------------------------------------------------

        public void CrearColumnasTime(AdvBandedGridView bgvGrid, GridBand bandPrincipal, int nIdTramo, int nControl, int nSituacion, string szUnidadTiempo,
            bool bPuntosEnSegundos, bool bPorSectores)
        /// PARAMETROS DE ENTRADA
        /// bgvGrid
        /// bandPrincipal
        /// nIdTramo
        /// nControl: Si es mayor de 100 quiere decir que es el ultimo control de un tr por sectores, y su número se halla diviendo por 100.
        /// nSituacion
        /// szUnidadTiempo
        /// bPuntosEnSegundos: si es true, se debe mostrar los puntos dividos por un divisor luego habrá decimales. Usamos el formato 'f'
        /// bPorSectores: número de controles válidos en el tramo.

            /// Aquí llegamos por ser un tramo por sectores. Si nControl trae 99, quiere decir que es el último control que tiene sus particularidades.
            /// 
        {

            GridBand gbNueva = new GridBand();

            int nControlAux = nControl;

            if (nControl > 100)
            {
                nControl /= 100;
                nControlAux = 99;
            }
               
            gbNueva.AppearanceHeader.Options.UseTextOptions = true;
            gbNueva.AppearanceHeader.Options.UseFont = true;
            gbNueva.RowCount = 2;
            gbNueva.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            gbNueva.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);

            if (bPorSectores)
                switch (nControlAux)
                { 
                    case 1:
                        gbNueva.Caption = ("Entrada a Tramo" + "\n" + nSituacion.ToString("Inicio"));
                        break;
                    case 2:
                        gbNueva.Caption = ("Sector 1" + "\n" + "Referencia");
                        break;
                    case 99:
                        gbNueva.Caption = ("Total del Tramo");   // + "\n" + nSituacion.ToString("Inicio"));
                        break;
                    default:
                        gbNueva.Caption = ("Sector "  + (nControl -1).ToString() + "\n" + nSituacion.ToString("n0"));
                        break;

                
                }
            else
                gbNueva.Caption = ("Ctr " + nIdTramo.ToString() + "." + nControl.ToString() + "\n" + nSituacion.ToString("n0"));

            gbNueva.Name = "Control" + nControl.ToString();
            bandPrincipal.Children.Add(gbNueva);


            //GridColumn unbColumn;
            BandedGridColumn unbColumn;


            // ------------ Tiempo Teorico de paso por dorsal-------
            unbColumn = bgvGrid.Columns.AddField("ucTeo" + nControl.ToString());
 
         
            
            if (bPorSectores)
                switch (nControlAux)
                {
                    case 1:
                        unbColumn.Caption = "Teorico";
                        break;
                    case 99:
                        unbColumn.Caption = "Inicio Tr";
                        break;
                    default:
                        unbColumn.Caption = "Inicio Sec";
                        break;

                }
            else
                unbColumn.Caption = "Teorico";

            unbColumn.Name = "TT_Control_" + nControl.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            //No permitimos el filtrado
            unbColumn.OptionsFilter.AllowFilter = false;

            switch (szUnidadTiempo)
            {
                case "Segundos":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss";
                    unbColumn.Width = 70;
                    break;
                case "Décimas":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,f";
                    unbColumn.Width = 70;
                    break;
                case "Centésimas":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,ff";
                    unbColumn.Width = 70;
                    break;
                case "Milésimas":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,fff";
                    unbColumn.Width = 70;
                    break;
                default:
                    unbColumn.Width = 70;
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,fff";
                    break;
            }

            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.Columns.Add(unbColumn);



            /// ----------------------- Tiempo Real de paso por dorsal  -------------------------
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucRea" + nControl.ToString());


             
            if (bPorSectores)
                switch (nControlAux)
                {
                    case 1:
                        unbColumn.Caption = "Real";
                        break;
                    case 99:
                        unbColumn.Caption = "Fin Tr";
                        break;
                    default:
                        unbColumn.Caption = "Fin Sec";
                        break;

                }
            else
                unbColumn.Caption = "Real";

            unbColumn.Name = "TR_Control_" + nControl.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            //No permitimos el filtrado
            unbColumn.OptionsFilter.AllowFilter = false;

            switch (szUnidadTiempo)
            {
                case "Segundos":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss";
                    unbColumn.Width = 55;
                    break;
                case "Décimas":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,f";
                    unbColumn.Width = 60;
                    break;
                case "Centésimas":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,ff";
                    unbColumn.Width = 65;
                    break;
                case "Milésimas":
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,fff";
                    unbColumn.Width = 65;
                    break;
                default:
                    unbColumn.Width = 65;
                    unbColumn.DisplayFormat.FormatString = "HH:mm:ss,fff";
                    break;
            }

            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.Columns.Add(unbColumn);



            // ------------ TIEMPO PARA EL CASO DE LOS SECTORES -------

            unbColumn = bgvGrid.Columns.AddField("ucTiempo" + nControl.ToString());
            unbColumn.Caption = "Tiempo";
            unbColumn.Name = "T_Control_" + nControl.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //No permitimos el filtrado
            unbColumn.OptionsFilter.AllowFilter = false;

            unbColumn.Width = 50;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            if (!bPorSectores || nControl == 1)
                unbColumn.Visible = false;
                            //unbColumn.Width = 0;
            else
                unbColumn.Visible = true;
                        //unbColumn.Width = 50;

            //if (bPuntosEnSegundos)
            //    unbColumn.DisplayFormat.FormatString = "f1";
            //else
            //    unbColumn.DisplayFormat.FormatString = "d";

            gbNueva.Columns.Add(unbColumn);



            // ------------ PUNTOS EN EL CONTROL-------

            unbColumn = bgvGrid.Columns.AddField("ucPuntos" + nControl.ToString());
            unbColumn.Caption = "Ptos";
            unbColumn.Name = "P_Control_" + nControl.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            //No permitimos el filtrado
            unbColumn.OptionsFilter.AllowFilter = false;

            unbColumn.Width = 50;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            if (bPuntosEnSegundos)
                unbColumn.DisplayFormat.FormatString = "f1";
            else
                unbColumn.DisplayFormat.FormatString = "d";

            if (bPorSectores && nControl == 2)
                unbColumn.Visible = false;
            else
                unbColumn.Visible = true;

            gbNueva.Columns.Add(unbColumn);



            ///COLUMNA DE ADELANTO O RETRASO
            ///
            unbColumn = bgvGrid.Columns.AddField("ucA-R" + nControl.ToString());
            unbColumn.Caption = "";
            unbColumn.Name = "R_Control_" + nControl.ToString().PadLeft(2, '0');
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            unbColumn.OptionsFilter.AllowFilter = false;

            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
            unbColumn.Width = 8;
            //unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);

        }

        //----------------------------------------------------------

        public void CrearColumnaBlanco(AdvBandedGridView bgvGrid, GridBand bandPrincipal)
        {

            GridBand gbNueva = new GridBand();

            gbNueva.AppearanceHeader.Options.UseTextOptions = true;
            gbNueva.AppearanceHeader.Options.UseFont = true;

            gbNueva.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);

            gbNueva.Caption = ("");
            gbNueva.Name = "Blanco";
            bandPrincipal.Children.Add(gbNueva);



            BandedGridColumn unbColumn;

            /// COLUMNA SEPARADORA EN BLANCO
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucBlanco");
            unbColumn.Caption = "";
            unbColumn.Name = "R_Blanco";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 5;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);
        }

        //----------------------------------------------------------

        public void CrearColumnaPenalizacion(AdvBandedGridView bgvGrid, GridBand bandPrincipal, bool bPuntosEnSegundos, DataTable dtbEvento)
        {
            GridBand gbNueva = new GridBand();

            gbNueva.AppearanceHeader.Options.UseTextOptions = true;
            gbNueva.AppearanceHeader.Options.UseFont = true;

            gbNueva.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);

            gbNueva.Caption = ("Pen.");
            gbNueva.Name = "Penalizacion";
            bandPrincipal.Children.Add(gbNueva);



            BandedGridColumn unbColumn;
            /// COLUMNA SEPARADORA EN BLANCO
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucBlanco");
            unbColumn.Caption = " ";
            unbColumn.Name = "R_Blanco";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.MinWidth = 2;
            unbColumn.Width = 2;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;


            gbNueva.Columns.Add(unbColumn);

            ///COLUMNA DE PENALIZACIONES
            ///
            unbColumn = bgvGrid.Columns.AddField("ucPen");
            unbColumn.Caption = "Pen";
            unbColumn.Name = "Penaliz";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            unbColumn.OptionsFilter.AllowFilter = false;

            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 50;
            //unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            if (bPuntosEnSegundos)
                unbColumn.DisplayFormat.FormatString = "f1";
            else
                unbColumn.DisplayFormat.FormatString = "d";


            gbNueva.Columns.Add(unbColumn);


        }
        
        //----------------------------------------------------------

        public void CrearColumnasTotales(AdvBandedGridView bgvGrid, GridBand bandPrincipal, bool bPuntosEnSegundos, DataTable dtbEvento)
        {

            GridBand gbNueva = new GridBand();

            gbNueva.AppearanceHeader.Options.UseTextOptions = true;
            gbNueva.AppearanceHeader.Options.UseFont = true;

            gbNueva.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);

            gbNueva.Caption = ("Resultado ");
            gbNueva.Name = "Resultado";
            bandPrincipal.Children.Add(gbNueva);



            BandedGridColumn unbColumn;


            /// COLUMNA SEPARADORA EN BLANCO
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucBlanco");
            unbColumn.Caption = " ";
            unbColumn.Name = "R_Blanco";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.MinWidth = 2;
            unbColumn.Width = 2;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);


            /// COLUMNA DEL TOTAL DE PUNTOS
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucTotal");
            unbColumn.Caption = "Total";
            unbColumn.Name = "P_Total";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True; unbColumn.AppearanceCell.Options.UseTextOptions = true;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;

            if (bPuntosEnSegundos)
                unbColumn.DisplayFormat.FormatString = "f1";
            else
                unbColumn.DisplayFormat.FormatString = "d";



            // Specify format settings.

            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 60;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.BackColor = Color.Azure;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            gbNueva.Columns.Add(unbColumn);


            /// COLUMNA SEPARADORA EN BLANCO
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucBlanco");
            unbColumn.Caption = " ";
            unbColumn.Name = "R_Blanco";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.MinWidth = 2;
            unbColumn.Width = 2;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);


            /// COLUMNA DEL TOTAL DE CEROS
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucCeros");
            unbColumn.Caption = "0s";
            unbColumn.Name = "R_Ceros";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 25;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);

            /// COLUMNA DEL TOTAL DE UNOS
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucUnos");
            unbColumn.Caption = "1s";
            unbColumn.Name = "R_Unos";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 25;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);

            /// COLUMNA DEL TOTAL DE DOSES
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucDoses");
            unbColumn.Caption = "2s";
            unbColumn.Name = "R_Doses";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 25;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);

            /// COLUMNA DEL TOTAL DE PUNTOS POR CONTROL
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucPpC");
            unbColumn.Caption = "PpC";
            unbColumn.Name = "R_PpCs";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
            //unbColumn.DisplayFormat.Format = "d2";
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.AppearanceCell.Options.UseTextOptions = true;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.DisplayFormat.FormatString = "f2";
            unbColumn.Width = 45;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;

            gbNueva.Columns.Add(unbColumn);


            /// COLUMNA SEPARADORA EN BLANCO
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucBlanco");
            unbColumn.Caption = " ";
            unbColumn.Name = "R_Blanco";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.MinWidth = 2;
            unbColumn.Width = 2;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);


            /// COLUMNA PARA EL DORSAL
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucDorsal");
            unbColumn.Caption = "Nº";
            unbColumn.Name = "R_Dorsal";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            // Specify format settings.
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 30;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Regular);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            gbNueva.Columns.Add(unbColumn);

            /// COLUMNA DE POSICION
            /// 
            unbColumn = bgvGrid.Columns.AddField("ucPosi");
            unbColumn.Caption = "Pos";
            unbColumn.Name = "P_Posicion";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

            // Disable editing.
            unbColumn.OptionsColumn.AllowEdit = false;
            unbColumn.OptionsFilter.AllowFilter = false;
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;


            // Specify format settings.

            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.Width = 30;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            //           unbColumn.AppearanceCell.BackColor = Color.Azure;
            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            gbNueva.Columns.Add(unbColumn);

            /// COLUMNA DE POSICION Especial 1
            /// 
            if ((bool)dtbEvento.Rows[0]["HayEsp1"])
            {
                unbColumn = bgvGrid.Columns.AddField("ucPE1");

                //unbColumn.Caption = "RT";
                unbColumn.Caption = dtbEvento.Rows[0]["AbrEsp1"].ToString();
                //unbColumn.Caption = secEspeciales.GetAbrEspecial1();
                unbColumn.Name = "P_PosE1";
                unbColumn.VisibleIndex = bgvGrid.Columns.Count;
                unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                // Disable editing.
                unbColumn.OptionsColumn.AllowEdit = false;
                unbColumn.OptionsFilter.AllowFilter = false;
                unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;


                // Specify format settings.

                unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                unbColumn.Width = 30;
                unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                //           unbColumn.AppearanceCell.BackColor = Color.Azure;
                unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gbNueva.Columns.Add(unbColumn);
            }

            //if (secEspeciales.GetHayEspecial2())
            if ((bool)dtbEvento.Rows[0]["HayEsp2"])
            {
                /// COLUMNA DE POSICION Especial 2
                /// 
                unbColumn = bgvGrid.Columns.AddField("ucPE2");


                //unbColumn.Caption = secEspeciales.GetAbrEspecial2();
                unbColumn.Caption = dtbEvento.Rows[0]["AbrEsp2"].ToString();
                unbColumn.Name = "P_PosE2";
                unbColumn.VisibleIndex = bgvGrid.Columns.Count;
                unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                // Disable editing.
                unbColumn.OptionsColumn.AllowEdit = false;
                unbColumn.OptionsFilter.AllowFilter = false;
                unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;


                // Specify format settings.

                unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                unbColumn.Width = 30;
                unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                //           unbColumn.AppearanceCell.BackColor = Color.Azure;
                unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gbNueva.Columns.Add(unbColumn); 
            }

            //if (secEspeciales.GetHayEspecial3())
            if ((bool)dtbEvento.Rows[0]["HayEsp3"])
            {
                /// COLUMNA DE POSICION Especial 3
                /// 
                unbColumn = bgvGrid.Columns.AddField("ucPE3");


                //unbColumn.Caption = secEspeciales.GetAbrEspecial3();
                unbColumn.Caption = dtbEvento.Rows[0]["AbrEsp3"].ToString();
                unbColumn.Name = "P_PosE3";
                unbColumn.VisibleIndex = bgvGrid.Columns.Count;
                unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                // Disable editing.
                unbColumn.OptionsColumn.AllowEdit = false;
                unbColumn.OptionsFilter.AllowFilter = false;
                unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;


                // Specify format settings.

                unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                unbColumn.Width = 30;
                unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                //           unbColumn.AppearanceCell.BackColor = Color.Azure;
                unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gbNueva.Columns.Add(unbColumn);
            }


            if ((bool)dtbEvento.Rows[0]["HayEsp4"])
            {
                /// COLUMNA DE POSICION Especial 4
                /// 
                unbColumn = bgvGrid.Columns.AddField("ucPE4");


                //unbColumn.Caption = secEspeciales.GetAbrEspecial4();
                unbColumn.Caption = dtbEvento.Rows[0]["AbrEsp4"].ToString();
                unbColumn.Name = "P_PosE4";
                unbColumn.VisibleIndex = bgvGrid.Columns.Count;
                unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;

                // Disable editing.
                unbColumn.OptionsColumn.AllowEdit = false;
                unbColumn.OptionsFilter.AllowFilter = false;
                unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;


                // Specify format settings.

                unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                unbColumn.Width = 30;
                unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                //           unbColumn.AppearanceCell.BackColor = Color.Azure;
                unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 7F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                gbNueva.Columns.Add(unbColumn);
            }


        }

        //----------------------------------------------------------

        public void CrearColumnasGeneral(AdvBandedGridView bgvGrid, GridBand bandPrincipal, int nEvento)
        {

            BandedGridColumn unbColumn;
            unbColumn = bgvGrid.Columns.AddField("ucPtE" + nEvento.ToString());
            //unbColumn.Caption = "Evento" + nEvento.ToString();
            unbColumn.Caption = nEvento.ToString();
            unbColumn.Name = "P_Evento_" + nEvento.ToString();
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.String;
            // Disable editing.
         
            unbColumn.OptionsColumn.AllowEdit = false;
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            unbColumn.OptionsFilter.AllowFilter = false;

            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.MinWidth = 50;
            unbColumn.Width = 50;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //gbNueva.Columns.Add(unbColumn);
            bandPrincipal.Columns.Add(unbColumn);


        }

        //----------------------------------------------------------

        public void CrearTotalesGeneral(AdvBandedGridView bgvGrid, GridBand bandPrincipal, string szTipoCla)
        {
            /// ¿¿¿DEBERIA CONSIDERAR EL TIPO DE CLASIFICACION PARA SOLO CREAR LA COLUMNA DESCARTE Y TOTAL PONDERADO PARA CONDUCTORES Y NAVEGANTES.
            /// 

            BandedGridColumn unbColumn;
            unbColumn = bgvGrid.Columns.AddField("ucToG");
            //unbColumn.Caption = "Evento" + nEvento.ToString();
            unbColumn.Caption = "Total";
            unbColumn.Name = "P_Total_Gral";
            unbColumn.VisibleIndex = bgvGrid.Columns.Count;
            unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
            // Disable editing.

            unbColumn.OptionsColumn.AllowEdit = false;
            unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            unbColumn.OptionsFilter.AllowFilter = false;

            // Specify format settings.
            unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            unbColumn.MinWidth = 60;
            unbColumn.Width = 60;
            unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

            unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            //gbNueva.Columns.Add(unbColumn);
            bandPrincipal.Columns.Add(unbColumn);

            ///DESCARTES
            //if (szTipoCla == "C" || szTipoCla == "N")
            {
                unbColumn = bgvGrid.Columns.AddField("ucDes");
                //unbColumn.Caption = "Evento" + nEvento.ToString();
                unbColumn.Caption = "Descartes";
                unbColumn.Name = "P_Descartes";
                unbColumn.VisibleIndex = bgvGrid.Columns.Count;
                unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                // Disable editing.

                unbColumn.OptionsColumn.AllowEdit = false;
                unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                unbColumn.OptionsFilter.AllowFilter = false;

                // Specify format settings.
                unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                unbColumn.MinWidth = 70;
                unbColumn.Width = 70;
                unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                //unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                //gbNueva.Columns.Add(unbColumn);
                bandPrincipal.Columns.Add(unbColumn);

                ///TOTAL PONDERADO

                unbColumn = bgvGrid.Columns.AddField("ucToP");
                //unbColumn.Caption = "Evento" + nEvento.ToString();
                unbColumn.Caption = "Total Ponderado";
                unbColumn.Name = "P_Total_Pon";
                unbColumn.VisibleIndex = bgvGrid.Columns.Count;
                unbColumn.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
                unbColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
                // Disable editing.

                unbColumn.OptionsColumn.AllowEdit = false;
                unbColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
                unbColumn.OptionsFilter.AllowFilter = false;

                // Specify format settings.
                unbColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                unbColumn.MinWidth = 60;
                unbColumn.Width = 60;
                unbColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                unbColumn.AppearanceCell.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
                unbColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                //gbNueva.Columns.Add(unbColumn);
                bandPrincipal.Columns.Add(unbColumn);
            }
        }

        //----------------------------------------------------------

        public DateTime Redondear(DateTime dtEntrada, string szUnidad)
        {

            /// 1 Ticks son 100 nanosegundos
            /// 
            DateTime dtAux;
            long lDivisor;
            double dbTicks = dtEntrada.Ticks;
            decimal dTicksOri = dtEntrada.Ticks;
            switch (szUnidad)
            {
                case "Segundos":
                    lDivisor = TimeSpan.TicksPerSecond;
                    break;
                case "Décimas":
                    lDivisor = (TimeSpan.TicksPerSecond / 10);
                    break;
                case "Centésimas":
                    lDivisor = (TimeSpan.TicksPerSecond / 100);
                    break;
                case "Milésimas":
                    lDivisor = (TimeSpan.TicksPerSecond / 1000);
                    break;
                default:
                    lDivisor = TimeSpan.TicksPerSecond;
                    break;

            }
            dbTicks /= lDivisor;


            dbTicks = Math.Round(dbTicks, 0, MidpointRounding.AwayFromZero);
            if (dtEntrada != DateTime.MaxValue)
                dtAux = dtEntrada.AddTicks(Convert.ToInt64((decimal)(dbTicks * lDivisor) - dTicksOri));
            else
                dtAux = DateTime.MaxValue;
            return (dtAux);
            
        }

        public DateTime Despreciar(DateTime dtEntrada, string szUnidad)
        {

            /// 1 Ticks son 100 nanosegundos
            /// 
            DateTime dtAux;
            long lDivisor;
            double dbTicks = dtEntrada.Ticks;
            long lTicks = dtEntrada.Ticks;
            decimal dTicksOri = dtEntrada.Ticks;
            switch (szUnidad)
            {
                case "Segundos":
                    //dtEntrada.Millisecond /= 100;
                    lDivisor = TimeSpan.TicksPerSecond;
                    break;
                case "Décimas":
                    lDivisor = (TimeSpan.TicksPerSecond / 10);
                    break;
                case "Centésimas":
                    lDivisor = (TimeSpan.TicksPerSecond / 100);
                    break;
                case "Milésimas":
                    lDivisor = (TimeSpan.TicksPerSecond / 1000);
                    break;
                default:
                    lDivisor = TimeSpan.TicksPerSecond;
                    break;

            }
            //dbTicks -= dbTicks%lDivisor;


            //dbTicks = Math.Round(dbTicks, 0, MidpointRounding.ToEven);
            dtAux = dtEntrada.AddTicks( -(lTicks % lDivisor));
            
            //dtAux.Ticks = dbTicks;
            return (dtAux);

        }


        public void EjecutarSql(string queryString)
        {
            //queryString = "SELECT * FROM Cronometros";
            ////BBDD BaseDatos = new BBDD();

            
            ////using (OleDbConnection connection = new OleDbConnection(connectionString))
            //using (BaseDatos.con)
            //{
            //    OleDbCommand command = new OleDbCommand(queryString, BaseDatos.con);
            //    BaseDatos.con.Open();
            //    OleDbDataReader reader = command.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        Console.WriteLine(reader.GetInt32(0) + ", " + reader.GetString(1));
            //    }
            //    // always call Close when done reading.
            //    reader.Close();
            //}

        }

        public void CargaCombo(int nAnio, int nIdEvento, ComboBox cbCombo)
        {
            //DataTable dtbTramos;

            //Eventos_TRECEDataSetTableAdapters.TramosTableAdapter tramosTableAdapter =
            //    new ACERATrece.Eventos_TRECEDataSetTableAdapters.TramosTableAdapter();

            //DataTable dtbTramos = tramosTableAdapter.GetData(nAnio, nIdEvento);

            //cbCombo.Items.Clear();

            //for (int nInd = 0; nInd < dtbTramos.Rows.Count; nInd++)
            //    cbCombo.Items.Add(dtbTramos.Rows[nInd]["IdTramo"].ToString() + " - " +
            //                        dtbTramos.Rows[nInd]["Nombre"].ToString());

            //cbCombo.ResetText();
        }

        public string Capitalizar(string szEntrada)
        {
            return (System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(szEntrada.ToLower()));
        
        }

        public bool IsNumeric(object Expression)
        {
            bool isNum;
            double retNum;

            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

  
    }
}


