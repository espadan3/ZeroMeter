using System.Linq;

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

  //  class AddTramo
    {
        //Utiles Util = new Utiles();
        //short nTramo;

        public void AddTramo(string szTipoTramo)
        {
            bReCargaTramo = false;

            // DataTable sobre el que trabajamos para crear los cambios de media de un tramo.
            DataTable dtDatos;
            DateTime dtAux;
            DateTime dtmTParcial = DateTime.Today, dtmTAcumulado = DateTime.Today;

            //DataTable dtTablaDatos = dsDatos.Tables["Datos"];
            DataRow rwDatos = dsDatos.Tables["Datos"].NewRow();

            int nDesde, nHasta, nParcial;
            decimal dbVelocidad;
            short nRegs = Convert.ToInt16(datosTableAdapter.ContarDatos(nTramo));

            string[] arAux;
            int nSegundo;
            int nMili;
            int nUltReg; //Indice base 0 del ultimo registro registrado en la tabla Datos (Cambios de media)


            NumberFormatInfo provider = new NumberFormatInfo();

            provider.NumberDecimalSeparator = ",";
            provider.NumberGroupSeparator = ".";
            provider.NumberGroupSizes = new int[] { 3 };

            dtDatos = datosTableAdapter.GetUltimoDato(nTramo, nTramo);

            nUltReg = dsDatos.Tables["Datos"].Rows.Count;


            switch (szTipoTramo)
            {
                case "Medias":
                case "RefExternas":
                    nHasta = int.Parse(tHasta.Text.Replace(".", ""));

                    dbVelocidad = decimal.Parse(tVelocidad.Text.ToString(), provider);

                    if (dbVelocidad == 0)
                    {
                        Util.AvisoInformativo("La velocidad no puede ser 0.", "Error en entrada de datos.");
                        return;
                    }

                    if (dtDatos.Rows.Count == 0)
                    {
                        nRegs = 0;
                        nDesde = 0;
                        nParcial = nHasta;
                        dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        dtmTAcumulado = dtmTParcial;
                    }
                    else
                    {
                        nRegs = Convert.ToInt16(dtDatos.Rows[0]["IdDato"]);
                        nDesde = Convert.ToInt32(dtDatos.Rows[0]["Hasta"]);
                        if ((nHasta == 0) || (nDesde > nHasta))
                        {
                            Util.AvisoInformativo("La distancia Hasta no puede ser 0 o menor de Desde.", "Error en entrada de datos.");
                            return;
                        }
                        nParcial = nHasta - nDesde;
                        dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        dtmTAcumulado = dtmTParcial.Add(Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]).TimeOfDay);
                    }

                    tHasta.Focus();
                    tHasta.SelectionStart = 0;
                    tHasta.SelectionLength = tHasta.Text.Length;
                    break;

                case "Hitos":
                case "Viñetas":
                case "Sectores":
                case "HitosK":

                    arAux = tSec.Text.Split(new Char[] { ',' });
                    nSegundo = Convert.ToInt16(arAux[0]);
                    nMili = Convert.ToInt16(arAux[1]);

                    if (szTipoTramo == "HitosK")
                        nParcial = Convert.ToInt32(teDistHitos.Text);

                    nHasta = int.Parse(tHasta.Text.Replace(".", ""));

                    dbVelocidad = decimal.Parse(tVelocidad.Text.ToString(), provider);

                    //if (dbVelocidad == 0)
                    //{
                    //    Util.AvisoInformativo("La velocidad no puede ser 0.", "Error en entrada de datos.");
                    //    return;
                    //}

                    if (dtDatos.Rows.Count == 0)
                    {
                        nRegs = 0;
                        nDesde = 0;
                        nParcial = nHasta;

                        dtmTParcial = DateTime.Parse(tHor.Text + ":" +
                            tMin.Text + ":" + nSegundo.ToString() + "." + nMili.ToString());
                        //dtmTParcial = teTPaso.Time;
                        dtmTAcumulado = dtmTParcial;
                    }
                    else
                    {
                        nRegs = Convert.ToInt16(dtDatos.Rows[0]["IdDato"]);
                        nDesde = Convert.ToInt32(dtDatos.Rows[0]["Hasta"]);
                        if ((nHasta == 0) || (nDesde > nHasta))
                        {
                            Util.AvisoInformativo("La distancia Hasta no puede ser 0 o menor de Desde.", "Error en entrada de datos.");
                            return;
                        }
                        nParcial = nHasta - nDesde;
                        // dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        //dtmTParcial = dtmTAcumulado.Subtract(Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]).TimeOfDay);
                        //dtmTParcial = Util.Tiempo(nParcial, dbVelocidad);
                        //teTPaso.Time.Hour = Convert.ToInt16(tHor.Text);

                        dtmTAcumulado = DateTime.Parse(tHor.Text + ":" +
                            tMin.Text + ":" + nSegundo.ToString() + "." + nMili.ToString());

                        //dtmTAcumulado = teTPaso.Time;
                        dtmTParcial = dtmTAcumulado.Subtract(Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]).TimeOfDay);
                    }

                    if (szTipoTramo == "HitosK" || szTipoTramo == "Hitos")
                        tHasta.Text = (nHasta + Convert.ToInt32(teDistHitos.Text)).ToString();
                    //tHasta.Text = (nHasta + nParcial).ToString();

                    if ((dtmTParcial.TimeOfDay.TotalHours == 0))
                    {
                        Util.AvisoInformativo("La Diferencia de Tiempo no puede ser 0.", "Error en entrada de datos.");
                        return;
                    }
                    dbVelocidad = (decimal)((Convert.ToDouble(nParcial) / 1000) / (dtmTParcial.TimeOfDay.TotalHours));
                    //tHasta.Focus();
                    //tMin.Focus();
                    //tMin.SelectionStart = 0;
                    //tMin.SelectionLength = tHasta.Text.Length;
                    //tHasta.SelectionStart = 0;
                    //tHasta.SelectionLength = tHasta.Text.Length;

                    break;
                case "Tablas":

                    nParcial = Convert.ToInt32(teDistTablas.Text);

                    if (dtDatos.Rows.Count == 0)
                    {
                        nDesde = 0;
                        //nHasta = Convert.ToInt32(teDistTablas.Text);
                        nHasta = Convert.ToInt32(tHasta.Text.Replace(".", ""));
                        dtmTParcial = dtmTParcial.AddSeconds(Convert.ToDouble(tVelocidad.Text.ToString()));
                        dtmTAcumulado = dtmTParcial;
                        tHasta.Text = (nHasta + Convert.ToInt32(teDistTablas.Text)).ToString();

                    }
                    else
                    {
                        nDesde = Convert.ToInt32(dtDatos.Rows[0]["Hasta"]);
                        nHasta = Convert.ToInt32(tHasta.Text.Replace(".", ""));
                        //nHasta = Convert.ToInt32(dtDatos.Rows[0]["Hasta"]) + Convert.ToInt32(teDistTablas.Text);
                        tHasta.Text = (nHasta + Convert.ToInt32(teDistTablas.Text)).ToString();
                        //dtmTParcial = 0 ;
                        dtAux = Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]);
                        //string szAux = dtAux.TimeOfDay.ToString();

                        int nHora = dtAux.Hour;
                        int nMinuto = dtAux.Minute;
                        string[] arAux2 = tVelocidad.Text.Split(new Char[] { ',' });
                        int nSegundo2 = Convert.ToInt16(arAux2[0]);
                        int nMili2 = Convert.ToInt16(arAux2[1]);
                        double dSigSeg;

                        if (dtAux.Second > nSegundo2)
                        {
                            nMinuto += 1;
                            if (nMinuto == 60)
                            {
                                nMinuto = 0;
                                nHora += 1;
                            }

                        }

                        dtmTAcumulado = DateTime.Parse(nHora.ToString() + ":" +
                            nMinuto.ToString() + ":" + nSegundo2.ToString() + "." + nMili2.ToString());
                        dtmTParcial = dtmTAcumulado.Subtract(Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]).TimeOfDay);

                        dSigSeg = ((Convert.ToDouble(nSegundo2) + Convert.ToDouble(nMili2)/10)) + ((Convert.ToDouble(dtmTParcial.Second) + Convert.ToDouble(dtmTParcial.Millisecond)/1000));
                        if (dSigSeg >= 60)
                            dSigSeg -= 60;
                        tVelocidad.Text = dSigSeg.ToString();
                    }

                    //if (dtmTParcial)
                    if (dtmTParcial.TimeOfDay == DateTime.MinValue.TimeOfDay)
                    {
                        Util.AvisoInformativo("La diferencia de tiempo no puede ser 0.", "Error en entrada de datos.");
                        return;
                    }

                    // Meto esto aquí porque se ha podido cambiar la distancia Hasta.
                    nParcial = nHasta - nDesde;

                    dbVelocidad = (decimal)((Convert.ToDouble(nParcial) / 1000) / (dtmTParcial.TimeOfDay.TotalHours));
                    tVelocidad.Focus();
                    //tVelocidad.SelectionStart = 0;
                    //tVelocidad.SelectionLength = tVelocidad.Text.Length;
                    break;

                case "HitosH":

                    if (szTipoTramo == "HitosH" || szTipoTramo == "Hitos")
                        nParcial = 100;
                    else
                        nParcial = 1000;


                    if (dtDatos.Rows.Count == 0)
                    {
                        nDesde = 0;
                        nHasta = int.Parse(tHasta.Text.Replace(".", ""));
                        nParcial = nHasta;
                        tHasta.Enabled = false;

                        dtmTParcial = dtmTParcial.AddSeconds(Convert.ToDouble(tVelocidad.Text.ToString()));
                        dtmTAcumulado = dtmTParcial;

                    }
                    else
                    {
                        nDesde = Convert.ToInt32(dtDatos.Rows[0]["Hasta"]);
                        nHasta = Convert.ToInt32(dtDatos.Rows[0]["Hasta"]) + nParcial;
                        tHasta.Text = (nHasta + nParcial).ToString();
                        //dtmTParcial = 0 ;
                        dtAux = Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]);
                        //string szAux = dtAux.TimeOfDay.ToString();

                        int nHora = dtAux.Hour;
                        int nMinuto = dtAux.Minute;
                        string[] arAux2 = tVelocidad.Text.Split(new Char[] { ',' });
                        int nSegundo2 = Convert.ToInt16(arAux2[0]);
                        int nMili2 = Convert.ToInt16(arAux2[1]);
                        if (dtAux.Second > nSegundo2)
                        {
                            nMinuto += 1;
                            if (nMinuto == 60)
                            {
                                nMinuto = 0;
                                nHora += 1;
                            }

                        }

                        dtmTAcumulado = DateTime.Parse(nHora.ToString() + ":" +
                            nMinuto.ToString() + ":" + nSegundo2.ToString() + "." + nMili2.ToString());
                        dtmTParcial = dtmTAcumulado.Subtract(Convert.ToDateTime(dtDatos.Rows[0]["TiempoAcum"]).TimeOfDay);


                    }

                    dbVelocidad = (decimal)((Convert.ToDouble(nParcial) / 1000) / (dtmTParcial.TimeOfDay.TotalHours));
                    tVelocidad.Focus();
                    //tVelocidad.SelectionStart = 0;
                    //tVelocidad.SelectionLength = tVelocidad.Text.Length;

                    break;

                default:
                    return;

            }

            rwDatos["IdTramo"] = nTramo;
            rwDatos["IdDato"] = Convert.ToInt16(nRegs + 1);
            rwDatos["Desde"] = nDesde;
            rwDatos["Hasta"] = nHasta;
            rwDatos["Parcial"] = nParcial;
            rwDatos["Velocidad"] = (double)dbVelocidad;
            rwDatos["TiempoAcum"] = dtmTAcumulado;
            rwDatos["TiempoParcial"] = dtmTParcial;
            rwDatos["TipoTramo"] = rgTipoTramo.Text;

            
            dsDatos.Tables["Datos"].Rows.Add(rwDatos);
            int a = dsDatos.Tables["Datos"].Rows.Count;
            if (dsDatos.Tables["Datos"].Rows[dsDatos.Tables["Datos"].Rows.Count - 1]["IdTramo"] == DBNull.Value)
            {
                dsDatos.Tables["Datos"].Rows[dsDatos.Tables["Datos"].Rows.Count - 1].Delete();
            }

            //datosTableAdapter.Insert(nTramo,
            //                        Convert.ToInt16(nRegs + 1),
            //                        nDesde,
            //                        nHasta,
            //                        nParcial,
            //                        (double)dbVelocidad,
            //                        dtmTParcial,
            //                        dtmTAcumulado,r
            //                        rgTipoTramo.Text);

            if (dsDatos.Tables["Datos"].GetChanges() != null)
            {
                datosTableAdapter.Update(dsDatos);
                

                dsDatos.AcceptChanges();

            }


            datosTableAdapter.Fill(dsDatos.Datos, nTramo);
            gcMedias.RefreshDataSource();

            gvMedias.MoveLast();

            bReCargaTramo = true;

            //// Si el tramo cargado en memoria es el que acabamos de modificar, lo recargamos.
            //if (nTramoCron == Convert.ToInt16(cbTramos.Text.Substring(6)))
            //{
            //    btRecarga_Click(sender, e);
            //}

        }


        public void SelectTipo(string szTipoTramo)
        {
            int nParcial;

            switch (szTipoTramo)
                {
                    case "Medias":
                    case "RefExternas":
                        lbDos.Text = "Velocidad";
                        if (dsDatos.Datos.Rows.Count != 0)
                            tHasta.Text = (Convert.ToInt32(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["Hasta"])).ToString();
                        else
                            tHasta.Text = "0";

                        tVelocidad.Text = "0,000";
                        gcAdd.Text = "Entrada de datos para Medias";
                        lbUno.Text = "Hasta";
                        lbDos.Text = "Velocidad";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = true;
                        teTPaso.Visible = false;
                        tHor.Visible = false;
                        tSec.Visible = false;
                        tMin.Visible = false;
                        tVelocidad.Visible = true;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n4";
                        tVelocidad.Properties.EditFormat.FormatString = "n4";
                        tVelocidad.Properties.Mask.EditMask = "n4";

                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcVelocidad.OptionsColumn.AllowEdit = true;
                        gcVelocidad.OptionsColumn.AllowFocus = true;
                        if (szTipoTramo == "RefExternas")
                        {
                            gcTiempoParcial.OptionsColumn.AllowEdit = true;
                            gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        }
                        else
                        {
                            gcTiempoParcial.OptionsColumn.AllowEdit = false;
                            gcTiempoAcum.OptionsColumn.AllowEdit = false;
                        }
                        break;
                    case "Tablas":
                        lbDos.Text = "Segundo";
                        if (dsDatos.Datos.Rows.Count != 0)
                            tHasta.Text = (Convert.ToInt32(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["Hasta"]) + Convert.ToInt32(teDistTablas.Text)).ToString();
                        else
                            tHasta.Text = "0";

                        tVelocidad.Text = "0,000";
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

                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = true;
                        gcParcial.OptionsColumn.AllowFocus = true;
                        gcVelocidad.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;

                        break;
                    case "Viñetas":
                    case "Sectores":
                        lbDos.Text = "Tiempo";
                        if (dsDatos.Datos.Rows.Count != 0)
                        {
                            tHasta.Text = (Convert.ToInt32(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["Hasta"]) + 100).ToString();
                            teTPaso.Time = (Convert.ToDateTime(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TiempoAcum"]));
                            tHor.Text = (Convert.ToDateTime(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TiempoAcum"])).Hour.ToString();
                            tMin.Text = (Convert.ToDateTime(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TiempoAcum"])).Minute.ToString();
                            tSec.Text = (Convert.ToDateTime(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TiempoAcum"])).Second.ToString();
                        }

                        tHasta.Text = "0";
                        gcAdd.Text = "Entrada de datos para Viñetas y Sectores";
                        lbUno.Text = "Por";
                        lbDos.Text = "Tiempo de paso";
                        tHasta.Enabled = true;
                        tVelocidad.Visible = false;
                        teTPaso.Visible = false;
                        tHor.Visible = true;
                        tSec.Visible = true;
                        tMin.Visible = true;
                        tHasta.Focus();


                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcVelocidad.OptionsColumn.AllowEdit = true;
                        gcVelocidad.OptionsColumn.AllowFocus = true;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;

                        break;

                    case "HitosH":
                        //   case "HitosK":

                        if (cbTipo.Text.ToString() == "HitosK")
                            nParcial = 1000;
                        else
                            nParcial = 100;

                        lbDos.Text = "Segundo";
                        if (dsDatos.Datos.Rows.Count != 0)
                            tHasta.Text = (Convert.ToInt32(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["Hasta"]) + nParcial).ToString();
                        teTPaso.Visible = false;
                        tHor.Visible = false;
                        tSec.Visible = false;
                        tMin.Visible = false;
                        tHasta.Enabled = true;
                        tVelocidad.Visible = true;
                        tVelocidad.Properties.DisplayFormat.FormatString = "n1";
                        tVelocidad.Properties.EditFormat.FormatString = "n1";
                        tVelocidad.Properties.Mask.EditMask = "n1";

                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcVelocidad.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;

                        break;

                    case "Hitos":
                    case "HitosK":
                        nParcial = 1000;
                        lbDos.Text = "Tiempo";
                        if (dsDatos.Datos.Rows.Count != 0)
                        {
                            tHasta.Text = (Convert.ToInt32(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["Hasta"]) + nParcial).ToString();
                            teTPaso.Time = (Convert.ToDateTime(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TiempoAcum"]));
                        }
                        teTPaso.Visible = false;

                        tHor.Visible = true;
                        tSec.Visible = true;
                        tMin.Visible = true;

                        tVelocidad.Visible = false;
                        tHasta.Enabled = true;

                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcVelocidad.OptionsColumn.AllowEdit = false;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;

                        break;

                    case "Varias":
                        nParcial = 0;
                        //rgTipoTramo.Enabled = true;
                        //lbDos.Text = "Tiempo";
                        if (dsDatos.Datos.Rows.Count != 0)
                        {
                            tHasta.Text = (Convert.ToInt32(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["Hasta"]) + nParcial).ToString();
                            teTPaso.Time = (Convert.ToDateTime(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TiempoAcum"]));
                            if (dsDatos.Datos.Rows.Count != 0)
                                rgTipoTramo.EditValue = (dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TipoTramo"]).ToString();
                            else
                                rgTipoTramo.EditValue = "Medias";
                            //(Convert.ToDateTime(dsDatos.Datos.Rows[dsDatos.Datos.Rows.Count - 1]["TipoTramo"]));
                        }
                        //teTPaso.Visible = false;

                        //if (rgTipoTramo.Text == "Medias")
                        //{
                        //    tVelocidad.Visible = true;
                        //    tHor.Visible = false;
                        //    tSec.Visible = false;
                        //    tMin.Visible = false;
                        //}
                        //else
                        //{
                        //    tVelocidad.Visible = false;

                        //    tHor.Visible = true;
                        //    tSec.Visible = true;
                        //    tMin.Visible = true;
                        //}

                        //tHasta.Enabled = true;

                        gcDesde.OptionsColumn.AllowEdit = false;
                        gcHasta.OptionsColumn.AllowEdit = true;
                        gcHasta.OptionsColumn.AllowFocus = true;
                        gcParcial.OptionsColumn.AllowEdit = false;
                        gcVelocidad.OptionsColumn.AllowEdit = true;
                        gcVelocidad.OptionsColumn.AllowFocus = true;
                        gcTiempoParcial.OptionsColumn.AllowEdit = false;
                        gcTiempoAcum.OptionsColumn.AllowEdit = true;
                        gcTiempoAcum.OptionsColumn.AllowFocus = true;

                        break;
                    default:
                        break;
                }

        } // End de SelectTipo

    } // End de Class
}
