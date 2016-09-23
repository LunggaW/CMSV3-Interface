using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KBS.KBS.CMSV3.INTERFACE.FUNCTION;
using KBS.KBS.CMSV3.INTERFACE.DATAMODEL;

namespace Test
{
    public class Program
    {
        private static Function CMSV3Function = new Function();

        static void Main(string[] args)
        {

            LicenseCheck();
        }

        private static void LicenseCheck()
        {
            try
            {
                String licenseText;
                License license = new License();

                CMSV3Function.DisableAllStoreFlagandStatus();

                licenseText = CMSV3Function.GetLicense();
                licenseText = CMSV3Function.Decrypt(licenseText);

                license = CMSV3Function.ParseLicenseText(licenseText);

                CMSV3Function.ValidateLicenseEndDate(license.EndDate);
                CMSV3Function.ValidateLicenseStore(Int32.Parse(license.StoreTotal));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException);
                throw;
            }
        }
    }
}
