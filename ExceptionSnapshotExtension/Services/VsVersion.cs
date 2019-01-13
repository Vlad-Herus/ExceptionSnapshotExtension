using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionSnapshotExtension.Services
{
    // Credit : Daniel Peñalba https://stackoverflow.com/questions/11082436/detect-the-visual-studio-version-inside-a-vspackage

    public class VsVersionDetect
    {
        private Version m_VsVersion;

        public Version FullVersion
        {
            get
            {
                if (m_VsVersion == null)
                {
                    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msenv.dll");

                    if (File.Exists(path))
                    {
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(path);

                        string verName = fvi.ProductVersion;

                        for (int i = 0; i < verName.Length; i++)
                        {
                            if (!char.IsDigit(verName, i) && verName[i] != '.')
                            {
                                verName = verName.Substring(0, i);
                                break;
                            }
                        }

                        m_VsVersion = new Version(verName);
                    }
                    else
                    {
                        m_VsVersion = new Version(0, 0); // Not running inside Visual Studio!
                    }
                }

                return m_VsVersion;
            }
        }

        public bool VS2017OrLater => FullVersion >= new Version(15, 0);

        public bool VS2015 => FullVersion.Major == 14;
    }
}
