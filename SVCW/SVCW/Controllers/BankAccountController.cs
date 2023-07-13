using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SVCW.DTOs.Activities;
using SVCW.DTOs;
using SVCW.Interfaces;
using SVCW.Models;
using SVCW.DTOs.BankAccount;

namespace SVCW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountController : ControllerBase
    {
        private IBankAccount service;
        public BankAccountController(IBankAccount service)
        {
            this.service = service;
        }

        [Route("Insert-bank-account")]
        [HttpPost]
        public async Task<IActionResult> Insert(BankAccountDTO dto)
        {
            ResponseAPI<List<BankAccount>> responseAPI = new ResponseAPI<List<BankAccount>>();
            try
            {
                responseAPI.Data = await this.service.create(dto);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("Insert-bank-account-activity")]
        [HttpPost]
        public async Task<IActionResult> InsertActivity(string bankId, string activityId)
        {
            ResponseAPI<List<BankAccount>> responseAPI = new ResponseAPI<List<BankAccount>>();
            try
            {
                responseAPI.Data = await this.service.useBankForActivity(bankId,activityId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("update-activity")]
        [HttpPut]
        public async Task<IActionResult> update(string bankIdOld, string activityId, string newBankId)
        {
            ResponseAPI<List<BankAccount>> responseAPI = new ResponseAPI<List<BankAccount>>();
            try
            {
                responseAPI.Data = await this.service.updateBankForActivity(bankIdOld,activityId,newBankId);
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }


        [Route("getall-bankaccount")]
        [HttpGet]
        public async Task<IActionResult> getall()
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.getAll();
                return Ok(responseAPI);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }

        [Route("get-bankaccount-user")]
        [HttpGet]
        public async Task<IActionResult> getforuser(string id)
        {
            ResponseAPI<List<Activity>> responseAPI = new ResponseAPI<List<Activity>>();
            try
            {
                responseAPI.Data = await this.service.getForUser(id);
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
