using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using RSAEncrypt.Net;

namespace RSA.JS.Net.Web.Account
{
    public partial class Login : System.Web.UI.Page
    {
        protected string strPublicKeyExponent;
        protected string strPublicKeyModulus;
        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterHyperLink.NavigateUrl = "Register.aspx?ReturnUrl=" + HttpUtility.UrlEncode(Request.QueryString["ReturnUrl"]);
            RSAEncryptProvider rsaEncryptProvider = new RSAEncryptProvider("login");
            strPublicKeyExponent = rsaEncryptProvider.RSAExponent;
            strPublicKeyModulus = rsaEncryptProvider.RSAModulus;
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            RSAEncryptProvider rsaEncryptProvider = new RSAEncryptProvider("login");
            string pwd = Request.Form["Password"];
            string realPwd = rsaEncryptProvider.Decrypt(pwd, true);
        }
    }
}
