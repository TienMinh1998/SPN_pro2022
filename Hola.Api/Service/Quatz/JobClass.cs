﻿using Hola.Api.Models.Accounts;
using Hola.Api.Service.UserServices;
using Hola.Api.Service.V1;
using Hola.Core.Venly.Base;
using Hola.Core.Venly.WalletHandlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hola.Api.Service.Quatz
{
    public class JobClass : IJob
    {

        private readonly IConfiguration _Configuration;
        private readonly AccountService accountService;
        private readonly FirebaseService firebaseService;
        private readonly QuestionService qesQuestionService;
        private readonly IQuestionService _questionService;
        private readonly IUserService _userServices;
        public JobClass(IConfiguration configuration, AccountService accountService, 
            FirebaseService firebaseService, QuestionService qesQuestionService, 
            IUserService userServices, 
            IQuestionService questionService)
        {
            _Configuration = configuration;
            this.accountService = accountService;
            this.firebaseService = firebaseService;
            this.qesQuestionService = qesQuestionService;
            _userServices = userServices;
            _questionService = questionService;
        }
        public async Task CheckTaskService()
        {
            try
            {
                // Get ListUser Noti 

                var listUser =await _userServices.GetAllAsync(x=>(x.isnotification==1 && x.IsDeleted==0));
                var response = listUser.ToList();
                foreach (var item in response)
                {
                    // Category 
                    //  var result = await qesQuestionService.GetListQuestionByCategoryId(item.Id, 0);
                    var listQuestion =await _questionService.GetAllAsync(x => x.category_id == 7 && x.is_delete != 1);

                    Random rnd = new Random();
                    var index = rnd.Next(listQuestion.Count);
                    var questionRadom = listQuestion[index];
                    // Lấy ra thông tin deviceToken 
                    var devideFirebaseToken = item.DeviceToken;
                    PushNotificationRequest request = new PushNotificationRequest()
                    {
                        notification = new NotificationMessageBody()
                        {
                            title = questionRadom.questionname,
                            body = questionRadom.answer
                        },
                    };
                    request.registration_ids.Add(devideFirebaseToken);
                    await firebaseService.Push(request,item.Id);
                }

              
            }
            catch (System.Exception ex)
            {

                throw;
            }

        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.WhenAll(CheckTaskService());
            }
            catch
            {
                return;
            }
        }
    }
}
