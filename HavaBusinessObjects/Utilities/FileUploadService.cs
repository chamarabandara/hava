using HavaBusiness;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using HavaBusinessObjects;


namespace HavaBusinessObjects.Utilities
{
    public class FileUploadService : IDisposable
    {

        //#region Check if file already exists

        //public bool checkFileExists(string fileName, String fileTypeSent)
        //{
        //    bool fileExist = false;
        //    var fileTypeId = (from fileType in this.ObjContext.FileTypes where fileType.Code == fileTypeSent select fileType.Id).FirstOrDefault();
        //    var file = (from responseFile in this.ObjContext.ResponseFiles
        //                where responseFile.FileTypeId == fileTypeId && responseFile.FileName == fileName
        //                select responseFile).FirstOrDefault();
        //    if (file != null)
        //    {
        //        fileExist = true;
        //    }

        //    return fileExist;
        //}
        //#endregion

        #region Repository db context

        private HAVA_DBModelEntities context;

        private HAVA_DBModelEntities ObjContext
        {
            get
            {
                if (context == null)
                    context = new HAVA_DBModelEntities();
                return context;
            }
        }

        #endregion db context

        #region File Upload Service

        // You could extract these two private methods to a separate utility class since
        // they do not really belong to a controller class but that is up to you
        public MultipartFormDataStreamProvider GetMultipartProvider(string uploadFolder)
        {
            try
            {
                //var uploadFolder = ConfigurationManager.AppSettings["ClaimDocumentTemp"].ToString();
                var root = HttpContext.Current.Server.MapPath(uploadFolder);

                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                return new MultipartFormDataStreamProvider(root);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Extracts Request FormatData as a strongly typed model
        public FileUploadViewModels GetFormData<T>(MultipartFormDataStreamProvider result)
        {
            FileUploadViewModels model = new FileUploadViewModels();
            for (int i = 0; i < result.FormData.Count; i++)
            {
                switch (result.FormData.GetKey(i))
                {
                    case "type":
                        model.fileType = result.FormData.GetValues(i).FirstOrDefault() ?? String.Empty;
                        break;
                    case "fileTypeId":
                        model.fileTypeId = Convert.ToInt32(result.FormData.GetValues(i).FirstOrDefault());
                        break;
                    default: break;
                }
            }
            return model;
        }

        public string GetDeserializedFileName(MultipartFileData fileData)
        {
            var fileName = GetFileName(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetDeserializedFileSize(MultipartFileData fileData)
        {
            var fileName = GetFileSize(fileData);
            return JsonConvert.DeserializeObject(fileName).ToString();
        }

        public string GetFileName(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.FileName;
        }

        public string GetFileSize(MultipartFileData fileData)
        {
            return fileData.Headers.ContentDisposition.Size.ToString();
        }

        #endregion

        #region Convert Base64 To Image
        /// <summary>
        /// Convert Base64 To Image
        ///    Date                            Author/(Reviewer)                       Description
        /// -----------------------------------------------------------------------------------------          
        /// 11-Jan-2015                          zayan safwan                           Created
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }
        #endregion



        #region Dispose

        public void Dispose()
        {
            this.ObjContext.Dispose();
        }

        #endregion
    }

    public class FileUploadViewModels
    {
        public string fileType { get; set; }
        public int fileTypeId { get; set; }
        public FileInfo file { get; set; }
    }
}