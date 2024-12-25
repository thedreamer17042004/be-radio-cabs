using radioCabs.Dtos;
using radioCabs.Dtos.Advertise;
using radioCabs.Dtos.Payment;
using radioCabs.Dtos.PaymentPlan;

namespace radioCabs.Common
{
    public interface ICommonService<T> where T : class
    {
        Task<PagedResponse<T>> GetPagedDataAsync(QueryParams queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);
        Task<string> UploadFile(IFormFile? uploadFile);
        Task<PagedResponse<T>> SearchAd(SearchAdFilter queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);
        Task<PagedResponse<T>> SearchPaymentPlan(SearchPaymentPlanFilter queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);
        Task<PagedResponse<T>> SearchPayment(SearchPaymentFilter queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null);
    }
}
