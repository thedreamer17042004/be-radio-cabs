using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using radioCabs.Data;
using radioCabs.Dtos;
using radioCabs.Dtos.Advertise;
using radioCabs.Dtos.Payment;
using radioCabs.Dtos.PaymentPlan;
using System.Globalization;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Claims;

namespace radioCabs.Common
{
    public class CommonService<T> : ICommonService<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;
        private const string ROOT_IMAGE = "wwwroot/images";
        public CommonService(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<PagedResponse<T>> GetPagedDataAsync(QueryParams queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
        {
            var query = _dbSet.AsQueryable();

            // Apply filtering
            if (!string.IsNullOrEmpty(queryParams.Keyword))
            {
                query = ApplyFilter(query, filterBy, queryParams.Keyword);
            }

            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                bool status = queryParams.Status.Equals("1");
                query = query.Where(item => EF.Property<bool>(item, "Active") == status);
            }

           


            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy) && !string.IsNullOrEmpty(queryParams.SortDir))
            {
                query = ApplySorting(query, queryParams.SortBy, string.Equals(queryParams.SortDir, "asc", StringComparison.OrdinalIgnoreCase));
            }

            // Get total records
            var totalRecords = await query.CountAsync();

            if (includeFunc != null)
            {
                query = includeFunc(query);
            }

            // Apply pagination
            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // Create and return paginated response
            return new PagedResponse<T>(items, totalRecords, queryParams.PageNumber, queryParams.PageSize);
        }

        public async Task<PagedResponse<T>> SearchAd(SearchAdFilter queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Keyword))
            {
                query = ApplyFilter(query, filterBy, queryParams.Keyword);
            }

            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                bool status = queryParams.Status.Equals("1");
                query = query.Where(item => EF.Property<bool>(item, "Active") == status);
            }

            if (!string.IsNullOrEmpty(queryParams.CompanyId + "") && queryParams.CompanyId != 0)
            {
                query = ApplyFilterByType(query, "CompanyId", queryParams.CompanyId);
            }

            if (!string.IsNullOrEmpty(queryParams.FromDate))
            {
                query = ApplyFilterByTypeRange(query, "StartDate", TryParseDateTime(queryParams.FromDate), true);
            }

            if (!string.IsNullOrEmpty(queryParams.ToDate))
            {
                query = ApplyFilterByTypeRange(query, "EndDate", TryParseDateTime(queryParams.ToDate), false);
            }


            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy) && !string.IsNullOrEmpty(queryParams.SortDir))
            {
                query = ApplySorting(query, queryParams.SortBy, string.Equals(queryParams.SortDir, "asc", StringComparison.OrdinalIgnoreCase));
            }

            // Get total records
            var totalRecords = await query.CountAsync();

            if (includeFunc != null)
            {
                query = includeFunc(query);
            }

            // Apply pagination
            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // Create and return paginated response
            return new PagedResponse<T>(items, totalRecords, queryParams.PageNumber, queryParams.PageSize);
        }

        private IQueryable<T> ApplyFilterByType(IQueryable<T> query, string propertyName, object value)
        {
            if (value != null)
            {
                // Create the parameter for the lambda expression: (item)
                var parameter = Expression.Parameter(typeof(T), "item");

                // Create the property access: item.PropertyName
                var property = Expression.Property(parameter, propertyName);

                // Convert the value to the correct type
                var propertyType = property.Type; // Get the property type dynamically
                // Create the constant expression with the correct type
                var constant = Expression.Constant(value, propertyType);

                // Create the equality expression: item.PropertyName == value
                var equal = Expression.Equal(property, constant);

                // Create the full lambda expression: item => item.PropertyName == value
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

                // Apply the filter to the query
                query = query.Where(lambda);
            }

            return query;
        }

        private IQueryable<T> ApplyFilterByTypeRange(IQueryable<T> query, string propertyName, object value, bool isFrom)
        {
            if (value == null) return query;

            // Parameter for the lambda expression: "item"
            var parameter = Expression.Parameter(typeof(T), "item");

            // Access the property: item.PropertyName
            var property = Expression.Property(parameter, propertyName);

            // Check if the property type is DateTime or nullable DateTime
            var propertyType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;
            if (propertyType != typeof(DateTime))
            {
                throw new InvalidOperationException($"Property '{propertyName}' is not of type DateTime.");
            }

            // Create the constant value for comparison
            var constant = Expression.Constant(value, property.Type);

            // Build comparison expression (>= or <=)
            Expression comparison = isFrom
                ? Expression.GreaterThanOrEqual(property, constant)
                : Expression.LessThanOrEqual(property, constant);

            // Build lambda: item => item.PropertyName >= value or item.PropertyName <= value
            var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

            // Apply the filter
            return query.Where(lambda);
        }

        public async Task<string> UploadFile(IFormFile? uploadFile)
        {
            var uniqueFileName = "";
            if (uploadFile != null && uploadFile.Length > 0)
            {
                uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadFile.FileName);
                var filePath = Path.Combine(ROOT_IMAGE, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(stream);
                }
            }
            else
            {
                return null;
            }
            return uniqueFileName;
        }

        public static IQueryable<T> ApplySorting<T>(IQueryable<T> source, string sortBy, bool isAscending)
        {
            // Ensure the property name is valid
            var propertyInfo = typeof(T).GetProperty(sortBy);
            if (propertyInfo == null)
            {
                throw new ArgumentException($"Property '{sortBy}' does not exist on type '{typeof(T).Name}'.");
            }

            // Create the expression: item => item.SortBy
            var parameter = Expression.Parameter(typeof(T), "item");
            var property = Expression.Property(parameter, propertyInfo.Name);
            var sortExpression = Expression.Lambda(property, parameter);

            // Create the method call expression for OrderBy or OrderByDescending
            var method = isAscending ? "OrderBy" : "OrderByDescending";
            var expression = Expression.Call(
                typeof(Queryable),
                method,
                new Type[] { typeof(T), propertyInfo.PropertyType },
                source.Expression,
                Expression.Quote(sortExpression)
            );

            // Apply the sorting to the source query
            return source.Provider.CreateQuery<T>(expression);
        }

        private static IQueryable<T> ApplyFilter(IQueryable<T> query, string[] propertyNames, string keyword)
        {
            if (propertyNames.Length <= 0 || string.IsNullOrEmpty(keyword))
            {
                return query;
            }

            var parameter = Expression.Parameter(typeof(T), "item");


            var expressions = new List<Expression>();

            foreach (var propertyName in propertyNames)
            {
                var property = Expression.Property(parameter, propertyName);
                var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                var searchValue = Expression.Constant(keyword, typeof(string));
                var propertyExpression = Expression.Call(property, containsMethod, searchValue);

                expressions.Add(propertyExpression);
            }

            // Combine all expressions with OR
            Expression combinedExpression = expressions
            .Aggregate((left, right) => Expression.OrElse(left, right));

            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return query.Where(lambda);
        }

        public static DateTime TryParseDateTime(string data)
        {
            DateTime date = DateTime.ParseExact(data, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            return date ;
        }

        public async Task<PagedResponse<T>> SearchPaymentPlan(SearchPaymentPlanFilter queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Keyword))
            {
                query = ApplyFilter(query, filterBy, queryParams.Keyword);
            }

            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                bool status = queryParams.Status.Equals("1");
                query = query.Where(item => EF.Property<bool>(item, "Active") == status);
            }

            if (!string.IsNullOrEmpty(queryParams.PlanType))
            {
                query = ApplyFilterByType(query, "PlanType", queryParams.PlanType);
            }

            if (!string.IsNullOrEmpty(queryParams.Duration))
            {
                query = ApplyFilterByType(query, "Duration", queryParams.Duration);
            }


            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy) && !string.IsNullOrEmpty(queryParams.SortDir))
            {
                query = ApplySorting(query, queryParams.SortBy, string.Equals(queryParams.SortDir, "asc", StringComparison.OrdinalIgnoreCase));
            }

            // Get total records
            var totalRecords = await query.CountAsync();

            if (includeFunc != null)
            {
                query = includeFunc(query);
            }

            // Apply pagination
            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // Create and return paginated response
            return new PagedResponse<T>(items, totalRecords, queryParams.PageNumber, queryParams.PageSize);
       
        }

        public async Task<PagedResponse<T>> SearchPayment(SearchPaymentFilter queryParams, string[] filterBy, Func<IQueryable<T>, IQueryable<T>>? includeFunc = null)
        {
            var query = _dbSet.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Keyword))
            {
                query = ApplyFilter(query, filterBy, queryParams.Keyword);
            }

            if (!string.IsNullOrEmpty(queryParams.Status))
            {
                bool status = queryParams.Status.Equals("1");
                query = query.Where(item => EF.Property<bool>(item, "Active") == status);
            }

            if (!string.IsNullOrEmpty(queryParams.PaymentDate + ""))
            {
                query = ApplyFilterByType(query, "PaymentDate", queryParams.PaymentDate);
            }

            if (!string.IsNullOrEmpty(queryParams.PaymentStatus + ""))
            {
                query = ApplyFilterByType(query, "PaymentStatus", queryParams.PaymentStatus);
            }

            if (!string.IsNullOrEmpty(queryParams.PaymentType + ""))
            {
                query = ApplyFilterByType(query, "PaymentType", queryParams.PaymentType);
            }

            if (!string.IsNullOrEmpty(queryParams.PlanId ))
            {
                query = ApplyFilterByType(query, "PlanId", int.Parse(queryParams.PlanId));
            }


            // Apply sorting
            if (!string.IsNullOrEmpty(queryParams.SortBy) && !string.IsNullOrEmpty(queryParams.SortDir))
            {
                query = ApplySorting(query, queryParams.SortBy, string.Equals(queryParams.SortDir, "asc", StringComparison.OrdinalIgnoreCase));
            }

            // Get total records
            var totalRecords = await query.CountAsync();

            if (includeFunc != null)
            {
                query = includeFunc(query);
            }

            // Apply pagination
            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            // Create and return paginated response
            return new PagedResponse<T>(items, totalRecords, queryParams.PageNumber, queryParams.PageSize);

        }
    }
}
