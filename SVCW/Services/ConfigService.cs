using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SVCW.DTOs.Config;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class ConfigService : IConfig
    {
        protected readonly SVCWContext context;
        public ConfigService(SVCWContext context)
        {
            this.context = context;
        }
        public ConfigService() { }
        public adminConfig GetAdminConfig()
        {
            adminConfig adminConfig = new adminConfig();

            string filename = "AdminConfig.json";
            using (StreamReader sr = File.OpenText(filename))
            {
                var obj = sr.ReadToEnd();
                adminConfig = JsonConvert.DeserializeObject<adminConfig>(obj);
            }

            return adminConfig;
        }

        public  userCreateActivityConfig getConfig(configDTO dto)
        {
            try
            {
                var admin = new ConfigService();
                var adConfig = new adminConfig();
                adConfig = admin.GetAdminConfig();
                userCreateActivityConfig config = new userCreateActivityConfig();
                var check = this.context.User
                    .Include(p=>p.Fanpage)
                    .Where(x => x.UserId.Equals(dto.userId) || x.Email.Equals(dto.email)).FirstOrDefault();
                if(check == null)
                {
                    return new userCreateActivityConfig();
                }
                if(check.Fanpage == null)
                {
                    config.isFanpage = false;
                    if (check.NumberActivityJoin < adConfig.NumberActivityJoinSuccess1)
                    {
                        config.isValidCreate = false;
                        config.isDonatable = false;
                        config.maxDonate= 0;
                        config.activityJoin = (int)check.NumberActivityJoin;
                        config.target = adConfig.NumberActivityJoinSuccess1;
                        config.message = " Muốn tạo chiến dịch phải tham gia đủ "+ adConfig.NumberActivityJoinSuccess1 +" chiến dịch"+
                            "\n số chiến dịch bạn đã tham gia thành công: "+check.NumberActivityJoin;
                    }
                    else
                    {
                        if(check.NumberActivityJoin >= adConfig.NumberActivityJoinSuccess1) 
                        {
                            config.isValidCreate = true;
                            config.isDonatable = true;
                            config.maxDonate = adConfig.maxTargetDonate1;
                            config.activityJoin = (int)check.NumberActivityJoin;
                            config.target = adConfig.NumberActivityJoinSuccess1;
                            config.message = " Bạn đã tham gia " +check.NumberActivityJoin +" chiến dịch " +
                                "\n và chiến dịch có quyên góp tối đa bạn có thể tạo là: " + String.Format("{0:0,0}", (decimal)adConfig.maxTargetDonate1);
                        }
                        if (check.NumberActivityJoin >= adConfig.NumberActivityJoinSuccess2)
                        {
                            config.isValidCreate = true;
                            config.isDonatable = true;
                            config.maxDonate = adConfig.maxTargetDonate2;
                            config.activityJoin = (int)check.NumberActivityJoin;
                            config.target = adConfig.NumberActivityJoinSuccess2;
                            config.message = " Bạn đã tham gia " + check.NumberActivityJoin + " chiến dịch " +
                                "\n và chiến dịch có quyên góp tối đa bạn có thể tạo là: " + String.Format("{0:0,0}", (decimal)adConfig.maxTargetDonate2);
                        }
                        if (check.NumberActivityJoin >= adConfig.NumberActivityJoinSuccess3)
                        {
                            config.isValidCreate = true;
                            config.isDonatable = true;
                            config.maxDonate = adConfig.maxTargetDonate3;
                            config.activityJoin = (int)check.NumberActivityJoin;
                            config.target = adConfig.NumberActivityJoinSuccess3;
                            config.message = " Bạn đã tham gia " + check.NumberActivityJoin + " chiến dịch " +
                                "\n và chiến dịch có quyên góp tối đa bạn có thể tạo là: " + String.Format("{0:0,0}", (decimal)adConfig.maxTargetDonate3);
                        }
                    }
                }
                else
                {
                    config.isValidCreate = true;
                    config.isDonatable = true;
                    config.isFanpage = true;
                    config.maxDonate = adConfig.maxTargetDonate3;
                    config.activityJoin = (int)check.NumberActivityJoin;
                    config.target = adConfig.NumberActivityJoinSuccess3;
                    config.message = " Chiến dịch có quyên góp tối đa bạn có thể tạo là: " + String.Format("{0:0,0}", (decimal)adConfig.maxTargetDonate3);
                }
                
                return config;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public adminConfig updateAdminConfig(adminConfig update)
        {
            adminConfig adminConfig = new adminConfig();

            string filename = "AdminConfig.json";
            using (StreamReader sr = File.OpenText(filename))
            {
                var obj = sr.ReadToEnd();
                adminConfig = JsonConvert.DeserializeObject<adminConfig>(obj);
            }

            adminConfig.NumberActivityJoinSuccess1 = update.NumberActivityJoinSuccess1;
            adminConfig.NumberActivityJoinSuccess2 = update.NumberActivityJoinSuccess2;
            adminConfig.NumberActivityJoinSuccess3 = update.NumberActivityJoinSuccess3;
            adminConfig.maxTargetDonate1 = update.maxTargetDonate1;
            adminConfig.maxTargetDonate2 = update.maxTargetDonate2;
            adminConfig.maxTargetDonate3 = update.maxTargetDonate3;

            using (StreamWriter sw = File.CreateText(filename))
            {
                var admindata = JsonConvert.SerializeObject(adminConfig);
                sw.WriteLine(admindata);
            }

            return adminConfig;
        }

        public async Task<ProcessConfigDTO> getConfigForUser(configDTO dto)
        {
            try
            {
                var result = new ProcessConfigDTO();

                var admin = getConfig(dto);
                var check = await this.context.ProcessType.Where(x => x.ProcessTypeId.Equals(dto.processTypeId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    if (dto.processTypeId.Equals("pt001"))
                    {
                        result.IsDonateProcess = true;
                        result.MaxDonation = (decimal?)admin.maxDonate;
                    }
                    if (dto.processTypeId.Equals("pt002"))
                    {
                        result.IsParticipant = true;
                    }
                }

                return result;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }
    }
}
