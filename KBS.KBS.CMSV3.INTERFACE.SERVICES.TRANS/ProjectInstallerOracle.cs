using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace KBS.KBS.CMSV3.INTERFACE.SERVICES.TRANS
{
    [RunInstaller(true)]
    public partial class ProjectInstallerOracle : System.Configuration.Install.Installer
    {
        public ProjectInstallerOracle()
        {
            InitializeComponent();
        }
    }
}
