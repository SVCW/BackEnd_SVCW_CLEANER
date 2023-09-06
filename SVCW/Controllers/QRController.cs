using IronBarCode;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs;
using SVCW.DTOs.Activities;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRController : ControllerBase
    {
        private IActivity service;
        public QRController(IActivity service)
        {
            this.service = service;
        }

        [Route("QR")]
        [HttpGet]
        public async Task<IActionResult> QR(string activityId)
        {
            try
            {
                ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();

                responseAPI.Data = await this.service.checkQR(activityId);

                if(responseAPI.Data != null)
                {
                    GeneratedBarcode barcode = IronBarCode.BarcodeWriter.CreateBarcode(activityId, BarcodeEncoding.QRCode);

                    var tempFileName = Path.GetTempFileName() + ".png";
                    barcode.SaveAsPng(tempFileName);

                    byte[] fileContents = System.IO.File.ReadAllBytes(tempFileName);
                    var response = File(fileContents, "image/png", "QR.png");

                    System.IO.File.Delete(tempFileName);

                    return response;
                }
                else
                {
                    return BadRequest(responseAPI);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Route("check-in")]
        [HttpPost]
        public async Task<IActionResult> checkIn(string userId, string activityId)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.checkIn(activityId,userId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
    }
}
