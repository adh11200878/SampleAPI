using SampleAPI.Helper;
using System.Diagnostics;
using System.Reflection;

namespace SampleAPI.Extension
{
    public static class ServiceCollectionExtensions
    {
        //將應用程式所需的所有服務進行註冊 (IConfiguration configuration就是appsettings.json)
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 註冊 JWT Helper(使用AddSingleton)因為都使共用 這樣效能較佳
            services.AddSingleton<JwtHelper>();
            services.AddSingleton<FileUploadHelper>();

            var sw = Stopwatch.StartNew();
            // 業務邏輯層（Service）
            var assembly = Assembly.GetExecutingAssembly();
            //var assembly = Assembly.Load("SampleAPI");
            //這個屬性是 System.Type 的成員，代表：是一個類別（class）如:public class MyClass
            //!t.IsAbstract這表示這個類別不是抽象類別（abstract class）如:public class UserService : BaseService 
            //x.Namespace == "SampleAPI.Service" (資料夾名稱Service 前面是專案名稱SampleAPI)
            var serviceList = assembly.GetTypes()
                .Where(x =>
                    x.IsClass && !x.IsAbstract
                    && x.Namespace == "SampleAPI.Service"
                    //&& x.Name.EndsWith("Service")//篩選特定後面的名稱
                    )
                .ToList();
            //直接抓取那個檔案DI注入
            foreach (var item in serviceList)
            {
                services.AddScoped(item);
            }
            //如果有使用介面的話 通常命名加I(這邊不使用) 
            //foreach (var item in serviceList)
            //{
            //    // 根據命名規則自動找對應介面
            //    var interfaceType = item.GetInterface("I" + item.Name);
            //    if (interfaceType != null)
            //    {
            //        services.AddScoped(interfaceType, item);
            //    }
            //    else
            //    {
            //        // 若無介面，直接註冊具體類別（選配）
            //        services.AddScoped(item);
            //    }
            //}

            sw.Stop();
            Console.WriteLine($"DI 掃描註冊耗時: {sw.ElapsedMilliseconds} ms");

            return services;
        }
    }
}
