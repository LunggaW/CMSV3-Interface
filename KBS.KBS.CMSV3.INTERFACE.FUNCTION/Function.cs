using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using KBS.KBS.CMSV3.INTERFACE.DATAMODEL;
using NLog;
using Oracle.DataAccess.Client;

namespace KBS.KBS.CMSV3.INTERFACE.FUNCTION
{
    public class Function
    {
        private String ConnectionStringOracle = ConfigurationManager.AppSettings["ConnectionStringOracle"];
        private String ConnectionStringOracleLocal = ConfigurationManager.AppSettings["ConnectionStringOracleLocal"];
        private String ConnectionStringSqlServer = ConfigurationManager.AppSettings["ConnectionStringSqlServer"];
        private String ConnectionStringSqlServerLocal = ConfigurationManager.AppSettings["ConnectionStringSqlServerLocal"];

        private String CSVLocationOracle;

        private Int32 IntervalDay = Int32.Parse(ConfigurationManager.AppSettings["IntervalDay"]);
        private Int32 IntervalHour = Int32.Parse(ConfigurationManager.AppSettings["IntervalHour"]);
        private Int32 IntervalMinute = Int32.Parse(ConfigurationManager.AppSettings["IntervalMinute"]);
        private Int32 IntervalSecond = Int32.Parse(ConfigurationManager.AppSettings["IntervalSecond"]);


        private static Logger logger = LogManager.GetCurrentClassLogger();
        private String ErrorString;

        private OracleConnection con;
        private SqlConnection conSql;


        /// <summary>
        /// Connects this instance.
        /// </summary>
        public void ConnectOracle()
        {
            try
            {
                logger.Trace("Start Starting Connection Server");
                con = new OracleConnection();
                con.ConnectionString = ConnectionStringOracle;
                logger.Debug("Connection String : " + con.ConnectionString.ToString());
                con.Open();
                logger.Debug("End Starting Connection Server");
            }
            catch (OracleException ex)
            {
                logger.Error("Connect Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        public void ConnectSQLServer()
        {
            try
            {
                logger.Trace("Start Starting Connection Server");
                conSql = new SqlConnection();
                conSql.ConnectionString = ConnectionStringSqlServer;
                logger.Debug("Connection String : " + conSql.ConnectionString.ToString());
                conSql.Open();
                logger.Debug("End Starting Connection Server");
            }
            catch (OracleException ex)
            {
                logger.Error("ConnectSQLServer Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        public void ConnectOracleLocal()
        {
            try
            {
                logger.Trace("Start Starting Connection Local");
                con = new OracleConnection();
                con.ConnectionString = ConnectionStringOracleLocal;
                logger.Debug("Connection String : " + con.ConnectionString.ToString());
                con.Open();
                logger.Debug("End Starting Connection Local");
            }
            catch (OracleException ex)
            {
                logger.Error("Connect Local Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void CloseOracle()
        {
            try
            {
                logger.Debug("Closing Connection");
                con.Close();
                con.Dispose();
                logger.Debug("End Close Connection");
            }
            catch (Exception e)
            {
                logger.Error("Close Function");
                logger.Error(e.Message);
            }

        }

        public void CloseSqlServer()
        {
            try
            {
                logger.Debug("Closing Connection");
                conSql.Close();
                conSql.Dispose();
                logger.Debug("End Close Connection");
            }
            catch (Exception e)
            {
                logger.Error("Close Function");
                logger.Error(e.Message);
            }

        }


        public String getCSVLocationOracle()
        {
            try
            {
                return ConfigurationManager.AppSettings["CSVLocationOracle"];
            }
            catch (OracleException ex)
            {
                logger.Error("getCSVLocationOracle Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        public Int32 ConvertDaytoSeconds()
        {
            try
            {
                return IntervalDay * 24 * 60 * 60;
            }
            catch (OracleException ex)
            {
                logger.Error("ConvertDaytoSeconds Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        public Int32 ConvertHourtoSeconds()
        {
            try
            {
                return IntervalHour*60*60;
            }
            catch (OracleException ex)
            {
                logger.Error("ConvertHourtoSeconds Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        public Int32 ConvertMinutestoSeconds()
        {
            try
            {
                return IntervalMinute*60;
            }
            catch (OracleException ex)
            {
                logger.Error("ConvertMinutestoSeconds Function");
                logger.Error(ex.Message);
                throw;
            }
        }



        public Int32 GetIntervalinSeconds()
        {
            try
            {
               return ConvertDaytoSeconds()+ ConvertHourtoSeconds()+ ConvertMinutestoSeconds()+ IntervalSecond;
            }
            catch (OracleException ex)
            {
                logger.Error("GetInterval Function");
                logger.Error(ex.Message);
                throw;
            }
        }

        public bool GetIsDaily()
        {
            try
            {
                return bool.Parse(ConfigurationManager.AppSettings["isDaily"]);
            }
            catch (OracleException ex)
            {
                logger.Error("GetIsDaily function");
                logger.Error(ex.Message);
                logger.Error("Error getting isDaily Boolean, returning false as default");
                return false;
            }
        }

        public bool GetIsMonthly()
        {
            try
            {
                return bool.Parse(ConfigurationManager.AppSettings["isMonthly"]);
            }
            catch (OracleException ex)
            {
                logger.Error("GetIsMonthly function");
                logger.Error(ex.Message);
                logger.Error("Error getting GetIsMonthly Boolean, returning false as default");
                return false;
            }
        }

        public bool GetIsWeekly()
        {
            try
            {
                return bool.Parse(ConfigurationManager.AppSettings["isWeekly"]);
            }
            catch (OracleException ex)
            {
                logger.Error("GetIsWeekly function");
                logger.Error(ex.Message);
                logger.Error("Error getting GetIsWeekly Boolean, returning false as default");
                return false;
            }
        }

        public DayOfWeek GetDayofWeek()
        {
            try
            {
                return (DayOfWeek)Enum.Parse(typeof(DayOfWeek), ConfigurationManager.AppSettings["DayOfWeek"], true);
            }
            catch (OracleException ex)
            {
                logger.Error("GetDayofWeek function");
                logger.Error(ex.Message);
                logger.Error("Error getting GetDayofWeek Boolean, returning monday as default");
                return DayOfWeek.Monday;
            }
        }
        public int GetDayofMonth()
        {
            try
            {
                return Int32.Parse(ConfigurationManager.AppSettings["DayOfMonth"]);
            }
            catch (OracleException ex)
            {
                logger.Error("GetDayofMonth function");
                logger.Error(ex.Message);
                logger.Error("Error getting GetDayofMonth Boolean, returning 1 as default");
                return 1;
            }
        }

        public Int32 GetHour()
        {
            try
            {
                return Int32.Parse(ConfigurationManager.AppSettings["StartHour"]);
            }
            catch (OracleException ex)
            {
                logger.Error("GetHour Function");
                logger.Error(ex.Message);
                logger.Error("Error getting hour, returning 12 as default");
                return 12;
            }
        }

        public Int32 GetMinutes()
        {
            try
            {
                return Int32.Parse(ConfigurationManager.AppSettings["StartMinutes"]);
            }
            catch (OracleException ex)
            {
                logger.Error("GetHour Function");
                logger.Error(ex.Message);
                logger.Error("Error getting minutes, returning 0 as default");
                return 0;
            }
        }

        public DataTable SelectSales()
        {
            try
            {
                this.ConnectOracle();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT CMSSALNOTA, " +
                                  "CMSSALBRCD, " +
                                  "CMSSALQTY, " +
                                  "CMSSALSKU, " +
                                  "CMSSALFLAG, " +
                                  "CMSSALSTAT, " +
                                  "CMSSALCOMM, " +
                                  "CMSSALCDAT, " +
                                  "CMSSALMDAT " +
                                  "FROM KDSCMSSALES_INT";
                cmd.CommandType = CommandType.Text;

                logger.Debug(cmd.CommandText);

                OracleDataReader dr = cmd.ExecuteReader();


                DataTable DT = new DataTable();
                DT.Load(dr);
                this.CloseOracle();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectSales Function");
                logger.Error(e.Message);
                this.CloseOracle();
                return null;
            }

        }

        public DataTable SelectSalesSql()
        {
            try
            {
                this.ConnectSQLServer();
                SqlCommand cmdSql = new SqlCommand();


                cmdSql.Connection = conSql;
                cmdSql.CommandText = "SELECT * " +
                                  "FROM ITEM_MST";
                cmdSql.CommandType = CommandType.Text;

                logger.Debug(cmdSql.CommandText);

                SqlDataReader dr = cmdSql.ExecuteReader();


                DataTable DT = new DataTable();
                DT.Load(dr);
                this.CloseSqlServer();
                return DT;
            }
            catch (Exception e)
            {
                logger.Error("SelectSalesSql Function");
                logger.Error(e.Message);
                this.CloseSqlServer();
                return null;
            }

        }

        public string ExecExportDeliveryNote()
        {
            String ErrorString;
            try
            {
                this.ConnectSQLServer();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conSql;
                cmd.CommandText = "KDS_EXPORT_DELIVERY_NOTE";


                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                this.CloseSqlServer();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("ExecExportDeliveryNote Function");
                logger.Error(e.Message);
                this.CloseSqlServer();
                return null;
            }
        }

        public string ExecExportItemMaster()
        {
            String ErrorString;
            try
            {
                this.ConnectSQLServer();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conSql;
                cmd.CommandText = "KDS_EXPORT_ITEM_MASTER";


                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                this.CloseSqlServer();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("ExecExportItemMaster Function");
                logger.Error(e.Message);
                this.CloseSqlServer();
                return null;
            }
        }

        public string ExecExportBarcodeMaster()
        {
            String ErrorString;
            try
            {
                this.ConnectSQLServer();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conSql;
                cmd.CommandText = "KDS_EXPORT_BARCODE_MASTER";


                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                this.CloseSqlServer();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("ExecExportBarcodeMaster Function");
                logger.Error(e.Message);
                this.CloseSqlServer();
                return null;
            }
        }

        public string ExecExportSalesPriceMaster()
        {
            String ErrorString;
            try
            {
                this.ConnectSQLServer();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conSql;
                cmd.CommandText = "KDS_EXPORT_SALES_PRICE_MASTER";


                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                this.CloseSqlServer();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("ExecExportSalesPriceMaster Function");
                logger.Error(e.Message);
                this.CloseSqlServer();
                return null;
            }
        }

        public string ExecExportStoreMaster()
        {
            String ErrorString;
            try
            {
                this.ConnectSQLServer();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conSql;
                cmd.CommandText = "KDS_EXPORT_STORE_MASTER";


                logger.Debug(cmd.CommandText);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                this.CloseSqlServer();
                ErrorString = "Success";
                return ErrorString;
            }
            catch (Exception e)
            {
                logger.Error("ExecExportStoreMaster Function");
                logger.Error(e.Message);
                this.CloseSqlServer();
                return null;
            }
        }


        public string Encrypt(string clearText)
        {
            string EncryptionKey = "PT.KDSBS";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "PT.KDSBS";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public License ParseLicenseText(string DecryptedText)
        {
            string[] values = DecryptedText.Split("|".ToCharArray());

            License license = new License();

            license.CompanyName = values[0];
            license.StoreTotal = values[1];
            license.Val1 = values[3];
            license.Val2 = values[4];
            license.Val3 = values[5];
            license.EndDate = DateTime.Parse(values[2]);

            return license;
        }

        public string GetLicense()
        {
            logger.Debug("Start Connect");
            this.ConnectOracle();
            logger.Debug("End Connect");
            try
            {
                String Value = "";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "select KDSCMSDLDESC from KDSCMSDL where KDSCMSDLID = 1";

                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                OracleDataReader dr = cmd.ExecuteReader();
                //OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");

                while (dr.Read())
                {
                    Value = dr["KDSCMSDLDESC"].ToString();
                }
                logger.Debug("Start Close Connection");
                this.CloseOracle();
                logger.Debug("End Close Connection");
                return Value;
            }
            catch (Exception e)
            {
                logger.Error("GetLicense");
                logger.Error(e.Message);
                this.CloseOracle();
                return null;
            }
        }

        public string DisableAllStoreFlagandStatus()
        {
            logger.Debug("Start Connect");
            this.ConnectOracle();
            logger.Debug("End Connect");
            try
            {
                String Value = "";
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = con;
                cmd.CommandText = "update kdscmssite " +
                                  "set sitesiteflag = 0 " +
                                  ", SITESITESTATUS = 0 ";


                logger.Debug("Execute Command");
                logger.Debug(cmd.CommandText.ToString());

                cmd.ExecuteNonQuery();
                //OracleDataReader dr = cmd.ExecuteReader();
                logger.Debug("End Execute Command");


                logger.Debug("Start Close Connection");
                this.CloseOracle();
                logger.Debug("End Close Connection");
                Value = Decrypt(Value);
                return Value;
            }
            catch (Exception e)
            {
                logger.Error("DisableAllStoreFlagandStatus");
                logger.Error(e.Message);
                this.CloseOracle();
                return null;
            }


        }

        public string ValidateLicenseEndDate(DateTime EndDate)
        {
            if (EndDate > DateTime.Now)
            {
                logger.Debug("Start Connect");
                this.ConnectOracle();
                logger.Debug("End Connect");
                try
                {
                    String Value = "";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "update kdscmssite set SITESITESTATUS = 1";

                    logger.Debug("Execute Command");
                    logger.Debug(cmd.CommandText.ToString());

                    cmd.ExecuteNonQuery();
                    //OracleDataReader dr = cmd.ExecuteReader();
                    logger.Debug("End Execute Command");


                    logger.Debug("Start Close Connection");
                    this.CloseOracle();
                    logger.Debug("End Close Connection");
                    Value = Decrypt(Value);
                    return Value;
                }
                catch (Exception e)
                {
                    logger.Error("ValidateLicenseEndDate");
                    logger.Error(e.Message);
                    this.CloseOracle();
                    return null;
                }
            }
            else
            {
                return "expired";
            }
            
        }

        public string ValidateLicenseStore(int StoreTotal)
        {
                logger.Debug("Start Connect");
                this.ConnectOracle();
                logger.Debug("End Connect");
                try
                {
                    String Value = "";
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = con;
                    cmd.CommandText = "update kdscmssite " +
                                      "set sitesiteflag = 1 " +
                                      "where  rownum <= :TotalStore ";

                    cmd.Parameters.Add(new OracleParameter(":TotalStore", OracleDbType.Varchar2)).Value = StoreTotal;

                    logger.Debug("Execute Command");
                    logger.Debug(cmd.CommandText.ToString());

                    cmd.ExecuteNonQuery();
                    //OracleDataReader dr = cmd.ExecuteReader();
                    logger.Debug("End Execute Command");


                    logger.Debug("Start Close Connection");
                    this.CloseOracle();
                    logger.Debug("End Close Connection");
                    Value = Decrypt(Value);
                    return Value;
                }
                catch (Exception e)
                {
                    logger.Error("GetLicense");
                    logger.Error(e.Message);
                    this.CloseOracle();
                    return null;
                }
            

        }
    }
}
