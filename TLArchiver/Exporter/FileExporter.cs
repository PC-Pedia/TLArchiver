﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TeleSharp.TL;
using TeleSharp.TL.Storage;
using TeleSharp.TL.Upload;
using TLArchiver.Core;
using TLArchiver.Entities;

namespace TLArchiver.Exporter
{
    public abstract class FileExporter : IExporter
    {
        protected const string c_sUnknownAuthor = "Unknown ({0})"; // Export the id of the unknonw author
        protected const string c_sPhoto = "Photo"; // Caption use for photo 'YYYY-MM-DD-PhotoXX.EXT'
        protected const string c_sVideo = "Video"; // Caption use for video 'YYYY-MM-DD-VideoXX.EXT'
        protected const int c_iInitialIndex = 1; // XX starts at 01

        protected Config m_config;
        protected TLAArchiver m_archiver;
        protected string m_sExporterDirectory; // Path of the exporter
        protected string m_sMessagesFile; // Path of the file to export messages of a dialog
        protected string m_sDialogDirectory; // Directory for a dialog
        protected string m_sAuthor; // The Author of a message
        protected string m_sPrefix; // The date of a message 'YYYY-MM-DD' used to prefix a file
        protected StringBuilder m_messages; // Used to write the dialog content
        protected Dictionary<string, int> m_fileNames; // Dictionary to maintain the unicity of each file of a dialog

        public FileExporter(Config config)
        {
            m_config = config;
            m_archiver = config.Archiver;
            m_messages = new StringBuilder();
            m_fileNames = new Dictionary<string, int>();
        }

        protected void Initialize(string directory)
        {
            m_sExporterDirectory = Path.Combine(directory, m_sExporterDirectory);
            Directory.CreateDirectory(m_sExporterDirectory);
        }

        public virtual void BeginDialogs(ICollection<TLADialog> dialogs)
        {

        }

        public virtual void BeginDialog(TLADialog dialog)
        {
            string sub = dialog.Title;
            if (dialog.Type != TLADialogType.User)
                sub += " - " + dialog.Type.ToString();
            m_sDialogDirectory = Path.Combine(m_sExporterDirectory, sub);
            Directory.CreateDirectory(m_sDialogDirectory);

            m_messages.Clear();
            m_fileNames.Clear(); // Reset as dialog directory changed
        }

        public virtual void BeginMessage(TLAbsMessage absMessage)
        {

        }

        public virtual void ExportMessage(TLMessage message)
        {

        }

        public virtual void ExportMessageService(TLMessageService message)
        {

        }

        public virtual void EndMessage(TLAbsMessage absMessage)
        {

        }

        public virtual void EndDialog(TLADialog dialog)
        {
            if (m_config.ExportMessages)
            {
                // Write the dialog content from the StringBuilder to a file
                string sFile = Path.Combine(m_sDialogDirectory, m_sMessagesFile);
                using (StreamWriter file = new StreamWriter(sFile))
                    file.Write(m_messages.ToString());
                m_messages.Clear();
            }
        }

        public virtual void EndDialogs(ICollection<TLADialog> m_dialogs)
        {

        }

        public virtual void Abort()
        {

        }

        protected void Prepend(string sMessage = "")
        {
            if (m_config.ExportMessages)
                // Insert message at the beginning as messages are received in a reverse order
                m_messages.Insert(0, sMessage + Environment.NewLine);
        }

        protected string GetAuthor(int? iAuthor)
        {
            if (iAuthor != null && m_config.Contacts.ContainsKey(iAuthor.Value))
                return m_config.Contacts[iAuthor.Value];
            else
                return String.Format(c_sUnknownAuthor, iAuthor);
        }

        protected string ExportPhoto(TLMessageMediaPhoto media)
        {
            // TLPhoto contains a collection of TLPhotoSize
            TLPhoto photo = media.photo as TLPhoto;
            if (photo == null)
                throw new TLCoreException("The photo is not an instance of TLPhoto");

            if (photo.sizes.lists.Count <= 0)
                throw new TLCoreException("TLPhoto does not have any element");

            // Pick max size photo from TLPhoto
            TLPhotoSize photoMaxSize = null;
            foreach (TLAbsPhotoSize absPhotoSize in photo.sizes.lists)
            {
                TLPhotoSize photoSize = absPhotoSize as TLPhotoSize;
                if (photoSize == null)
                    throw new TLCoreException("The photosize is not an instance of TLPhotoSize");
                if (photoMaxSize != null && photoSize.w < photoMaxSize.w)
                    continue;
                photoMaxSize = photoSize;
            }

            // a TLPhotoSize contains a TLFileLocation
            TLFileLocation fileLocation = photoMaxSize.location as TLFileLocation;
            if (fileLocation == null)
                throw new TLCoreException("The file location is not an instance of TLFileLocation");

            TLFile file = m_archiver.GetFile(fileLocation, photoMaxSize.size);

            string ext = "";
            if (file.type.GetType() == typeof(TLFileJpeg))
                ext = "jpg";
            else if (file.type.GetType() == typeof(TLFileGif))
                ext = "gif";
            else if (file.type.GetType() == typeof(TLFilePng))
                ext = "png";
            else
                throw new TLCoreException("The photo has an unknown file type");

            // Take a caption if exists or set the default one
            string sCaption = String.IsNullOrEmpty(media.caption) ? c_sPhoto : media.caption;

            // Key: YYYY-MM-DD-Caption{0:00}.EXT
            string key = String.Format("{0}-{1}{2}.{3}",
                m_sPrefix,
                sCaption,
                "{0:00}",
                ext);

            string sFileName = GetUniqueFileName(key); // YYYY-MM-DD-CaptionXX.EXT

            if (m_config.ExportPhotos)
            {
                // Export the photo to a file
                string sFullFileName = Path.Combine(m_sDialogDirectory, sFileName);
                using (FileStream f = new FileStream(sFullFileName, FileMode.Create, FileAccess.Write))
                    f.Write(file.bytes, 0, photoMaxSize.size);
            }

            return sFileName; // YYYY-MM-DD-CaptionXX.EXT
        }

        protected string ExportDocument(TLMessageMediaDocument media)
        {
            // TLPhoto contains a collection of TLPhotoSize
            TLDocument document = media.document as TLDocument;
            if (document == null)
                throw new TLCoreException("The document is not an instance of TLDocument");

            if (document.attributes.lists.Count <= 0)
                throw new TLCoreException("TLDocument does not have any attributes");

            TLDocumentAttributeFilename attr = document.attributes.lists[0] as TLDocumentAttributeFilename;
            if (attr == null)
                throw new TLCoreException("The TLDocumentAttributeFilename has not been found");
            if (String.IsNullOrEmpty(attr.file_name))
                throw new TLCoreException("The file_name of the document is empty");

            // Key: YYYY-MM-DD-Caption{0:00}.EXT
            string key = String.Format("{0}-{1}{2}",
                m_sPrefix,
                "{0:00}",
                attr.file_name);

            string sFileName = GetUniqueFileName(key); // YYYY-MM-DD-CaptionXX.EXT

            if (m_config.ExportFiles)
            {
                // Export the photo to a file
                string sFullFileName = Path.Combine(m_sDialogDirectory, sFileName);
                using (FileStream f = new FileStream(sFullFileName, FileMode.Create, FileAccess.Write))
                    foreach (TLFile file in m_archiver.GetDocument(document))
                        f.Write(file.bytes, 0, file.bytes.Length);
            }

            return sFileName; // YYYY-MM-DD-CaptionXX.EXT
        }

        protected string GetUniqueFileName(string key)
        {
            int id = c_iInitialIndex;
            if (m_fileNames.ContainsKey(key))
                id = m_fileNames[key];
            else
                m_fileNames[key] = id;

            // Transform 'YYYY-MM-DD-Caption{0:00}.EXT' to 'YYYY-MM-DD-CaptionXX.EXT'
            string sFileName = String.Format(key, id);
            m_fileNames[key] = id + 1;
            return sFileName;
        }
    }
}
