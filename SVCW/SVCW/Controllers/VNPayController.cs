using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVCW.Models;
using SVCW.Services;

namespace SVCW.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        protected readonly SVCWContext context;
        //private readonly double _exchangeRate;
        private readonly IConfiguration _configuration;
        public string URL_VNPAY_REFUND;
        public string VNPAY_TMNCODE = "5VUJNSBM";
        public string VNPAY_HASH_SECRECT = "CIEUIULSLBVJYZLCZUEXBDRWFHGYSSAB";
        public string VNPAY_VERSION = "2.0.0";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public VNPayController(IConfiguration configuration, SVCWContext context)
        {
            _configuration = configuration;
            this.context = context;
        }

        /// <summary>
        /// [Guest] Endpoint for company create url payment with condition
        /// </summary>
        /// <returns>List of user</returns>
        /// <response code="200">Returns the list of user</response>
        /// <response code="204">Returns if list of user is empty</response>
        /// <response code="403">Return if token is access denied</response>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get(string donateID)
        {
            try
            {
                var check = await this.context.Donation.Where(x => x.DonationId.Equals(donateID)).FirstOrDefaultAsync();
                if (check != null)
                {
                    string ip = "256.256.256.1";
                    string url = _configuration["VnPay:Url"];
                    string returnUrl = _configuration["VnPay:ReturnAdminPath"];
                    string tmnCode = _configuration["VnPay:TmnCode"];
                    string hashSecret = _configuration["VnPay:HashSecret"];
                    VnPayLibrary pay = new VnPayLibrary();

                    pay.AddRequestData("vnp_Version", "2.1.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
                    pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
                    pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
                    pay.AddRequestData("vnp_Amount", check.Amount.ToString("F").TrimEnd('0').TrimEnd('.') + "00"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
                    pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
                    pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
                    pay.AddRequestData("vnp_IpAddr", ip); //Địa chỉ IP của khách hàng thực hiện giao dịch
                    pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
                    pay.AddRequestData("vnp_OrderInfo", "ĐASADASOOPAO23SDSD"); //Thông tin mô tả nội dung thanh toán
                    //pay.AddRequestData("vnp_OrderInfo", "Thanh toán SVCW");
                    pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
                    pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
                    // tách hóa đơn ra để thêm vào db
                    string taxVNPay = DateTime.Now.Ticks.ToString();
                    pay.AddRequestData("vnp_TxnRef", taxVNPay); //mã hóa đơn
                    pay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddHours(1).ToString("yyyyMMddHHmmss")); //Thời gian kết thúc thanh toán
                    string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

                    // update db
                    check.TaxVnpay= taxVNPay;
                    if(await this.context.SaveChangesAsync() > 0)
                    {
                        return Ok(paymentUrl);
                    }
                    else
                    {
                        throw new Exception("Lỗi trong quá trình lưu vào cơ sở dữ liệu");
                    }
                    
                }
                else
                {
                    throw new Exception("không tồn tại donate id");
                }
                
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        /// <summary>
        /// [Guest] Endpoint for company confirm payment with condition
        /// </summary>
        /// <returns>List of user</returns>
        /// <response code="200">Returns the list of user</response>
        /// <response code="204">Returns if list of user is empty</response>
        /// <response code="403">Return if token is access denied</response>
        [HttpGet("PaymentConfirm")]
        public async Task<IActionResult> Confirm()
        {
            string returnUrl = _configuration["VnPay:ReturnPath"];
            float amount = 0;
            string status = "failed";
            if (Request.Query.Count > 0)
            {
                string vnp_HashSecret = _configuration["VnPay:HashSecret"]; //Secret key
                var vnpayData = Request.Query;
                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (string s in vnpayData.Keys)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //Lay danh sach tham so tra ve tu VNPAY
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
                float vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                amount = vnp_Amount;
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.Query["vnp_SecureHash"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
                //Guid companyId = Guid.Parse(vnp_OrderInfo);
                status = "success";

                string taxVNPay = orderId.ToString();
                var check = await this.context.Donation.Where(x=>x.TaxVnpay.Equals(taxVNPay)).FirstOrDefaultAsync();
                check.Status = status;
                check.PayDate= DateTime.Now;
                
                await this.context.SaveChangesAsync();
            }

            return Redirect(returnUrl + "?amount=" + amount + "&status=" + status);
        }


        //[HttpPost("Refund-money")]
        //public async Task<IActionResult> Refund(string IDVNPay, string OrderId, DateTime payDate, float Amount, string info, string ip, string user)
        //{
        //    /*var vnpay_api_url = URL_VNPAY_REFUND;
        //    var vnpHashSecret = VNPAY_HASH_SECRECT;
        //    string vnp_TmnCode = VNPAY_TMNCODE;
        //    var vnpay = new VnPayLibrary();
        //    var createDate = DateTime.Now;
        //    var strDatax = "";*/

        //    var vnp_Api = _configuration["VnPay:vnp_Api"];
        //    var vnp_HashSecret = _configuration["VnPay:HashSecret"]; //Secret KEy
        //    var vnp_TmnCode = _configuration["VnPay:TmnCode"]; // Terminal Id
        //    var vnpay = new VnPayLibrary();

        //    var strDatax = "";

        //    try
        //    {
        //        var vnp_RequestId = DateTime.Now.Ticks.ToString(); //Mã hệ thống merchant tự sinh ứng với mỗi yêu cầu hoàn tiền giao dịch. Mã này là duy nhất dùng để phân biệt các yêu cầu truy vấn giao dịch. Không được trùng lặp trong ngày.
        //        var vnp_Version = VnPayLibrary.VERSION; //2.1.0
        //        var vnp_Command = "refund";
        //        var vnp_TransactionType = "02";
        //        var vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
        //        var vnp_Amount = Convert.ToInt64(Amount) * 100;
        //        var vnp_TxnRef = OrderId; // Mã giao dịch thanh toán tham chiếu
        //        var vnp_OrderInfo = "Hoan tien giao dich:" + OrderId;
        //        var vnp_TransactionNo = ""; //Giả sử giá trị của vnp_TransactionNo không được ghi nhận tại hệ thống của merchant.
        //        var vnp_TransactionDate = payDate;
        //        var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
        //        var vnp_CreateBy = user;
        //        var vnp_IpAddr = ip;
        //        var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionStatus + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
        //        var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

        //        // Console.WriteLine(signData + "giá trị 1");
        //        //Console.WriteLine(vnp_SecureHash + "kq nè");
        //        var rfData = new
        //        {
        //            vnp_RequestId = vnp_RequestId,
        //            vnp_Version = vnp_Version,
        //            vnp_Command = vnp_Command,
        //            vnp_TmnCode = vnp_TmnCode,
        //            vnp_TransactionType = vnp_TransactionType,
        //            vnp_TransactionStatus = vnp_TransactionStatus,
        //            vnp_TxnRef = vnp_TxnRef,
        //            vnp_Amount = vnp_Amount,
        //            vnp_OrderInfo = vnp_OrderInfo,
        //            vnp_TransactionNo = vnp_TransactionNo,
        //            vnp_TransactionDate = vnp_TransactionDate,
        //            vnp_CreateBy = vnp_CreateBy,
        //            vnp_CreateDate = vnp_CreateDate,
        //            vnp_IpAddr = vnp_IpAddr,
        //            vnp_SecureHash = vnp_SecureHash


        //        };

        //        // string strDatax = "";

        //        var refundtUrl = vnpay.CreateRequestUrl(vnp_Api, vnp_HashSecret);

        //        // var jsonData = new Serialize(rfData);
        //        var request = (HttpWebRequest)WebRequest.Create(refundtUrl);
        //        request.AutomaticDecompression = DecompressionMethods.GZip;
        //        using (var response = (HttpWebResponse)request.GetResponse())
        //        using (var stream = response.GetResponseStream())
        //            if (stream != null)
        //                using (var reader = new StreamReader(stream))
        //                {
        //                    strDatax = reader.ReadToEnd();
        //                }
        //        return Redirect(vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionStatus + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo);
        //        // display.InnerHtml = "<b>VNPAY RESPONSE:</b> " + strDatax;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Có lỗi sảy ra trong quá trình hoàn tiền:" + ex);
        //    }
        //    /*var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
        //    httpWebRequest.ContentType = "application/json";
        //    httpWebRequest.Method = "POST";

        //    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //    {
        //        streamWriter.Write(jsonData);
        //    }
        //    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //    var strData = "";
        //    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //    {
        //        strData = streamReader.ReadToEnd();
        //    }*/
        //    //display.InnerHtml = "<b>VNPAY RESPONSE:</b> " + strData;
        //}

    }
}
