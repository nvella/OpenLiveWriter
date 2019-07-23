﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenLiveWriter.ApplicationFramework.Preferences;
using OpenLiveWriter.BlogClient;
using OpenLiveWriter.BlogClient.Clients.StaticSite;
using OpenLiveWriter.Localization;
using OpenLiveWriter.PostEditor.Configuration.Wizard;

namespace OpenLiveWriter.PostEditor.Configuration.StaticSiteAdvanced
{
    public class PreferencesController
    {
        private TemporaryBlogSettings _temporarySettings;
        private PreferencesForm _form;

        private GeneralPanel panelGeneral;

        public PreferencesController(TemporaryBlogSettings blogSettings)
        {
            _temporarySettings = blogSettings;
            _form = new PreferencesForm();

            panelGeneral = new GeneralPanel(this);
        }

        private bool EditWeblogTemporarySettings(IWin32Window owner)
        {
            LoadFromStaticSiteConfig(CreateSiteConfig());

            // Show form
            using (var preferencesForm = new StaticSitePreferencesForm())
            {
                using (BlogClientUIContextScope uiContextScope = new BlogClientUIContextScope(preferencesForm))
                {
                    // Customize form title and behavior
                    preferencesForm.Text = $"Static Site Configuration for '{_temporarySettings.BlogName}'"; //String.Format(CultureInfo.CurrentCulture, Res.Get(StringId.WeblogSettings), blogSettings.BlogName);

                    // Add panels
                    int iPanel = 0;
                    preferencesForm.SetEntry(iPanel++, panelGeneral);

                    preferencesForm.SelectedIndex = 0;
                    
                    // Show the dialog
                    preferencesForm.ShowDialog(owner);
                }
            }

            return false;
        }

        private StaticSiteConfig CreateSiteConfig()
        {
            var ssgConfig = new StaticSiteConfig();
            ssgConfig.LoadFromBlogSettings(_temporarySettings);
            return ssgConfig;
        }

        private void LoadFromStaticSiteConfig(StaticSiteConfig ssgConfig)
        {
            // General
            panelGeneral.SiteTitle = ssgConfig.SiteTitle;
            panelGeneral.SiteUrl = ssgConfig.SiteUrl;
            panelGeneral.LocalSitePath = ssgConfig.LocalSitePath;
        }

        public void GeneralPanel_RunAccountWizard()
        {
            WeblogConfigurationWizardController.EditTemporarySettings(_form, _temporarySettings);
            // Reload the settings into the form
            LoadFromStaticSiteConfig(CreateSiteConfig());
        }

        public void GeneralPanel_RunAutoDetect()
        {
            var ssgConfig = CreateSiteConfig();
            var result = StaticSiteConfigDetector.AttmeptAutoDetect(ssgConfig);

            if(result)
            {
                // Successful detection of parameters
                MessageBox.Show(
                    string.Format(Res.Get(StringId.CWStaticSiteConfigDetection), Res.Get(StringId.ProductNameVersioned)),
                    Res.Get(StringId.ProductNameVersioned),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadFromStaticSiteConfig(ssgConfig);
            }
        }

        public static bool EditTemporarySettings(IWin32Window owner, TemporaryBlogSettings settings)
            => new PreferencesController(settings).EditWeblogTemporarySettings(owner);
    }
}