// --------------------------------------------------------------------------
// <copyright file="ProjectInstaller.Designer.cs" company=" Solutions Vietnam">
// Copyright (c)  Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Thai.Tran" date="2013.1.7">Created</change>
// </history>

namespace Fanex.OddsFeed.WindowsService
{
    using System.Diagnostics.CodeAnalysis;   

    /// <summary>
    /// Project Installer.
    /// </summary>    
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
                                                                                                Justification = "Reviewed")]
    public partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer _components = null;

        private System.ServiceProcess.ServiceProcessInstaller _oddsFeedServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller _oddsFeedServiceInstaller;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._oddsFeedServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this._oddsFeedServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // OddsFeedServiceProcessInstaller
            // 
            this._oddsFeedServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this._oddsFeedServiceProcessInstaller.Password = null;
            this._oddsFeedServiceProcessInstaller.Username = null;
            // 
            // OddsFeedServiceInstaller
            // 
            this._oddsFeedServiceInstaller.ServiceName = "OddsFeedWindowsService";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this._oddsFeedServiceProcessInstaller,
            this._oddsFeedServiceInstaller});

        }

        #endregion
    }
}