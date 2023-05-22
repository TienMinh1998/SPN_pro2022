﻿using DatabaseCore.Domain.Entities.Normals;
using Hola.Api.Models.Dic;
using Hola.Api.Models.Questions;
using Hola.Api.Service;
using Hola.Core.Helper;
using Hola.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Hola.Api.Requests.Reading;
using Hola.Core.DapperExtension;
using OfficeOpenXml.FormulaParsing.ExpressionGraph;
using System.Linq.Expressions;

namespace Hola.Api.Controllers
{
    [Route("reading")]
    [ApiController]
    public class ReadingController : ControllerBase
    {
        private readonly IReadingService _readingService;
        private readonly IUploadFileService _uploadService;
        public ReadingController(IReadingService readingService, IUploadFileService uploadService)
        {
            _readingService = readingService;
            _uploadService = uploadService;
        }

        // SEARCH
        /// <summary>
        /// Tìm kiếm bài luận
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [Authorize]
        public async Task<JsonResponseModel> Search([FromBody] SearchReadingRequest model)
        {
            try
            {
                var search = model.Search;
                string title = search.GetValueByKey<string>("title");
                bool checkHasTime = false;

                DateTime? startDate = search.GetValueByKey<DateTime?>("startDate");
                DateTime? endDate = search.GetValueByKey<DateTime?>("endDate");


                DateTime ed = DateTime.Parse("2022-01-01");
                DateTime st = DateTime.Parse("3000-01-01");

                if (startDate.HasValue && endDate.HasValue)
                {
                    // tức là có thời gian
                    checkHasTime = true;
                    st = startDate.Value.Date;
                    ed = endDate.Value.Date.AddDays(1).AddMilliseconds(-1);
                }
                // Điều kiện tìm kiếm
                Func<Reading, bool> condition = x => x.IsDeleted == 0
                && (string.IsNullOrEmpty(title) ? true : x.Title.Contains(title))
                && (checkHasTime ? (x.CreatedDate >= st && x.CreatedDate <= ed) : true);


                var list = _readingService.GetListPaged(model.PageIndex, model.PageSize, condition, "CreatedDate", false);
                return JsonResponseModel.Success(list);
            }
            catch (Exception ex)
            {
                return JsonResponseModel.SERVER_ERROR(ex.Message);
            }
        }

        // DETAIL
        /// <summary>
        /// Lấy chi tiết của bài luận
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<JsonResponseModel> Detail(int id)
        {
            try
            {
                var model = await _readingService.GetFirstOrDefaultAsync(x => x.Id == id);
                if (model == null)
                {
                    return JsonResponseModel.Success(null);
                }
                else
                {
                    return JsonResponseModel.Success(model);
                }
            }
            catch (Exception ex)
            {
                return JsonResponseModel.SERVER_ERROR(ex.Message);
            }
        }

        // ADD 
        /// <summary>
        /// Thêm mới bài luận
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize]
        public async Task<JsonResponseModel> Add([FromForm] AddReadingRequest model)
        {
            try
            {
                // Add Image
                string url = await _uploadService.UploadImage(model.file, HttpContext);

                var _object = await _readingService.GetFirstOrDefaultAsync(x => x.Title == model.Title && x.IsDeleted == 0);
                if (_object != null)
                {
                    return JsonResponseModel.Error("Đã tồn tại", 400);
                }

                Reading easay = new Reading
                {
                    Content = model.Content,
                    CreatedDate = DateTime.UtcNow,
                    Definetion = model.Definetion,
                    Image = url,
                    IsDeleted = 0,
                    Status = "new",
                    Title = model.Title,
                    Translate = model.Translate
                };
                // add to data base
                var response = await _readingService.AddAsync(easay);
                return JsonResponseModel.Success(response);
            }
            catch (Exception ex)
            {
                return JsonResponseModel.SERVER_ERROR(ex.Message);
            }
        }


        // UPDATE
        /// <summary>
        /// Cập nhật bài luận
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize]
        public async Task<JsonResponseModel> update([FromBody] UpdateReadingRequest model)
        {
            try
            {
                var entity = await _readingService.GetFirstOrDefaultAsync(x => x.Id == model.Id && (x.IsDeleted == 0 || x.IsDeleted != 1));
                if (entity != null)
                {
                    if (entity.Status != "OK")
                    {
                        entity.Title = model.Title;
                        entity.Content = model.Content;
                        entity.Definetion = model.Definetion;
                        entity.Translate = model.Translate;
                        entity.Status = model.Status;
                    }
                    var response = await _readingService.UpdateAsync(entity);
                    return JsonResponseModel.Success(response);
                }
                else
                {
                    return JsonResponseModel.Error($"Không tìm thấy bản ghi '{model.Id}'", 400);
                }
            }
            catch (Exception ex)
            {
                return JsonResponseModel.SERVER_ERROR(ex.Message);
            }
        }

        /// <summary>
        /// xóa bài luận
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<JsonResponseModel> Delete(int id)
        {
            try
            {
                var entity = await _readingService.GetFirstOrDefaultAsync(x => x.Id == id && (x.IsDeleted == 0 || x.IsDeleted != 1));
                if (entity != null)
                {
                    entity.IsDeleted = 1;
                    var response = await _readingService.UpdateAsync(entity);
                    return JsonResponseModel.Success(response);
                }
                else
                {
                    return JsonResponseModel.Error($"Không tìm thấy bản ghi có 'id ={id}'", 400);
                }
            }
            catch (Exception ex)
            {
                return JsonResponseModel.SERVER_ERROR(ex.Message);
            }
        }
    }
}