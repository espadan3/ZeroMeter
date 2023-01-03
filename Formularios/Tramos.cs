using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Globalization;
using Microsoft.VisualBasic;


namespace ZeroTrip
{

    partial class frPrincipal
    {

        Utiles Util = new Utiles();
        short nTramo;


        #region CONTROLES
        private void teSalida_Leave(object sender, EventArgs e)
        {
          
            if (Util.AvisoConRespuesta("¿Quieres actualizar los datos del tramo?", "Actualizar tramo si o no"))
                btActTramo_Click(sender, e);

        }

        private void btActTramo_Click(object sender, EventArgs e)
        {
            // Aquí se trata el pulsado del botón Actualizar. Guardamos la información general del tramo.
            if (cbTramos.Text == "")
            {
                Util.AvisoConError("Debe elegir un tramo.", "Error en entrada de datos.");
                return;
            }
            if (cbTipo.Text == "")
            {
                Util.AvisoConError("Debe elegir un tipo de tramo.", "Error en entrada de datos.");
                return;
            }
            if (teSalida.Text == "0:00:00")
            {
                if (Util.AvisoConRespuesta("¿Estás seguro que la salida es a las 00:00:00?", "Tenemos una duda."))
                    return;
            }

            try
            {

                tramosTableAdapter.GetData(Convert.ToInt16(cbTramos.Text.Substring(7)));
                //tramosTableAdapter.Update(
                //                             cbTramos.Text,
                //                             cbTipo.Text,
                //                             Convert.ToDateTime(teSalida.Text),
                //                             Convert.ToInt16(cbTramos.Text.Substring(7)));
                tramosTableAdapter.Insert(
                            Convert.ToInt16(cbTramos.Text.Substring(6)),
                            cbTramos.Text,
                            cbTipo.Text,
                            teSalida.Time);
            }
            catch (Exception ex)
            {
                //Util.AvisoConEx(ex, "ll", "kk");
                //tramosTableAdapter.Insert(
                //                            Convert.ToInt16(cbTramos.Text.Substring(7)),
                //                            cbTramos.Text,
                //                            cbTipo.Text,
                //                            Convert.ToDateTime(teSalida.Text));
                tramosTableAdapter.Update(
                             cbTramos.Text,
                             cbTipo.Text,
                             teSalida.Time,
                             Convert.ToInt16(cbTramos.Text.Substring(6)));


            }

            dsTramos.Tramos.AcceptChanges();
        }

        //-----------------------------------------------------------------------------------

        private void cbTramos_EditValueChanged(object sender, EventArgs e)
        {

            bReCargaTramo = false;

            EventHandler handler = new EventHandler(cbTipo_SelectedIndexChanged);
            if (cbTramos.Text != "")
            {
                nTramo = Convert.ToInt16(cbTramos.Text.Substring(6));
                DataTable dtbTramo = tramosTableAdapter.GetData(nTramo);
                if (dtbTramo.Rows.Count != 0)
                {
                    cbTipo.SelectedIndexChanged -= handler;
                    cbTipo.Text = dtbTramo.Rows[0]["TipoTramo"].ToString();
                    cbTipo.SelectedIndexChanged += handler;
                    teSalida.Time = Convert.ToDateTime(dtbTramo.Rows[0]["HoraSalida"].ToString());
                }
                else
                {
                    cbTipo.Text = "";
                    teSalida.Time = Convert.ToDateTime("0:00:00");
                }
                gcIncidencias.EmbeddedNavigator.Enabled = true;
                datosTableAdapter.Fill(dsDatos.Datos, nTramo);
                incidenciasTableAdapter.Fill(dsIncidencias.Incidencias, nTramo);

                bReCargaTramo = true;

                tePosicion.Text = "0";
                cbDescripcion.ResetText();
                cbOrientacion.ResetText();

                rgTipoTramo.Enabled = false;
                if (cbTipo.Text == "Varias")
                {
                    rgTipoTramo.EditValue = ((dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TipoTramo"]).ToString());
                    rgTipoTramo.Enabled = true;
                }
                else
                    rgTipoTramo.EditValue = cbTipo.Text;
 
            }

            gcMedias.Focus();
        }

        //-----------------------------------------------------------------------------------

        private void rgTipoTramo_EditValueChanged(object sender, EventArgs e)
        {
            SelectTipo(rgTipoTramo.Text);

            btActTramo_Click(sender, e);
        }

        //-----------------------------------------------------------------------------------

        private void cbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {

            tHasta.Text = "0";
            tVelocidad.Text = "0,000";

            if (cbTipo.Text == "" || cbTramos.Text == "")
            {
                gcMedias.EmbeddedNavigator.Enabled = false;
                gcIncidencias.EmbeddedNavigator.Enabled = false;
            }   
            else
            {
                gcMedias.EmbeddedNavigator.Enabled = true;
                gcIncidencias.EmbeddedNavigator.Enabled = true;

                if (cbTipo.OldEditValue != null && cbTipo.OldEditValue.ToString() != "" && cbTipo.OldEditValue.ToString() != cbTipo.Text.ToString())
                {
                    if (Util.AvisoConRespuesta("Vas a cambiar el tipo de tramo, eso implica borrar los "
                         + "registros anteriores ¿Estas de acuerdo?", "Cambio de tipo de tramo. OJO!!."))
                        datosTableAdapter.BorrraTramo(nTramo);
                    else
                    {
                        gcMedias.Focus();
                        return;
                    }
                    dsDatos.Datos.AcceptChanges();
                    datosTableAdapter.Fill(dsDatos.Datos, nTramo);
                    incidenciasTableAdapter.Fill(dsIncidencias.Incidencias, nTramo);

                }



                // Habilitamos/deshabilitamos columnas en función del valor del tipo de tramo

                // el radio button para los tipos de tramo, solo lo habilitamos para el tipo VARIOS.
                rgTipoTramo.Enabled = false;
                if (cbTipo.Text != "Varios")
                    rgTipoTramo.EditValue = cbTipo.Text;

                switch (cbTipo.Text)
                {
                    case "Medias":
                    case "RefExternas":
                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcDesde.OptionsColumn.AllowFocus = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcParcial.OptionsColumn.AllowFocus = false;
                        gcVelocidad.OptionsColumn.AllowEdit = true;
                        gcVelocidad.OptionsColumn.AllowFocus = true;
                        gcTiempoAcum.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowFocus = false;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowFocus = false;

                        gcAdd.Text = "Entrada de datos para Medias";
                        lbUno.Text = "Hasta";
                        lbDos.Text = "Velocidad";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = true;
                        teTPaso.Visible = false;
                        tHor.Visible = false;
                        tSec.Visible = false;
                        tMin.Visible = false;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n4";
                        tVelocidad.Properties.EditFormat.FormatString = "n4";
                        tVelocidad.Properties.Mask.EditMask = "n4";

                        break;

                    case "Tablas":
                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcDesde.OptionsColumn.AllowFocus = false;
                        gcHasta.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowFocus = false;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcParcial.OptionsColumn.AllowFocus = false;
                        gcVelocidad.OptionsColumn.AllowEdit = false;
                        gcVelocidad.OptionsColumn.AllowFocus = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowFocus = false;

                        gcAdd.Text = "Entrada de datos para Tablas";
                        lbUno.Text = "Hasta";
                        lbDos.Text = "Segundo";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = true;
                        teTPaso.Visible = false;
                        tHor.Visible = false;
                        tSec.Visible = false;
                        tMin.Visible = false;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n1";
                        tVelocidad.Properties.EditFormat.FormatString = "n1";
                        tVelocidad.Properties.Mask.EditMask = "n1";

                        if (dsDatos.Datos.Rows.Count == 0)
                        {
                            tHasta.Text = (teDistTablas.Text);
                        }
                        else
                        {
                            tHasta.Text = (Convert.ToInt16(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count]["Desde"]) + 100).ToString();
                        }
                        break;


                    case "HitosH":
                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcDesde.OptionsColumn.AllowFocus = false;
                        gcHasta.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowFocus = false;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcParcial.OptionsColumn.AllowFocus = false;
                        gcVelocidad.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowFocus = false;

                        gcAdd.Text = "Entrada de datos para Hitos";
                        lbUno.Text = "Hito";
                        lbDos.Text = "Tiempo";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = true;
                        teTPaso.Visible = false;
                        tHor.Visible = false;
                        tSec.Visible = false;
                        tMin.Visible = false;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n1";
                        tVelocidad.Properties.EditFormat.FormatString = "n1";
                        tVelocidad.Properties.Mask.EditMask = "n1";
                        if (cbTipo.Text == "HitosK")
                            tHasta.Text = Convert.ToInt32(teDistHitos.Text).ToString();
                        else
                            tHasta.Text = "100";

                        break;

                    case "Hitos":
                    case "HitosK":
                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcDesde.OptionsColumn.AllowFocus = false;
                        gcHasta.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowFocus = false;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcParcial.OptionsColumn.AllowFocus = false;
                        gcVelocidad.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowFocus = false;

                        gcAdd.Text = "Entrada de datos para Hitos";
                        lbUno.Text = "Hito";
                        lbDos.Text = "Tiempo";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = false;
                        teTPaso.Visible = false;
                        tHor.Visible = true;
                        tSec.Visible = true;
                        tMin.Visible = true;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n0";
                        tVelocidad.Properties.EditFormat.FormatString = "n0";
                        tVelocidad.Properties.Mask.EditMask = "n0";
                        tHasta.Text = Convert.ToInt32(teDistHitos.Text).ToString();
                        break;

                    case "Viñetas":
                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcDesde.OptionsColumn.AllowFocus = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcParcial.OptionsColumn.AllowFocus = false;
                        gcVelocidad.OptionsColumn.AllowEdit = false;
                        gcVelocidad.OptionsColumn.AllowFocus = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowFocus = false;

                        gcAdd.Text = "Entrada de datos para Viñetas";
                        lbUno.Text = "Por";
                        lbDos.Text = "Tiempo";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = false;
                        teTPaso.Visible = false;
                        tHor.Visible = true;
                        tSec.Visible = true;
                        tMin.Visible = true;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n0";
                        tVelocidad.Properties.EditFormat.FormatString = "n0";
                        tVelocidad.Properties.Mask.EditMask = "n0";
                        break;

                    case "Sectores":
                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcDesde.OptionsColumn.AllowFocus = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcParcial.OptionsColumn.AllowFocus = false;
                        gcVelocidad.OptionsColumn.AllowEdit = true;
                        gcVelocidad.OptionsColumn.AllowFocus = true;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowFocus = false;

                        gcAdd.Text = "Entrada de datos para Sectores";
                        lbUno.Text = "Por";
                        lbDos.Text = "Tiempo";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = false;
                        teTPaso.Visible = false;
                        tHor.Visible = true;
                        tSec.Visible = true;
                        tMin.Visible = true;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n0";
                        tVelocidad.Properties.EditFormat.FormatString = "n0";
                        tVelocidad.Properties.Mask.EditMask = "n0";
                        break;

                    case "Varias":
                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcDesde.OptionsColumn.AllowFocus = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcParcial.OptionsColumn.AllowFocus = false;
                        gcVelocidad.OptionsColumn.AllowEdit = true;
                        gcVelocidad.OptionsColumn.AllowFocus = true;
                        gcTiempoAcum.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowFocus = false;
                        gcTiempoParcial.OptionsColumn.AllowEdit = true;
                        gcTiempoParcial.OptionsColumn.AllowFocus = true;

                        gcAdd.Text = "OJO al tipo de tramo VA POR CONTROL";
                        lbUno.Text = "Hasta";
                        lbDos.Text = "Velocidad";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = true;
                        teTPaso.Visible = false;
                        tHor.Visible = false;
                        tSec.Visible = false;
                        tMin.Visible = false;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n4";
                        tVelocidad.Properties.EditFormat.FormatString = "n4";
                        tVelocidad.Properties.Mask.EditMask = "n4";

                        rgTipoTramo.Enabled = true;
                        rgTipoTramo.EditValue = "Medias";

                        break;
                    default:
                        Util.AvisoInformativo("Opción no contemplada", "Error en la elección del tipo de tramo");
                        break;
                }

            }
            gcMedias.Focus();
        }

        //-----------------------------------------------------------------------------------

        private void btAdd_Click(object sender, EventArgs e)
        {


            if (cbTipo.Text.ToString() != "Varias")
                AddTramo(cbTipo.Text.ToString());
            else
                AddTramo(rgTipoTramo.Text.ToString());

             
            // Si el tramo cargado en memoria es el que acabamos de modificar, lo recargamos.
            if (nTramoCron == Convert.ToInt16(cbTramos.Text.Substring(6)))
            {
                btRecarga_Click( sender,  e);
            }



        }

        //---------------------------------------------------------------------------------------------------------

        private void btAddVineta_Click(object sender, EventArgs e)
        {
            bReCargaTramo = false;

            // DataTable dtIncidencias;

            short nRegs = Convert.ToInt16(incidenciasTableAdapter.CuentaIncidencias(nTramo));

            NumberFormatInfo provider = new NumberFormatInfo();

            provider.NumberDecimalSeparator = ",";
            provider.NumberGroupSeparator = ".";
            provider.NumberGroupSizes = new int[] { 3 };
            //nHasta = int.Parse(tHasta.Text.Replace(".", ""));

            incidenciasTableAdapter.Insert(nTramo, 
                                            Convert.ToInt16(nRegs + 1),
                                            Convert.ToInt32(tePosicion.Text.Replace(".", "")),
                                            cbDescripcion.SelectedItem.ToString(), 
                                            cbOrientacion.SelectedItem.ToString(),
                                           // Convert.ToByte(cbOrientacion.SelectedItem),
                                            "X");

            incidenciasTableAdapter.Fill(dsIncidencias.Incidencias, nTramo);
            gcMedias.RefreshDataSource();
            gcIncidencias.RefreshDataSource();
            gvIncidencias.MoveLast();
            tePosicion.Focus();

            // Si el tramo cargado en memoria es el que acabamos de modificar, lo recargamos.
            if (nTramoCron == Convert.ToInt16(cbTramos.Text.Substring(6)))
            {
                btRecarga_Click(sender, e);
            }

            bReCargaTramo = true;

        }

        //---------------------------------------------------------------------------------------------------------

        private void btRecarga_Click(object sender, EventArgs e)
        {
            if (cbTramosRace.Text != "")
            {
                nTramoCron = Convert.ToInt16(cbTramos.Text.Substring(6));

                CargaTramo(Convert.ToInt16(nTramoCron));
            }
        }       
        
        //-----------------------------------------------------------------------------------

        private void datosBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            DataRow drFila = dsDatos.Tables["Datos"].NewRow();

            if (e.ListChangedType.ToString() == "ItemAdded" && e.NewIndex + 1 == dsDatos.Tables["Datos"].Rows.Count)
            {
                dsDatos.Tables["Datos"].Rows.Add(drFila);
            }
            else
                if ((e.ListChangedType.ToString() == "ItemChanged") && (e.OldIndex != -1))
                //if ((e.ListChangedType.ToString() == "ItemChanged") && cbTipo.Text == "Medias")
                {
                    if ( cbTipo.Text.ToString() != "Viñetas" && cbTipo.Text.ToString() != "Sectores" && cbTipo.Text.ToString() != "Tablas")
                        for (Int16 i = Convert.ToInt16(e.NewIndex); i < dsDatos.Tables["Datos"].Rows.Count; i++)
                            Modificar(i);
                    else
                    {
                        Modificar(Convert.ToInt16(e.NewIndex));
                        if (e.NewIndex + 1 < dsDatos.Datos.Rows.Count)
                            // Quiere decir que estamos modificando la última fila, luego no modificamos la siguiente.
                            Modificar(Convert.ToInt16(e.NewIndex + 1));

                        //gcMedias.RefreshDataSource();
                        //dsDatos.Datos.AcceptChanges();
                        //datosTableAdapter.Fill(dsDatos.Datos, nTramo);
                        //gvMedias.MoveLast(); 

                    }

                    if (dsDatos.Tables["Datos"].GetChanges() != null)
                    {
                        datosTableAdapter.Update(dsDatos);


                        dsDatos.AcceptChanges();

                    }

                    bReCargaTramo = false;
                    datosTableAdapter.Fill(dsDatos.Datos, nTramo);
                    gcMedias.RefreshDataSource();
                    bReCargaTramo = true;
                }

        }

        //-----------------------------------------------------------------------------------

        private void gvMedias_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {

            // AQUI también se puede tratar las modificaciones para cuando nos movemos por el grid
            int a;
            a = 1;

        }

        //-----------------------------------------------------------------------------------

        private void gvMedias_DataSourceChanged(object sender, EventArgs e)
        {
            int a;
            a = 1;
        }

        //-----------------------------------------------------------------------------------

        private void gvMedias_RowCountChanged(object sender, EventArgs e)
        {

            if (dsDatos.Tables["Datos"].Rows.Count > 0 && bReCargaTramo)
                datosTableAdapter.Fill(dsDatos.Datos, nTramo);

            //string b = dsDatos.Datos.Rows[1]["Velocidad"].ToString();
        }

        //-----------------------------------------------------------------------------------

        private void gcMedias_EmbeddedNavigator_ButtonClick(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e)
        {
            DevExpress.XtraEditors.NavigatorButtonType btypAccion = e.Button.ButtonType;
            switch (btypAccion.ToString())
            {
                //Comprobar si ha habido cambios antes para hacer el Update.
                case "Append":
                    //dsCronometros.Tables["Cronometros"].Rows[gvCronometros.GetDataSourceRowIndex(i]["HoraStart"] = dtEvento;
                    break;
                case "Remove":
                    BorrarDato();

                    dsDatos.Datos.AcceptChanges();
                    //datosTableAdapter.Fill(dsDatos.Datos, nTramo);
                    //gcMedias.RefreshDataSource();
                    break;

                case "EndEdit":

                    break;
                default:
                    break;

            }
        }

        //-----------------------------------------------------------------------------------

        private void gcIncidencias_EmbeddedNavigator_ButtonClick(object sender, DevExpress.XtraEditors.NavigatorButtonClickEventArgs e)
        {
            DevExpress.XtraEditors.NavigatorButtonType btypAccion = e.Button.ButtonType;
            switch (btypAccion.ToString())
            {
                //Comprobar si ha habido cambios antes para hacer el Update.
                case "Append":
                    break;
                case "Remove":
                    BorrarIncidencia();

                    //                  dsIncidencias.Incidencias.AcceptChanges();

                    //Si no hago el fill pinta bien despues de borrar pero falla si voy a modificar
                    incidenciasTableAdapter.Fill(dsDatos.Incidencias, nTramo);
                    //                 gcIncidencias.RefreshDataSource();
                    break;

                case "EndEdit":

                    break;
                default:
                    break;

            }
        }

        //-----------------------------------------------------------------------------------

        private void gvIncidencias_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            int ids = 0;
            try
            {
                foreach (int i in gvIncidencias.GetSelectedRows())
                {
                    if (i >= 0)
                    {
                        /// OJO tengo que usar el indice del DataSet y no el del grid, por si han reordenado el grid
                        ids = gvIncidencias.GetDataSourceRowIndex(i);
                        if (dsIncidencias.Tables["Incidencias"].Rows[ids].RowState.ToString() == "Added")
                        {
                            if (!Validar(ids, "Alta"))
                                dsIncidencias.Incidencias.RejectChanges();
                            break;
                        }
                        else
                            /// Si se modifica el campo ControlesPrevistos, actualizamos el numero de controles para el tramo según se añadan o borren
                            /// 
                            if (dsIncidencias.Tables["Incidencias"].Rows[ids].RowState.ToString() == "Modified")
                            {
                                if (!Validar(ids, "Modificacion"))
                                    dsIncidencias.Incidencias.RejectChanges();

                                break;
                            }
                    }
                }
            }
            catch (NullReferenceException nex)
            {
                Util.AvisoConEx(nex, "Se ha producido un error en el tratamiento. Tome nota de la incidencia y comuníquela al Servicio Técnico.", "Error en BBDD.");
            }

            if (dsIncidencias.Tables["Incidencias"].GetChanges() != null)
            {
                //if (dsIncidencias.Tables["Incidencias"].Rows[ids].RowState.ToString() == "Added")


                incidenciasTableAdapter.Update(dsIncidencias.Incidencias);
                dsIncidencias.Incidencias.AcceptChanges();
                if (dsIncidencias.Tables["Incidencias"].Rows[ids].RowState.ToString() == "Added")
                    gvIncidencias.MoveLast();


            }
        }

        //-----------------------------------------------------------------------------------

        private bool Validar(int nFila, string szTipo)
        {

            //if (!Information.IsNumeric(nTramo))
            //    return (false);

            if (nTramo == 0)
                return (false);
            if (dsIncidencias.Incidencias.Rows.Count != 0)
            {
                if (string.IsNullOrEmpty(dsIncidencias.Incidencias.Rows[nFila]["Orientacion"].ToString()))
                    return (false);

                //if (Convert.ToInt32(dsIncidencias.Incidencias.Rows[nFila]["Orientacion"]) < 1 ||
                //    Convert.ToInt32(dsIncidencias.Incidencias.Rows[nFila]["Orientacion"]) > 12)
                //    return (false);

                if (dsIncidencias.Incidencias.Rows[nFila]["Posicion"].ToString() == "")
                    return (false);



                dsIncidencias.Incidencias.Rows[nFila]["Tramo"] = nTramo;
                dsIncidencias.Incidencias.Rows[nFila]["Comentarios"] = "X";

                short sT = Convert.ToInt16(incidenciasTableAdapter.CuentaIncidencias((nTramo)));

                if (szTipo == "Alta")
                // Solo para el alta validamos esto, pues es un campo clave y no lo podemos modificar
                {
                    if (nFila == 0)
                        dsIncidencias.Incidencias.Rows[nFila]["IdIncidencia"] = 1;
                    else
                        dsIncidencias.Incidencias.Rows[nFila]["IdIncidencia"] =
                        Convert.ToInt32(dsIncidencias.Incidencias.Rows[nFila - 1]["IdIncidencia"]) + 1;
                }
            }
            return (true);

        }

        //-----------------------------------------------------------------------------------

        private void btBloqueo_Click(object sender, EventArgs e)
        {
            if (btBloqueo.Tag.ToString() == "Cerrado")
            {
                btBloqueo.Image = ZeroTrip.Properties.Resources.lock_open;
                btBloqueo.Tag = "Abierto";
                gcAdd.Enabled = true;
                gcMedias.EmbeddedNavigator.Enabled = true;
            }
            else
            {
                btBloqueo.Image = ZeroTrip.Properties.Resources._lock;
                btBloqueo.Tag = "Cerrado";
                gcAdd.Enabled = false;
                gcMedias.EmbeddedNavigator.Enabled = false;
            }
        }

        //-----------------------------------------------------------------------------------

        private void incidenciasBindingSource_DataSourceChanged(object sender, EventArgs e)
        {
            int a = 1;
        }

        //-----------------------------------------------------------------------------------

        private void incidenciasBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            int a = 1;
            //      dsIncidencias.Incidencias.Reset();

        }

        private void gvMedias_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            int a = 1;
        }

        private void gvMedias_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            int a = e.RowHandle;

            DataRow drFila = dsDatos.Tables["Datos"].Rows[e.RowHandle];
          //  Aqui controlar cuando pinchamos en una fila del gvMedias, que leamos el tipo dec  tramo de  la columna 

            string szTipo = drFila["TipoTramo"].ToString();

            rgTipoTramo.EditValue = drFila["TipoTramo"].ToString();

            
        }

        #endregion CONTROLES


        #region VARIOS


        //-----------------------------------------------------------------------------------

        private void BorrarDato()
        {
            foreach (int j in gvMedias.GetSelectedRows())
            {
                if (j >= 0)
                {
                    /// OJO tengo que usar el indice del DataSet y no el del grid, por si han reordenado el grid
                    short iDs = Convert.ToInt16(gvMedias.GetDataSourceRowIndex(j));
                    //dsDatos.Datos.Rows[iDs].BeginEdit();
                    short nFila = Convert.ToInt16(dsDatos.Tables["Datos"].Rows[iDs]["IdDato"].ToString());
                    datosTableAdapter.Delete(nTramo,nFila);
                    for (Int16 i = Convert.ToInt16(iDs); i < dsDatos.Tables["Datos"].Rows.Count; i++)
                        Modificar(iDs);
                    //dsDatos.Datos.Rows[iDs].EndEdit();

                    gvMedias.MoveLast();

                }
            }
        }

        //-----------------------------------------------------------------------------------

        private void BorrarIncidencia()
        {


            foreach (int j in gvIncidencias.GetSelectedRows())
            {
                if (j >= 0)
                {
                    /// OJO tengo que usar el indice del DataSet y no el del grid, por si han reordenado el grid
                    short iDs = Convert.ToInt16(gvIncidencias.GetDataSourceRowIndex(j));

                    incidenciasTableAdapter.Delete(nTramo, Convert.ToInt16(dsIncidencias.Tables["Incidencias"].Rows[iDs]["IdIncidencia"].ToString()));
                    dsIncidencias.Incidencias.AcceptChanges();
                    // incidenciasTableAdapter.Fill(dsIncidencias.Incidencias, nTramo);
                    gcIncidencias.RefreshDataSource();

                }
            }

            if (dsIncidencias.Incidencias.GetChanges() != null)
            {
                //if (dsIncidencias.Tables["Incidencias"].Rows[ids].RowState.ToString() == "Added")
                incidenciasTableAdapter.Update(dsIncidencias.Incidencias);

                dsIncidencias.Incidencias.AcceptChanges();

                gvIncidencias.MoveLast();
            }
        }

        //-----------------------------------------------------------------------------------

        private void Modificar(short nFila)
        {
            DataTable dtDatos;
            //DateTime dtAux;
            DateTime dtmTParcial = DateTime.Today, dtmTAcumulado = DateTime.Today;
            //DataRow drFila;
            DataRow drFila = dsDatos.Tables["Datos"].NewRow();
            Int32 nDesde, nHasta, nParcial;
            decimal dbVelocidad;
            //short nRegs = Convert.ToInt16(datosTableAdapter.ContarDatos(nTramo));
            Int16 nIdDato = Convert.ToInt16(dsDatos.Datos.Rows[nFila]["IdDato"]);

            NumberFormatInfo provider = new NumberFormatInfo();

            provider.NumberDecimalSeparator = ",";
            provider.NumberGroupSeparator = ".";
            provider.NumberGroupSizes = new int[] { 3 };

            dtDatos = datosTableAdapter.GetFila(nTramo, nIdDato);


            switch (cbTipo.Text.ToString())
            {
                case "Medias":
                case "RefExternas":
                    drFila = dsDatos.Tables["Datos"].Rows[nFila];
                    nHasta = 0;
                    if (drFila.HasVersion(DataRowVersion.Original))
                        nHasta = int.Parse(drFila["Hasta", DataRowVersion.Current].ToString().Replace(".", ""));

                    //nHasta = int.Parse(dsDatos.Tables["Datos"].Rows[nFila - 1]["Hasta"].ToString().Replace(".", ""));
                    if (drFila.HasVersion(DataRowVersion.Proposed))
                        dbVelocidad = decimal.Parse(drFila["Velocidad", DataRowVersion.Proposed].ToString(), provider);
                    dbVelocidad = decimal.Parse(dsDatos.Datos.Rows[nFila]["Velocidad"].ToString(), provider);
                    dbVelocidad = decimal.Parse(drFila["Velocidad", DataRowVersion.Current].ToString(), provider);

                    if (dbVelocidad == 0)
                    {
                        Util.AvisoInformativo("La velocidad no puede ser 0.", "Error en entrada de datos.");
                        return;
                    }

                    if (dtDatos.Rows.Count == 0)
                    {
                        //nRegs = 0;
                        nDesde = 0;
                        nParcial = nHasta;
                        dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        dtmTAcumulado = dtmTParcial;
                    }
                    else
                    {
                        //nRegs = Convert.ToInt16(dtDatos.Rows[0]["IdDato"]);
                        if (nFila == 0)
                            nDesde = 0;
                        else
                            nDesde = Convert.ToInt32(dsDatos.Tables["Datos"].Rows[nFila - 1]["Hasta"].ToString());
                        if ((nHasta == 0) || (nDesde > nHasta))
                        {
                            Util.AvisoInformativo("La distancia Hasta no puede ser 0 o menor de Desde.", "Error en entrada de datos.");
                            return;
                        }
                        nParcial = nHasta - nDesde;
                        dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        if (nFila == 0)
                            dtmTAcumulado = dtmTParcial;
                        else
                            dtmTAcumulado = dtmTParcial.Add(Convert.ToDateTime(dsDatos.Tables["Datos"].Rows[nFila - 1]["TiempoAcum"]).TimeOfDay);
                    }

                    //tHasta.Focus();
                    break;


                case "Viñetas":
                case "Sectores":
                    nHasta = 0;
                    drFila = dsDatos.Tables["Datos"].Rows[nFila];
                    //nHasta = int.Parse(tHasta.Text.Replace(".", ""));
                    if (drFila.HasVersion(DataRowVersion.Original))
                        nHasta = int.Parse(drFila["Hasta", DataRowVersion.Current].ToString().Replace(".", ""));
                    dbVelocidad = decimal.Parse(tVelocidad.Text.ToString(), provider);

                    //if (dbVelocidad == 0)
                    //{
                    //    Util.AvisoInformativo("La velocidad no puede ser 0.", "Error en entrada de datos.");
                    //    return;
                    //}

                    if (dtDatos.Rows.Count == 0)
                    {
                        //nRegs = 0;
                        nDesde = 0;
                        nParcial = nHasta;
                        dtmTParcial = Convert.ToDateTime(dsDatos.Tables["Datos"].Rows[nFila]["TiempoAcum"]);
                        dtmTAcumulado = dtmTParcial;
                    }
                    else
                    {
                        //nRegs = Convert.ToInt16(dtDatos.Rows[0]["IdDato"]);
                        if (nFila == 0)
                            nDesde = 0;
                        else
                            nDesde = Convert.ToInt32(dsDatos.Tables["Datos"].Rows[nFila - 1]["Hasta"].ToString());

                        if ((nHasta == 0) || (nDesde > nHasta))
                        {
                            Util.AvisoInformativo("La distancia Hasta no puede ser 0 o menor de Desde.", "Error en entrada de datos.");
                            return;
                        }

                        nParcial = nHasta - nDesde;
                        // dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        //dtmTParcial = dtmTAcumulado.Subtract(Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]).TimeOfDay);
                        //dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        dtmTAcumulado = Convert.ToDateTime(dsDatos.Tables["Datos"].Rows[nFila]["TiempoAcum"]);
                        if (nFila == 0)
                            dtmTParcial = dtmTAcumulado;
                        else
                            dtmTParcial = dtmTAcumulado.Subtract(Convert.ToDateTime(dsDatos.Tables["Datos"].Rows[nFila - 1]["TiempoAcum"]).TimeOfDay);
                    }


                    dbVelocidad = (decimal)((Convert.ToDouble(nParcial) / 1000) / (dtmTParcial.TimeOfDay.TotalHours));
                    //tHasta.Focus();

                    break;

                case "Hitos":
                case "HitosH":
                case "HitosK":
                case "Tablas":
                    drFila = dsDatos.Tables["Datos"].Rows[nFila];
                    if (cbTipo.Text.ToString() != "HitosK")
                        nParcial = 100;
                    else
                        nParcial = 1000;

                    if (dtDatos.Rows.Count == 0)
                    {
                        nDesde = 0;
                        nHasta = Convert.ToInt32(dsDatos.Datos.Rows[nFila]["Hasta"]);
                        if (dsDatos.Datos.Rows[nFila].HasVersion(DataRowVersion.Current))
                            nHasta = Convert.ToInt32(dsDatos.Datos.Rows[nFila]["Hasta", DataRowVersion.Current]);
                        //int.Parse(tHasta.Text.Replace(".", ""));
                        nParcial = nHasta;
                        // tHasta.Enabled = false;
                        // tHasta.Enabled = true;
                        dtmTParcial = Convert.ToDateTime(dsDatos.Datos[nFila]["TiempoAcum"]);
                        dtmTAcumulado = dtmTParcial;

                    }
                    else
                    {
                        if (nFila == 0)
                            nDesde = 0;
                        else
                            nDesde = Convert.ToInt32(dsDatos.Datos.Rows[nFila - 1]["Hasta"]);
                        nHasta = Convert.ToInt32(dsDatos.Datos.Rows[nFila]["Hasta"]);
                        if (dsDatos.Datos.Rows[nFila].HasVersion(DataRowVersion.Current))
                            nHasta = Convert.ToInt32(dsDatos.Datos.Rows[nFila]["Hasta", DataRowVersion.Current]);

                        nParcial = nHasta - nDesde;

                        dtmTAcumulado = Convert.ToDateTime(dsDatos.Datos[nFila]["TiempoAcum"]);
                        if (nFila == 0)
                            dtmTParcial = dtmTAcumulado;
                        else
                            dtmTParcial = dtmTAcumulado.Subtract(Convert.ToDateTime(dsDatos.Datos[nFila - 1]["TiempoAcum"]).TimeOfDay);


                    }
                    dbVelocidad = (decimal)((Convert.ToDouble(nParcial) / 1000) / (dtmTParcial.TimeOfDay.TotalHours));
                    //tVelocidad.Focus();
                    break;
                default:
                    return;
                    // break;

            }

            
            drFila["IdTramo"] = nTramo;
            drFila["IdDato"] = nIdDato;
            drFila["Desde"] = nDesde;
            drFila["Hasta"] = nHasta;
            drFila["Parcial"] = nParcial;
            drFila["Velocidad"] = (double)dbVelocidad;
            drFila["TiempoAcum"] = dtmTAcumulado;
            drFila["TiempoParcial"] = dtmTParcial;
            drFila["TipoTramo"] = rgTipoTramo.Text;


            //datosTableAdapter.ModificaFila(nDesde,
            //                        nHasta,
            //                        nParcial,
            //                        (dbVelocidad),
            //                        dtmTParcial,
            //                        dtmTAcumulado,
            //      //                  cbTipo.Text,
                                    
            //                        nTramo,
            //                        nIdDato);
            //(short)(nFila+1));

            //if (dsDatos.Tables["Datos"].GetChanges() != null)
            //{
            //    datosTableAdapter.Update(dsDatos);


            //    dsDatos.AcceptChanges();

            //}


            //////bReCargaTramo = false;
            //////datosTableAdapter.Fill(dsDatos.Datos, nTramo);
            //////gcMedias.RefreshDataSource();
            //////bReCargaTramo = true;

        }

        //-----------------------------------------------------------------------------------

        private void tHasta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                // Lo que hará al presionarse Enter
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    // btAdd_Click(sender, e);
                    if (rgTipoTramo.Text == "Viñetas" || rgTipoTramo.Text == "Hitos")
                    {
                        tMin.Focus();
                        tMin.SelectionStart = 0;
                        tMin.SelectionLength = tMin.Text.Length;
                    }
                    else
                    {
                        tVelocidad.Focus();
                        tVelocidad.SelectionStart = 0;
                        tVelocidad.SelectionLength = tVelocidad.Text.Length;
                    }
                }
            }
        }

        //-----------------------------------------------------------------------------------
        
        private void tVelocidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                // Lo que hará al presionarse Enter
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {

                    btAdd_Click(sender, e);
                    if (rgTipoTramo.Text != "Tablas")
                    {
                        tHasta.Focus();
                        tHasta.SelectionStart = 0;
                        tHasta.SelectionLength = tHasta.Text.Length;
                    }
                    else
                    {
                        tVelocidad.Focus();
                        tVelocidad.SelectionStart = 0;
                        tVelocidad.SelectionLength = tVelocidad.Text.Length;
                    }

                }
            }
        }


        //-----------------------------------------------------------------------------------

        private void tePosicion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                // Lo que hará al presionarse Enter
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                   // btAdd_Click(sender, e);
                    cbDescripcion.Focus();
                    cbDescripcion.SelectionStart = 0;
                    cbDescripcion.SelectionLength = cbDescripcion.Text.Length;
                }

            }
        }

        //-----------------------------------------------------------------------------------

        private void tMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            {
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    // Lo que hará al presionarse Enter
                    if (e.KeyChar == Convert.ToChar(Keys.Enter))
                    {
                        // btAdd_Click(sender, e);
                        tSec.Focus();
                        tSec.SelectionStart = 0;
                        tSec.SelectionLength = tSec.Text.Length;
                    }

                }
            }
        }

        //-----------------------------------------------------------------------------------

        private void tSec_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                // Lo que hará al presionarse Enter
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    btAdd_Click(sender, e);
                    if (rgTipoTramo.Text != "Viñetas")
                    {
                        tMin.Focus();
                        tMin.SelectionStart = 0;
                        tMin.SelectionLength = tMin.Text.Length;
                    }
                    else
                    {
                        tHasta.Focus();
                        tHasta.SelectionStart = 0;
                        tHasta.SelectionLength = tHasta.Text.Length;
                    }

                }
            }
        }

        //-----------------------------------------------------------------------------------

        private void tVelocidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Right)
            {

                tVelocidad.Select(tVelocidad.Text.Length, 0);
            }
            //if (rgSegDec.EditValue == "Segundo")
            //    tVelocidad.Text = Convert.ToString(Convert.ToDouble(tVelocidad.Text) - 1);
            //else
            //    tVelocidad.Text = Convert.ToString(Convert.ToDouble(tVelocidad.Text) - 0.1);
        }

        //-----------------------------------------------------------------------------------

        private void cbDescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                // Lo que hará al presionarse Enter
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    // btAdd_Click(sender, e);
                    cbOrientacion.Focus();
                    cbOrientacion.SelectionStart = 0;
                    cbOrientacion.SelectionLength = cbDescripcion.Text.Length;
                }

            }
        }

        //-----------------------------------------------------------------------------------

        private void cbOrientacion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                // Lo que hará al presionarse Enter
                if (e.KeyChar == Convert.ToChar(Keys.Enter))
                {
                    btAddVineta_Click(sender, e);
                    tePosicion.Focus();
                    tePosicion.SelectionStart = 0;
                    tePosicion.SelectionLength = tePosicion.Text.Length;
                }

            }
        }

        //-----------------------------------------------------------------------------------

        private void tVelocidad_Enter(object sender, EventArgs e)
        {
            
            tVelocidad.SelectionStart = 0;
            tVelocidad.SelectionLength = tVelocidad.Text.Length;
        }

        //-----------------------------------------------------------------------------------

        private void tHasta_Enter(object sender, EventArgs e)
        {
            
            tHasta.SelectionStart = 0;
            tHasta.SelectionLength = tHasta.Text.Length;
        }

        //-----------------------------------------------------------------------------------

        private void tHasta_MouseClick(object sender, MouseEventArgs e)
        {
            tHasta.SelectionStart = 0;
            tHasta.SelectionLength = tHasta.Text.Length;
        }

        //-----------------------------------------------------------------------------------

        private void tHor_Click(object sender, EventArgs e)
        {
            tHor.SelectionStart = 0;
            tHor.SelectionLength = tHor.Text.Length;
        }

        //-----------------------------------------------------------------------------------

        private void tMin_Click(object sender, EventArgs e)
        {
            tMin.SelectionStart = 0;
            tMin.SelectionLength = tMin.Text.Length;
        }

        //-----------------------------------------------------------------------------------

        private void tSec_Click(object sender, EventArgs e)
        {
            tSec.SelectionStart = 0;
            tSec.SelectionLength = tSec.Text.Length;
        }

        //-----------------------------------------------------------------------------------

        private void rgTipoTramo_SelectedIndexChanged(object sender, EventArgs e)
        {
            int a = 1;
        }


        #endregion VARIOS
    }
}