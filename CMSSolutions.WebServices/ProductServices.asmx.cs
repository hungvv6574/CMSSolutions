using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Services;

namespace CMSSolutions.WebServices
{
    public class ProductServices : WebService, IProductServices
    {
        private readonly DataContexService contexBase;
        public ProductServices()
        {
            contexBase = new DataContexService();
        }

        public bool CheckCopyright(string domain)
        {
            try
            {
                var list = new List<SqlParameter>
                {
                    contexBase.AddInputParameter("@Domain", domain)
                };

                var result = (int)contexBase.ExecuteReaderResult("sp_Copyrights_CheckProducts", list.ToArray());
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetTokenKey(string domain)
        {
            try
            {
                var list = new List<SqlParameter>
                {
                    contexBase.AddInputParameter("@Domain", domain)
                };

                return (string)contexBase.ExecuteReaderResult("sp_Copyrights_GetTokenKey", list.ToArray());
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
