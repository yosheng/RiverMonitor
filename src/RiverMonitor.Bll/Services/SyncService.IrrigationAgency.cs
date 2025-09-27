using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using RiverMonitor.Model.ApiModels;
using RiverMonitor.Model.Entities;

namespace RiverMonitor.Bll.Services;

public partial class SyncService
{
    public async Task SyncIrrigationAgencyAsync()
    {
        var response = await _moaApiService.GetOpenThematicAsync(new ThematicRequestForm
        {
            Page = 1,
            Limit = 1000
        });

        var data = response.Data.Where(x => x.Title != null && x.Title.Contains("水質監測結果")).ToList();

        var addEntities = new List<IrrigationAgency>();
        foreach (var dataItem in data)
        {
            if (dataItem.Title == null || string.IsNullOrEmpty(dataItem.ID))
            {
                _logger.LogWarning("監測處名稱或鍵為空");
                continue;
            }
            
            var title = Regex.Match(dataItem.Title, @".*(?=灌溉水質監測結果)").Value;
            var entity = new IrrigationAgency()
            {
                Id = Guid.NewGuid(),
                Name = title,
                OpenUnitId = dataItem.ID,
            };
            addEntities.Add(entity);
        }
        
        _dbContext.IrrigationAgencies.AddRange(addEntities);
    }

    public Task SyncIrrigationAgencyStationAsync()
    {
        throw new NotImplementedException();
    }
}