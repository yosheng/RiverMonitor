using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using RiverMonitor.Model.ApiModels;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Bll.Services;

public partial class SyncService
{
    public async Task SyncIrrigationAgencyAsync()
    {
        if (_dbContext.IrrigationAgencies.Any())
        {
            return;
        }

        var officeListHtmlContent = await _iaApiService.GetOfficeListPageAsync();

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(officeListHtmlContent);

        var infobox = htmlDoc.GetElementbyId("infobox");
        var addEntities = new List<IrrigationAgency>();

        // 取得所有管理處（office）資料
        foreach (var officeNode in infobox.SelectNodes(".//div[@data-info]"))
        {
            var name = officeNode.GetAttributeValue("data-info", "")?.Trim();

            if (name == null)
            {
                continue;
            }

            // 選取包含地址和電話的 div，並處理其內部文字
            var contentNode = officeNode.SelectSingleNode(".//div[contains(@class, 'card-content')]/div[contains(., '地址')]");
            
            // 使用 InnerHtml 並以 <br> 分割，可以準確地得到每一行
            var lines = contentNode.InnerHtml.Split(new[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

            // 初始化變數
            string? address = null;
            string? phone = null;
            foreach (var line in lines)
            {
                var trimmedLine = HtmlEntity.DeEntitize(line).Trim(); // 清理 HTML 實體並修剪空白

                if (trimmedLine.StartsWith("地址："))
                {
                    address = trimmedLine.Replace("地址：", "").Trim();
                }
                else if (trimmedLine.StartsWith("電話："))
                {
                    phone = trimmedLine.Replace("電話：", "").Trim();
                }
            }

            addEntities.Add(new IrrigationAgency()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Phone = phone,
                Address = address,
            });
        }

        var response = await _moaApiService.GetOpenDataAsync(new ThematicRequestForm
        {
            Page = 1,
            Limit = 1000,
            Keyword = "水質監測結果"
        });

        var data = response.Data.Where(x => x.Title != null && x.Title.Contains("水質監測結果")).ToList();

        foreach (var dataItem in data)
        {
            if (dataItem.Title == null || string.IsNullOrEmpty(dataItem.ID))
            {
                _logger.LogWarning("監測處名稱或鍵為空");
                continue;
            }

            var title = Regex.Match(dataItem.Title, @".*(?=灌溉水質監測結果)").Value;

            var irrigationAgency = addEntities.FirstOrDefault(x => x.Name == title);

            if (title == "台東管理處")
            {
                irrigationAgency = addEntities.FirstOrDefault(x => x.Name == "臺東管理處");
            }
            if (title == "台中管理處")
            {
                irrigationAgency = addEntities.FirstOrDefault(x => x.Name == "臺中管理處");
            }

            if (irrigationAgency != null)
            {
                irrigationAgency.OpenUnitId = dataItem.ID;
            }
        }

        _dbContext.IrrigationAgencies.AddRange(addEntities);
        await _dbContext.SaveChangesAsync();
    }

    public Task SyncIrrigationAgencyStationAsync()
    {
        throw new NotImplementedException();
    }
}