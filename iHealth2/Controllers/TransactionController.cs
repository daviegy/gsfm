using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iHealth2.Models;
using iHealth2.CustomClasses;
using System.Net;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace iHealth2.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TransactionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        string product_id = "1076";
        // GET: Transaction
        public ActionResult All_Transaction()
        {
            var trs = db.InterswtichTransactionsTables.ToList().OrderByDescending(t=>t.Transaction_date);
            return View(trs);
        }
        public ActionResult success_trs()
        {
            var trs = db.InterswtichTransactionsTables.Where(t => t.Transaction_ResponseCode == "00" && (t.Transaction_Response != "(Pending)" || t.Transaction_Response != "Pending")).ToList().OrderByDescending(t => t.Transaction_date);
            return View(trs);
        }
        public ActionResult fail_trs()
        {
            var trs = db.InterswtichTransactionsTables.Where(t => t.Transaction_ResponseCode != "00" && (t.Transaction_Response != "(Pending)" || t.Transaction_Response != "Pending")).ToList().OrderByDescending(t => t.Transaction_date);
            return View(trs);
        }
        public ActionResult pending_trs()
        {
            var trs = db.InterswtichTransactionsTables.Where(t => t.Transaction_Response == "(Pending)" || t.Transaction_Response == "Pending").ToList().OrderByDescending(t => t.Transaction_date);
            return View(trs);
        }
        public async Task<ActionResult> Requery_trs(string txnref)
        {
            try
            {
                var trstb = db.InterswtichTransactionsTables.Single(t => t.Transaction_Id == txnref);
                var input = string.Concat(product_id, txnref, "D3D1D05AFE42AD50818167EAC73C109168A0F108F32645C8B59E897FA930DA44F9230910DAC9E20641823799A107A02068F7BC0F4CC41D2952E249552255710F");
                var hash = Cryptoclass.GenerateSHA512String(input);
                string response_url = "https://sandbox.interswitchng.com/collections/api/v1/gettransaction.json?productid=1076&transactionreference=" + txnref + "&amount=" + trstb.Transaction_Amount + "";
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var client = new WebClient();
                client.Headers.Add("Hash", hash);
                var content = client.DownloadString(response_url);
                // Create the Json serializer and parse the response
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(PaymentResponse));
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(content)))
                {
                    var pmtData = (PaymentResponse)serializer.ReadObject(ms);

                    if (pmtData.ResponseCode == "00")
                    {
                        var iswtb = db.InterswtichTransactionsTables.Single(s => s.Transaction_Id == txnref);
                        iswtb.Transaction_ResponseCode = pmtData.ResponseCode;
                        iswtb.Transaction_Response = pmtData.ResponseDescription;
                        await db.SaveChangesAsync();
                    }
                }
                return RedirectToAction("All_Transaction");
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        
        }
    }
}