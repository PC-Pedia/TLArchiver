﻿using TLArchiver.Core;

namespace TLArchiver.Exporter
{
    public class HtmlExporter : FileExporter, IExporter
    {
        private static readonly string c_sExporterDirectory = "Html";
        private static readonly string c_sMessagesFile = "Messages.html";

        public HtmlExporter(Config config, string sDirectory) : base(config)
        {
            m_sExporterDirectory = c_sExporterDirectory;
            m_sMessagesFile = c_sMessagesFile;
            Initialize(sDirectory);
        }
    }
}
