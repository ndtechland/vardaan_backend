using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace VardaanCab.Utilities
{
    
    public class SmsOperation
    {
        public static void SendSms(string mobileNumber, string m)
        {
            //Your authentication key
            //string authKey = "310705AfgJnZ1MTD5e09eeceP1";
            string authKey = "310705ABvkvD26Y7zx61c1bf11P1";
            //Multiple mobiles numbers separated by comma
            //Sender ID,While using route4 sender id should be 6 characters long.
            //string senderId = "VARDAN";
            string senderId = "VRCCAB";
            //Your message to send, Add URL encoding here.
            string message = HttpUtility.UrlEncode(m);

            //Prepare you post parameters
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", authKey);
            sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
            sbPostData.AppendFormat("&country={0}", "91"); 
            sbPostData.AppendFormat("&message={0}", message);
            sbPostData.AppendFormat("&sender={0}", senderId);
            sbPostData.AppendFormat("&route={0}", "4");
            sbPostData.AppendFormat("&DLT_TE_ID={0}", "1107161286355148435");
            sbPostData.AppendFormat("&dev_mode={0}", "1");
          
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        | SecurityProtocolType.Tls11
        | SecurityProtocolType.Tls12
        | SecurityProtocolType.Ssl3;

                //Call Send SMS API 
                //string sendSMSUri = "https://api.msg91.com/api/sendhttp.php?group_id=group_id";
                string sendSMSUri = "https://control.msg91.com/api/sendhttp.php";
                //Create HTTPWebrequest
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                httpWReq.KeepAlive = false;
                //Prepare and Add URL Encoded data
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(sbPostData.ToString());
                //Specify post method
                httpWReq.Method = "POST";

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                // Skip validation of SSL/TLS certificate
                //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;
                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                //Get the response
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();

                //Close the response
                reader.Close();
                response.Close();
            }
            catch(Exception ex)
            {

            }
        }


        public static void SendSms(string mobileNumber, string m,string dlt_tem_id)
        {
            //Your authentication key
           // string authKey = "310705AfgJnZ1MTD5e09eeceP1";
            string authKey = "310705ABvkvD26Y7zx61c1bf11P1";
            // string authKey = "310705AfgJnZ1MTD5e09eeceP1fgfg";//for testing
            //Multiple mobiles numbers separated by comma
            //Sender ID,While using route4 sender id should be 6 characters long.
            //string senderId = "VARDAN";
            string senderId = "VRCCAB";
            //Your message to send, Add URL encoding here.
            string message = HttpUtility.UrlEncode(m);

            //Prepare you post parameters
            StringBuilder sbPostData = new StringBuilder();
            sbPostData.AppendFormat("authkey={0}", authKey);
            sbPostData.AppendFormat("&mobiles={0}", mobileNumber);
            sbPostData.AppendFormat("&country={0}", "91");
            sbPostData.AppendFormat("&message={0}", message);
            sbPostData.AppendFormat("&sender={0}", senderId);
            sbPostData.AppendFormat("&route={0}", "4");
            sbPostData.AppendFormat("&DLT_TE_ID={0}", dlt_tem_id);
            sbPostData.AppendFormat("&dev_mode={0}", "1");

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
        | SecurityProtocolType.Tls11
        | SecurityProtocolType.Tls12
        | SecurityProtocolType.Ssl3;

                //Call Send SMS API 
                //string sendSMSUri = "https://api.msg91.com/api/sendhttp.php?group_id=group_id";
                string sendSMSUri = "https://control.msg91.com/api/sendhttp.php";
                //Create HTTPWebrequest
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                //Prepare and Add URL Encoded data
                httpWReq.KeepAlive = false;
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] data = encoding.GetBytes(sbPostData.ToString());
                //Specify post method
                httpWReq.Method = "POST";

               

                httpWReq.ContentType = "application/x-www-form-urlencoded";
                httpWReq.ContentLength = data.Length;
                using (Stream stream = httpWReq.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                //Get the response
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string responseString = reader.ReadToEnd();

                //Close the response
                reader.Close();
                response.Close();
            }
            catch(Exception ex)
            {

            }
        }
    }
}