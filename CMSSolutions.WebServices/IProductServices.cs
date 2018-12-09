using System.ServiceModel.Web;
using System.Web.Services;

namespace CMSSolutions.WebServices
{
    [WebService(Namespace = Constants.NamespaceSite)]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public interface IProductServices
    {
        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, RequestFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = Constants.UrlCheckCopyright)]
        bool CheckCopyright(string domain);

        [WebMethod]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Xml, RequestFormat = WebMessageFormat.Xml, BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = Constants.UrlGetTokenKey)]
        string GetTokenKey(string domain);
    }
}
