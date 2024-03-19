using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;


namespace ZeroTrip
{
    // Define a custom section.
    public sealed class DatosGeneralesSection :  ConfigurationSection
    {
        
        public enum Permissions
        {
            FullControl = 0,
            Modify = 1,
            ReadExecute = 2,
            Read = 3,
            Write = 4,
            SpecialPermissions = 5
        }

        public DatosGeneralesSection()
        {

        }

        [ConfigurationProperty("Sonido", DefaultValue = "Si")]
        // [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Sonido
        {
            get
            {
                return (String)this["Sonido"];
            }
            set
            {
                this["Sonido"] = value;
            }
        }

        [ConfigurationProperty("Log", DefaultValue = "Si")]
        // [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public String Log
        {
            get
            {
                return (String)this["Log"];
            }
            set
            {
                this["Log"] = value;
            }
        }

        [ConfigurationProperty("SonidoMetros", DefaultValue = @"No")]
        public String SonidoMetros
        {
            get
            {
                return (String)this["SonidoMetros"];
            }

            set
            {
                this["SonidoMetros"] = value;
            }

        }

        [ConfigurationProperty("DiaNoche", DefaultValue = @"Dia")]
        public String DiaNoche
        {
            get
            {
                return (String)this["DiaNoche"];
            }

            set
            {
                this["DiaNoche"] = value;
            }

        }

        [ConfigurationProperty("AvisoCM", DefaultValue = @"75")]
        public string AvisoCM
        {
            get
            {
                return (string)this["AvisoCM"];
            }

            set
            {
                this["AvisoCM"] = value;
            }

        }

        [ConfigurationProperty("AvisoCruces", DefaultValue = @"200")]
        public string AvisoCruces
        {
            get
            {
                return (string)this["AvisoCruces"];
            }

            set
            {
                this["AvisoCruces"] = value;
            }

        }

        [ConfigurationProperty("MaxCorreccion", DefaultValue = @"100")]
        public string MaxCorreccion
        {
            get
            {
                return (string)this["MaxCorreccion"];
            }

            set
            {
                this["MaxCorreccion"] = value;
            }

        }

        [ConfigurationProperty("DistanciaHitos", DefaultValue = @"1000")]
        public string DistanciaHitos
        {
            get
            {
                return (string)this["DistanciaHitos"];
            }

            set
            {
                this["DistanciaHitos"] = value;
            }

        }

        [ConfigurationProperty("DistanciaTablas", DefaultValue = @"100")]
        public string DistanciaTablas
        {
            get
            {
                return (string)this["DistanciaTablas"];
            }

            set
            {
                this["DistanciaTablas"] = value;
            }

        }


    }
 
//###################################################################

  #region Declaraciones para Datos de calibracion

    public sealed class CalibracionSection : ConfigurationSection
  {

      public enum Permissions
      {
          FullControl = 0,
          Modify = 1,
          ReadExecute = 2,
          Read = 3,
          Write = 4,
          SpecialPermissions = 5
      }

      public CalibracionSection()
      {

      }

      [ConfigurationProperty("Cal1", DefaultValue = 1950.0)]
      public double Cal1
      {
          get
          {
              return (double)this["Cal1"];
          }

          set
          {
              this["Cal1"] = value;
          }

      }

      [ConfigurationProperty("SelCal1", DefaultValue = "No")]
      public string SelCal1
      {
          get
          {
              return (string)this["SelCal1"];
          }

          set
          {
              this["SelCal1"] = value;
          }

      }

      [ConfigurationProperty("Cal2", DefaultValue = 1950.0)]
      public double Cal2
      {
          get
          {
              return (double)this["Cal2"];
          }

          set
          {
              this["Cal2"] = value;
          }

      }


      [ConfigurationProperty("SelCal2", DefaultValue = "No")]     
        public string SelCal2
      {
          get
          {
              return (string)this["SelCal2"];
          }

          set
          {
              this["SelCal2"] = value;
          }

      }
      
       [ConfigurationProperty("Cal3", DefaultValue = 195.0)]
        public double Cal3
      {
          get
          {
              return (double)this["Cal3"];
          }

          set
          {
              this["Cal3"] = value;
          }

      }

        [ConfigurationProperty("SelCal3", DefaultValue = "No")]
        public string SelCal3
        {
            get
            {
                return (string)this["SelCal3"];
            }

            set
            {
                this["SelCal3"] = value;
            }

        }
     
        [ConfigurationProperty("CalMopu", DefaultValue = 888.0)]
        public double CalMopu
        {
            get
            {
                return (double)this["CalMopu"];
            }

            set
            {
                this["CalMopu"] = value;
            }

        }

       [ConfigurationProperty("TipoMedidor", DefaultValue = @"Biciclometro")]
        public String TipoMedidor
      {
          get
          {
              return (String)this["TipoMedidor"];
          }

          set
          {
              this["TipoMedidor"] = value;
          }

      }
        
       [ConfigurationProperty("DecaMetros", DefaultValue = @"Decametros")]
        public string DecaMetros
        {
            get
            {
                return (string)this["DecaMetros"];
            }

            set
            {
                this["DecaMetros"] = value;
            }

        }
 
       [ConfigurationProperty("TamanioRueda", DefaultValue = @"L")]
        public string TamanioRueda
        {
            get
            {
                return (string)this["TamanioRueda"];
            }

            set
            {
                this["TamanioRueda"] = value;
            }

        }

    }

  #endregion Declaraciones para para Datos de calibracion

    //###################################################################

  #region Declaraciones para Categorias Especiales
/*
  public sealed class EspecialesSection : ConfigurationSection
  {

      public enum Permissions
      {
          FullControl = 0,
          Modify = 1,
          ReadExecute = 2,
          Read = 3,
          Write = 4,
          SpecialPermissions = 5
      }

      public EspecialesSection()
      {

      }

      [ConfigurationProperty("AbrEspecial1", DefaultValue = @"---")]
      public string AbrEspecial1
      {
          get
          {
              return (String)this["AbrEspecial1"];
          }

          set
          {
              this["AbrEspecial1"] = value;
          }

      }

      [ConfigurationProperty("HayEspecial1", DefaultValue = @"No")]
      public string HayEspecial1
      {
          get
          {
              return (String)this["HayEspecial1"];
          }

          set
          {
              this["HayEspecial1"] = value;
          }

      }

      [ConfigurationProperty("AbrEspecial2", DefaultValue = @"---")]
      public string AbrEspecial2
      {
          get
          {
              return (String)this["AbrEspecial2"];
          }

          set
          {
              this["AbrEspecial2"] = value;
          }

      }

      [ConfigurationProperty("HayEspecial2", DefaultValue = @"No")]
      public string HayEspecial2
      {
          get
          {
              return (String)this["HayEspecial2"];
          }

          set
          {
              this["HayEspecial2"] = value;
          }

      }

      [ConfigurationProperty("AbrEspecial3", DefaultValue = @"---")]
      public string AbrEspecial3
      {
          get
          {
              return (String)this["AbrEspecial3"];
          }

          set
          {
              this["AbrEspecial3"] = value;
          }

      }

      [ConfigurationProperty("HayEspecial3", DefaultValue = @"No")]
      public string HayEspecial3
      {
          get
          {
              return (String)this["HayEspecial3"];
          }

          set
          {
              this["HayEspecial3"] = value;
          }

      }

  }
    */
  #endregion Declaraciones para Categorias Especiales

//########################################################################
  
    
    public class GestionConfig
    {
    
        ///Propiedades
        ///

        private Configuration configFile;

        private string szDatosGenerales = "DatosGenerales";
        private ConfigurationSection seccionDatosGenerales;

        private string szSecCalibracion = "Calibracion";
        private ConfigurationSection seccionCalibracion;
/*
        private string szSecSMS = "SMS";
        private ConfigurationSection seccionSMS;

        private string szSectores = "Sectores";
        private ConfigurationSection seccionSectores;

        private string szEspeciales = "Especiales";
        private ConfigurationSection seccionEspeciales;
*/
//-----------------------------------------------------------------------

        public GestionConfig()
        { }

        public GestionConfig(string ficheroConfiguracion)
        { 
            ///Constructor.
            ///En ficheroConfiguracion tenemos el nombre del fichero de configuración en formato XML.

            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = @ficheroConfiguracion;  // relative path names possible

            // Abrimos el fichero de configuración. 
            configFile = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (configFile.Sections[szDatosGenerales] == null)
            {
                // Si no existe lo creamos con sus valores por defecto.
                DatosGeneralesSection DGSection = new DatosGeneralesSection();
                configFile.Sections.Add(szDatosGenerales, DGSection);

                DGSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

                DGSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Full);
                
            }

            if (configFile.Sections[szSecCalibracion] == null)
            {
                // Si no existe lo creamos con sus valores por defecto.
                CalibracionSection calSection = new CalibracionSection();
                configFile.Sections.Add(szSecCalibracion, calSection);

                calSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;


                calSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Full);
            }

#region  Comentarios
/*
            if (configFile.Sections[szSecPuntuaciones] == null)
            {
                // Si no existe lo creamos con sus valores por defecto.
                PuntuacionesSection custSection = new PuntuacionesSection();
                configFile.Sections.Add(szSecPuntuaciones, custSection);

                custSection = configFile.GetSection(szSecPuntuaciones) as PuntuacionesSection;

                custSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Full);
            }

            if (configFile.Sections[szSecSMS] == null)
            {
                // Si no existe lo creamos con sus valores por defecto.
                SMSSection custSection = new SMSSection();
                configFile.Sections.Add(szSecSMS, custSection);

                custSection = configFile.GetSection(szSecSMS) as SMSSection;

                custSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Full);
            }

            if (configFile.Sections[szSectores] == null)
            {
                // Si no existe lo creamos con sus valores por defecto.
                SectoresSection custSection = new SectoresSection();
                configFile.Sections.Add(szSectores, custSection);

                custSection = configFile.GetSection(szSectores) as SectoresSection;

                custSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Full);
            }

            if (configFile.Sections[szEspeciales] == null)
            {
                // Si no existe lo creamos con sus valores por defecto.
                EspecialesSection custSection = new EspecialesSection();
                configFile.Sections.Add(szEspeciales, custSection);

                custSection = configFile.GetSection(szEspeciales) as EspecialesSection;

                custSection.SectionInformation.ForceSave = true;
                configFile.Save(ConfigurationSaveMode.Full);
            }
            //
*/
  #endregion  Comentarios



            seccionDatosGenerales = configFile.GetSection(szDatosGenerales);
            //string a = ((ZeroTrip.CustomSection)(seccionDatosGenerales)).FicheroDatos;
            configFile.SectionGroups.Clear(); // make changes to it 
            configFile.Save(ConfigurationSaveMode.Full);  // Save changes

            seccionCalibracion = configFile.GetSection(szSecCalibracion);
            //string b = ((ZeroTrip.CustomSection)(szSecDescartes)).AplicarDescartes;
            configFile.SectionGroups.Clear(); // make changes to it 
            configFile.Save(ConfigurationSaveMode.Full);  // Save changes
/*
            seccionPuntuaciones = configFile.GetSection(szSecPuntuaciones);
            //string b = ((ZeroTrip.PuntuacionesSection)(seccionPuntuaciones)).EnSegundos.ToString();
            configFile.SectionGroups.Clear(); // make changes to it 
            configFile.Save(ConfigurationSaveMode.Full);  // Save changes

            seccionSMS = configFile.GetSection(szSecSMS);
            //string b = ((ZeroTrip.PuntuacionesSection)(seccionPuntuaciones)).EnSegundos.ToString();
            configFile.SectionGroups.Clear(); // make changes to it 
            configFile.Save(ConfigurationSaveMode.Full);  // Save changes

            seccionSectores = configFile.GetSection(szSectores);
            //string b = ((ZeroTrip.PuntuacionesSection)(seccionPuntuaciones)).EnSegundos.ToString();
            configFile.SectionGroups.Clear(); // make changes to it 
            configFile.Save(ConfigurationSaveMode.Full);  // Save changes

            seccionEspeciales = configFile.GetSection(szEspeciales);
            //string b = ((ZeroTrip.PuntuacionesSection)(seccionPuntuaciones)).EnSegundos.ToString();
            configFile.SectionGroups.Clear(); // make changes to it 
            configFile.Save(ConfigurationSaveMode.Full);  // Save changes
*/
        }

        public bool GetSonido()
        {
            if (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).Sonido == "Si")
                return (true);
            else
                return (false);

        }

        public void SetSonido(bool bSonido)
        {
            DatosGeneralesSection dgSection = new DatosGeneralesSection();

            dgSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            if (bSonido)
                dgSection.Sonido = @"Si";
            else
                dgSection.Sonido = @"No";


            dgSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public bool GetLog()
        {
            if (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).Log == "Si")
                return (true);
            else
                return (false);

        }

        public void SetLog(bool bLog)
        {
            DatosGeneralesSection dgSection = new DatosGeneralesSection();

            dgSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            if (bLog)
                dgSection.Log = @"Si";
            else
                dgSection.Log = @"No";


            dgSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public bool GetSonidoMetros()
        {
            if (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).SonidoMetros == "Si")
                return (true);
            else
                return (false);

        }
     
        public void SetSonidoMetros(bool bSonidoMetros)
        {
            DatosGeneralesSection dgSection = new DatosGeneralesSection();

            dgSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            if (bSonidoMetros)
                dgSection.SonidoMetros = @"Si";
            else
                dgSection.SonidoMetros = @"No";


            dgSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public void updateConfig(bool bSonidoMetros)
      {
          Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

          DatosGeneralesSection section = (DatosGeneralesSection)config.Sections["szDatosGenerales"];
          //section.SonidoMetros = value;
          if (bSonidoMetros)
              section.SonidoMetros = @"Si";
          else
              section.SonidoMetros = @"No";
          config.Save();

      }


        public string GetDiaNoche()
        {
            if (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).DiaNoche == "Dia")
                return ("Dia");
            else
                return ("Noche");

        }

        public void SetDiaNoche(string  stDiaNoche)
        {
            DatosGeneralesSection dgSection = new DatosGeneralesSection();

            dgSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            if (stDiaNoche == "Dia")
                dgSection.DiaNoche = @"Dia";
            else
                dgSection.DiaNoche = @"Noche";


            dgSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public void updateConfig(string stDiaNoche)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            DatosGeneralesSection section = (DatosGeneralesSection)config.Sections["szDatosGenerales"];
            //section.SonidoMetros = value;
            if (stDiaNoche == "Dia")
                section.SonidoMetros = @"Dia";
            else
                section.SonidoMetros = @"Noche";
            config.Save();

        }
        public string GetAvisoCM()
        {

            return (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).AvisoCM);

        }

        public void SetAvisoCM(string szAvisoCM)
        {
            DatosGeneralesSection custSection = new DatosGeneralesSection();

            custSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            custSection.AvisoCM = szAvisoCM;

            custSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public string GetAvisoCruces()
        {

            return (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).AvisoCruces);

        }

        public void SetAvisoCruces(string szAvisoCruces)
        {
            DatosGeneralesSection custSection = new DatosGeneralesSection();

            custSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            custSection.AvisoCruces = szAvisoCruces;

            custSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public string GetMaxCorreccion()
        {

            return (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).MaxCorreccion);

        }

        public void SetMaxCorreccion(string szMaxCorreccion)
        {
            DatosGeneralesSection custSection = new DatosGeneralesSection();

            custSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            custSection.MaxCorreccion = szMaxCorreccion;

            custSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public string GetDistanciaHitos()
        {

            return (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).DistanciaHitos);

        }

        public void SetDistanciaHitos(string szDistanciaHitos)
        {
            DatosGeneralesSection custSection = new DatosGeneralesSection();

            custSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            custSection.DistanciaHitos = szDistanciaHitos;

            custSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public string GetDistanciaTablas()
        {

            return (((ZeroTrip.DatosGeneralesSection)(seccionDatosGenerales)).DistanciaTablas);

        }

        public void SetDistanciaTablas(string szDistanciaTablas)
        {
            DatosGeneralesSection custSection = new DatosGeneralesSection();

            custSection = configFile.GetSection(szDatosGenerales) as DatosGeneralesSection;

            custSection.DistanciaTablas = szDistanciaTablas;

            custSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }


        


        //-----------------------       CALIBRACION -------------------

        public double GetCal1()
        {

            return (((ZeroTrip.CalibracionSection)(seccionCalibracion)).Cal1);

        }

        public void SetCal1(double nCal1)
        {
            CalibracionSection custSection = new CalibracionSection();

            custSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            custSection.Cal1 = nCal1;

            custSection.SectionInformation.ForceSave = false;
            configFile.Save(ConfigurationSaveMode.Full, true);

        }

        public bool GetSelCal1()
        {
            if (((ZeroTrip.CalibracionSection)(seccionCalibracion)).SelCal1 == "Si")
                return (true);
            else
                return (false);

        }

        public void SetSelCal1(bool bSelCal1)
        {
            CalibracionSection dgSection = new CalibracionSection();

            dgSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            if (bSelCal1)
                dgSection.SelCal1 = @"Si";
            else
                dgSection.SelCal1 = @"No";


            dgSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public double GetCal2()
        {

            return (((ZeroTrip.CalibracionSection)(seccionCalibracion)).Cal2);

        }

        public void SetCal2(double nCal2)
        {
            CalibracionSection custSection = new CalibracionSection();

            custSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            custSection.Cal2 = nCal2;

            custSection.SectionInformation.ForceSave = false;
            configFile.Save(ConfigurationSaveMode.Full);

        }


        public bool GetSelCal2()
        {
            if (((ZeroTrip.CalibracionSection)(seccionCalibracion)).SelCal2 == "Si")
                return (true);
            else
                return (false);

        }

        public void SetSelCal2(bool bSelCal2)
        {
            CalibracionSection dgSection = new CalibracionSection();

            dgSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            if (bSelCal2)
                dgSection.SelCal2 = @"Si";
            else
                dgSection.SelCal2 = @"No";

            dgSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }
        public double GetCal3()
        {

            return (((ZeroTrip.CalibracionSection)(seccionCalibracion)).Cal3);

        }

        public void SetCal3(double nCal3)
        {
            CalibracionSection custSection = new CalibracionSection();

            custSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            custSection.Cal3 = nCal3;

            custSection.SectionInformation.ForceSave = false;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public bool GetSelCal3()
        {
            if (((ZeroTrip.CalibracionSection)(seccionCalibracion)).SelCal3 == "Si")
                return (true);
            else
                return (false);

        }

        public void SetSelCal3(bool bSelCal3)
        {
            CalibracionSection dgSection = new CalibracionSection();

            dgSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            if (bSelCal3)
                dgSection.SelCal3 = @"Si";
            else
                dgSection.SelCal3 = @"No";

            dgSection.SectionInformation.ForceSave = true;
            configFile.Save(ConfigurationSaveMode.Full);

        }
        public double GetCalMopu()
        {

            return (((ZeroTrip.CalibracionSection)(seccionCalibracion)).CalMopu);

        }

        public void SetCalMopu(double nCalMopu)
        {
            CalibracionSection custSection = new CalibracionSection();

            custSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            custSection.CalMopu = nCalMopu;

            custSection.SectionInformation.ForceSave = false;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public string GetTipoMedidor()
        {

            return (((ZeroTrip.CalibracionSection)(seccionCalibracion)).TipoMedidor);

        }

        public void SetTipoMedidor(string szTipoMedidor)
        {
            CalibracionSection custSection = new CalibracionSection();

            custSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            custSection.TipoMedidor = szTipoMedidor;

            custSection.SectionInformation.ForceSave = false;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public string GetDecaMetros()
        {

            return (((ZeroTrip.CalibracionSection)(seccionCalibracion)).DecaMetros);

        }

        public void SetDecaMetros(string szDecaMetros)
        {
            CalibracionSection custSection = new CalibracionSection();

            custSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            custSection.DecaMetros = szDecaMetros;

            custSection.SectionInformation.ForceSave = false;
            configFile.Save(ConfigurationSaveMode.Full);

        }

        public string GetTamanioRueda()
        {

            return (((ZeroTrip.CalibracionSection)(seccionCalibracion)).TamanioRueda);

        }

        public void SetTamanioRueda(string TamanioRueda)
        {
            CalibracionSection custSection = new CalibracionSection();

            custSection = configFile.GetSection(szSecCalibracion) as CalibracionSection;

            custSection.TamanioRueda = TamanioRueda;

            custSection.SectionInformation.ForceSave = false;
            configFile.Save(ConfigurationSaveMode.Full);

        }

    } // End Class
}
