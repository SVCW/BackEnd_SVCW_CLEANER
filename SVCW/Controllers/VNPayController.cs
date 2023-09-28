using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Email;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.Services;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace SVCW.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class VNPayController : ControllerBase
    {
        protected readonly SVCWContext context;
        //private readonly double _exchangeRate;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEmail _emailService;

        public string URL_VNPAY_REFUND;
        public string VNPAY_TMNCODE = "QEXW80Z4";
        public string VNPAY_HASH_SECRECT = "GVVBRNETROHVFHCFGWHXHZFKXQHMRQZC";
        public string VNPAY_VERSION = "2.0.0";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public VNPayController(IConfiguration configuration, SVCWContext context , IHttpClientFactory httpClientFactory, IEmail service)
        {
            _configuration = configuration;
            this.context = context;
            _httpClient = new HttpClient();
            _httpClientFactory = httpClientFactory;
            _emailService = service;
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
                    pay.AddRequestData("vnp_Amount", check.Amount.ToString("F").TrimEnd('0').TrimEnd('.').TrimEnd(',') + "00"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
                    pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
                    pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
                    pay.AddRequestData("vnp_IpAddr", ip); //Địa chỉ IP của khách hàng thực hiện giao dịch
                    pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
                    //pay.AddRequestData("vnp_OrderInfo", "ĐASADASOOPAO23SDSD"); //Thông tin mô tả nội dung thanh toán
                    pay.AddRequestData("vnp_OrderInfo", "Quyên góp từ thiện thông qua hệ thống SVCW");
                    pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
                    pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
                    // tách hóa đơn ra để thêm vào db
                    string taxVNPay = DateTime.Now.Ticks.ToString();
                    pay.AddRequestData("vnp_TxnRef", taxVNPay); //mã hóa đơn
                    pay.AddRequestData("vnp_ExpireDate", DateTime.Now.AddHours(8).ToString("yyyyMMddHHmmss")); //Thời gian kết thúc thanh toán
                    string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

                    // update db
                    check.TaxVnpay= taxVNPay;
                    this.context.Donation.Update(check);
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
                var vnp_TransDate = vnpay.GetResponseData("vnp_PayDate");
                //Guid companyId = Guid.Parse(vnp_OrderInfo);
                status = "success";

                string taxVNPay = orderId.ToString();
                var check = await this.context.Donation.Where(x=>x.TaxVnpay.Equals(taxVNPay)).FirstOrDefaultAsync();
                check.Status = status;
                check.PayDate= DateTime.Now;
                check.VnpTransDate = vnp_TransDate;
                this.context.Donation.Update(check);
                
                await this.context.SaveChangesAsync();
                var activity = await this.context.Activity.Where(x=>x.ActivityId.Equals(check.ActivityId)).FirstOrDefaultAsync();
                activity.RealDonation += check.Amount;
                this.context.Activity.Update(activity);
                await this.context.SaveChangesAsync();

                var process = await this.context.Process.Where(x=>x.ActivityId.Equals(check.ActivityId) && x.ProcessTypeId.Equals("pt001")).ToListAsync();
                foreach(var x in process)
                {
                    if(DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate)
                    {
                        x.RealDonation += check.Amount;
                        this.context.Process.Update(x);
                        await this.context.SaveChangesAsync();
                    }
                }
            }

            return Redirect(returnUrl + "?amount=" + amount + "&status=" + status);
        }


        [HttpPost("Refund-money")]
        public async Task<IActionResult> Refund(string activityId)
        {
            var vnp_Api = "http://sandbox.vnpayment.vn/merchant_webapi/merchant.html";
            var vnp_HashSecret = _configuration["VnPay:HashSecret"]; //Secret KEy
            var vnp_TmnCode = _configuration["VnPay:TmnCode"]; // Terminal Id
            var vnpay = new VnPayLibrary();

            var strDatax = "";

            try
            {
                //lấy data các bên đã ủng hộ
                var donate = await this.context.Donation.Where(x => x.ActivityId.Equals(activityId) && x.Status.Equals("success")).ToListAsync();
                if(donate == null ||   donate.Count == 0)
                {
                    return Ok("Chiến dịch không có khoản ủng hộ cần hoàn tiền");
                }
                var activity = await this.context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                foreach(var donation in donate)
                {
                    // xử lý hoàn tiền VNPay
                    vnpay = new VnPayLibrary();
                    var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    var vnp_RequestId = DateTime.Now.Ticks.ToString();

                    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION.ToString());
                    vnpay.AddRequestData("vnp_Command", "refund");
                    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode.ToString());
                    vnpay.AddRequestData("vnp_TransactionType", "02");
                    vnpay.AddRequestData("vnp_CreateBy", "Hệ thống Student Volunteer");
                    vnpay.AddRequestData("vnp_TxnRef", donation.TaxVnpay);
                    vnpay.AddRequestData("vnp_Amount", donation.Amount+"00");
                    vnpay.AddRequestData("vnp_OrderInfo", "Hoàn tiền từ chiến dịch: "+ activity.Title);
                    vnpay.AddRequestData("vnp_TransDate", donation.VnpTransDate);
                    vnpay.AddRequestData("vnp_CreateDate", vnp_CreateDate);
                    vnpay.AddRequestData("vnp_IpAddr", "1");

                    var refundtUrl = vnpay.CreateRequestUrl(vnp_Api, vnp_HashSecret);

                    var request = (HttpWebRequest)WebRequest.Create(refundtUrl);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                strDatax = reader.ReadToEnd();
                            }
                    if (strDatax.Contains("ResponseCode=00"))
                    {
                        donation.Status = "refund";
                        this.context.Donation.Update(donation);
                        await this.context.SaveChangesAsync();

                        var email = new SendEmailReqDTO();
                        email.sendTo = donation.Email;
                        email.subject = "Hoàn tiền cho chiến dịch " + activity.Title;
                        email.body = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Hoàn tiền cho chiến dịch "+activity.Title+"</title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n        }\r\n\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            border: 1px solid #ccc;\r\n            border-radius: 5px;\r\n        }\r\n\r\n        .header {\r\n            background-color: #FFC107;\r\n            color: #fff;\r\n            text-align: center;\r\n            padding: 10px;\r\n        }\r\n\r\n        .content {\r\n            padding: 20px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n    <div class=\"header\">\r\n        <h1>Hoàn Tiền Cho Chiến Dịch</h1>\r\n    </div>\r\n    <div class=\"content\">\r\n        <p>Xin chào "+donation.Name+",</p>\r\n        <p>Chúng tôi hy vọng bạn đang có một ngày tốt lành.</p>\r\n        <p>Chúng tôi xin thông báo rằng chiến dịch "+activity.Title+ " đã gặp khó khăn và không thể tiếp tục hoạt động. Do đó, chúng tôi sẽ hoàn tiền cho chiến dịch này.</p>\r\n        <p>Xin vui lòng chờ trong thời gian xử lý. Đội ngũ hỗ trợ của chúng tôi sẽ liên hệ với bạn trong thời gian sớm nhất để thực hiện quy trình hoàn tiền thành công.</p>\r\n        <p>Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua địa chỉ email svcw.company@gmail.com</p>\r\n        <p>Xin cảm ơn bạn và chúc bạn có một ngày tốt lành.</p>\r\n        <p>Trân trọng,<br>Hệ thống SVCW</p>\r\n    </div>\r\n</div>\r\n</body>\r\n</html>\r\n";
                        _emailService.sendEmail(email);

                        if(activity.RealDonation>= donation.Amount)
                        {
                            activity.RealDonation -= donation.Amount;
                            this.context.Activity.Update(activity);
                            await this.context.SaveChangesAsync();

                            bool flag = false;
                            if (donate.Last().DonationId.Equals(donation.DonationId))
                            {
                                activity.RealDonation = 0;
                                this.context.Activity.Update(activity);
                                await this.context.SaveChangesAsync();
                                flag = true;
                            }
                            var process = await this.context.Process.Where(x => x.ActivityId.Equals(activity.ActivityId)&&x.ProcessTypeId.Equals("pt001")).ToListAsync();
                            foreach(var x in process)
                            {
                                if (x.RealDonation >= donation.Amount)
                                {
                                    x.RealDonation -= donation.Amount;
                                    this.context.Process.Update(x);
                                    await this.context.SaveChangesAsync();
                                }
                                if (flag)
                                {
                                    x.RealDonation = 0;
                                    this.context.Process.Update(x);
                                    await this.context.SaveChangesAsync();
                                }

                            }
                        }
                        
                        
                    }
                    else
                    {
                        throw new Exception("Lỗi trong quá trình hoàn tiền");
                    }
                }  
                return Ok("Đã hoàn tiền thành công chiến dịch: " + activity.Title);
            }
            catch (Exception ex)
            {
                throw new Exception("Có lỗi sảy ra trong quá trình hoàn tiền:" + ex);
            }
        }
    }
}
