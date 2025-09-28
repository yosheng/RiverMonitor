using System.Text.RegularExpressions;
using Flurl.Http;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RiverMonitor.Bll.ApiServices;
using RiverMonitor.Dal;
using RiverMonitor.Model.ApiModels;
using RiverMonitor.Model.Entities;
using RiverMonitor.Model.Models;

namespace RiverMonitor.Bll.Services;

public partial class SyncService
{
    private readonly Dictionary<string, string> _irrigationAgencyWorkStationUrlDict = new()
    {
        { "雲林管理處", "https://www.iayli.nat.gov.tw/about/WorkStationPage?a=10411" },
        { "彰化管理處", "https://www.iachu.nat.gov.tw/about/WorkStationPage?a=10461" },
        { "臺中管理處", "https://www.iatch.nat.gov.tw/about/WorkStationPage?a=10361" },
        { "石門管理處", "https://www.iasme.nat.gov.tw/about/WorkStationPage?a=10211" },
        { "嘉南管理處", "https://www.iacna.nat.gov.tw/about/WorkStationPage?a=10561" },
        { "桃園管理處", "https://www.iatyu.nat.gov.tw/about/WorkStationPage?a=10161" },
        { "瑠公管理處", "https://www.ialgo.nat.gov.tw/about/WorkStationPage?a=10017" },
        { "高雄管理處", "https://www.iakhs.nat.gov.tw/about/WorkStationPage?a=10611" },
        { "新竹管理處", "https://www.iahch.nat.gov.tw/about/WorkStationPage?a=10261" },
        { "花蓮管理處", "https://www.iahli.nat.gov.tw/about/WorkStationPage?a=10761" },
        { "北基管理處", "https://www.iapke.nat.gov.tw/about/WorkStationPage?a=10061" },
        { "南投管理處", "https://www.ianto.nat.gov.tw/about/WorkStationPage?a=10511" },
        { "七星管理處", "" },
        { "宜蘭管理處", "https://www.iaila.nat.gov.tw/about/WorkStationPage?a=10711" },
        { "臺東管理處", "https://www.iattu.nat.gov.tw/about/WorkStationPage?a=10811" },
        { "屏東管理處", "https://www.iaptu.nat.gov.tw/about/WorkStationPage?a=10661" },
        { "苗栗管理處", "https://www.iamli.nat.gov.tw/about/WorkStationPage?a=10311" }
    };
    
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
            var contentNode =
                officeNode.SelectSingleNode(".//div[contains(@class, 'card-content')]/div[contains(., '地址')]");

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

        foreach (var keyValuePair in _irrigationAgencyWorkStationUrlDict)
        {
            var irrigationAgency = addEntities.FirstOrDefault(x => x.Name == keyValuePair.Key);
            if (irrigationAgency != null)
                irrigationAgency.WorkStationUrl = keyValuePair.Value;
        }

        _dbContext.IrrigationAgencies.AddRange(addEntities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task SyncIrrigationAgencyStationAsync()
    {
        if (_dbContext.IrrigationAgencyStations.Any())
        {
            return;
        }
        
        var irrigationAgencies = await _dbContext.IrrigationAgencies.Select(x => new
        {
            x.Id,
            x.WorkStationUrl
        }).ToListAsync();

        foreach (var irrigationAgency in irrigationAgencies)
        {
            if (string.IsNullOrWhiteSpace(irrigationAgency.WorkStationUrl))
            {
                var irrigationAgencyStation = new IrrigationAgencyStation()
                {
                    IrrigationAgencyId = irrigationAgency.Id,
                    Name = "本處"
                };
                _dbContext.IrrigationAgencyStations.Add(irrigationAgencyStation);
                await _dbContext.SaveChangesAsync();
                continue;
            }

            var workStationHttpContent = await irrigationAgency.WorkStationUrl.GetStringAsync();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(workStationHttpContent);

            var siteMenu = htmlDoc.GetElementbyId("siteMenu");

            var stations = new List<IrrigationAgencyStation>();

            // 取得所有工作站資料
            foreach (var stationNode in siteMenu.SelectNodes(".//li[contains(@class, 'siteMenu-item')]"))
            {
                var name = stationNode.SelectSingleNode(".//div[contains(@class, 'h4')]")?.InnerText?.Trim();

                // 如果找不到名稱，就跳過這筆資料
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var address = stationNode
                    .SelectSingleNode(
                        ".//div[contains(@class, 'siteMenu-info') and .//i[contains(@class, 'icon-location')]]")
                    ?.InnerText?.Trim();
                var phone = stationNode
                    .SelectSingleNode(
                        ".//div[contains(@class, 'siteMenu-info') and .//i[contains(@class, 'icon-phone')]]")?.InnerText
                    ?.Trim();

                // 避免同步數據錯誤做的補丁
                if (name.Contains("金山工作站"))
                {
                    name = "金山工作站";
                }

                stations.Add(new IrrigationAgencyStation()
                {
                    IrrigationAgencyId = irrigationAgency.Id,
                    Name = name,
                    Address = address,
                    Phone = phone
                });
                _dbContext.IrrigationAgencyStations.AddRange(stations);
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task SyncIrrigationAgencyStationMonitoringDataAsync()
    {
        _logger.LogInformation("開始同步農田水利署水質監測數據");

        await _dbContext.IrrigationAgencyStationMonitoringData.ExecuteDeleteAsync();

        var irrigationAgencyStations = await _dbContext.IrrigationAgencyStations
            .Select(station => new SingleAgencyStationDto
            {
                AgencyName = station.Agency.Name,
                OpenUnitId = station.Agency.OpenUnitId,
                Name = station.Name,             
                Id = station.Id
            })
            .ToListAsync();
        
        var irrigationAgencyDict = irrigationAgencyStations
            .GroupBy(x => new SingleAgencyDto
            {
                AgencyName = x.AgencyName,
                OpenUnitId = x.OpenUnitId
            })
            .ToDictionary(x => x.Key, x => x.ToList());

        // 過濾出有 OpenUnitId 的農田水利署
        var validAgencies = irrigationAgencyDict.Where(x => !string.IsNullOrEmpty(x.Key.OpenUnitId)).ToList();
        
        if (!validAgencies.Any())
        {
            _logger.LogWarning("沒有找到任何有OpenUnitId的農田水利署");
            return;
        }

        _logger.LogInformation("找到 {Count} 個農田水利署需要同步數據", validAgencies.Count);

        // 併發處理，每次最多10個
        const int maxConcurrency = 10;
        var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        var tasks = new List<Task>();
        var processedCount = 0;

        foreach (var irrigationAgency in validAgencies)
        {
            await semaphore.WaitAsync();

            var task = Task.Run(async () =>
            {
                try
                {
                    await ProcessSingleAgencyMonitoringDataAsync(irrigationAgency.Key, irrigationAgency.Value);
                    Interlocked.Increment(ref processedCount);
                    _logger.LogInformation("已處理 {Processed}/{Total} 個農田水利署的監測數據", 
                        processedCount, validAgencies.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "處理農田水利署 {AgencyName} 的監測數據時發生錯誤", irrigationAgency.Key.AgencyName);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("農田水利署水質監測數據同步完成，共處理 {Count} 個農田水利署", processedCount);
    }

    /// <summary>
    /// 處理單個農田水利署的監測數據（使用獨立的 scope）
    /// </summary>
    private async Task ProcessSingleAgencyMonitoringDataAsync(
        SingleAgencyDto agencyKey, 
        List<SingleAgencyStationDto> stations)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RiverMonitorDbContext>();
        var moaApiService = scope.ServiceProvider.GetRequiredService<IMoaApiService>();
        var validationService = scope.ServiceProvider.GetRequiredService<IValidationService>();

        var agencyName = agencyKey.AgencyName;
        var openUnitId = agencyKey.OpenUnitId;

        _logger.LogInformation("開始處理農田水利署 {AgencyName} 的監測數據", agencyName);

        try
        {
            // 1. 建立工作站名稱到ID的映射（避免N+1查詢）
            var stationMappings = stations.Where(x => x.Name.Contains("站"))
                .ToDictionary(x =>
                {
                    if (x.Name.Length > 3 )
                    {
                        x.Name = x.Name.Length > 3
                            ? x.Name.Substring(0, x.Name.Length - 3)
                            : x.Name;
                    }
                    return x.Name.Contains("站") ? x.Name : $"{x.Name}站";
                }, x => x.Id);

            // 針對本處單獨處理
            if (stations.Count == 1 && stations[0].Name == "本處")
            {
                // 原始政府資料有空格!
                stationMappings = new Dictionary<string, int>() {
                    { "本處 ", stations[0].Id }
                };
            }
            
            // 2. 調用API獲取水質監測數據
            var waterQualityData = await moaApiService.GetWaterQualityOpenDataAsync(openUnitId);
            if (!waterQualityData.Any())
            {
                _logger.LogWarning("農田水利署 {AgencyName} 沒有獲取到任何水質監測數據", agencyName);
                return;
            }

            // 3. 過濾並匹配數據
            var validMonitoringData = new List<IrrigationAgencyStationMonitoringData>();
            var matchedCount = 0;
            var unmatchedCount = 0;

            foreach (var data in waterQualityData)
            {
                // 根據 ManagementOfficeName 和 Workstation 匹配工作站
                if (string.IsNullOrEmpty(data.ManagementOfficeName) || 
                    string.IsNullOrEmpty(data.Workstation) ||
                    string.IsNullOrEmpty(data.SiteName) ||
                    string.IsNullOrEmpty(data.SampleDate))
                {
                    continue;
                }

                // 匹配管理處名稱
                if (!data.ManagementOfficeName.Equals(agencyName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // 匹配工作站
                if (!stationMappings.TryGetValue(data.Workstation, out var stationId))
                {
                    unmatchedCount++;
                    _logger.LogDebug("找不到匹配的工作站：{Workstation}，農田水利署：{AgencyName}", 
                        data.Workstation, agencyName);
                    continue;
                }

                // 解析採樣日期
                if (!DateTime.TryParse(data.SampleDate, out var sampleDate))
                {
                    _logger.LogWarning("無法解析採樣日期：{SampleDate}", data.SampleDate);
                    continue;
                }

                // 數據驗證
                var validationResult = await validationService.ValidateAsync(data);
                if (!validationResult.IsValid)
                {
                    PrintErrorRecord(data);
                    _logger.LogWarning("水質監測數據驗證失敗：{Errors}", 
                        string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    continue;
                }

                // 創建監測數據實體
                var monitoringData = new IrrigationAgencyStationMonitoringData
                {
                    Id = Guid.NewGuid(),
                    SiteName = data.SiteName,
                    SampleDate = sampleDate,
                    WaterTemperatureC = decimal.TryParse(data.WaterTemperatureC, out var temp) ? temp : null,
                    PhValue = decimal.TryParse(data.PhValue, out var ph) ? ph : null,
                    ElectricalConductivity = decimal.TryParse(data.ElectricalConductivity, out var ec) ? ec : null,
                    Note = data.Note,
                    Version = data.Version,
                    IrrigationAgencyStationId = stationId
                };

                validMonitoringData.Add(monitoringData);
                matchedCount++;
            }

            // 4. 批量保存數據
            if (validMonitoringData.Any())
            {
                dbContext.IrrigationAgencyStationMonitoringData.AddRange(validMonitoringData);
                await dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("農田水利署 {AgencyName} 處理完成：匹配 {MatchedCount} 條，未匹配 {UnmatchedCount} 條，保存 {SavedCount} 條", 
                agencyName, matchedCount, unmatchedCount, validMonitoringData.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "處理農田水利署 {AgencyName} 的數據時發生錯誤", agencyName);
            throw;
        }
    }
}