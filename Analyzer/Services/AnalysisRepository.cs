using Analyzer.Models;
using Analyzer.Models.TableEntities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly ITableService<AnalysisEntity> _analysisTable;

        public AnalysisRepository(ITableService<AnalysisEntity> analysisTable)
        {
            _analysisTable = analysisTable;
        }

        public async Task<IList<Analysis>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Analysis> GetAsync(string platform, string analysisId)
        {
            var entity = await _analysisTable.GetEntityAsync(platform, analysisId);
            return entity.Map();
        }

        public async Task SaveAsync(Analysis analysis)
        {
            await _analysisTable.InsertEntityAsync(new AnalysisEntity(analysis));
        }
    }
}
