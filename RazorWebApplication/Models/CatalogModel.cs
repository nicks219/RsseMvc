using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RandomSongSearchEngine.Classes;
using RandomSongSearchEngine.DBContext;

namespace RandomSongSearchEngine.Models
{
    /// <summary>
    /// ������� �����
    /// </summary>
    public class CatalogModel : BaseMvcModel
    {
        //private readonly ILogger<BasePageModel> _logger;
        public CatalogModel() { }

        public CatalogModel(IServiceScopeFactory serviceScopeFactory, ILogger<BaseMvcModel> logger) : base(serviceScopeFactory, logger)
        {
            //_logger = logger;
            //pageSize = 3;
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())//
                {
                    var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();//
                    SongsCount = database.Text.Count();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "[CatalogModel]: no database");
                //� ������ ���������� �� �� �� ����� � null referenece exception ��-�� TitleAndTextID
                TitleAndTextID = new List<Tuple<string, int>>();
            }
        }

        /// <summary>
        /// ������������ ��� ����������� ����� ������ ���� ������ �� �����
        /// </summary>
        [BindProperty]
        public List<int> NavigationButtons { get; set; }

        /// <summary>
        /// ���������� ����� � ���� ������
        /// </summary>
        [BindProperty]
        public int SongsCount { get; set; }

        /// <summary>
        /// ����� ��������� ������������� ��������
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// ���������� ����� �� ����� ��������
        /// </summary>
        public readonly int pageSize = 3;
    }
}
